namespace MegaCrit.Sts2.Core.MonsterMoves.Intents;

        [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.RelicPools.SharedRelicPool))]
    public class HealIntent : AbstractIntent
{
	public override IntentType IntentType => IntentType.Heal;

	protected override string IntentPrefix => "HEAL";

	protected override string SpritePath => "atlases/intent_atlas.sprites/intent_heal.tres";
}
