using System;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace InnoHelp.Server.Data
{
	public class Care
	{
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string Id { get; set; }

		public DateTime OpenTime { get; set; }

		public string Title { get; set; }

		public string Description { get; set; }

		[BsonRepresentation(BsonType.ObjectId)]
		public string Creator { get; set; }

		public double Benefit { get; set; }

		[BsonRepresentation(BsonType.ObjectId)]
		public string TakenBy { get; set; }

		public bool Closed { get; set; }
	}
}