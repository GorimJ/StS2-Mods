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
/// Draining Common. [Claude draft, 2026-11-14: "Gain 4 Block, plus 1 per 5 ml in your Mouth".
/// Gorim put it in Build with the note, 2026-11-19: "Add 'then gulp'. Gulp meaning eat all in
/// your mouth, heal for half that as normal."]
///
/// <b>Gulp</b> is a swallow on demand: the Mouth empties, heals at the ordinary swallow rate, cum
/// goes to the Stomach and precum is lost. [Claude] The note's "half" was the rate on the day it
/// was written; the rate became 1 HP per 10 ml on 2026-11-20 and Gulp follows it. The per-ml step
/// is 25 ml on the x5 scale, so a full Mouth is still +10 Block.
///
/// [Gorim, 2026-11-24] "It's actually only good for the gulp early. Maybe it's actually too bad.
/// Make it cost zero I think. And maybe draw a card after you unlock auto gulp?" Costs 0, and from
/// the 500 ml Stomach milestone (the automatic Gulp) it also draws 1. [Claude] The draw line shows
/// on the card only once the milestone is reached, the way Sigil-stage lines do.
/// </summary>
public class Mouthful : SeductressCard
{
    public Mouthful() : base(0, CardType.Skill, CardRarity.Common, TargetType.Self) { }

    private const int MlPerBlock = 25;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(4m, ValueProp.Move)
    ];

    public override List<(string, string)>? Localization => new CardLoc(
        "Mouthful",
        "Gain {MouthBlock} [gold]Block[/gold]. Then [gold]Gulp[/gold].{InCombat:\n({Block:diff()}, plus 1 per 25 ml in your Mouth)|}{AutoGulp:cond:\nDraw 1.|}");

    // [Gorim, 2026-11-21] Gulp is a pop-out reminder, not card text (said of Milking Throat;
    // [Claude] applied here too so the two cards match).

    /// <summary>Whether the 500 ml Stomach milestone (automatic Gulp) is reached.</summary>
    private bool AutoGulp => Owner is not null && Stomach.SwallowsAtFifty(Reservoirs.Of(Owner.RunState).Stomach);

    protected override void AddExtraArgsToDescription(MegaCrit.Sts2.Core.Localization.LocString description)
    {
        base.AddExtraArgsToDescription(description);
        description.Add("AutoGulp", AutoGulp);
        // [Gorim, standing rule] Card text prints the number, never the formula: the Block it
        // would give now, 4 (6 upgraded) plus 1 per 25 ml in the Mouth. (2026-11-24)
        description.Add("MouthBlock", DynamicVars.Block.BaseValue + MouthExtra);
    }

    /// <summary>Block from the Mouth right now: 1 per 25 ml.</summary>
    private int MouthExtra => Owner is null ? 0 : Reservoirs.Of(Owner.RunState).MouthTotal / MlPerBlock;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var r = Reservoirs.Of(Owner.RunState);
        var extra = r.MouthTotal / MlPerBlock;

        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block.BaseValue + extra, ValueProp.Move, cardPlay);
        MainFile.Logger.Info($"[Seductress] Mouthful: +{extra} Block from {r.MouthTotal} ml in the Mouth, then Gulp");
        var draws = AutoGulp; // read before the Gulp, which may move the Stomach
        await Stomach.SwallowAll(Owner, "GULP (Mouthful)");
        if (draws) await CardPileCmd.Draw(choiceContext, 1m, Owner);
    }

    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(2m);
}
