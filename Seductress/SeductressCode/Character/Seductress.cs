using BaseLib.Abstracts;
using BaseLib.Utils.NodeFactories;
using Seductress.SeductressCode.Cards;
using Seductress.SeductressCode.Cards.Basics;
using Seductress.SeductressCode.Extensions;
using Seductress.SeductressCode.Relics;
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

    /// <summary>
    /// [Gorim] Eleven cards: 4 Pump, 4 Guard, 1 Let Go, plus two class cards (design §4).
    ///
    /// Two class cards rather than one, each demonstrating a different half of the mechanic —
    /// Breed Me! is the Pussy half and teaches intake, Suckle is the Mouth half. As previously
    /// designed the deck had no tagged card at all, so nothing routed anywhere until the first
    /// draft and the central mechanic was invisible for the opening floors.
    ///
    /// The slice-2 Take Him In test scaffold is gone; Breed Me! replaces it properly.
    /// </summary>
    public override IEnumerable<CardModel> StartingDeck =>
    [
        ModelDb.Card<Pump>(),
        ModelDb.Card<Pump>(),
        ModelDb.Card<Pump>(),
        ModelDb.Card<Pump>(),
        ModelDb.Card<Guard>(),
        ModelDb.Card<Guard>(),
        ModelDb.Card<Guard>(),
        ModelDb.Card<Guard>(),
        ModelDb.Card<LetGo>(),
        ModelDb.Card<BreedMe>(),
        ModelDb.Card<Suckle>()
    ];

    /// <summary>
    /// Burning Blood stands in for the character's own starting relic, which is not designed yet.
    /// Fertile Sign is TEST SCAFFOLDING and comes out with Conception — see SigilScaffold.
    /// </summary>
    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<BurningBlood>(),
        ModelDb.Relic<SigilScaffold>()
    ];

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
