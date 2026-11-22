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
/// </summary>
public class Mouthful : SeductressCard
{
    public Mouthful() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self) { }

    private const int MlPerBlock = 25;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new BlockVar(4m, ValueProp.Move),
        new DynamicVar("Ml", MlPerBlock)
    ];

    public override List<(string, string)>? Localization => new CardLoc(
        "Mouthful",
        "Gain {Block:diff()} [gold]Block[/gold], plus 1 per {Ml:diff()} ml in your Mouth. Then [gold]Gulp[/gold].");

    // [Gorim, 2026-11-21] Gulp is a pop-out reminder, not card text (said of Milking Throat;
    // [Claude] applied here too so the two cards match).

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var r = Reservoirs.Of(Owner.RunState);
        var extra = r.MouthTotal / MlPerBlock;

        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block.BaseValue + extra, ValueProp.Move, cardPlay);
        MainFile.Logger.Info($"[Seductress] Mouthful: +{extra} Block from {r.MouthTotal} ml in the Mouth, then Gulp");
        await Stomach.SwallowAll(Owner, "GULP (Mouthful)");
    }

    protected override void OnUpgrade() => DynamicVars.Block.UpgradeValueBy(2m);
}
