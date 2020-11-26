using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InnoHelp.Server.Data;
using MongoDB.Bson;
using MongoDB.Driver;

namespace InnoHelp.Server.Context
{
	public class CareContext
	{
		private readonly IMongoCollection<Care> _careCollection;
		private readonly UsersContext _usersContext;

		public CareContext(MainContext context, UsersContext usersContext)
		{
			_careCollection = context.Cares;
			_usersContext = usersContext;
		}

		public async Task CreateNewCareAsync(Care care)
		{
			care.OpenTime = DateTime.Now;
			await _careCollection.InsertOneAsync(care);
		}

		public async Task<Care> FindCareByIdAsync(string careId)
		{
			var searchFilter = Builders<Care>.Filter.Eq("_id", new BsonObjectId(new ObjectId(careId)));
			var result = await (await _careCollection.FindAsync(searchFilter)).ToListAsync();
			return result.Count == 0 ? null : result[0];
		}

		public async Task TakeCareAsync(string careId, string userId)
		{
			var care = await FindCareByIdAsync(careId);
			if (care == null) return;

			var searchFilter = Builders<Care>.Filter.Eq("_id", new BsonObjectId(new ObjectId(careId)));
			var careUpdater = Builders<Care>.Update.Set(nameof(Care.TakenBy), userId);
			await _careCollection.UpdateOneAsync(searchFilter, careUpdater);
			await _usersContext.JoinCareAsync(careId, userId);
		}

		public async Task LeaveCareAsync(string careId)
		{
			var care = await FindCareByIdAsync(careId);
			if (care == null) return;

			var careUpdater = Builders<Care>.Update.Set(nameof(Care.TakenBy), "");
			var searchFilter = Builders<Care>.Filter.Eq("_id", new BsonObjectId(new ObjectId(careId)));

			await _careCollection.UpdateOneAsync(searchFilter, careUpdater);
			await _usersContext.LeaveCareAsync(careId, care.TakenBy, UserTakeInfo.Status.Canceled);
		}

		public async Task CloseCareAsync(string careId, UserTakeInfo.Status status)
		{
			var care = await FindCareByIdAsync(careId);
			if (care == null) return;

			var careUpdater = Builders<Care>.Update.Set(nameof(Care.Closed), true);
			if (status == UserTakeInfo.Status.ClosedByCreator) careUpdater.Set(nameof(Care.TakenBy), "");

			var searchFilter = Builders<Care>.Filter.Eq("_id", new BsonObjectId(new ObjectId(careId)));
			await _careCollection.UpdateOneAsync(searchFilter, careUpdater);
			await _usersContext.LeaveCareAsync(careId, care.TakenBy, status);
		}

		public async Task<IEnumerable<Care>> GetAllCaresAsync()
		{
			var deliveries = await (await _careCollection.FindAsync(Builders<Care>.Filter.Empty)).ToListAsync();
			deliveries.Sort((care1, care2) => care1.OpenTime.CompareTo(care2.OpenTime));
			return deliveries;
		}
	}
}