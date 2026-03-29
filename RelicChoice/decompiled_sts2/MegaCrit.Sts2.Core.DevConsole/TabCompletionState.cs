using System.Collections.Generic;

namespace MegaCrit.Sts2.Core.DevConsole;

        [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.RelicPools.SharedRelicPool))]
    public class TabCompletionState
{
	public int SelectionIndex { get; set; } = -1;

	public List<string> CompletionCandidates { get; } = new List<string>();

	public bool InSelectionMode { get; set; }

	public bool ProgrammaticTextChange { get; set; }

	public CompletionResult? LastCompletionResult { get; set; }

	public void Reset()
	{
		InSelectionMode = false;
		SelectionIndex = -1;
		CompletionCandidates.Clear();
		LastCompletionResult = null;
	}
}
