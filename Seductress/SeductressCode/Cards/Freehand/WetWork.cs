using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Seductress.SeductressCode.Systems;

namespace Seductress.SeductressCode.Cards;

/// <summary>
/// Freehand Uncommon. The Skin payoff.
///
/// [Gorim, 2026-11-24] Asked about his note ("I think this is just pointless? ... the word
/// 'spend' should be retired"): "1 block and 1 vigor per 10ml." [Claude] Per 10 ml on your Skin,
/// gain 1 Block and 1 Vigor. Nothing is used up and the holes are no longer touched; the old
/// spend order (Skin, Mouth, Ass, Womb) is gone from this card. Lexicon note for later, not done
/// here: Vigor is to be called "Lubrication".
///
/// [Claude] Unplayable under 10 ml of Skin, so it cannot be a dead click. [Gorim] Upgraded, it
/// gains Retain and nothing else.
///
/// [Claude-Code] Untagged: a Skill, so never tagged, and it fills nothing.
/// </summary>
public class WetWork : SeductressCard
{
    public WetWork() : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    /// <summary>Ml of Skin per 1 Block and 1 Vigor.</summary>
    public const int MlPerPoint = 10;

    public override List<(string, string)>? Localization => new CardLoc(
        "Wet Work",
        "Gain {SkinBlock} [gold]Block[/gold] and {SkinVigor} [gold]Vigor[/gold].\n[i](10% of your cum on Skin)[/i]");

    // [Gorim, 2026-11-24] "Also we should make wetwork exhaust". Upgraded keeps Exhaust and adds Retain.
    public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

    protected override IEnumerable<IHoverTip> ExtraHoverTips =>
        base.ExtraHoverTips.Append(HoverTipFactory.FromPower<VigorPower>());

    /// <summary>Block and Vigor this card gives right now.</summary>
    private int Points => Owner is null ? 0 : Reservoirs.Of(Owner.RunState).Skin / MlPerPoint;

    // Card text gives the number, not the formula.
    protected override void AddExtraArgsToDescription(MegaCrit.Sts2.Core.Localization.LocString description)
    {
        base.AddExtraArgsToDescription(description);
        description.Add("SkinBlock", (decimal)Points);
        description.Add("SkinVigor", (decimal)Points);
    }

    protected override bool IsPlayable => Points > 0;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var points = Points;
        MainFile.Logger.Info(
            $"[Seductress] Wet Work: +{points} Block, +{points} Vigor from {Reservoirs.Of(Owner.RunState).Skin} ml Skin");
        if (points <= 0) return;
        await CreatureCmd.GainBlock(Owner.Creature, points, ValueProp.Move, cardPlay);
        await PowerCmd.Apply<VigorPower>(choiceContext, Owner.Creature, points, Owner.Creature, this);
    }

    // [Gorim, 2026-11-24] Upgrade: Retain (picked from Retain / 1 per 8 ml / Draw 1).
    protected override void OnUpgrade() => AddKeyword(CardKeyword.Retain);
}
