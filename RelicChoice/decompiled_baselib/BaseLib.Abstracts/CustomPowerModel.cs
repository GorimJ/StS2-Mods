using MegaCrit.Sts2.Core.Models;

namespace BaseLib.Abstracts;

public abstract class CustomPowerModel : PowerModel, ICustomModel
{
	public virtual string? CustomPackedIconPath => null;

	public virtual string? CustomBigIconPath => null;

	public virtual string? CustomBigBetaIconPath => null;
}
