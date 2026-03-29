using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BaseLib.Abstracts;
using BaseLib.Utils;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.Models.RelicPools;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Models.Enchantments;
using MegaCrit.Sts2.Core.Nodes;
using MegaCrit.Sts2.Core.Nodes.GodotExtensions;
using MegaCrit.Sts2.Core.Nodes.Vfx;

namespace GachaShopMod.Relics;

[Pool(typeof(SharedRelicPool))]
public class CorruptedGachaBallRelic : CustomRelicModel
{
    public override RelicRarity Rarity => RelicRarity.Event;
    public override bool HasUponPickupEffect => true;
    public override string PackedIconPath => "res://Images/CorruptedGachaBall.png";
    protected override string BigIconPath => "res://Images/CorruptedGachaBall.png";
    protected override string PackedIconOutlinePath => "res://Images/CorruptedGachaBall.png";
    
    public CorruptedGachaBallRelic() : base(true) { }

    public override async Task AfterObtained()
    {
        EnchantmentModel enchantment = ModelDb.Enchantment<Corrupted>();
        List<CardModel> cards = PileType.Deck.GetPile(base.Owner).Cards.Where((CardModel c) => 
            enchantment.CanEnchant(c) && c.Type != CardType.Skill && c.Type != CardType.Power).ToList();
            
        CardModel chosenCard = (await CardSelectCmd.FromDeckForEnchantment(
            prefs: new CardSelectorPrefs(CardSelectorPrefs.EnchantSelectionPrompt, 1),
            cards: cards,
            enchantment: enchantment,
            amount: 1
        )).FirstOrDefault();

        if (chosenCard != null)
        {
            CardCmd.Enchant<Corrupted>(chosenCard, 1m);
            NCardEnchantVfx nCardEnchantVfx = NCardEnchantVfx.Create(chosenCard);
            if (nCardEnchantVfx != null)
            {
                NRun.Instance?.GlobalUi.CardPreviewContainer.AddChild(nCardEnchantVfx);
            }
        }
    }
}
