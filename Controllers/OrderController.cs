using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using InnoHelp.Server.Context;
using InnoHelp.Server.Data;

namespace InnoHelp.Server.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	public class OrderController : ControllerBase
	{
		private readonly OrderContext _context;

		public OrderController(OrderContext context)
		{
			_context = context;
		}

		[HttpPost]
		public async Task<IActionResult> Create([FromBody] Order order)
		{
			order.Items = new List<OrderItem>();
			order.Participants = new List<string>();
			await _context.CreateNewOrderAsync(order);
			return Ok();
		}

		public async Task<IActionResult> Get(string orderId)
		{
			var order = await _context.FindOrderByIdAsync(orderId);
			if (order == null) return NotFound();
			return Ok(order);
		}

		public async Task<IActionResult> AddParticipant(string orderId, string userId)
		{
			await _context.AddParticipantAsync(orderId, userId);
			return Ok();
		}

		public async Task<IActionResult> RemoveParticipant(string orderId, string userId)
		{
			await _context.RemoveParticipantAsync(orderId, userId);
			return Ok();
		}

		public async Task<IActionResult> AddItemToOrder(string orderId, [FromBody] OrderItem item)
		{
			await _context.AddItemToOrderAsync(orderId, item);
			return Ok();
		}

		public async Task<IActionResult> RemoveItemFromOrder(string orderId, string itemId)
		{
			await _context.RemoveItemFromOrderAsync(orderId, itemId);
			return Ok();
		}

		public async Task<IActionResult> Close(string orderId)
		{
			await _context.CloseOrderAsync(orderId);
			return Ok();
		}

		public async Task<IActionResult> GetAll()
		{
			return Ok(await _context.GetAllOrdersAsync());
		}
	}
}
