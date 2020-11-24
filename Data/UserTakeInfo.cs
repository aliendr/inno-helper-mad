using System;

namespace InnoHelp.Server.Data
{
	public class UserTakeInfo
	{

		public enum Status
		{
			Open,
			Completed,
			Canceled,
			ClosedByCreator
		}

		public string Id { get; set; }

		public Status TakeStatus { get; set; }

		public DateTime TakeTime { get; set; }

		public DateTime CompletionTime { get; set; }
	}
}