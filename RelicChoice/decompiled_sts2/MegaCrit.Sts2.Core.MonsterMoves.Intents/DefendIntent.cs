namespace MegaCrit.Sts2.Core.MonsterMoves.Intents;

        [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.RelicPools.SharedRelicPool))]
    public class DefendIntent : AbstractIntent
{
	public override IntentType IntentType => IntentType.Defend;

	protected override string IntentPrefix => "DEFEND";

	protected override string SpritePath => "atlases/intent_atlas.sprites/intent_defend.tres";
}
