using System.Threading.Tasks;
using InnoHelp.Server.Context;
using InnoHelp.Server.Data;
using Microsoft.AspNetCore.Mvc;

namespace InnoHelp.Server.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	public class CareController : ControllerBase
	{
		private readonly CareContext _context;


		public CareController(CareContext context)
		{
			_context = context;
		}
		
		[HttpPost]
		public async Task<IActionResult> Create([FromBody] Care care)
		{
			await _context.CreateNewCareAsync(care);
			return Ok(care.Id);
		}

		public async Task<IActionResult> Get(string careId)
		{
			var care = await _context.FindCareByIdAsync(careId);
			if (care == null) return NotFound();
			return Ok(care);
		}

		public async Task<IActionResult> Take(string careId, string userId)
		{
			await _context.TakeCareAsync(careId, userId);
			return Ok();
		}

		public async Task<IActionResult> Close(string careId)
		{
			await _context.CloseCareAsync(careId, UserTakeInfo.Status.ClosedByCreator);
			return Ok();
		}

		public async Task<IActionResult> Complete(string careId)
		{
			await _context.CloseCareAsync(careId, UserTakeInfo.Status.Canceled);
			return Ok();
		}

		public async Task<IActionResult> Cancel(string careId)
		{
			await _context.LeaveCareAsync(careId);
			return Ok();
		}

		public async Task<IActionResult> GetAll()
		{
			return Ok(await _context.GetAllCaresAsync());
		}
	}
}