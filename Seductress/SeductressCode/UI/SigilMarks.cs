using System.Collections.Generic;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Nodes.Cards;
using Seductress.SeductressCode.Cards;
using Seductress.SeductressCode.Systems;

namespace Seductress.SeductressCode.UI;

/// <summary>
/// Four small Sigil icons along the bottom of a card's text box, one per tier (1, 3, 5, 7
/// Sigils): pink when that tier is active, grey when not.
///
/// [Gorim, 2026-11-22] Chose "Yes, four small Sigil icons" when the tier lines came off the card
/// face. [Claude] Size, spacing and position are mine.
/// </summary>
public partial class SigilMarks : Control
{
    private const float Icon = 26f;
    private const float Gap = 5f;
    private const float Width = 4 * Icon + 3 * Gap;

    private static readonly Color On = new(1f, 0.55f, 0.80f);
    private static readonly Color Off = new(0.55f, 0.50f, 0.55f, 0.45f);
    private static readonly int[] Tiers = [Sigils.Tier1, Sigils.Tier2, Sigils.Tier3, Sigils.Tier4];

    private readonly List<TextureRect> _icons = [];

    public override void _Ready()
    {
        MouseFilter = MouseFilterEnum.Ignore;
        Size = new Vector2(Width, Icon);
        var texture = GD.Load<Texture2D>($"{MainFile.ResPath}/images/ui/sigil.png");
        for (var i = 0; i < Tiers.Length; i++)
        {
            var icon = new TextureRect
            {
                // ExpandMode first: until it is set the picture's own size is the minimum size.
                ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
                StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
                Texture = texture, Position = new Vector2(i * (Icon + Gap), 0f), Size = new Vector2(Icon, Icon),
                MouseFilter = MouseFilterEnum.Ignore,
            };
            AddChild(icon);
            _icons.Add(icon);
        }
    }

    /// <summary>Height taken off the bottom of the text box so the icons never sit on the text.</summary>
    public const float Reserved = Icon + 2f;

    public void Show(int sigils, Control box)
    {
        Visible = true;
        // In the strip just under the (shortened) text box.
        Position = box.Position + new Vector2((box.Size.X - Width) / 2f, box.Size.Y + 2f);
        for (var i = 0; i < _icons.Count; i++) _icons[i].Modulate = sigils >= Tiers[i] ? On : Off;
    }
}

/// <summary>
/// [Gorim, 2026-11-22] The icons overlapped the last line of text: "if we can make sure it
/// doesn't overlap the text it's fine where it is." [Claude] So a Sigil card's text box is made
/// shorter by the height of the icon row before the game fits the text into it, and the icons go
/// in the freed strip. Cards are reused between models, so the box is put back for other cards.
/// </summary>
[HarmonyPatch(typeof(NCard), nameof(NCard.UpdateVisuals))]
internal static class SigilMarksPatch
{
    private const string OriginalBottom = "seductress_text_bottom";

    [HarmonyPrefix]
    private static void MakeRoom(NCard __instance)
    {
        if (!__instance.IsNodeReady()) return;
        var box = __instance.GetNodeOrNull<Control>("%DescriptionLabel");
        if (box is null) return;

        if (!box.HasMeta(OriginalBottom)) box.SetMeta(OriginalBottom, box.OffsetBottom);
        var original = box.GetMeta(OriginalBottom).AsSingle();
        var tiered = __instance.Model is SeductressCard { HasSigilTiers: true };
        box.OffsetBottom = tiered ? original - SigilMarks.Reserved : original;
    }

    [HarmonyPostfix]
    private static void Mark(NCard __instance)
    {
        if (!__instance.IsNodeReady()) return;
        var box = __instance.GetNodeOrNull<Control>("%DescriptionLabel");
        if (box?.GetParent() is not Node parent) return;

        var marks = parent.GetNodeOrNull<SigilMarks>("SigilMarks");
        if (__instance.Model is SeductressCard { HasSigilTiers: true } card)
        {
            if (marks is null)
            {
                marks = new SigilMarks { Name = "SigilMarks" };
                parent.AddChild(marks);
            }
            marks.Show(card.SigilCount, box);
        }
        else if (marks is not null)
        {
            marks.Visible = false;
        }
    }
}
