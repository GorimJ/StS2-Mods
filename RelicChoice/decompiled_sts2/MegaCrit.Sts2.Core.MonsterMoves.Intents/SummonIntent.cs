namespace MegaCrit.Sts2.Core.MonsterMoves.Intents;

        [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.RelicPools.SharedRelicPool))]
    public class SummonIntent : AbstractIntent
{
	public override IntentType IntentType => IntentType.Summon;

	protected override string IntentPrefix => "SUMMON";

	protected override string SpritePath => "atlases/intent_atlas.sprites/intent_summon.tres";
}
