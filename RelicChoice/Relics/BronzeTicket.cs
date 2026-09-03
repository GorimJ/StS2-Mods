using BaseLib.Utils;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Models.RelicPools;

namespace RelicChoice.Relics;

[Pool(typeof(SharedRelicPool))]
public class BronzeTicket : TicketRelic
{
    public override RelicRarity Rarity => RelicRarity.Common;
    protected override int Index => 0;
    public override string PackedIconPath => "res://images/atlases/relic_atlas.sprites/relicchoice-bronze_ticket.tres";
    protected override string PackedIconOutlinePath => "res://images/atlases/relic_outline_atlas.sprites/relicchoice-bronze_ticket.tres";
    protected override string BigIconPath => "res://images/Bronze_Ticket.png";
}
