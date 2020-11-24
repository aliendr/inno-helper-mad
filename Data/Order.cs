using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace InnoHelp.Server.Data
{
	public class Order
	{
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string Id { get; set; }

		public DateTime OpenTime { get; set; }

		public string Title { get; set; }

		public string Description { get; set; }

		[BsonRepresentation(BsonType.ObjectId)]
		public string Creator { get; set; }

		public List<string> Participants { get; set; }

		public List<OrderItem> Items { get; set; }

		public double TotalPrice { get; set; }

		public bool Closed { get; set; }
	}
}