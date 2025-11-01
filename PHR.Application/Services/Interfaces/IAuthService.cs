using System.Threading.Tasks;
using PHR.Application.Auth.Commands;
using PHR.Application.DTOs;
namespace PHR.Application.Services.Interfaces
{
	public interface IAuthService
	{
		Task<ApiResponse<object>> RegisterAsync(RegisterUserCommand command);
		Task<ApiResponse<object>> LoginAsync(LoginCommand command);
		Task<ApiResponse<object>> RefreshTokenAsync(string refreshToken);
		Task<ApiResponse<object>> AdminCreateUserAsync(AdminCreateUserCommand command);
		Task<ApiResponse> LogoutAsync(string refreshToken);
		Task<ApiResponse> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword);
		Task<ApiResponse<object>> ResetPasswordRequestAsync(string email);
		Task<ApiResponse> ResetPasswordAsync(string token, string newPassword);
	}
}