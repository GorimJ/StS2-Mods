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
public class DeepThroat : SeductressCard
{
    public DeepThroat() : base(2, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) { }

    public override Hole Hole => Hole.Mouth;
    protected override IEnumerable<CardTag> ExtraTags => [SeductressTags.DeepThroat];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DamageVar(16m, ValueProp.Move),
        new HpLossVar(2m)
    ];

    public override List<(string, string)>? Localization => new CardLoc(
        "Deep Throat", "Mouth. Deep Throat.\nDeal {Damage:diff()} damage. Lose {HpLoss:diff()} HP. Draw 1.");

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
        await CardPileCmd.Draw(choiceContext, 1m, Owner);
    }

    protected override void OnUpgrade() => DynamicVars.Damage.UpgradeValueBy(4m);
}
