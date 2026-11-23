using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using Seductress.SeductressCode.Systems;

namespace Seductress.SeductressCode.Powers;

public class RitualMarkPower : SeductressPower
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override List<(string, string)>? Localization => new PowerLoc(
        "Ritual Mark", "Your Pussy and Breasts cards deal additional damage.", "Your Pussy and Breasts cards deal additional damage.");

    /// <summary>Tier 1 onwards: the Sigil passive also gains a bonus. Set by the card.</summary>
    public int SigilBonus { get; set; }

    /// <summary>
    /// Tier 4: both bonuses become +1 per Sigil instead of a flat amount. Set by the card.
    ///
    /// At 7 Sigils this is +7 against the +8 plain doubling would give, so it is a marginal
    /// downgrade at exactly the threshold that unlocks it and only pulls ahead from 8. Kept
    /// deliberately — converting a static buff into a scaling one is the fantasy, and 7 Sigils is
    /// already past the run target.
    /// </summary>
    public bool PerSigil { get; set; }

    /// <summary>
    /// [Gorim, 2026-11-23, card note] "Doesn't seem to stack correctly with multiple." [Claude]
    /// A second copy overwrote the Sigil bonus instead of adding to it, and added nothing at all
    /// once the bonus was per Sigil. Copies played are now counted and each one adds its share.
    /// </summary>
    public int Copies { get; set; }

    public decimal PussyBonus(MegaCrit.Sts2.Core.Entities.Players.Player player) =>
        PerSigil ? Sigils.Effective(player) * System.Math.Max(1, Copies) : Amount;

    /// <summary>
    /// [Claude-Code] The bonus rides the vanilla damage hook rather than being added by hand inside
    /// each card, which is what <see cref="MegaCrit.Sts2.Core.Models.Powers.StrengthPower"/> does.
    ///
    /// The payoff is that it <b>previews itself</b>: a plain DamageVar runs Hook.ModifyDamage when
    /// it builds the card's preview, so the buffed number appears on the card face with no
    /// calculated-var scaffolding at all. Adding it in the card body — which is how this worked
    /// until now — put it in the same invisible class as the Ass bonus that Gorim found in
    /// slice 3.
    ///
    /// Gated exactly like Strength: only when this power's owner is the dealer, and only on a
    /// powered attack, so it never touches the character's own self-damage.
    /// </summary>
    public override decimal ModifyDamageAdditive(Creature? target, decimal amount, ValueProp props,
        Creature? dealer, CardModel? cardSource, MegaCrit.Sts2.Core.Entities.Cards.CardPlay? cardPlay)
    {
        if (Owner != dealer) return 0m;
        if (!props.IsPoweredAttack()) return 0m;
        if (cardSource is not Cards.SeductressCard card) return 0m;
        if (!card.IsRitualMarked) return 0m;
        return Owner.Player is null ? 0m : PussyBonus(Owner.Player);
    }

    /// <summary>
    /// NOT YET LIVE: the Sigil passive (design §7.0, start-of-turn AoE) is out of scope, so there
    /// is nothing for this half to modify. Kept so the card is complete when the passive lands.
    /// </summary>
    public decimal SigilPassiveBonus(MegaCrit.Sts2.Core.Entities.Players.Player player) =>
        PerSigil ? Sigils.Effective(player) * System.Math.Max(1, Copies) : SigilBonus;
}
