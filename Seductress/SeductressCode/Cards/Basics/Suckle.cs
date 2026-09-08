using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Seductress.SeductressCode.Systems;

namespace Seductress.SeductressCode.Cards;

/// <summary>
/// Basic, the Mouth half of the starting pair. An Attack rather than a Skill because Skills are
/// untagged by decision.
///
/// [Gorim] <b>It grants precum, not Block.</b> As a Block card it was plain and taught nothing —
/// Guard already covers defence, so the defensive slot was never the one going spare. What was
/// missing was any starting card showing that the Mouth economy exists.
///
/// [Claude-Cowork] <b>The "instead" is the whole point of the card.</b> It teaches the single most
/// confusable rule in the character on one line: precum normally, real cum on a kill. The
/// temporary resource and the banking resource, and the difference between them, demonstrated by
/// the same card on two outcomes of one play — so both class starters are intake teachers from
/// opposite ends, <i>Breed Me!</i> showing a kill filling the Womb and this showing the Mouth
/// filling whether you kill or not, and only banking if you do.
///
/// [Claude-Code] The kill's ordinary routed intake (10 ml to the Mouth for a normal enemy) lands
/// on top of this and is a separate thing — so a Suckle kill actually banks 5 + 10. The card's own
/// 5 ml is what changes kind; the routed intake was always real cum.
/// </summary>
public class Suckle : SeductressCard
{
    public Suckle() : base(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy) { }

    public override Hole Hole => Hole.Mouth;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(4m, ValueProp.Move),
        new DynamicVar("Ml", 5m)
    ];

    public override List<(string, string)>? Localization => new CardLoc(
        "Suckle",
        "Mouth. Deal {Damage:diff()} damage. Gain {Ml:diff()} ml precum. If this kills, gain {Ml:diff()} ml instead.");

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var target = cardPlay.Target;

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(target)
            .WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);

        var ml = (int)DynamicVars["Ml"].BaseValue;
        var r = Reservoirs.Of(Owner.RunState);

        // Read the target after the attack resolves. A Climaxed enemy is no longer alive, which is
        // the same test the intake routing uses.
        var killed = !target.IsAlive;
        var added = killed ? r.AddMouthCum(ml) : r.AddPrecum(ml);

        MainFile.Logger.Info(
            $"[Seductress] Suckle: +{added} ml {(killed ? "CUM (kill — banks at the bell)" : "precum (evaporates at the bell)")}" +
            (added < ml ? $" — {ml - added} ml did not fit" : "") +
            $" | mouth {r.Mouth}+{r.Precum}p");

        if (killed) await Stomach.SwallowIfFull(Owner);
    }

    /// <summary>
    /// [Gorim 2026-09-08] <b>The upgrade raises damage, not the ml.</b> Cards doc §4 says "10 ml",
    /// which was written before swallowing healed — at 10 ml this floor-1 basic was generating
    /// 5 HP of healing per play on a 20 HP bar, on top of being a Mouth filler.
    ///
    /// [Claude-Code] +3 matches what the other basics do (Pump 6 to 9, Guard 5 to 8) rather than
    /// being tuned. If the card is still too strong, the lever is the base 5 ml — that is where its
    /// power actually sits, since it pays every single play whether or not you kill.
    /// </summary>
    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m);
}
