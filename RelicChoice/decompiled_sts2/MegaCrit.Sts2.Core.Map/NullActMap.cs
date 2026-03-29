namespace MegaCrit.Sts2.Core.Map;

        [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.RelicPools.SharedRelicPool))]
    public class NullActMap : ActMap
{
	public static NullActMap Instance { get; } = new NullActMap();

	public override MapPoint BossMapPoint { get; } = new MapPoint(0, 0);

	public override MapPoint StartingMapPoint { get; } = new MapPoint(0, 0);

	protected override MapPoint?[,] Grid { get; } = new MapPoint[0, 0];
}
