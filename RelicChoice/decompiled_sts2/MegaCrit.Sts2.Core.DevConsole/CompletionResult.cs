using System.Collections.Generic;

namespace MegaCrit.Sts2.Core.DevConsole;

        [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.RelicPools.SharedRelicPool))]
    public class CompletionResult
{
	public List<string> Candidates { get; set; } = new List<string>();

	public string CommonPrefix { get; set; } = string.Empty;

	public bool HasMultipleMatches => Candidates.Count > 1;

	public CompletionType Type { get; set; }

	public string CommandPrefix { get; set; } = "";

	public int ArgumentIndex { get; set; } = -1;

	public string ArgumentContext { get; set; } = "";
}
