using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace RelicChoice.Relics;

[Pool(typeof(SharedRelicPool))]
public class ShinyGoldenTicket : TicketRelic
{
    public override RelicRarity Rarity => RelicRarity.Rare;
    protected override int Index => 1;
    public override string PackedIconPath => "res://images/atlases/relic_atlas.sprites/relicchoice-shiny_golden_ticket.tres";
    protected override string PackedIconOutlinePath => "res://images/atlases/relic_outline_atlas.sprites/relicchoice-shiny_golden_ticket.tres";
    protected override string BigIconPath => "res://images/Shiny_Golden_Ticket.png";
}
