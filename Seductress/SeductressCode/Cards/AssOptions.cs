using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;

namespace Seductress.SeductressCode.Cards;

/// <summary>
/// The two choices a spender card (Blowout, Overflow) offers when played.
///
/// [Gorim, 2026-11-22] Chose "Ask when played": Spend drains the bank for the big bonus, Keep
/// plays the card with only the standing bonus. [Claude] They are cards only because the game's
/// pick-one screen shows cards; they never enter a pile and do nothing if played. The amounts
/// are filled in by <see cref="SeductressCard.AssSpendOrKeep"/>.
/// </summary>
public class AssSpendOption : SeductressCard
{
    public AssSpendOption() : base(0, CardType.Skill, CardRarity.Token, TargetType.Self) { }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("Ml", 0m), new DynamicVar("Bonus", 0m)];

    public override List<(string, string)>? Localization => new CardLoc(
        "Spend", "Lose {Ml} ml of Ass.\n+{Bonus} damage on every hit.");

    protected override Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay) => Task.CompletedTask;
}

/// <summary>See <see cref="AssSpendOption"/>.</summary>
public class AssKeepOption : SeductressCard
{
    public AssKeepOption() : base(0, CardType.Skill, CardRarity.Token, TargetType.Self) { }

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Bonus", 0m)];

    public override List<(string, string)>? Localization => new CardLoc(
        "Keep", "Lose nothing.\n+{Bonus} damage on every hit.");

    protected override Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay) => Task.CompletedTask;
}
