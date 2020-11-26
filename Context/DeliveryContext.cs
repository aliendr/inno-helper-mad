using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using InnoHelp.Server.Data;
using MongoDB.Bson;
using MongoDB.Driver;

namespace InnoHelp.Server.Context
{
	public class DeliveryContext
	{
		private readonly IMongoCollection<Delivery> _deliveryCollection;
		private readonly UsersContext _usersContext;

		public DeliveryContext(MainContext context, UsersContext usersContext)
		{
			_deliveryCollection = context.Delivery;
			_usersContext = usersContext;
		}

		public async Task CreateNewDeliveryAsync(Delivery delivery)
		{
			delivery.OpenTime = DateTime.Now;
			await _deliveryCollection.InsertOneAsync(delivery);
		}

		public async Task<Delivery> FindDeliveryByIdAsync(string deliveryId)
		{
			var searchFilter = Builders<Delivery>.Filter.Eq("_id", new BsonObjectId(new ObjectId(deliveryId)));
			var result = await (await _deliveryCollection.FindAsync(searchFilter)).ToListAsync();
			return result.Count == 0 ? null : result[0];
		}

		public async Task TakeDeliveryAsync(string deliveryId, string userId)
		{
			var delivery = await FindDeliveryByIdAsync(deliveryId);
			if (delivery == null) return;

			var searchFilter = Builders<Delivery>.Filter.Eq("_id", new BsonObjectId(new ObjectId(deliveryId)));
			var deliveryUpdater = Builders<Delivery>.Update.Set(nameof(Delivery.TakenBy), userId);
			await _deliveryCollection.UpdateOneAsync(searchFilter, deliveryUpdater);
			await _usersContext.JoinDeliveryAsync(deliveryId, userId);
		}

		public async Task LeaveDeliveryAsync(string deliveryId)
		{
			var delivery = await FindDeliveryByIdAsync(deliveryId);
			if (delivery == null) return;

			var deliveryUpdater = Builders<Delivery>.Update.Set(nameof(Delivery.TakenBy), "");
			var searchFilter = Builders<Delivery>.Filter.Eq("_id", new BsonObjectId(new ObjectId(deliveryId)));

			await _deliveryCollection.UpdateOneAsync(searchFilter, deliveryUpdater);
			await _usersContext.LeaveDeliveryAsync(deliveryId, delivery.TakenBy, UserTakeInfo.Status.Canceled);
		}
		
		public async Task CloseDeliveryAsync(string deliveryId, UserTakeInfo.Status status)
		{
			var delivery = await FindDeliveryByIdAsync(deliveryId);
			if (delivery == null) return;

			var deliveryUpdater = Builders<Delivery>.Update.Set(nameof(Delivery.Closed), true);
			if (status == UserTakeInfo.Status.ClosedByCreator) deliveryUpdater.Set(nameof(Delivery.TakenBy), "");

			var searchFilter = Builders<Delivery>.Filter.Eq("_id", new BsonObjectId(new ObjectId(deliveryId)));
			await _deliveryCollection.UpdateOneAsync(searchFilter, deliveryUpdater);
			await _usersContext.LeaveDeliveryAsync(deliveryId, delivery.TakenBy, status);
		}

		public async Task<IEnumerable<Delivery>> GetAllDeliveriesAsync()
		{
			var deliveries =  await (await _deliveryCollection.FindAsync(Builders<Delivery>.Filter.Empty)).ToListAsync();
			deliveries.Sort((delivery1, delivery2) => delivery1.OpenTime.CompareTo(delivery2.OpenTime));
			return deliveries;
		}
	}
}