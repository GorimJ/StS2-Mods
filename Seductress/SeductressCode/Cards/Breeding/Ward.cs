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

/// <summary>Breeding Common. Tier 1 adds reflect, 3 grows the Block, 5 grows the reflect, 7 repeats.</summary>
public class Ward : SeductressCard
{
    public Ward() : base(1, CardType.Skill, CardRarity.Common, TargetType.Self) { }

    protected override bool RepeatsAtTier4 => true;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("T1", Sigils.Tier1),
        new DynamicVar("T2", Sigils.Tier2),
        new DynamicVar("T3", Sigils.Tier3),
        new DynamicVar("T4", Sigils.Tier4),
        new BlockVar(6m, ValueProp.Move),
        new DynamicVar("TierBlock", 12m)
    ];

    public override List<(string, string)>? Localization => new CardLoc(
        "Ward",
        "Gain {Block:diff()} [gold]Block[/gold].\n{T1} Sigil: reflect 5 damage to attackers this turn.\n{T2} Sigils: gain {TierBlock:diff()} [gold]Block[/gold] instead.\n{T3} Sigils: reflect 10 instead.\n{T4} Sigils: repeats 1 time.");

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var block = Sigils.At(Owner, Sigils.Tier2) ? DynamicVars["TierBlock"].BaseValue : DynamicVars.Block.BaseValue;
        await CreatureCmd.GainBlock(Owner.Creature, block, ValueProp.Move, cardPlay);

        if (Sigils.At(Owner, Sigils.Tier1))
        {
            var reflect = Sigils.At(Owner, Sigils.Tier3) ? 10m : 5m;
            await PowerCmd.Apply<ThornsPower>(choiceContext, Owner.Creature, reflect, Owner.Creature, this);
        }
    }

    protected override void OnUpgrade()
    {
        DynamicVars.Block.UpgradeValueBy(3m);
        DynamicVars["TierBlock"].UpgradeValueBy(3m);
    }
}
