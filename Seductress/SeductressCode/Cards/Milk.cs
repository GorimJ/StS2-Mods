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
/// [Gorim 2026-09-08] <b>It is a heal card first and an attack second</b>, so the numbers are
/// inverted from the card list's 4 damage / 2 heal: it heals 4 and deals 2, and the 3-Sigil tier
/// grows the <b>healing</b> rather than the damage. The tier shape is unchanged — rung 2 is still
/// "grows the base" — it is the base that moved.
///
/// [Gorim 2026-09-08] Tagged <b>Breasts</b>: the Breeding subcategory for cards that do not feed
/// the Womb, which is what <i>Ritual Mark</i> now buffs alongside Pussy.
///
/// [Claude-Cowork] <b>It heals; it must not Block.</b> That distinction is load-bearing. Healing
/// cannot prevent a one-shot, so at Stage 3 — where you take +150% — you can top up to full every
/// turn and still be raising the chance a single enemy intent kills you outright. The carried
/// state is a rising probability of sudden death rather than a grind, which Block would have made
/// it instead. [Claude-Code] Inverting the numbers sharpens that rather than softening it: more
/// sustain, less output, and still nothing that prevents a big hit.
///
/// [Claude-Cowork] It rides the <b>standard Sigil ladder</b> rather than inventing a scale, and
/// the 0-cost tier-4 rule lands unusually well: a 0-cost card sheds its non-energy cost at 7
/// Sigils, and Milk's non-energy cost is Exhaust. So at the top of the ladder Milk stops
/// exhausting and accumulates, from a rule that already existed.
///
/// [Claude-Code] <b>Untagged for routing</b>, like <i>Spit</i>. Breasts is a category, not a hole,
/// so this neither fills anything nor sets the Continuing mode — which matters, because at Stage 3
/// three of these arrive per turn and a hole tag would flip a Breeding deck's routing away from
/// Pussy several times a turn. A kill it lands routes by whatever the Continuing mode already is.
///
/// [Claude-Code] <b>Written in vanilla words.</b> SEDUCTRESS_CARDS.md §9a states this card as
/// "Tease 4 pleasure. Soothe 2", which is the display lexicon from design §9a — but that converter
/// is explicitly not in this slice, and both docs' own naming rule says design and code keep the
/// vanilla words.
/// </summary>
public class Milk : SeductressCard
{
    public Milk() : base(0, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy) { }

    protected override IEnumerable<CardTag> ExtraTags => [SeductressTags.Breasts];

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
        new HealVar(4m),
        new DynamicVar("TierHeal", 7m),
        new CalculationBaseVar(2m),
        new ExtraDamageVar(1m),
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier(BonusCalc)
    ];

    /// <summary>
    /// Ritual Mark, live on the card face. Breasts is buffed alongside Pussy, so this card's damage
    /// moves with the Power even though it carries no hole.
    ///
    /// [Claude-Code] The tier-2 swap is deliberately <b>not</b> in here, because the swap now lands
    /// on the healing and there is no calculated-heal var to hang it on. So "heal {TierHeal}
    /// instead" is documentation on the tier line rather than a number the top line updates to —
    /// <b>the one place left in the pool where a tier swap does not preview.</b> Damage previews
    /// correctly; healing does not.
    /// </summary>
    protected override decimal PreviewBonus() => RitualMarkBonus;

    public override List<(string, string)>? Localization => new CardLoc(
        "Milk",
        "Breasts. Heal {Heal:diff()}. Deal {CalculatedDamage:diff()} damage.\n{T1} Sigil: draw 1.\n{T2} Sigils: heal {TierHeal} instead.\n{T3} Sigils: draw 2 instead.\n{T4} Sigils: does not Exhaust.");

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        // Heal first — it is what the card is for, and it is the order the card reads in.
        var heal = Sigils.At(Owner, Sigils.Tier2)
            ? DynamicVars["TierHeal"].BaseValue
            : DynamicVars.Heal.BaseValue;
        await CreatureCmd.Heal(Owner.Creature, heal);

        await DamageCmd.Attack(DynamicVars.CalculatedDamage.Calculate(cardPlay.Target))
            .FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);

        var draw = Sigils.At(Owner, Sigils.Tier3) ? 2 : Sigils.At(Owner, Sigils.Tier1) ? 1 : 0;
        if (draw > 0) await CardPileCmd.Draw(choiceContext, draw, Owner);
    }
}
