using System.Collections.Generic;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using Seductress.SeductressCode.Powers;

namespace Seductress.SeductressCode.Cards;

/// <summary>
/// Ancient.
///
/// [Gorim, 2026-11-20] Its old effect (attacks gain ml in the hole they used) is now the base
/// rule, so it is reworked: <b>whenever an Attack gains cum, gain that much again as precum, in
/// the same hole</b> ("Per attack, +100% cum as precum. Upgrade to cost less").
///
/// [Claude] This is what brings precum to the Ass and Womb. It behaves there as it does in the
/// Mouth: usable like cum for the rest of the combat, spent first, gone at the bell. It copies the
/// per-hit gain only, not the kill bonus, and does nothing for Skin, which is not a hole.
/// </summary>
public class EdgingForm : SeductressCard
{
    public EdgingForm() : base(1, CardType.Power, CardRarity.Ancient, TargetType.Self) { }

    public override List<(string, string)>? Localization => new CardLoc(
        "Edging Form", "Whenever an Attack gains cum, gain that much again as precum in the same hole.");

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await PowerCmd.Apply<EdgingFormPower>(choiceContext, Owner.Creature, 1m, Owner.Creature, this);
    }

    protected override void OnUpgrade() => EnergyCost.UpgradeBy(-1);
}
