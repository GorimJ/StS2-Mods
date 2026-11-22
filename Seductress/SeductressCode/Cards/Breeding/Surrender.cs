using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Seductress.SeductressCode.Powers;
using Seductress.SeductressCode.Systems;

namespace Seductress.SeductressCode.Cards;

/// <summary>
/// Breeding Common, 0 cost — so tier 4 sheds its non-energy cost (the HP) rather than repeating.
/// A Climax-count payoff: strictly worse than Let Go before your first Climax, free draw after.
/// </summary>
public class Surrender : SeductressCard
{
    public Surrender() : base(0, CardType.Skill, CardRarity.Common, TargetType.Self) { }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("T1", Sigils.Tier1),
        new DynamicVar("T2", Sigils.Tier2),
        new DynamicVar("T3", Sigils.Tier3),
        new DynamicVar("T4", Sigils.Tier4),
        new HpLossVar(4m),
        new BlockVar(5m, ValueProp.Move),
        new DynamicVar("TierBlock", 10m)
    ];

    public override List<(string, string)>? Localization => new CardLoc(
        "Surrender",
        "{S4:cond:|Lose {HpLoss:diff()} HP.\n}Draw {S2:cond:2|1}, plus 1 per Climax this combat.{S3:cond:\nGain {TierBlock:diff()} [gold]Block[/gold].|{S1:cond:\nGain {Block:diff()} [gold]Block[/gold].|}}");

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        // Tier 4 on a 0-cost card sheds the non-energy cost instead of repeating.
        if (!Sigils.At(Owner, Sigils.Tier4))
        {
            await CreatureCmd.Damage(choiceContext, Owner.Creature, DynamicVars.HpLoss.BaseValue,
                ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, this, cardPlay);
        }

        var baseDraw = Sigils.At(Owner, Sigils.Tier2) ? 2 : 1;
        await CardPileCmd.Draw(choiceContext, baseDraw + ClimaxModel.Instance.ClimaxesThisCombat, Owner);

        if (Sigils.At(Owner, Sigils.Tier1))
        {
            var block = Sigils.At(Owner, Sigils.Tier3) ? DynamicVars["TierBlock"].BaseValue : DynamicVars.Block.BaseValue;
            await CreatureCmd.GainBlock(Owner.Creature, block, ValueProp.Move, cardPlay);
        }
    }

    protected override void OnUpgrade() => DynamicVars.HpLoss.UpgradeValueBy(-1m);
}
