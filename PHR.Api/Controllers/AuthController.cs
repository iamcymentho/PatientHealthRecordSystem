using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PHR.Application.Services.Interfaces;
using PHR.Application.Auth.Commands;
using PHR.Application.Auth.Models;
using PHR.Infrastructure.Authorization;
namespace PHR.Api.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AuthController : BaseController
	{
		private readonly IAuthService _authService;
		public AuthController(IAuthService authService)
		{
			_authService = authService;
		}
		[HttpPost("register")]
		public async Task<IActionResult> Register([FromBody] RegisterUserCommand command)
		{
			var response = await _authService.RegisterAsync(command);
			return Ok(response);
		}
		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginCommand command)
		{
			var response = await _authService.LoginAsync(command);
			return Ok(response);
		}
		[HttpPost("refresh")]
		public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
		{
			var response = await _authService.RefreshTokenAsync(request.RefreshToken);
			return Ok(response);
		}
		[HttpPost("admin/users")]
		[Authorize(Policy = AuthorizationPolicies.ManageUsers)]
		public async Task<IActionResult> AdminCreateUser([FromBody] AdminCreateUserCommand command)
		{
			var response = await _authService.AdminCreateUserAsync(command);
			return Ok(response);
		}
		[HttpPost("logout")]
		[Authorize]
		public async Task<IActionResult> Logout([FromBody] RefreshTokenRequest request)
		{
			var response = await _authService.LogoutAsync(request.RefreshToken);
			return Ok(response);
		}
		[HttpPost("change-password")]
		[Authorize]
		public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
		{
			var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
			{
				return Unauthorized("Invalid user token");
			}
			var response = await _authService.ChangePasswordAsync(userId, request.CurrentPassword, request.NewPassword);
			return Ok(response);
		}
		[HttpPost("reset-password-request")]
		public async Task<IActionResult> ResetPasswordRequest([FromBody] ResetPasswordRequestDto request)
		{
			var response = await _authService.ResetPasswordRequestAsync(request.Email);
			return Ok(response);
		}
		[HttpPost("reset-password")]
		public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto request)
		{
			var response = await _authService.ResetPasswordAsync(request.Token, request.NewPassword);
			return Ok(response);
		}
	}
}