namespace MegaCrit.Sts2.Core.MonsterMoves.Intents;

        [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.RelicPools.SharedRelicPool))]
    public class BuffIntent : AbstractIntent
{
	public override IntentType IntentType => IntentType.Buff;

	protected override string IntentPrefix => "BUFF";

	protected override string SpritePath => "atlases/intent_atlas.sprites/intent_buff.tres";
}
