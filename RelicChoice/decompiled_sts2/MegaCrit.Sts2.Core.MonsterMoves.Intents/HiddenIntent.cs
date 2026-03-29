namespace MegaCrit.Sts2.Core.MonsterMoves.Intents;

        [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.RelicPools.SharedRelicPool))]
    public class HiddenIntent : AbstractIntent
{
	public override IntentType IntentType => IntentType.Hidden;

	protected override string IntentPrefix => "HIDDEN";

	protected override string? SpritePath => null;

	public override bool HasIntentTip => false;
}
