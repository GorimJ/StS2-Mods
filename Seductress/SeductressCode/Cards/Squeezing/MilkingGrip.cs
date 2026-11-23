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
/// Squeezing's engine. Half the drain, full Strength, double intake on Ass kills — it solves the
/// archetype's structural problem of a bank that depletes faster than kills refill it.
/// </summary>
public class MilkingGrip : SeductressCard
{
    public MilkingGrip() : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self) { }

    public override List<(string, string)>? Localization => new CardLoc(
        "Milking Grip",
        "Ass attacks lose {IfUpgraded:show:only 30%|half} as much Ass, but gain [gold]Strength[/gold] as though they had lost the full amount.\nAss kills gain double intake.");

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var power = await PowerCmd.Apply<MilkingGripPower>(choiceContext, Owner.Creature, 1m, Owner.Creature, this);
        // [Claude, 2026-11-14] The upgrade was specified (cards doc: "lose 3%") and the power already
        // supported it, but nothing ever set it. 3% of the bank against the normal 10% is 30% as much.
        if (IsUpgraded) power?.SetDrainPercent(UpgradedDrainPercent);
        power?.Refresh();
    }

    /// <summary>Upgraded drain, as a percentage of the bank. The base card's is the power's default 5.</summary>
    private const int UpgradedDrainPercent = 3;

    /// <summary>Nothing to change on the card itself: the upgrade is read from IsUpgraded when played.</summary>
    protected override void OnUpgrade() { }
}
