using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace InnoHelp.Server.Data
{
	public class OrderItem
	{
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string Id { get; set; }

		[BsonRepresentation(BsonType.ObjectId)]
		public string AddedBy { get; set; }

		public string Link { get; set; }

		public string Title { get; set; }

		public string Description { get; set; }

		public double Price { get; set; }
	}
}