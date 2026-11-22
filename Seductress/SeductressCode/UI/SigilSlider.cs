using System.Collections.Generic;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.Nodes.CommonUi;
using MegaCrit.Sts2.Core.Nodes.Screens;
using Seductress.SeductressCode.Cards;
using Seductress.SeductressCode.Systems;

namespace Seductress.SeductressCode.UI;

/// <summary>
/// A 0 to 7 Sigil slider on the right-click card view, for cards whose text changes with Sigils.
/// Moving it rewrites the big card as it would read at that many Sigils.
///
/// [Gorim, 2026-11-22] "Perhaps the sigil stuff is only visible in the right click menu? Could do
/// a sliding bar of activating sigils to see how the card changes?" Then: "Sigil slider should be
/// vertical, and show the sigil hearts, not X sigils. Don't need the 'drag to see' instructions."
///
/// [Claude] A vertical slider with seven Sigil hearts beside it, lit from the bottom up to the
/// slider's value; clicking a heart also sets it. It starts at the count you have, sits left of
/// the card, and changes nothing in the run: it only sets
/// <see cref="SeductressCard.SigilPreview"/> on the view's own copy of the card.
/// </summary>
public partial class SigilSlider : Control
{
    private const int Count = Sigils.Tier4;
    private const float Heart = 46f;
    private const float Step = 52f;
    private const float Track = Step * Count;       // slider height: one step per Sigil

    private static readonly Color On = new(1f, 0.55f, 0.80f);
    private static readonly Color Off = new(0.55f, 0.50f, 0.55f, 0.45f);

    private readonly List<TextureRect> _hearts = [];
    private VSlider? _slider;

    /// <summary>The card the slider was last set up for; a new card resets it to your own count.</summary>
    public CardModel? Source;

    public System.Action? Changed;

    public int Value => _slider is null ? 0 : (int)_slider.Value;

    public override void _Ready()
    {
        // Left of the big card, centred on its middle.
        var area = GetParentControl()?.Size ?? GetViewportRect().Size;
        if (area.X < 100f) area = GetViewportRect().Size;
        Size = new Vector2(110f, Track + Heart);
        Position = area / 2f + new Vector2(-480f, -Size.Y / 2f);
        MouseFilter = MouseFilterEnum.Ignore;
        ZIndex = 5;

        // The slider's 0 is level with the bottom of the first heart; each step up lights one more.
        _slider = new VSlider
        {
            MinValue = 0, MaxValue = Count, Step = 1,
            Position = new Vector2(0f, Heart / 2f), Size = new Vector2(28f, Track),
        };
        _slider.ValueChanged += _ =>
        {
            Light();
            Changed?.Invoke();
        };
        AddChild(_slider);

        var texture = GD.Load<Texture2D>($"{MainFile.ResPath}/images/ui/sigil.png");
        for (var i = 1; i <= Count; i++)
        {
            var n = i;
            var heart = new TextureRect
            {
                // ExpandMode first: until it is set the picture's own size is the minimum size.
                ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize,
                StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
                Texture = texture, Size = new Vector2(Heart, Heart),
                // Heart n is centred on the slider's position for n.
                Position = new Vector2(44f, Track - n * Step),
                MouseFilter = MouseFilterEnum.Stop, MouseDefaultCursorShape = CursorShape.PointingHand,
            };
            heart.GuiInput += e =>
            {
                if (e is not InputEventMouseButton { Pressed: true, ButtonIndex: MouseButton.Left }) return;
                // Clicking the top lit heart turns it off, so 0 is reachable by clicking too.
                if (_slider is not null) _slider.Value = Value == n ? n - 1 : n;
            };
            AddChild(heart);
            _hearts.Add(heart);
        }
        Light();
    }

    private void Light()
    {
        for (var i = 0; i < _hearts.Count; i++) _hearts[i].Modulate = i < Value ? On : Off;
    }

    public void Set(int sigils)
    {
        if (_slider is not null) _slider.Value = Mathf.Clamp(sigils, 0, Count);
    }

    public void Reset(int sigils)
    {
        _slider?.SetValueNoSignal(Mathf.Clamp(sigils, 0, Count));
        Light();
    }
}

[HarmonyPatch(typeof(NInspectCardScreen), "UpdateCardDisplay")]
internal static class SigilSliderPatch
{
    [HarmonyPostfix]
    private static void Apply(NInspectCardScreen __instance, NCard ____card, List<CardModel>? ____cards, int ____index,
        NTickbox ____upgradeTickbox)
    {
        var slider = __instance.GetNodeOrNull<SigilSlider>("SigilSlider");
        var source = ____cards is not null && ____index >= 0 && ____index < ____cards.Count ? ____cards[____index] : null;

        if (source is not SeductressCard { HasSigilTiers: true } mine || ____card.Model is not SeductressCard shown)
        {
            if (slider is not null) slider.Visible = false;
            return;
        }

        if (slider is null)
        {
            slider = new SigilSlider { Name = "SigilSlider" };
            __instance.AddChild(slider);
            var screen = __instance;
            slider.Changed = () => AccessTools.Method(typeof(NInspectCardScreen), "UpdateCardDisplay").Invoke(screen, null);
        }
        slider.Visible = true;
        if (!ReferenceEquals(slider.Source, source))
        {
            slider.Source = source;
            slider.Reset(mine.SigilCount);
        }

        shown.SigilPreview = slider.Value;
        ____card.UpdateVisuals(____card.DisplayingPile,
            ____upgradeTickbox.IsTicked ? MegaCrit.Sts2.Core.Entities.Cards.CardPreviewMode.Upgrade
                                        : MegaCrit.Sts2.Core.Entities.Cards.CardPreviewMode.Normal);
    }
}
