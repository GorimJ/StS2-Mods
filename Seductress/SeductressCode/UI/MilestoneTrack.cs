using System.Collections.Generic;
using Godot;

namespace Seductress.SeductressCode.UI;

/// <summary>
/// One ladder on the milestones screen, drawn as a tube of fluid: it fills to the current
/// amount, with a bead and a label at every rung. Reached rungs are lit.
///
/// [Gorim, 2026-11-21] Picked "tracks for now" from three infographic concepts, then asked for
/// the tracks to "look more organic". [Claude] How: the tube's outline wanders instead of being a
/// rectangle, its ends are round, the fluid has a moving meniscus and a highlight, and the rungs
/// are beads rather than ruled lines. Sizes and colours are mine.
/// </summary>
public partial class MilestoneTrack : Control
{
    public record Rung(int At, string Label, bool Reached);

    public const float TrackWidth = 1180f;
    protected const float BarTop = 38f;
    protected const float BarHeight = 26f;
    protected const float Pad = 95f;       // room either side so end labels are not cut off

    protected static readonly Color Ink = new(1f, 0.92f, 0.96f);
    protected static readonly Color Gold = new(1f, 0.82f, 0.48f);
    protected static readonly Color Lit = new(0.56f, 0.86f, 0.48f);
    protected static readonly Color Dim = new(0.60f, 0.56f, 0.60f);
    private static readonly Color Trough = new(0.20f, 0.12f, 0.19f);
    private static readonly Color Wall = new(0.62f, 0.42f, 0.54f);
    private static readonly Color Fluid = new(0.96f, 0.93f, 0.88f);
    private static readonly Color Shade = new(0.84f, 0.78f, 0.74f);
    private static readonly Color Shine = new(1f, 1f, 1f, 0.55f);

    private string _name = "", _now = "";
    private int _value, _max, _marker = -1;
    private float _seed, _time;
    private IReadOnlyList<Rung> _rungs = [];

    /// <param name="marker">A second position on the bar (the Ass's fullest point), or -1.</param>
    public void Setup(string name, string now, int value, int max, IReadOnlyList<Rung> rungs, int marker = -1)
    {
        _name = name; _now = now; _value = value; _max = max; _rungs = rungs; _marker = marker;
        _seed = name.Length * 1.7f;
        CustomMinimumSize = new Vector2(TrackWidth, 136f);
        MouseFilter = MouseFilterEnum.Ignore;
    }

    private float X(int amount) => Pad + (TrackWidth - 2f * Pad) * Mathf.Clamp(amount / (float)_max, 0f, 1f);

    protected void Text(string text, Vector2 position, float width, int size, Color colour, HorizontalAlignment align, int lines = 1)
    {
        var label = new Label
        {
            Text = text, Position = position, Size = new Vector2(width, (size + 6f) * lines),
            HorizontalAlignment = align, MouseFilter = MouseFilterEnum.Ignore,
        };
        label.AddThemeFontSizeOverride("font_size", size);
        label.AddThemeColorOverride("font_color", colour);
        AddChild(label);
    }

    protected void Header(string name, string now)
    {
        Text(name, new Vector2(Pad, 0f), 200f, 22, Gold, HorizontalAlignment.Left);
        Text(now, new Vector2(Pad + 130f, 3f), TrackWidth - 2f * Pad - 130f, 18, Ink, HorizontalAlignment.Left);
    }

    public override void _Ready()
    {
        Header(_name, _now);

        const float w = 170f;
        foreach (var rung in _rungs)
        {
            var colour = rung.Reached ? Lit : Dim;
            var x = X(rung.At) - w / 2f;
            Text(rung.At.ToString(), new Vector2(x, BarTop + BarHeight + 8f), w, 16, colour, HorizontalAlignment.Center);
            Text(rung.Label, new Vector2(x, BarTop + BarHeight + 28f), w, 15, colour, HorizontalAlignment.Center, 2);
        }
    }

    public override void _Process(double delta)
    {
        _time += (float)delta;
        QueueRedraw();
    }

    // The tube's walls wander a little, so it reads as something grown rather than ruled.
    private float TopAt(float x) => BarTop + 2.2f * Mathf.Sin(x * 0.013f + _seed) + 1.1f * Mathf.Sin(x * 0.041f + _seed * 2f);
    private float BottomAt(float x) => BarTop + BarHeight + 2.2f * Mathf.Sin(x * 0.017f + _seed * 3f) + 1.1f * Mathf.Sin(x * 0.037f + _seed);

