using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using Seductress.SeductressCode.Powers;
using Seductress.SeductressCode.Systems;

namespace Seductress.SeductressCode.Cards;

/// <summary>Draining Power. Names a family rather than a card, which is what makes the tag pay.</summary>
public class NoGagReflex : SeductressCard
{
    private bool _upgraded;

    public NoGagReflex() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self) { }

    public override List<(string, string)>? Localization => new CardLoc(
        "No Gag Reflex", "Deep Throat cards cost 1 less and no longer lose HP.{IfUpgraded:show:\nWhenever you play one, draw 1.|}");

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var power = await PowerCmd.Apply<NoGagReflexPower>(
            choiceContext, Owner.Creature, 1m, Owner.Creature, this);
        if (power != null && _upgraded) power.DeepThroatDraws++; // [Gorim, 2026-11-23] once per copy
    }

    protected override void OnUpgrade() => _upgraded = true;
}
