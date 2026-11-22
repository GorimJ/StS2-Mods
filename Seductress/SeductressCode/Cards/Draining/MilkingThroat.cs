using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;
using Seductress.SeductressCode.Powers;
using Seductress.SeductressCode.Systems;

namespace Seductress.SeductressCode.Cards;

/// <summary>
/// Draining Uncommon Power. [Gorim, 2026-11-17, reworking the Throat Training draft] "Whenever you
/// gain precum, gain 3 more and eat it."
///
/// [Gorim, 2026-11-21] "Pretty sure it's meant to eat the whole lot of precum. Honestly should
/// probably change it to 'gulp', for a full mouth eat." So: whenever you gain precum, Gulp, which
/// swallows everything in the Mouth, cum and precum, healing at the swallow rate.
///
/// [Gorim, 2026-11-21, later] "Milking throat still needs the bonus precum. Probably gain twice
/// as much, then gulp. Have Gulp as a pop-out reminder rather than on the card."
///
/// [Claude] Triggers on card and power gains, not on the Stomach's combat-start precum. With no
/// ml amount to raise, the upgrade is cost 0; that is my choice.
/// </summary>
public class MilkingThroat : SeductressCard
{
    public MilkingThroat() : base(1, CardType.Power, CardRarity.Uncommon, TargetType.Self) { }

    public override List<(string, string)>? Localization => new CardLoc(
        "Milking Throat", "Whenever you gain precum, gain twice as much, then [gold]Gulp[/gold].");

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<MilkingThroatPower>(
            choiceContext, Owner.Creature, 1m, Owner.Creature, this);
    }

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}
