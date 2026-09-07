using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace Seductress.SeductressCode.Cards;

/// <summary>
/// A Lust card. Playable, gives a real benefit, and costs a large chunk of arousal.
/// Five in the deck ends the run (not implemented in the spike — the counter is what matters here).
/// </summary>
public class Aching : LustCard
{
    public Aching() : base(0, CardType.Curse, CardRarity.Curse, TargetType.Self) { }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new HpLossVar(12m),
        new EnergyVar(2)
    ];

    public override List<(string, string)>? Localization => new CardLoc(
        "Aching",
        "Playable: gain {Energy:energyIcons()} this turn. Lose {HpLoss:diff()} HP.");

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Owner);
        await CreatureCmd.Damage(choiceContext, Owner.Creature, DynamicVars.HpLoss.BaseValue,
            ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, this);
    }
}
