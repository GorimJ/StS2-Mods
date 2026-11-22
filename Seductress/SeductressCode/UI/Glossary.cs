using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization;

namespace Seductress.SeductressCode.UI;

/// <summary>
/// The hover pop-outs that explain the character's own words on cards.
///
/// [Gorim, 2026-11-21] "We should work out what hover text we want for cards. Ones we want for
/// sure are Gulp (built), Precum, Pussy, Ass, Mouth, Skin, Climax, Sigil, Eat."
///
/// [Claude] A card gets a term's pop-out when its text contains the word, so no card has to list
/// its own. The wording of every pop-out is a draft of mine for him to edit; it lives in
/// <c>localization/eng/static_hover_tips.json</c> under <c>SEDUCTRESS_&lt;TERM&gt;</c>.
/// </summary>
public static class Glossary
{
    // Order is the order the pop-outs stack in. "Eat" and "Ass" are matched with their capital
    // so that ordinary words do not trigger them.
    private static readonly (string Key, Regex Word)[] Terms =
    [
        ("GULP", new Regex(@"\bGulps?\b")),
        ("EAT", new Regex(@"\bEat\b")),
        // [Gorim, 2026-11-22] "Leg-locked also needs a popout text box."
        ("LEG_LOCKED", new Regex(@"\bLeg-Locked\b")),
        ("PRECUM", new Regex(@"\bpre-?cum\b", RegexOptions.IgnoreCase)),
        ("MOUTH", new Regex(@"\bMouth\b")),
        ("PUSSY", new Regex(@"\b(Pussy|Womb)\b")),
        ("ASS", new Regex(@"\bAss\b")),
        ("SKIN", new Regex(@"\bSkin\b")),
        ("CLIMAX", new Regex(@"\bClimax(es|ed)?\b")),
        ("SIGIL", new Regex(@"\bSigils?\b")),
    ];

    private static readonly Dictionary<Type, string[]> Cache = new();

    public static IHoverTip Tip(string key) => new HoverTip(
        new LocString("static_hover_tips", $"SEDUCTRESS_{key}.title"),
        new LocString("static_hover_tips", $"SEDUCTRESS_{key}.description"));

    /// <summary>The pop-outs for whichever terms appear in <paramref name="texts"/>.</summary>
    public static IEnumerable<IHoverTip> TipsFor(Type owner, Func<IEnumerable<string>> texts) =>
        KeysFor(owner, texts).Select(Tip);

    public static string[] AllKeys => Terms.Select(t => t.Key).ToArray();

    public static string[] KeysFor(Type owner, Func<IEnumerable<string>> texts)
    {
        if (!Cache.TryGetValue(owner, out var keys))
        {
            var all = string.Join("\n", texts());
            keys = Terms.Where(t => t.Word.IsMatch(all)).Select(t => t.Key).ToArray();
            Cache[owner] = keys;
        }
        return keys;
    }
}
