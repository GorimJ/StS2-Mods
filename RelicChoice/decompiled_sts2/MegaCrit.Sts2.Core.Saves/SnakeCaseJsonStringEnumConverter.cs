using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MegaCrit.Sts2.Core.Saves;

        [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.RelicPools.SharedRelicPool))]
    public class SnakeCaseJsonStringEnumConverter<TEnum> : JsonStringEnumConverter<TEnum> where TEnum : struct, Enum
{
	public SnakeCaseJsonStringEnumConverter()
		: base(JsonNamingPolicy.SnakeCaseLower, allowIntegerValues: true)
	{
	}
}
