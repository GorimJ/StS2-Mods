namespace MegaCrit.Sts2.Core.Saves;

        [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.RelicPools.SharedRelicPool))]
    public class StaticProfileIdProvider : IProfileIdProvider
{
	private int _profileId;

	public int CurrentProfileId => _profileId;

	public StaticProfileIdProvider(int profileId)
	{
		_profileId = profileId;
	}
}
