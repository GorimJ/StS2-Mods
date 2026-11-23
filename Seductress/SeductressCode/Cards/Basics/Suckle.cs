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
        new DynamicVar("Ml", 25m)
    ];

    public override List<(string, string)>? Localization => new CardLoc(
        "Suckle",
        "Mouth. Deal {Damage:diff()} damage. Gain {Ml:diff()} ml precum. If this kills, gain {Ml:diff()} ml of cum instead.");

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var target = cardPlay.Target;
        // [Gorim, 2026-11-23] Leg-Locked claims all cum the enemy produces, so Suckle's kill cum
        // goes to the Womb too. Read before the attack, while the enemy still holds the power.
        var legLocked = target.HasPower<Powers.LegLockedPower>();

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, cardPlay).Targeting(target)
            .WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);

        var ml = (int)DynamicVars["Ml"].BaseValue;
        var r = Reservoirs.Of(Owner.RunState);

        // Read the target after the attack resolves. A Climaxed enemy is no longer alive, which is
        // the same test the intake routing uses.
        var killed = !target.IsAlive;
        if (killed && legLocked)
        {
            r.Add(Hole.Pussy, ml);
            MainFile.Logger.Info($"[Seductress] Suckle: +{ml} ml cum -> Pussy (Leg-Locked kill)");
            return;
        }
        var added = killed ? r.AddMouthCum(ml) : r.AddPrecum(ml);

        MainFile.Logger.Info(
            $"[Seductress] Suckle: +{added} ml {(killed ? "CUM (kill — banks at the bell)" : "precum (evaporates at the bell)")}" +
            (added < ml ? $" — {ml - added} ml did not fit" : "") +
            $" | mouth {r.Mouth}+{r.Precum}p");

        await Stomach.SwallowIfFull(Owner, ml, added, cum: killed);
        if (!killed) await Stomach.AfterPrecumGained(Owner, ml);
    }

    /// <summary>
    /// [Gorim 2026-09-09] <b>The upgrade drops the cost to 0.</b> Third setting of this: cards doc
    /// §4 says "10 ml", which I built; that was cut on 2026-09-08 because 10 ml became 5 HP of
    /// healing per play once swallowing healed; +3 damage replaced it, and this replaces that —
    /// "it is not worth it as a damage card".
    ///
    /// [Gorim] Base damage stays at 4. Raising it to 5 was considered and declined.
    ///
    /// [Claude-Code] At 0 cost this stops being an attack that happens to make precum and becomes
    /// a free 5 ml every turn it is drawn — which, at the 1-per-2-ml swallow rate, is 2.5 HP of
    /// sustain a turn for no energy. That is a real engine rather than a tempo bump, and it is
    /// exactly what Draining wants, since the archetype's whole plan is staying high on the bar.
    /// It self-limits on the Mouth cap (250 ml) rather than on energy.
    /// </summary>
    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}
