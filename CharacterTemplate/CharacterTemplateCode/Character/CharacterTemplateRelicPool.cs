using BaseLib.Abstracts;
using CharacterTemplate.CharacterTemplateCode.Extensions;
using Godot;

namespace CharacterTemplate.CharacterTemplateCode.Character;

public class CharacterTemplateRelicPool : CustomRelicPoolModel
{
    public override Color LabOutlineColor => CharacterTemplate.Color;

    public override string BigEnergyIconPath => "charui/big_energy.png".ImagePath();
    public override string TextEnergyIconPath => "charui/text_energy.png".ImagePath();
}