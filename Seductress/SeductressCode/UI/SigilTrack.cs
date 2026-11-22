using System.Collections.Generic;
using Godot;

namespace Seductress.SeductressCode.UI;

/// <summary>
/// The Sigil ladder as seven Sigil icons in a row.
///
/// [Gorim, 2026-11-21] "Can you make the sigil track 7 of these? As in just the icon, not the
/// background. Make it pink and glowing probably." The icon is cut out of the picture he sent
/// (<c>images/ui/sigil.png</c>, with a blurred copy for the glow).
///
/// [Claude] Sigils you can use glow pink and pulse; ones exhausted for this combat are pink
/// without the glow; the rest are grey outlines of what is still to earn. The four tier labels
/// sit under icons 1, 3, 5 and 7.
/// </summary>
public partial class SigilTrack : MilestoneTrack
{
    private const float Icon = 92f;
    private const float RowTop = 34f;

    private static readonly Color Pink = new(1f, 0.74f, 0.88f);
    private static readonly Color GlowPink = new(1f, 0.30f, 0.62f);
    private static readonly Color Spent = new(0.80f, 0.45f, 0.62f, 0.85f);
    private static readonly Color Unearned = new(0.48f, 0.42f, 0.48f, 0.55f);

    private readonly List<TextureRect> _glows = [];
    private string _now = "";
    private int _usable, _owned;
    private float _clock;
    private IReadOnlyList<Rung> _tiers = [];

    public void Setup(string now, int usable, int owned, IReadOnlyList<Rung> tiers)
    {
        _now = now; _usable = usable; _owned = owned; _tiers = tiers;
        CustomMinimumSize = new Vector2(TrackWidth, RowTop + Icon + 58f);
        MouseFilter = MouseFilterEnum.Ignore;
    }

    private static TextureRect Picture(Texture2D texture, Vector2 position, float size, Color tint) => new()
    {
        // ExpandMode first: until it is set the picture's own 256 px is the minimum size.
        ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
        StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
        Texture = texture, Position = position, Size = new Vector2(size, size),
        Modulate = tint, MouseFilter = MouseFilterEnum.Ignore,
    };

    public override void _Ready()
    {
        Header("Sigils", _now);

        var icon = GD.Load<Texture2D>($"{MainFile.ResPath}/images/ui/sigil.png");
        var glow = GD.Load<Texture2D>($"{MainFile.ResPath}/images/ui/sigil_glow.png");
        if (icon is null) MainFile.Logger.Info("[Seductress] images/ui/sigil.png is missing from the pck");

        var span = TrackWidth - 2f * Pad;
        const int count = Systems.Sigils.Tier4;
        for (var i = 1; i <= count; i++)
        {
            var centre = Pad + span * (i - 0.5f) / count;
            var at = new Vector2(centre - Icon / 2f, RowTop);

            if (icon is not null)
            {
                if (i <= _usable && glow is not null)
                {
                    var halo = Picture(glow, at - new Vector2(10f, 10f), Icon + 20f, GlowPink);
                    AddChild(halo);
                    _glows.Add(halo);
                }
                AddChild(Picture(icon, at, Icon, i <= _usable ? Pink : i <= _owned ? Spent : Unearned));
            }

            foreach (var tier in _tiers)
            {
                if (tier.At != i) continue;
                var colour = tier.Reached ? Lit : Dim;
                Text($"{i}: {tier.Label}", new Vector2(centre - 110f, RowTop + Icon + 2f), 220f, 15, colour,
                    HorizontalAlignment.Center, 2);
            }
        }
    }

    public override void _Process(double delta)
    {
        _clock += (float)delta;
        for (var i = 0; i < _glows.Count; i++)
        {
            var pulse = 0.5f + 0.5f * Mathf.Sin(_clock * 2.4f - i * 0.7f);
            _glows[i].Modulate = new Color(GlowPink, 0.55f + 0.45f * pulse);
        }
    }

    public override void _Draw() { }
}
