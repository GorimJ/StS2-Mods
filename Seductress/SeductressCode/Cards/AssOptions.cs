using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using BaseLib.Utils;

namespace Seductress.SeductressCode.Cards;

/// <summary>
/// The two choices a spender card (Blowout, Overflow) offers when played.
///
/// [Gorim, 2026-11-22] Chose "Ask when played": Spend drains the bank for the big bonus, Keep
/// plays the card with only the standing bonus. [Claude] They are cards only because the game's
/// pick-one screen shows cards; they never enter a pile and do nothing if played. The amounts
/// are filled in by <see cref="SeductressCard.AssSpendOrKeep"/>.
///
/// [Gorim, 2026-11-24, seen in game] "you've put 'spend' in the normal skills pool. I got it from
/// a potion". [Claude] They sat in the Seductress pool, which the Skill/Attack potions and other
/// random card effects draw from (the game's combat filter skips Basic, Ancient and Event cards
/// but not Token). They now live in the game's own TokenCardPool, where the base game keeps its
/// Shiv, Soul and tarot cards; nothing in the base game draws random cards from that pool. They
/// also opt out of combat generation. Like the base game's tokens, they still show in the card
/// library under the Misc filter. "Spend" is now "Release": Gorim retired the word on 24 Nov.
/// </summary>
[Pool(typeof(TokenCardPool))]
public class AssSpendOption : SeductressCard
{
    public AssSpendOption() : base(0, CardType.Skill, CardRarity.Token, TargetType.Self) { }

    public override bool CanBeGeneratedInCombat => false;

    protected override IEnumerable<DynamicVar> CanonicalVars =>
        [new DynamicVar("Ml", 0m), new DynamicVar("Bonus", 0m)];

    public override List<(string, string)>? Localization => new CardLoc(
        "Release", "Lose {Ml} ml of Ass.\n+{Bonus} damage on every hit.");

    protected override Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay) => Task.CompletedTask;
}

/// <summary>See <see cref="AssSpendOption"/>.</summary>
[Pool(typeof(TokenCardPool))]
public class AssKeepOption : SeductressCard
{
    public AssKeepOption() : base(0, CardType.Skill, CardRarity.Token, TargetType.Self) { }

    public override bool CanBeGeneratedInCombat => false;

    protected override IEnumerable<DynamicVar> CanonicalVars => [new DynamicVar("Bonus", 0m)];

    public override List<(string, string)>? Localization => new CardLoc(
        "Keep", "Lose nothing.\n+{Bonus} damage on every hit.");

    protected override Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay) => Task.CompletedTask;
}
