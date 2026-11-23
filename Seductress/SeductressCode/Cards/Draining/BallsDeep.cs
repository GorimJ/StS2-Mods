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

/// <summary>Draining, Deep Throat family.</summary>
public class BallsDeep : SeductressCard
{
    public BallsDeep() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }

    public override Hole Hole => Hole.Mouth;
    protected override IEnumerable<CardTag> ExtraTags => [SeductressTags.DeepThroat];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(9m, ValueProp.Move),
        new HpLossVar(2m)
    ];

    public override List<(string, string)>? Localization => new CardLoc(
        "Balls Deep", "Mouth. Deep Throat.\nDeal {Damage:diff()} damage. Lose {HpLoss:diff()} HP.\nPut a card from your discard pile on top of your draw pile. If it is a different Mouth card, put it into your hand instead.",
        // [Claude, 2026-11-23] The card picker needs this line; without it the card threw on play.
        ("selectionScreenPrompt", "Choose a card to take back."));

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var hits = 1 + StretchBonusHits(true); // [Gorim, 2026-11-23] counts even when No Gag Reflex removes the HP loss
        for (var i = 0; i < hits; i++)
        {
            await DamageCmd.Attack(DynamicVars.Damage.BaseValue).FromCard(this, cardPlay).Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
        }
        if (!SuppressHpLoss)
        {
            await CreatureCmd.Damage(choiceContext, Owner.Creature, DynamicVars.HpLoss.BaseValue,
                ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, this, cardPlay);
        }
        if (Owner.Creature.GetPower<NoGagReflexPower>() is { DrawsOnDeepThroat: true })
        {
            await CardPileCmd.Draw(choiceContext, 1m, Owner);
        }
        var chosen = (await CardSelectCmd.FromCombatPile(
            context: choiceContext, pile: PileType.Discard.GetPile(Owner), player: Owner,
            prefs: new CardSelectorPrefs(SelectionScreenPrompt, 1))).FirstOrDefault();
        if (chosen != null)
        {
            // "Different" is what stops this becoming a soft infinite once No Gag Reflex has zeroed
            // the cost and the HP: retrieving Balls Deep itself only ever reaches the draw pile.
            var differentMouthCard = chosen is SeductressCard { Hole: Hole.Mouth }
                                     && chosen.Id.Entry != Id.Entry;
            await CardPileCmd.Add(chosen, differentMouthCard ? PileType.Hand : PileType.Draw,
                CardPilePosition.Top);
        }
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(3m);
}
