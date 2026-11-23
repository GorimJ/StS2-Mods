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

    /// <summary>
    /// [Gorim, 2026-11-13] Her own static render as the combat body, in a scene this mod owns.
    ///
    /// The placeholder default is the vanilla Ironclad scene by path, and a skin mod that replaces
    /// that path (SkinChanger + an Ironclad skin) left her with no body at all. The rest site,
    /// merchant and character-select visuals still borrow the Ironclad and can break the same way.
    /// Still image only — the rigged model replaces this scene when it exists.
    /// </summary>
    public override string CustomVisualPath => $"{MainFile.ResPath}/scenes/seductress_visuals.tscn";

    public override Color NameColor => Color;
    public override CharacterGender Gender => CharacterGender.Feminine;

    /// <summary>
    /// [Gorim] <b>Max HP is 20</b>, down from 65 (design §6.5).
    ///
    /// The bar is one life of four: 20 x 4 climaxes is ~80 effective, which is Ironclad's pool.
    /// Within a single fight it is less, because Sensitive compounds — 20 + 13 + 10 + 8 is ~51 —
    /// and that second number is the one that matters in a boss fight.
    ///
    /// [Gorim] <b>The character is not more fragile, she is more granular.</b> A Jaw Worm hitting
    /// for 11 takes the same 14% of an 80 HP pool it takes from Ironclad. What changes is the
    /// rhythm: the same total absorbed in four chunks with a Lust card owed at each boundary.
    ///
    /// [Gorim] <b>Only Max HP is worth 4x</b> — it is the only thing refilled four times. Block,
    /// healing and damage are absolute and unchanged; HP <i>costs</i> scale inversely, which is
    /// what §6.5's rescale table does.
    /// </summary>
    public override int StartingHp => 20;

    /// <summary>
    /// [Gorim] Eleven cards: 4 More, 4 Guard, 1 Seek Relief, plus two class cards (design §4).
    ///
    /// Two class cards rather than one, each demonstrating a different half of the mechanic —
    /// Breed Me! is the Pussy half and teaches intake, Suckle is the Mouth half. As previously
    /// designed the deck had no tagged card at all, so nothing routed anywhere until the first
    /// draft and the central mechanic was invisible for the opening floors.
    ///
    /// [Gorim] <i>Let Go</i> is <b>renamed Seek Relief</b> and stays in the deck, so the
    /// self-damage axis is still taught by a starter. The slice-3 arrangement — where a card I had
    /// called Seek Relief took Let Go's slot — is superseded: that card is now <i>Forced Orgasm</i>,
    /// a draftable Uncommon, and the starting deck keeps its original shape.
    ///
    /// The slice-2 Take Him In test scaffold is gone; Breed Me! replaces it properly.
    /// </summary>
    public override IEnumerable<CardModel> StartingDeck =>
    [
        ModelDb.Card<More>(),
        ModelDb.Card<More>(),
        ModelDb.Card<More>(),
        ModelDb.Card<More>(),
        ModelDb.Card<Guard>(),
        ModelDb.Card<Guard>(),
        ModelDb.Card<Guard>(),
        ModelDb.Card<Guard>(),
        ModelDb.Card<SeekRelief>(),
        ModelDb.Card<BreedMe>(),
        ModelDb.Card<Suckle>()
    ];

    /// <summary>
    /// Burning Blood stands in for the character's own starting relic, which is not designed yet.
    ///
    /// [Claude-Code] <b>Fertile Sign is gone.</b> Its own removal condition was "delete when
    /// Conception lands", and Conception has landed — but the reason to act on that now rather
    /// than later is measurement: a free Sigil from floor 1 turns on tier 1 across the whole
    /// Breeding deck before a single pregnancy, which is exactly the number this slice is meant to
    /// report. The class and the <c>sigils</c> console command are kept for setting a count on
    /// purpose.
    ///
    /// [Claude-Code] <b>Watch Burning Blood.</b> It heals 6 after every combat, which was 9% of a
    /// 65 HP bar and is <b>30%</b> of a 20 HP one. Healing is absolute and unchanged by decision
    /// (§6.5), so this is per the design rather than a bug — but it is now a very large per-combat
    /// heal on a character whose whole tension is arousal accumulating between fights, and it
    /// partly undoes what removing HEAL from the rest site was for.
    /// </summary>
    public override IReadOnlyList<RelicModel> StartingRelics =>
    [
        ModelDb.Relic<BurningBlood>()
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
    /// <summary>
    /// [Claude, 2026-11-23] Her own character-select splash. Without this, PlaceholderCharacterModel
    /// fell back to the Ironclad's. The scene is one TextureRect that covers the screen and keeps
    /// the picture's shape; the art is images/char_select/char_select_bg_seductress.png
    /// (1920 x 1080, a placeholder until Gorim's splash replaces it).
    /// </summary>
    public override string CustomCharacterSelectBg => $"{MainFile.ResPath}/scenes/char_select_bg_seductress.tscn";

    public override string CustomMapMarkerPath => "map_marker_char_name.png".CharacterUiPath();
}
