using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.CardSelection;
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
/// Freehand. The bridge card: a Skin attack that discounts your next Ass or Pussy card, so a
/// focused deck has a real reason to run one.
/// </summary>
public class Thighplay : SeductressCard
{
    public Thighplay() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }

    public override Hole Hole => Hole.Skin;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(9m, ValueProp.Move)];

    public override List<(string, string)>? Localization => new CardLoc(
        "Thighplay", "Skin. Deal {Damage:diff()} damage.\nYour next Ass or Pussy card costs 1 less.");

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);

        // "Next" is read as every eligible card in hand until one is played, which is how vanilla
        // one-shot discounts behave: SetUntilPlayed clears itself on the card that uses it.
        foreach (var card in PileType.Hand.GetPile(Owner).Cards.OfType<SeductressCard>())
        {
            if (card.Hole is Hole.Ass or Hole.Pussy) card.EnergyCost.AddUntilPlayed(-1, reduceOnly: true);
        }
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m);
}
