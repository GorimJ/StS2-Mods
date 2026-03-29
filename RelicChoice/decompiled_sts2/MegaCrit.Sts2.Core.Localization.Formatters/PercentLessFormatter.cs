using System;
using System.Globalization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using SmartFormat.Core.Extensions;

namespace MegaCrit.Sts2.Core.Localization.Formatters;

        [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.RelicPools.SharedRelicPool))]
    public class PercentLessFormatter : IFormatter
{
	public string Name
	{
		get
		{
			return "percentLess";
		}
		set
		{
			throw new NotImplementedException();
		}
	}

	public bool CanAutoDetect { get; set; }

	public bool TryEvaluateFormat(IFormattingInfo formattingInfo)
	{
		object currentValue = formattingInfo.CurrentValue;
		decimal num;
		if (currentValue is DynamicVar dynamicVar)
		{
			num = dynamicVar.BaseValue;
		}
		else
		{
			try
			{
				num = Convert.ToDecimal(formattingInfo.CurrentValue);
			}
			catch (FormatException)
			{
				return false;
			}
			catch (InvalidCastException)
			{
				return false;
			}
		}
		formattingInfo.Write(Convert.ToInt32((1m - num) * 100m).ToString(CultureInfo.InvariantCulture));
		return true;
	}
}
