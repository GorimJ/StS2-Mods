using BaseLib.Abstracts;
using BaseLib.Utils.NodeFactories;
using Seductress.SeductressCode.Cards;
using Seductress.SeductressCode.Cards.Basics;
using Seductress.SeductressCode.Extensions;
using Godot;
using MegaCrit.Sts2.Core.Entities.Characters;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Relics;

namespace Seductress.SeductressCode.Character;

/// <summary>
/// Spike scaffold. Uses the template's placeholder art deliberately — this mod stays asset-free
/// so the code can live in git (see SEDUCTRESS_SPIKE_BRIEF.md).
/// </summary>
public class Seductress : PlaceholderCharacterModel
{
    public const string CharacterId = "Seductress";

    public static readonly Color Color = new("d64f7e");

    public override Color NameColor => Color;
    public override CharacterGender Gender => CharacterGender.Feminine;

    /// <summary>65 proposed in the design (Ironclad is 80); not yet confirmed.</summary>
    public override int StartingHp => 65;

    public override IEnumerable<CardModel> StartingDeck =>
    [
        ModelDb.Card<Pump>(),
        ModelDb.Card<Pump>(),
        ModelDb.Card<Pump>(),
        ModelDb.Card<Pump>(),
        ModelDb.Card<Pump>(),
        ModelDb.Card<Guard>(),
        ModelDb.Card<Guard>(),
        ModelDb.Card<Guard>(),
        ModelDb.Card<Guard>(),
        ModelDb.Card<LetGo>()
    ];

    // Starting relic is out of spike scope; borrowing the Ironclad's keeps the run bootable.
    public override IReadOnlyList<RelicModel> StartingRelics => [ModelDb.Relic<BurningBlood>()];

    public override CardPoolModel CardPool => ModelDb.CardPool<SeductressCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<SeductressRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<SeductressPotionPool>();

    public override Control CustomIcon
    {
        get
        {
            var icon = NodeFactory<Control>.CreateFromResource(CustomIconTexturePath);
            icon.SetAnchorsAndOffsetsPreset(Control.LayoutPreset.FullRect);
            return icon;
        }
    }

    public override string CustomIconTexturePath => "character_icon_char_name.png".CharacterUiPath();
    public override string CustomCharacterSelectIconPath => "char_select_char_name.png".CharacterUiPath();
    public override string CustomCharacterSelectLockedIconPath => "char_select_char_name_locked.png".CharacterUiPath();
    public override string CustomMapMarkerPath => "map_marker_char_name.png".CharacterUiPath();
}
