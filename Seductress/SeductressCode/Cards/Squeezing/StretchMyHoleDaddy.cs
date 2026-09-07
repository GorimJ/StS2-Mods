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
/// Squeezing Power. Multiplies everything, because the Ass bonus applies per hit.
///
/// SCOPE NOTE: only this character's own cards consult it, which covers the whole intended pool.
/// A vanilla or cross-mod attack that costs HP will not gain the extra hit.
/// </summary>
public class StretchMyHoleDaddy : SeductressCard
{
    public StretchMyHoleDaddy() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self) { }

    public override List<(string, string)>? Localization => new CardLoc(
        "Stretch My Hole Daddy", "Attacks that cost you HP hit one additional time.");

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<StretchPower>(choiceContext, Owner.Creature, 1m, Owner.Creature, this);
    }

    protected override void OnUpgrade() => EnergyCost.SetCustomBaseCost(1);
}
