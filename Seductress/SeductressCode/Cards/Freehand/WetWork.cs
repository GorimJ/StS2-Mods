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
/// Freehand Uncommon. The generic ml spender, and the card the universal spend order exists for.
///
/// [Claude-Cowork] <b>Deliberately cheap and deliberately dangerous.</b> Spending runs
/// Skin -> Mouth -> Ass -> Womb, least valuable first, so this is nearly free for a Skin-heavy
/// deck and genuinely costly for a focused one: play it with a clean skin and an empty mouth and
/// it eats your Ass, or your Conception. That self-limits without a restriction clause — it reads
/// as free utility and quietly costs the most to exactly the decks that can least afford it.
///
/// [Claude-Code] Untagged: a Skill, so never tagged, and it fills nothing.
/// </summary>
public class WetWork : SeductressCard
{
    public WetWork() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    protected override IEnumerable<DynamicVar> CanonicalVars =>
    [
        new DynamicVar("Ml", 10m),
        new BlockVar(10m, ValueProp.Move)
    ];

    public override List<(string, string)>? Localization => new CardLoc(
        "Wet Work",
        "Spend {Ml:diff()} ml. Gain {Block:diff()} [gold]Block[/gold].\nSpending takes from your Skin, then Mouth, then Ass, then Womb.");

    /// <summary>Unplayable with nothing to spend, so it cannot be a dead click.</summary>
    protected override bool IsPlayable =>
        Reservoirs.Of(Owner.RunState) is { } r &&
        r.Skin + r.MouthTotal + r.Ass + r.Womb >= (int)DynamicVars["Ml"].BaseValue;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var r = Reservoirs.Of(Owner.RunState);
        var spent = r.SpendMl((int)DynamicVars["Ml"].BaseValue);
        MainFile.Logger.Info(
            $"[Seductress] Wet Work: spent {spent} ml | skin {r.Skin} mouth {r.Mouth}+{r.Precum}p ass {r.Ass} womb {r.Womb}");
        await CreatureCmd.GainBlock(Owner.Creature, DynamicVars.Block, cardPlay);
    }

    protected override void OnUpgrade() => DynamicVars["Ml"].UpgradeValueBy(-2m);
}
