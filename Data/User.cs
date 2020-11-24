using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace InnoHelp.Server.Data
{
	public class User
	{
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string Id { get; set; }

		public string UserName { get; set; }

		public string PasswordHash { get; set; }

		public string ProfilePhoto { get; set; }

		public string ContactPhone { get; set; }

		public double Rating { get; set; }

		public int RateCount { get; set; }

		public List<UserTakeInfo> TakenOrders { get; set; }

		public List<UserTakeInfo> TakenDeliveries { get; set; }

		public List<UserTakeInfo> TakenCares { get; set; }
	}
}