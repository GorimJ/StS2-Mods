namespace MegaCrit.Sts2.Core.Saves;

        [BaseLib.Utils.Pool(typeof(MegaCrit.Sts2.Core.Models.RelicPools.SharedRelicPool))]
    public class ReadSaveResult<T> where T : ISaveSchema
{
	public T? SaveData { get; }

	public ReadSaveStatus Status { get; }

	public bool Success
	{
		get
		{
			if (Status != ReadSaveStatus.Success && Status != ReadSaveStatus.MigrationRequired && Status != ReadSaveStatus.JsonRepaired)
			{
				return Status == ReadSaveStatus.RecoveredWithDataLoss;
			}
			return true;
		}
	}

	public string? ErrorMessage { get; }

	public ReadSaveResult(T data)
	{
		SaveData = data;
		Status = ReadSaveStatus.Success;
	}

	public ReadSaveResult(ReadSaveStatus status, string? errorMessage = null)
	{
		Status = status;
		ErrorMessage = errorMessage;
	}

	public ReadSaveResult(T data, ReadSaveStatus status, string? errorMessage = null)
	{
		SaveData = data;
		Status = status;
		ErrorMessage = errorMessage;
	}
}
