namespace MegaCrit.Sts2.Core.MonsterMoves.Intents;

        [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.RelicPools.SharedRelicPool))]
    public class UnknownIntent : AbstractIntent
{
	public override IntentType IntentType => IntentType.Unknown;

	protected override string IntentPrefix => "UNKNOWN";

	protected override string SpritePath => "atlases/intent_atlas.sprites/intent_unknown.tres";
}