    /// <summary>The tube from its left end to <paramref name="to"/>; the right end is round, or a moving meniscus.</summary>
    private Vector2[] Shape(float to, bool meniscus)
    {
        var left = X(0);
        var points = new List<Vector2>();
        var r = BarHeight / 2f;

        // Left cap, bottom to top.
        var midLeft = (TopAt(left) + BottomAt(left)) / 2f;
        var halfLeft = (BottomAt(left) - TopAt(left)) / 2f;
        for (var i = 0; i <= 8; i++)
        {
            var a = Mathf.Pi / 2f + Mathf.Pi * i / 8f;
            points.Add(new Vector2(left + r * Mathf.Cos(a), midLeft + halfLeft * Mathf.Sin(a)));
        }
        // Top wall.
        for (var x = left + 12f; x < to; x += 12f) points.Add(new Vector2(x, TopAt(x)));
        points.Add(new Vector2(to, TopAt(to)));

        // Right end, top to bottom.
        var mid = (TopAt(to) + BottomAt(to)) / 2f;
        var half = (BottomAt(to) - TopAt(to)) / 2f;
        for (var i = 1; i < 8; i++)
        {
            var t = i / 8f;
            if (meniscus)
            {
                var bulge = 5f * Mathf.Sin(Mathf.Pi * t) + 2.5f * Mathf.Sin(Mathf.Tau * t + _time * 2.2f + _seed) * Mathf.Sin(Mathf.Pi * t);
                points.Add(new Vector2(to + bulge, Mathf.Lerp(TopAt(to), BottomAt(to), t)));
            }
            else
            {
                points.Add(new Vector2(to + r * Mathf.Sin(Mathf.Pi * t), mid - half * Mathf.Cos(Mathf.Pi * t)));
            }
        }
        // Bottom wall, back to the left.
        points.Add(new Vector2(to, BottomAt(to)));
        for (var x = to - 12f; x > left; x -= 12f) points.Add(new Vector2(x, BottomAt(x)));
        return points.ToArray();
    }

    private void Bead(float x, bool reached)
    {
        var centre = new Vector2(x, (TopAt(x) + BottomAt(x)) / 2f);
        if (reached)
        {
            var pulse = 0.5f + 0.5f * Mathf.Sin(_time * 2f + x * 0.01f);
            DrawCircle(centre, 11f + 2f * pulse, new Color(Lit, 0.22f));
            DrawCircle(centre, 7.5f, Lit);
            DrawCircle(centre + new Vector2(-2f, -2.5f), 2.2f, new Color(1f, 1f, 1f, 0.8f));
        }
        else
        {
            DrawCircle(centre, 7.5f, Trough);
            DrawArc(centre, 7.5f, 0f, Mathf.Tau, 24, Dim, 2f, true);
        }
    }

    public override void _Draw()
    {
        var left = X(0);
        var right = X(_max);

        var tube = Shape(right, meniscus: false);
        DrawColoredPolygon(tube, Trough);

        var level = X(_value);
        if (level - left > 3f)
        {
            var full = level >= right - 2f;
            DrawColoredPolygon(Shape(full ? right : level, meniscus: !full), Fluid);

            // A darker belly and a shine along the top, following the wall.
            var shade = new List<Vector2>();
            var shine = new List<Vector2>();
            for (var x = left + 4f; x < level - 4f; x += 12f)
            {
                shade.Add(new Vector2(x, BottomAt(x) - 5f));
                shine.Add(new Vector2(x, TopAt(x) + 5f));
            }
            if (shade.Count >= 2)
            {
                DrawPolyline(shade.ToArray(), Shade, 4f, true);
                DrawPolyline(shine.ToArray(), Shine, 2.5f, true);
            }
        }

        var outline = new List<Vector2>(tube) { tube[0] };
        DrawPolyline(outline.ToArray(), Wall, 2f, true);

        if (_marker >= 0)
        {
            var mx = X(_marker);
            DrawLine(new Vector2(mx, TopAt(mx) - 7f), new Vector2(mx, BottomAt(mx) + 7f), Gold, 2.5f, true);
            DrawCircle(new Vector2(mx, TopAt(mx) - 8f), 3.5f, Gold);
        }

        foreach (var rung in _rungs) Bead(X(rung.At), rung.Reached);
    }
}
