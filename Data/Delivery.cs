using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace InnoHelp.Server.Data
{
	public class Delivery
	{
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string Id { get; set; }

		public DateTime OpenTime { get; set; }

		public string Title { get; set; }

		public string Location { get; set; }

		public double TotalCost { get; set; }

		public double Benefit { get; set; }

		public List<string> Items { get; set; }

		[BsonRepresentation(BsonType.ObjectId)]
		public string TakenBy { get; set; }

		public bool Closed { get; set; }
	}
}