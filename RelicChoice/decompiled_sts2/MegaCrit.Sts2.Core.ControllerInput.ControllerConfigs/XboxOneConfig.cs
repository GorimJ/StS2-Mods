namespace MegaCrit.Sts2.Core.ControllerInput.ControllerConfigs;

        [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.RelicPools.SharedRelicPool))]
    public class XboxOneConfig : ControllerConfig
{
	protected override string FolderPath => "atlases/controller_atlas.sprites/xbox_one";
}
