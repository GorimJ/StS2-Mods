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

/// <summary>
/// Draining. The seam between Freehand and Draining: Glazed collects nothing on its own, and this
/// is the one card that makes it collect after the fact. A transfer, so what it moves is real cum.
/// </summary>
public class ScoopItUp : SeductressCard
{
    public ScoopItUp() : base(1, CardType.Skill, CardRarity.Uncommon, TargetType.Self) { }

    public override List<(string, string)>? Localization => new CardLoc(
        "Scoop It Up", "Empty your Glazed. Gain that much Mouth.");

    protected override Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var r = Reservoirs.Of(Owner.RunState);
        var moved = r.Glazed;
        r.Glazed = 0;
        r.Add(Hole.Mouth, moved);
        MainFile.Logger.Info($"[Seductress] Scoop It Up moved {moved} ml Glazed -> Mouth (mouth now {r.Mouth})");
        return Task.CompletedTask;
    }

    protected override void OnUpgrade() => EnergyCost.SetCustomBaseCost(0);
}
