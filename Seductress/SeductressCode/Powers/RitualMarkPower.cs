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

    /// <summary>
    /// [Gorim, 2026-11-23] Card notes: copies must stack, and the bonus no longer scales with
    /// Sigils or touches Sigil damage. Counter stacking adds each copy's amount, so it is flat.
    /// </summary>
    public decimal PussyBonus(MegaCrit.Sts2.Core.Entities.Players.Player player) =>
        Amount; // [Gorim, 2026-11-23] flat; each copy's amount stacks into Amount

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
}
