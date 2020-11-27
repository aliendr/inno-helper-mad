using System.Collections.Generic;
using System.Threading.Tasks;
using InnoHelp.Server.Context;
using InnoHelp.Server.Data;
using Microsoft.AspNetCore.Mvc;

namespace InnoHelp.Server.Controllers
{
	[Route("api/[controller]/[action]")]
	[ApiController]
	public class UserController : ControllerBase
	{
		private readonly UsersContext _usersContext;

		public UserController(UsersContext context)
		{
			_usersContext = context;
		}

		public async Task<IActionResult> Login(string login, string passHash)
		{
			var user = await _usersContext.GetUserByLoginAndPasswordHashAsync(login, passHash);
			if (user == null) return Unauthorized();

			return Ok(user);
		}

		public async Task<IActionResult> Register(string login, string passHash)
		{
			var newUser = new User
			{
				UserName = login,
				PasswordHash = passHash,
				TakenDeliveries = new List<UserTakeInfo>(),
				TakenCares = new List<UserTakeInfo>(),
				TakenOrders = new List<UserTakeInfo>()
			};
			await _usersContext.AddUser(newUser);
			return Ok();
		}

		[HttpPost]
		public async Task<IActionResult> UpdateData([FromBody] User user)
		{
			await _usersContext.EditUserDataAsync(user.Id, user);
			return Ok();
		}

		public async Task<IActionResult> Get(string userId)
		{
			var user = await _usersContext.FindUserByIdAsync(userId);
			if (user == null) return NotFound();
			return Ok(user);
		}

		public async Task<IActionResult> RateUser(string userId, int rate)
		{
			await _usersContext.RateUserAsync(userId, rate);
			return Ok();
		}
	}
}
