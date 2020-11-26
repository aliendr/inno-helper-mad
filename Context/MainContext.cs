using InnoHelp.Server.Data;
using MongoDB.Driver;

namespace InnoHelp.Server.Context
{
	public class MainContext
	{
		private readonly IMongoDatabase _database;

		public MainContext()
		{
			var client = new MongoClient("mongodb+srv://admin:1234@cluster0.mmnzw.mongodb.net/innohelp?retryWrites=true&w=majority");
			_database = client.GetDatabase("innohelp");
		}

		public IMongoCollection<User> Users => _database.GetCollection<User>("users");

		public IMongoCollection<Care> Cares => _database.GetCollection<Care>("care");

		public IMongoCollection<Order> Orders => _database.GetCollection<Order>("orders");

		public IMongoCollection<Delivery> Delivery => _database.GetCollection<Delivery>("delivery");
	}
}