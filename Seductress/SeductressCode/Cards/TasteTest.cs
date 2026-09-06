using System;
using System.Collections.Generic;
using System.Linq;
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
/// SPIKE ONLY — not a design card. This is the deliverable for question 3: a card that reads
/// "is the last attack played tagged Mouth?" and answers.
///
/// It asks the question the way a real card would have to: it looks up the last Attack played
/// this combat and tests the vanilla <c>Tags</c> collection for a mod-defined member.
/// </summary>
public class TasteTest : SeductressCard
{
    public TasteTest() : base(0, CardType.Attack, CardRarity.Token, TargetType.AnyEnemy) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DamageVar(4m, ValueProp.Move)];

    public override List<(string, string)>? Localization => new CardLoc(
        "Taste Test",
        "Deal {Damage} damage. Deal 10 more if your last Attack was tagged Mouth.");

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        var lastAttack = ClimaxModel.Instance.LastAttackPlayed;
        var tags = lastAttack?.Tags.ToList() ?? [];
        var wasMouth = tags.Contains(SeductressTags.Mouth);

        MainFile.Logger.Info(
            $"[Seductress] TasteTest: last attack = {lastAttack?.Id.Entry ?? "(none)"}, " +
            $"tags = [{string.Join(", ", tags.Select(t => (int)t))}], Mouth? {wasMouth}");

        var damage = DynamicVars.Damage.BaseValue + (wasMouth ? 10m : 0m);
        await DamageCmd.Attack(damage).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }
}
