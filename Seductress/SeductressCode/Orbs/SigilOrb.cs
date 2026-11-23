using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Assets;
using MegaCrit.Sts2.Core.Models.Orbs;
using MegaCrit.Sts2.Core.ValueProps;
using Seductress.SeductressCode.Powers;
using Seductress.SeductressCode.Systems;

namespace Seductress.SeductressCode.Orbs;

/// <summary>
/// The Sigil passive (design §7.0): at the start of your turn, deal 1 damage to ALL enemies.
///
/// [Gorim] Displayed using the vanilla Glass Orb's art for now, with <b>orb capacity pinned to the
/// Sigil count</b> so there are never empty slots to imply a cap. §7 is explicit that Sigils are
/// not Defect orbs — no slots, no channelling, no evoking as a core loop — so the orb queue is
/// borrowed purely as a display, and nothing in the pool channels or evokes one.
///
/// Fixed at 1 damage rather than the Glass Orb's decaying 4. <i>Ritual Mark</i> raises it.
///
/// Damage is dealt with no card source, so a kill it lands has no finishing tag and falls to
/// Skin — which is exactly the leak §6.3a's Leg-Locked exists to close.
/// </summary>
public class SigilOrb : CustomOrbModel
{
    /// <summary>Vanilla's Lightning Orb assets, derived from its model id rather than hard-coded.</summary>
    private static string LightningEntry => ModelDb.GetId<LightningOrb>().Entry.ToLowerInvariant();

    private static readonly Color Pink = new(1f, 0.42f, 0.72f);

    public override string CustomIconPath => ImageHelper.GetImagePath("orbs/" + LightningEntry + ".png");
    public override string CustomSpritePath => SceneHelper.GetScenePath("orbs/orb_visuals/" + LightningEntry);

    public override Color DarkenedColor => new("6E2A55");

    /// <summary>
    /// Instantiates vanilla's Lightning Orb scene and tints it pink via Modulate. Modulate
    /// multiplies, so this darkens as well as tints — good enough as a placeholder, and it costs
    /// no art. The <i>icon</i> shown in tooltips is a plain texture and cannot be tinted this way,
    /// so it stays lightning-coloured until the orb has its own art.
    /// </summary>
    public override Node2D? CreateCustomSprite()
    {
        var scene = PreloadManager.Cache.GetScene(CustomSpritePath);
        if (scene is null) return null;

        var sprite = scene.Instantiate<Node2D>(PackedScene.GenEditState.Disabled);
        sprite.Modulate = Pink;

        // [Gorim, 2026-11-22] "I want to put the sigil mark in them." [Claude] The mark sits on
        // top of the orb. A child is multiplied by the parent's pink, so its own colour is
        // divided by that pink first to come out near white. Size is mine.
        var mark = GD.Load<Texture2D>($"{MainFile.ResPath}/images/ui/sigil.png");
        if (mark is not null)
        {
            const float size = 54f;
            sprite.AddChild(new Sprite2D
            {
                Texture = mark,
                Scale = Vector2.One * (size / mark.GetWidth()),
                Modulate = new Color(1f / Pink.R, 0.93f / Pink.G, 0.97f / Pink.B),
                ZIndex = 1,
            });
        }
        return sprite;
    }

    public override bool IncludeInRandomPool => false;

    /// <summary>
    /// The variables are {Passive} and {Evoke}, and <b>only the smartDescription receives them</b> —
    /// OrbModel.Description is documented as taking no variables, so a token there prints raw.
    ///
    /// Both lines describe the passive rather than the evoke: nothing in the pool evokes a Sigil,
    /// and §7 rules evoking out as a core loop, so advertising it would be misleading.
    /// </summary>
    public override List<(string, string)>? Localization => new OrbLoc(
        "Sigil",
        "Your cards grow stronger with each Sigil.",
        "Your cards grow stronger with each Sigil.");

    /// <summary>
    /// [Gorim, 2026-11-23] "Lets just not make it scale sigils. Lets actually try taking the 1
    /// damage off sigils too." Sigils dealt 1 to ALL enemies each turn, plus Ritual Mark's bonus;
    /// now they deal nothing. One number to put back if the trial goes the other way.
    /// </summary>
    public const decimal BaseDamage = 0m;

    /// <summary>The Sigil's own damage; nothing adds to it.</summary>
    public override decimal PassiveVal
    {
        get
        {
            return BaseDamage <= 0m ? 0m : ModifyOrbValue(BaseDamage);
        }
    }

    /// <summary>
    /// Nothing evokes a Sigil — §7 rules evoking out as a core loop — but the property is abstract,
    /// so it mirrors the passive rather than inventing a second number.
    /// </summary>
    public override decimal EvokeVal => PassiveVal;

    public override async Task AfterTurnStartOrbTrigger(PlayerChoiceContext choiceContext)
    {
        await Passive(choiceContext, null);

        // [Claude, 2026-11-23] The queue holds at most 10 orbs. The first orb also fires once
        // for every Sigil past that, so 13 Sigils still hit 13 times.
        var queue = Owner?.PlayerCombatState?.OrbQueue;
        if (queue is null || queue.Orbs.Count == 0 || !ReferenceEquals(queue.Orbs[0], this)) return;
        var extra = Systems.Sigils.Effective(Owner!) - queue.Orbs.Count;
        for (var i = 0; i < extra; i++) await Passive(choiceContext, null);
    }

    public override async Task Passive(PlayerChoiceContext choiceContext, Creature? target)
    {
        var amount = PassiveVal;
        if (amount <= 0m || CombatState is null) return;

        var targets = CombatState.HittableEnemies.Where(e => e.IsHittable).ToList();
        if (targets.Count == 0) return;

        ActivatePassive(); // 0.111: OrbModel.Trigger() renamed ActivatePassive()
        await CreatureCmd.Damage(choiceContext, targets, amount, ValueProp.Unpowered, Owner.Creature);
    }
}
