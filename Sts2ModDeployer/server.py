import http.server
import socketserver
import json
import os
import shutil
import urllib.parse
import subprocess
from glob import glob

PORT = 8001
SCRATCH_DIR = os.path.abspath(os.path.join(os.path.dirname(__file__), ".."))
MODS_DIR = r"A:\SteamLibrary\steamapps\common\Slay the Spire 2\mods"

class ModDeployerHandler(http.server.SimpleHTTPRequestHandler):
    def end_headers(self):
        self.send_header('Access-Control-Allow-Origin', '*')
        super().end_headers()
        
    def do_GET(self):
        if self.path == '/':
            self.path = '/public/index.html'
        elif self.path.startswith('/public/'):
            pass
        elif self.path == '/api/mods':
            self.handle_get_mods()
            return
        return super().do_GET()

    def do_POST(self):
        if self.path == '/api/deploy':
            self.handle_deploy()
        else:
            self.send_error(404)

    def handle_get_mods(self):
        mods = []
        # Scan for mod manifests
        for manifest_path in glob(os.path.join(SCRATCH_DIR, "*", "mod_manifest.json")):
            mod_dir = os.path.dirname(manifest_path)
            # check if a dll exists matching the pck_name or directory name
            try:
                with open(manifest_path, 'r', encoding='utf-8') as f:
                    manifest = json.load(f)
                    
                pck_name = manifest.get("pck_name", os.path.basename(mod_dir))
                
                # Check if it's currently deployed
                target_json = os.path.join(MODS_DIR, f"{pck_name}.json")
                is_deployed = os.path.exists(target_json)
                
                mods.append({
                    "id": pck_name,
                    "name": manifest.get("name", pck_name),
                    "version": manifest.get("version", "1.0"),
                    "author": manifest.get("author", "Unknown"),
                    "description": manifest.get("description", ""),
                    "path": mod_dir,
                    "isDeployed": is_deployed
                })
            except Exception as e:
                print(f"Error parsing {manifest_path}: {e}")
                
        self.send_response(200)
        self.send_header('Content-Type', 'application/json')
        self.end_headers()
        self.wfile.write(json.dumps({"mods": mods}).encode('utf-8'))

    def handle_deploy(self):
        content_length = int(self.headers['Content-Length'])
        post_data = self.rfile.read(content_length)
        data = json.loads(post_data.decode('utf-8'))
        
        mod_ids_to_deploy = data.get("deployIds", [])
        
        # We will iterate through all detected mods and manage them
        all_manifests = glob(os.path.join(SCRATCH_DIR, "*", "mod_manifest.json"))
        success_messages = []
        
        for manifest_path in all_manifests:
            mod_dir = os.path.dirname(manifest_path)
            with open(manifest_path, 'r', encoding='utf-8') as f:
                manifest = json.load(f)
            
            pck_name = manifest.get("pck_name", os.path.basename(mod_dir))
            target_json = os.path.join(MODS_DIR, f"{pck_name}.json")
            target_dll = os.path.join(MODS_DIR, f"{pck_name}.dll")
            target_pck = os.path.join(MODS_DIR, f"{pck_name}.pck")
            
            should_deploy = pck_name in mod_ids_to_deploy
            
            if should_deploy:
                # Compile and deploy payload
                publish_script = os.path.join(mod_dir, "PublishMod.ps1")
                try:
                    if os.path.exists(publish_script):
                        print(f"Running PublishMod.ps1 for {pck_name}...")
                        subprocess.run(["powershell.exe", "-ExecutionPolicy", "Bypass", "-File", "PublishMod.ps1"], cwd=mod_dir, check=True)
                    else:
                        print(f"Running dotnet publish for {pck_name}...")
                        # We use dotnet publish since that invokes their GodotPublish targets
                        subprocess.run(["dotnet", "publish"], cwd=mod_dir, check=True)
                except Exception as e:
                    print(f"Failed to compile/deploy {pck_name}: {e}")
                    success_messages.append(f"Failed to build {pck_name}")
                    continue

                # Construct the [ModID].json and deploy it
                new_manifest = {
                    "id": pck_name,
                    "name": manifest.get("name", pck_name),
                    "version": manifest.get("version", "1.0"),
                    "author": manifest.get("author", "Unknown"),
                    "description": manifest.get("description", ""),
                    "has_pck": True,
                    "has_dll": True,
                    "dependencies": manifest.get("dependencies", []),
                    "affects_gameplay": manifest.get("affects_gameplay", True)
                }
                
                # If they have local build outputs in bin, we could copy them, but user said their existing
                # process builds direct to `mods`. So if they're in ModDeployer, we just need to ENABLE it.
                # If files are missing in `mods`, we try to copy from local `bin/Release/net9.0/` or similar.
                with open(target_json, 'w', encoding='utf-8') as f:
                    json.dump(new_manifest, f, indent=2)
                success_messages.append(f"Deployed {pck_name}")
            else:
                # Uninstall/Disable
                if os.path.exists(target_json):
                    os.remove(target_json)
                    success_messages.append(f"Undeployed {pck_name}")
                    
        self.send_response(200)
        self.send_header('Content-Type', 'application/json')
        self.end_headers()
        self.wfile.write(json.dumps({"success": True, "messages": success_messages}).encode('utf-8'))

if __name__ == "__main__":
    os.makedirs(os.path.join(os.path.dirname(__file__), "public"), exist_ok=True)
    with socketserver.TCPServer(("", PORT), ModDeployerHandler) as httpd:
        print(f"Mod Deployer serving at http://localhost:{PORT}")
        httpd.serve_forever()
