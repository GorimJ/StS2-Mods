using System.Collections.Generic;
using BaseLib.Abstracts;
using Godot;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Entities.Characters;
using BuxomModPort.Code.Cards;
using BuxomModPort.Code.Relics;

namespace BuxomModPort.Code.Character;

public class TheBuxom : PlaceholderCharacterModel
{
    public const string CharacterId = "TheBuxom";
    public override int StartingHp => 75;
    public override Color NameColor => new Color("FFB6C1");
    public override CharacterGender Gender => CharacterGender.Feminine;

    public override CardPoolModel CardPool => ModelDb.CardPool<BuxomCardPool>();
    public override RelicPoolModel RelicPool => ModelDb.RelicPool<BuxomRelicPool>();
    public override PotionPoolModel PotionPool => ModelDb.PotionPool<BuxomPotionPool>();

    public override IEnumerable<CardModel> StartingDeck => new List<CardModel>
    {
        ModelDb.Card<BuxomStrike>(), ModelDb.Card<BuxomStrike>(), ModelDb.Card<BuxomStrike>(), ModelDb.Card<BuxomStrike>(),
        ModelDb.Card<BuxomDefend>(), ModelDb.Card<BuxomDefend>(), ModelDb.Card<BuxomDefend>(), ModelDb.Card<BuxomDefend>(),
        ModelDb.Card<BouncyExercise>(), ModelDb.Card<Omegabsorption>()
    };

    public override IReadOnlyList<RelicModel> StartingRelics => new List<RelicModel>
    {
        ModelDb.Relic<BuxomStarterRelic>()
    };

    public override List<string> GetArchitectAttackVfx() => new List<string> { "vfx/vfx_attack_blunt" };

    public override string CustomCharacterSelectIconPath => "res://images/character/TheBuxom_Button.png";
    public override string CustomCharacterSelectLockedIconPath => "res://images/character/TheBuxom_Button.png";
    public override string CustomIconTexturePath => "res://images/character/TheBuxom_Button.png";
    public override string CustomMapMarkerPath => "res://images/character/TheBuxom_Button.png";
    public override string CustomCharacterSelectBg => "res://images/character/TheBuxomBg.tscn";
}
