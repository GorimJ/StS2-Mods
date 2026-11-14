"""Drive the game through Spire Link.
  drive.py st                 -> compact status + numbered actions
  drive.py act <n|label-substr> [more ...]  -> perform actions in sequence, status after
  drive.py con <command...>   -> console command, then status
  drive.py raw <key...>       -> dig into state
Never prints the API key."""
import json, os, re, sys, time, uuid, urllib.request, urllib.error

sys.stdout.reconfigure(encoding="utf-8", errors="replace")
BASE = "http://127.0.0.1:15528"
# Last observe/action dumps go to the temp folder, not next to this script in the repo.
HERE = os.path.join(os.environ.get("TEMP", "."), "spirelink_drive")
os.makedirs(HERE, exist_ok=True)
_KEY = None


def key():
    global _KEY
    if _KEY is None:
        cfg = os.path.join(os.environ["LOCALAPPDATA"], "SpireLink", "SpireLink.cfg")
        m = re.search(r'api_key"?\s*[:=]\s*"?([A-Za-z0-9_\-+/=.]+)', open(cfg, encoding="utf-8").read())
        _KEY = m.group(1)
    return _KEY


def call(method, path, body=None):
    data = json.dumps(body).encode() if body is not None else None
    req = urllib.request.Request(BASE + path, data=data, method=method)
    if data:
        req.add_header("Content-Type", "application/json")
    req.add_header("Authorization", "Bearer " + key())
    try:
        with urllib.request.urlopen(req, timeout=60) as r:
            return r.status, json.loads(r.read().decode("utf-8"))
    except urllib.error.HTTPError as e:
        raw = e.read().decode("utf-8", "replace")
        try:
            return e.code, json.loads(raw)
        except Exception:
            return e.code, raw


def observe(wait_ready=True):
    for _ in range(40):
        status, o = call("GET", "/v1/observe")
        if status != 200:
            print("OBSERVE", status, str(o)[:400])
            return o if isinstance(o, dict) else {}
        if not wait_ready or (o.get("ready") and o.get("actions")):
            break
        time.sleep(0.5)
    json.dump(o, open(os.path.join(HERE, "observe.json"), "w", encoding="utf-8"), indent=1)
    return o


def short(v, n=160):
    s = json.dumps(v, ensure_ascii=False)
    return s if len(s) <= n else s[:n] + "..."


def status(o):
    s = o.get("state") or {}
    print(f"== {s.get('state_type')} v{o.get('state_version')} ready={o.get('ready')} "
          f"interaction={o.get('interaction_status')} reason={o.get('reason')}")
    p = s.get("player") or {}
    if p:
        print(f"   player hp {p.get('hp')}/{p.get('max_hp')} block {p.get('block')} energy {p.get('energy')}/{p.get('max_energy')} "
              f"gold {p.get('gold')} status {short(p.get('status'), 300)}")
    b = s.get("battle") or {}
    for e in b.get("enemies") or []:
        print(f"   enemy {e.get('name')} hp {e.get('hp')}/{e.get('max_hp')} block {e.get('block')} "
              f"status {short(e.get('status'), 200)} intents {short([(i.get('type'), i.get('label')) for i in e.get('intents') or []])}")
    m = (s.get("mods") or {}).get("Seductress") or {}
    if m.get("error"):
        print("   MOD ERROR", m["error"])
    elif m.get("is_seductress"):
        r = m.get("reservoirs_ml", {})
        c = m.get("combat") or {}
        print(f"   sed arousal {m['arousal']['arousal']}/{m['arousal']['limit']} mouth {r.get('mouth_cum')}+{r.get('mouth_precum')}p "
              f"ass {r.get('ass')} womb {r.get('womb')} skin {r.get('skin')} stomach {r.get('stomach')} sigils {short(m.get('sigils'))} "
              f"lust {m['lust']['count']} climaxes {m['run_totals']['climaxes']}")
        if c:
            print(f"   sed combat cont={c.get('continuing_hole')} sens={c.get('sensitive')} last={c.get('last_attack_played')} "
                  f"routes {[(h['id'].replace('SEDUCTRESS-', ''), h['routes_to']) for h in c.get('hand_routes') or []]}")
        print(f"   floor {short(m.get('this_floor'), 400)}")
    other = {k: v for k, v in s.items() if k not in ("state_type", "battle", "run", "player", "context", "engine", "mods", "decision_mechanics")}
    if other:
        print("   extra", short(other, 1200))
    for i, a in enumerate(o.get("actions") or []):
        print(f"   [{i}] {a.get('action')} | {a.get('label')} | {short(a.get('arguments'), 110)}")


def act(sel):
    o = observe()
    acts = o.get("actions") or []
    a = None
    if sel.isdigit() and int(sel) < len(acts):
        a = acts[int(sel)]
    else:
        for x in acts:
            if sel.lower() in (str(x.get("label")) + " " + str(x.get("action"))).lower():
                a = x
                break
    if a is None:
        print("NO SUCH ACTION", sel)
        status(o)
        return False
    body = {"request_id": uuid.uuid4().hex, "expected_session_id": o["session_id"],
            "expected_version": o["state_version"], "action_id": a["action_id"], "wait_timeout_ms": 15000}
    code, r = call("POST", "/v1/actions", body)
    keep = {k: r.get(k) for k in ("status", "result", "error", "message", "reason", "outcome", "settled") if isinstance(r, dict) and k in r} if isinstance(r, dict) else r
    print(f">> {a.get('action')} {a.get('label')} -> {code} {short(keep, 500)}")
    json.dump(r, open(os.path.join(HERE, "last_action.json"), "w", encoding="utf-8"), indent=1)
    return code == 200


def main():
    cmd = sys.argv[1]
    if cmd == "st":
        status(observe(wait_ready=False))
    elif cmd == "act":
        for sel in sys.argv[2:]:
            if not act(sel):
                break
            time.sleep(0.4)
        status(observe())
    elif cmd == "con":
        code, r = call("POST", "/v1/console", {"command": " ".join(sys.argv[2:])})
        print(">> console", code, short(r, 900))
        time.sleep(1.0)
        status(observe(wait_ready=False))
    elif cmd == "conraw":
        code, r = call("POST", "/v1/console", {"command": " ".join(sys.argv[2:])})
        print(code, re.sub(r"\[/?\w+\]", "", r.get("message") or "") if isinstance(r, dict) else r)
    elif cmd == "unstick":
        o = observe(wait_ready=False)
        code, r = call("POST", "/v1/recovery", {"request_id": uuid.uuid4().hex, "expected_session_id": o["session_id"],
                                                 "mode": "abandon_pending", "expected_version": o["state_version"]})
        print(">> recovery", code, short({k: r.get(k) for k in ("status", "error")} if isinstance(r, dict) else r, 400))
        status(observe(wait_ready=False))
    elif cmd == "raw":
        o = observe(wait_ready=False)
        v = o
        for k in sys.argv[2:]:
            v = v[int(k)] if isinstance(v, list) else v.get(k)
        print(json.dumps(v, indent=1, ensure_ascii=False)[:6000])


main()
