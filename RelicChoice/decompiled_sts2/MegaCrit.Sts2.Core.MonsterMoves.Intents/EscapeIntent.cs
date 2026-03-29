namespace MegaCrit.Sts2.Core.MonsterMoves.Intents;

        [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.RelicPools.SharedRelicPool))]
    public class EscapeIntent : AbstractIntent
{
	public override IntentType IntentType => IntentType.Escape;

	protected override string IntentPrefix => "ESCAPE";

	protected override string SpritePath => "atlases/intent_atlas.sprites/intent_escape.tres";
}
