using System;
using System.Threading.Tasks;
using InnoHelp.Server.Data;
using MongoDB.Bson;
using MongoDB.Driver;

namespace InnoHelp.Server.Context
{
	public class UsersContext
	{

		private readonly IMongoCollection<User> _usersCollection;
		public UsersContext(MainContext context)
		{
			_usersCollection = context.Users;
		}

		public async Task AddUser(User user)
		{
			await _usersCollection.InsertOneAsync(user);
		}

		public async Task<User> GetUserByLoginAndPasswordHashAsync(string login, string passwordHash)
		{
			var searchFilter = Builders<User>.Filter.Eq(nameof(User.UserName), login);

			var searchResult = await (await _usersCollection.FindAsync(searchFilter)).ToListAsync();
			if (searchResult.Count == 0) return null;

			var user = searchResult[0];
			return user.PasswordHash != passwordHash ? null : user;
		}

		public async Task<User> FindUserByIdAsync(string userId)
		{
			var filter = Builders<User>.Filter;
			var searchFilter = filter.Eq("_id", new BsonObjectId(new ObjectId(userId)));

			var searchResult = await (await _usersCollection.FindAsync(searchFilter)).ToListAsync();
			return searchResult.Count == 0 ? null : searchResult[0];
		}

		public async Task EditUserDataAsync(string userId, User newData)
		{
			var user = await FindUserByIdAsync(userId);
			if (user == null) return;

			var updater = Builders<User>.Update;
			var userUpdater = updater.Set(nameof(User.ProfilePhoto), newData.ProfilePhoto)
				.Set(nameof(User.UserName), newData.UserName).Set(nameof(User.ContactPhone), newData.ContactPhone);

			var searchFilter = Builders<User>.Filter.Eq("_id", new BsonObjectId(new ObjectId(newData.Id)));

			await _usersCollection.UpdateOneAsync(searchFilter, userUpdater);
		}
		
		public async Task RateUserAsync(string userId, int rate)
		{
			var user = await FindUserByIdAsync(userId);
			if (user == null) return;

			var newRate = (user.Rating * user.RateCount + rate) / (user.RateCount + 1);
			var userUpdater = Builders<User>.Update.Set(nameof(User.Rating), newRate)
				.Set(nameof(User.RateCount), user.RateCount + 1);
			var searchFilter = Builders<User>.Filter.Eq("_id", new BsonObjectId(new ObjectId(userId)));

			await _usersCollection.UpdateOneAsync(searchFilter, userUpdater);
		}

		public async Task JoinOrderAsync(string orderId, string userId)
		{
			var userTakeInfo = new UserTakeInfo {TakeTime = DateTime.Now, Id = orderId};
			var user = await FindUserByIdAsync(userId);
			if (user == null) return;

			var userUpdater = Builders<User>.Update.AddToSet(nameof(User.TakenOrders), userTakeInfo);
			var searchFilter = Builders<User>.Filter.Eq("_id", new BsonObjectId(new ObjectId(userId)));

			await _usersCollection.UpdateOneAsync(searchFilter, userUpdater);
		}

		public async Task LeaveOrderAsync(string orderId, string userId, UserTakeInfo.Status status)
		{
			var user = await FindUserByIdAsync(userId);

			var takeInfo = user?.TakenOrders.Find(info => info.Id == orderId);
			if (takeInfo == null) return;

			takeInfo.TakeStatus = status;
			takeInfo.CompletionTime = DateTime.Now;

			var userUpdater = Builders<User>.Update.Set(nameof(User.TakenOrders), user.TakenOrders);
			var searchFilter = Builders<User>.Filter.Eq("_id", new BsonObjectId(new ObjectId(userId)));
			await _usersCollection.UpdateOneAsync(searchFilter, userUpdater);
		}

		public async Task JoinDeliveryAsync(string deliveryId, string userId)
		{
			var userTakeInfo = new UserTakeInfo { TakeTime = DateTime.Now, Id = deliveryId };
			var user = await FindUserByIdAsync(userId);
			if (user == null) return;

			var userUpdater = Builders<User>.Update.AddToSet(nameof(User.TakenDeliveries), userTakeInfo);
			var searchFilter = Builders<User>.Filter.Eq("_id", new BsonObjectId(new ObjectId(userId)));

			await _usersCollection.UpdateOneAsync(searchFilter, userUpdater);
		}

		public async Task LeaveDeliveryAsync(string deliveryId, string userId, UserTakeInfo.Status status)
		{
			var user = await FindUserByIdAsync(userId);

			var takeInfo = user?.TakenDeliveries.Find(info => info.Id == deliveryId);
			if (takeInfo == null) return;

			takeInfo.TakeStatus = status;
			takeInfo.CompletionTime = DateTime.Now;

			var userUpdater = Builders<User>.Update.Set(nameof(User.TakenDeliveries), user.TakenDeliveries);
			var searchFilter = Builders<User>.Filter.Eq("_id", new BsonObjectId(new ObjectId(userId)));
			await _usersCollection.UpdateOneAsync(searchFilter, userUpdater);
		}

		public async Task JoinCareAsync(string careId, string userId)
		{
			var userTakeInfo = new UserTakeInfo { TakeTime = DateTime.Now, Id = careId };
			var user = await FindUserByIdAsync(userId);
			if (user == null) return;

			var userUpdater = Builders<User>.Update.AddToSet(nameof(User.TakenCares), userTakeInfo);
			var searchFilter = Builders<User>.Filter.Eq("_id", new BsonObjectId(new ObjectId(userId)));

			await _usersCollection.UpdateOneAsync(searchFilter, userUpdater);
		}

		public async Task LeaveCareAsync(string careId, string userId, UserTakeInfo.Status status)
		{
			var user = await FindUserByIdAsync(userId);

			var takeInfo = user?.TakenCares.Find(info => info.Id == careId);
			if (takeInfo == null) return;

			takeInfo.TakeStatus = status;
			takeInfo.CompletionTime = DateTime.Now;

			var userUpdater = Builders<User>.Update.Set(nameof(User.TakenCares), user.TakenCares);
			var searchFilter = Builders<User>.Filter.Eq("_id", new BsonObjectId(new ObjectId(userId)));
			await _usersCollection.UpdateOneAsync(searchFilter, userUpdater);
		}
	}
}