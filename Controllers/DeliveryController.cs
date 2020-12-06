using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using InnoHelp.Server.Context;
using InnoHelp.Server.Data;

namespace InnoHelp.Server.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	public class DeliveryController : ControllerBase
	{
		private readonly DeliveryContext _context;

		public DeliveryController(DeliveryContext context)
		{
			_context = context;
		}

		[HttpPost]
		public async Task<IActionResult> Create([FromBody] Delivery delivery)
		{
			delivery.Items = new List<string>();
			await _context.CreateNewDeliveryAsync(delivery);
			return Ok(delivery.Id);
		}

		public async Task<IActionResult> Get(string deliveryId)
		{
			var delivery = await _context.FindDeliveryByIdAsync(deliveryId);
			if (delivery == null) return NotFound();
			return Ok(delivery);
		}

		public async Task<IActionResult> Take(string deliveryId, string userId)
		{
			await _context.TakeDeliveryAsync(deliveryId, userId);
			return Ok();
		}

		public async Task<IActionResult> Close(string deliveryId)
		{
			await _context.CloseDeliveryAsync(deliveryId, UserTakeInfo.Status.ClosedByCreator);
			return Ok();
		}

		public async Task<IActionResult> Complete(string deliveryId)
		{
			await _context.CloseDeliveryAsync(deliveryId, UserTakeInfo.Status.Canceled);
			return Ok();
		}

		public async Task<IActionResult> Cancel(string deliveryId)
		{
			await _context.LeaveDeliveryAsync(deliveryId);
			return Ok();
		}

		public async Task<IActionResult> GetAll()
		{
			return Ok(await _context.GetAllDeliveriesAsync());
		}
	}
}