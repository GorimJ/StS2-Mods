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
/// Squeezing's only defensive card, and deliberately single-hit — which inverts everything else
/// here. Block is granted <b>per hit</b>, so under Stretch My Hole Daddy the Block doubles too
/// (design §7, decided).
/// </summary>
public class Grind : SeductressCard
{
    public Grind() : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy) { }

    public override Hole Hole => Hole.Ass;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new CalculationBaseVar(6m),
        new ExtraDamageVar(1m),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier(BonusCalc),
        new HpLossVar(1m)
    ];


    /// <summary>The live Ass bonus, so the card face shows what it will actually deal.</summary>
    protected override decimal PreviewBonus() => AssNotional();

    public override List<(string, string)>? Localization => new CardLoc(
        "Grind", "Ass. Deal {CalculatedDamage:diff()} damage. Gain [gold]Block[/gold] equal to the damage dealt. Lose {HpLoss:diff()} HP.\n[i](+1 per 50 ml in your Ass)[/i]");

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);
        var bonus = StandingAssBonus();
        var damage = DynamicVars.CalculationBase.BaseValue + bonus;
        var hits = 1; // Stretch My Hole Daddy adds its hit through StretchPower

        for (var i = 0; i < hits; i++)
        {
            var attack = await DamageCmd.Attack(damage).FromCard(this, cardPlay).Targeting(cardPlay.Target)
                .WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);
            // [Gorim, 2026-11-23] "doesn't gain you block for the additional hits from Stretch my
            // hole." [Claude] Block is now the damage every hit of the command actually dealt
            // (Block and HP together), so extra hits from Stretch count. It was the planned
            // damage of one hit.
            var dealt = attack.Results.SelectMany(hit => hit).Sum(hit => hit.TotalDamage);
            if (dealt > 0) await CreatureCmd.GainBlock(Owner.Creature, dealt, ValueProp.Move, cardPlay);
        }
        await CreatureCmd.Damage(choiceContext, Owner.Creature, DynamicVars.HpLoss.BaseValue,
            ValueProp.Unblockable | ValueProp.Unpowered | ValueProp.Move, this, cardPlay);
    }

    protected override void OnUpgrade()
    {
        DynamicVars.CalculationBase.UpgradeValueBy(2m);
        DynamicVars.CalculatedDamage.RecalculateForUpgradeOrEnchant();
    }
}
