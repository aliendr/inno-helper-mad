using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InnoHelp.Server.Data;
using MongoDB.Bson;
using MongoDB.Driver;

namespace InnoHelp.Server.Context
{
	public class OrderContext
	{
		private readonly IMongoCollection<Order> _orderCollection;
		private readonly UsersContext _usersContext;

		public OrderContext(MainContext context, UsersContext usersContext)
		{
			_orderCollection = context.Orders;
			_usersContext = usersContext;
		}
		
		public async Task CreateNewOrderAsync(Order order)
		{
			order.OpenTime = DateTime.Now;
			await _orderCollection.InsertOneAsync(order);
		}

		public async Task<Order> FindOrderByIdAsync(string orderId)
		{
			var searchFilter = Builders<Order>.Filter.Eq("_id", new BsonObjectId(new ObjectId(orderId)));
			var result = await (await _orderCollection.FindAsync(searchFilter)).ToListAsync();
			return result.Count == 0 ? null : result[0];
		}
		
		public async Task AddParticipantAsync(string orderId, string userId)
		{
			var order = await FindOrderByIdAsync(orderId);
			if (order == null) return;

			var orderUpdater = Builders<Order>.Update.AddToSet(nameof(Order.Participants), userId);
			var searchFilter = Builders<Order>.Filter.Eq("_id", new BsonObjectId(new ObjectId(orderId)));
			await _orderCollection.UpdateOneAsync(searchFilter, orderUpdater);
			await _usersContext.JoinOrderAsync(orderId, userId);
		}

		public async Task RemoveParticipantAsync(string orderId, string userId)
		{
			var order = await FindOrderByIdAsync(orderId);
			if (order == null) return;

			var newItems = order.Items.Where(item => item.Id != userId);
			var orderUpdater = Builders<Order>.Update.Set(nameof(Order.Items), newItems)
				.Pull(nameof(Order.Participants), userId);
			var searchFilter = Builders<Order>.Filter.Eq("_id", new BsonObjectId(new ObjectId(orderId)));
			await _orderCollection.UpdateOneAsync(searchFilter, orderUpdater);
			await _usersContext.LeaveOrderAsync(orderId, userId, UserTakeInfo.Status.Canceled);
			await RecalculateTotalCost(orderId);
		}

		public async Task AddItemToOrderAsync(string orderId, OrderItem item)
		{
			var order = await FindOrderByIdAsync(orderId);
			if (order == null) return;

			item.Id = ObjectId.GenerateNewId().ToString();

			var orderUpdater = Builders<Order>.Update.AddToSet(nameof(Order.Items), item);
			var searchFilter = Builders<Order>.Filter.Eq("_id", new BsonObjectId(new ObjectId(orderId)));

			await _orderCollection.UpdateOneAsync(searchFilter, orderUpdater);
			await RecalculateTotalCost(orderId);
		}

		public async Task RemoveItemFromOrderAsync(string orderId, string itemId)
		{
			var order = await FindOrderByIdAsync(orderId);

			var foundItem = order?.Items.Find(item => item.Id == itemId);
			if (foundItem == null) return;

			var orderUpdater = Builders<Order>.Update.Pull(nameof(Order.Items), foundItem);
			var searchFilter = Builders<Order>.Filter.Eq("_id", new BsonObjectId(new ObjectId(orderId)));

			await _orderCollection.UpdateOneAsync(searchFilter, orderUpdater);
			await RecalculateTotalCost(orderId);
		}

		public async Task CloseOrderAsync(string orderId)
		{
			var order = await FindOrderByIdAsync(orderId);
			if (order == null) return;

			var orderUpdater = Builders<Order>.Update.Set(nameof(Order.Closed), true);
			var searchFilter = Builders<Order>.Filter.Eq("_id", new BsonObjectId(new ObjectId(orderId)));

			await _orderCollection.UpdateOneAsync(searchFilter, orderUpdater);
			foreach (var participant in order.Participants)
				await _usersContext.LeaveOrderAsync(orderId, participant, UserTakeInfo.Status.ClosedByCreator);
		}

		public async Task<IEnumerable<Order>> GetAllOrdersAsync()
		{
			var orders =  await (await _orderCollection.FindAsync(Builders<Order>.Filter.Empty)).ToListAsync();
			orders.Sort((order1, order2) => order1.OpenTime.CompareTo(order2.OpenTime));
			return orders;
		}

		private async Task RecalculateTotalCost(string orderId)
		{
			var order = await FindOrderByIdAsync(orderId);
			if (order == null) return;

			var newTotalCost = order.Items.Select(item => item.Price).Aggregate((price1, price2) => price1 + price2);
			var orderUpdater = Builders<Order>.Update.Set(nameof(Order.TotalPrice), newTotalCost);
			var searchFilter = Builders<Order>.Filter.Eq("_id", new BsonObjectId(new ObjectId(orderId)));
			await _orderCollection.UpdateOneAsync(searchFilter, orderUpdater);
		}
	}
}