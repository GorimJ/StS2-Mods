using System;
using System.Collections.Generic;
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
/// Generated while Lactating — N shuffled into the <b>combat</b> draw pile each turn, where N is
/// the pregnancy stage. Never enters the master deck, so nothing persists between fights.
///
/// [Claude-Cowork] <b>It heals; it must not Block.</b> That distinction is load-bearing. Healing
/// cannot prevent a one-shot, so at Stage 3 — where you take +150% — you can top up to full every
/// turn and still be raising the chance a single enemy intent kills you outright. The carried
/// state is a rising probability of sudden death rather than a grind, which Block would have made
/// it instead.
///
/// [Claude-Cowork] It rides the <b>standard Sigil ladder</b> rather than inventing a scale, and
/// the 0-cost tier-4 rule lands unusually well: a 0-cost card sheds its non-energy cost at 7
/// Sigils, and Milk's non-energy cost is Exhaust. So at the top of the ladder Milk stops
/// exhausting and accumulates, from a rule that already existed.
///
/// [Claude-Code] Untagged, like <i>Spit</i>. The design gives it no hole, so a kill it lands
/// routes by the Continuing mode rather than setting one.
///
/// [Claude-Code] <b>Written in vanilla words.</b> SEDUCTRESS_CARDS.md §9a states this card as
/// "Tease 4 pleasure. Soothe 2", which is the display lexicon from design §9a — but that converter
/// is explicitly not in this slice, and both docs' own naming rule says design and code keep the
/// vanilla words. Rendering it literally would have printed lexicon text with nothing else
/// converted around it.
/// </summary>
public class Milk : SeductressCard
{
    public Milk() : base(0, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy) { }

    /// <summary>
    /// Tier 4 sheds the non-energy cost, which here is Exhaust.
    ///
    /// [Claude-Code] Declared dynamically rather than removed at play time so the card <i>reads</i>
    /// correctly in hand. CardModel caches its keyword set on first access, and every Milk is a
    /// fresh instance created that turn, so the tier is read after the card exists — the one gap
    /// is a Sigil exhausted by <i>Consummate</i> mid-combat, which will not retroactively put
    /// Exhaust back on Milk already in the pile.
    /// </summary>
    public override IEnumerable<CardKeyword> CanonicalKeywords =>
        Owner is not null && Sigils.At(Owner, Sigils.Tier4)
            ? []
            : [CardKeyword.Exhaust];

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("T1", Sigils.Tier1),
        new DynamicVar("T2", Sigils.Tier2),
        new DynamicVar("T3", Sigils.Tier3),
        new DynamicVar("T4", Sigils.Tier4),
        new DamageVar(4m, ValueProp.Move),
        new DynamicVar("TierDamage", 7m),
        new HealVar(2m)
    ];

    public override List<(string, string)>? Localization => new CardLoc(
        "Milk",
        "Deal {Damage:diff()} damage. Heal {Heal:diff()}.\n{T1} Sigil: draw 1.\n{T2} Sigils: deal {TierDamage:diff()} instead.\n{T3} Sigils: draw 2 instead.\n{T4} Sigils: does not Exhaust.");

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        var damage = Sigils.At(Owner, Sigils.Tier2)
            ? DynamicVars["TierDamage"].BaseValue
            : DynamicVars.Damage.BaseValue;

        await DamageCmd.Attack(damage).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);

        await CreatureCmd.Heal(Owner.Creature, DynamicVars.Heal.BaseValue);

        var draw = Sigils.At(Owner, Sigils.Tier3) ? 2 : Sigils.At(Owner, Sigils.Tier1) ? 1 : 0;
        if (draw > 0) await CardPileCmd.Draw(choiceContext, draw, Owner);
    }
}
