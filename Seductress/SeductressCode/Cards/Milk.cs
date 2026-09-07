using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Models;
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
/// [Gorim 2026-09-08] Tagged <b>Breasts · Skin</b>: the Breeding subcategory for cards that do not
/// feed the Womb, which is what <i>Ritual Mark</i> now buffs alongside Pussy.
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
/// [Gorim 2026-09-08] <b>Breasts is Skin</b>, the way Deep Throat is Mouth — a category tag paired
/// with a real hole. So Milk routes its kills to Skin and sets the Continuing mode to Skin when
/// played, and at Stage 3 that is three of them a turn dragging a Breeding deck off its own
/// routing. That is <b>intended</b>: multiple pregnancies are now possible, and each one should
/// make the next harder to reach.
///
/// [Claude-Code] <b>Written in vanilla words.</b> SEDUCTRESS_CARDS.md §9a states this card as
/// "Tease 4 pleasure. Soothe 2", which is the display lexicon from design §9a — but that converter
/// is explicitly not in this slice, and both docs' own naming rule says design and code keep the
/// vanilla words.
/// </summary>
public class Milk : SeductressCard
{
    public Milk() : base(0, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy) { }

    /// <summary>
    /// [Gorim 2026-09-08] <b>Breasts is Skin</b>, exactly as Deep Throat is Mouth: a category tag
    /// paired with a real hole rather than sitting outside the routing system. So this both routes
    /// its kills to Skin and sets the Continuing mode to Skin when played.
    ///
    /// [Gorim] Flipping a Breeding deck's routing away from Pussy while pregnant is <b>intended,
    /// and arguably the point</b> — you can now become pregnant several times over, and it should
    /// get harder the more pregnant you already are. Lactation actively fights the thing that
    /// caused it.
    /// </summary>
    public override Hole Hole => Hole.Skin;

    protected override IEnumerable<CardTag> ExtraTags => [SeductressTags.Breasts];

    /// <summary>
    /// The tier-2 rung, as an extra multiplier over the printed base: 4 heal normally, 4 + 1x3 = 7
    /// at three Sigils. Must be static — CalculatedVar rejects a calc whose target is a model.
    /// </summary>
    private static readonly Func<CardModel, Creature?, decimal> HealTierCalc =
        static (card, _) => card is SeductressCard { Owner: not null } s && Sigils.At(s.Owner, Sigils.Tier2)
            ? 3m
            : 0m;

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
        // The heal takes the generic calculated slot so its tier swap previews; the damage stays a
        // plain DamageVar, because Ritual Mark now previews itself through the vanilla damage hook.
        new CalculationBaseVar(4m),
        new CalculationExtraVar(1m),
        new CalculatedVar("CalculatedHeal").WithMultiplier(HealTierCalc),
        // Display only, so the tier line can state its number. Every token in a description must
        // resolve to a registered var — one unknown token makes SmartFormat print the whole
        // description raw, which is how this card's text broke a moment ago.
        new DynamicVar("TierHeal", 7m),
        new DamageVar(2m, ValueProp.Move)
    ];

    public override List<(string, string)>? Localization => new CardLoc(
        "Milk",
        "Skin. Breasts.\nHeal {CalculatedHeal:diff()}. Deal {Damage:diff()} damage.\n{T1} Sigil: draw 1.\n{T2} Sigils: heal {TierHeal} instead.\n{T3} Sigils: draw 2 instead.\n{T4} Sigils: does not Exhaust.");

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        // Heal first — it is what the card is for, and it is the order the card reads in.
        await CreatureCmd.Heal(Owner.Creature, ((CalculatedVar)DynamicVars["CalculatedHeal"]).Calculate(null));

        await DamageCmd.Attack(DynamicVars.Damage.BaseValue)
            .FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash").Execute(choiceContext);

        var draw = Sigils.At(Owner, Sigils.Tier3) ? 2 : Sigils.At(Owner, Sigils.Tier1) ? 1 : 0;
        if (draw > 0) await CardPileCmd.Draw(choiceContext, draw, Owner);
    }
}
