using Microsoft.EntityFrameworkCore;
using PHR.Application.Services.Interfaces;
using PHR.Application.Auth.Commands;
using PHR.Application.DTOs;
using PHR.Domain.Entities;
using PHR.Application.Abstractions.Repositories;
using PHR.Application.Auth.Abstractions;
using PHR.Application.Abstractions.Data;
namespace PHR.Application.Services.Implementation
{
	public class AuthService : IAuthService
	{
		private readonly ITokenService _tokenService;
		private readonly IPasswordVerifier _passwordVerifier;
		private readonly IUserRepository _userRepository;
		private readonly IRoleRepository _roleRepository;
		private readonly IPasswordHasherAbstraction _passwordHasher;
		private readonly IRefreshTokenRepository _refreshTokenRepository;
		private readonly IUserRoleRepository _userRoleRepository;
		private readonly IPasswordResetTokenRepository _passwordResetTokenRepository;
		public AuthService(
			ITokenService tokenService,
			IPasswordVerifier passwordVerifier,
			IUserRepository userRepository,
			IRoleRepository roleRepository,
			IPasswordHasherAbstraction passwordHasher,
			IRefreshTokenRepository refreshTokenRepository,
			IUserRoleRepository userRoleRepository,
			IPasswordResetTokenRepository passwordResetTokenRepository)
		{
			_tokenService = tokenService;
			_passwordVerifier = passwordVerifier;
			_userRepository = userRepository;
			_roleRepository = roleRepository;
			_passwordHasher = passwordHasher;
			_refreshTokenRepository = refreshTokenRepository;
			_userRoleRepository = userRoleRepository;
			_passwordResetTokenRepository = passwordResetTokenRepository;
		}
		public async Task<ApiResponse<object>> RegisterAsync(RegisterUserCommand command)
		{
			try
			{
				if (await _userRepository.EmailExistsAsync(command.Email))
				{
					return ApiResponse<object>.BadRequestResponse("Email already exists");
				}
				var user = new User
				{
					Id = Guid.NewGuid(),
					FullName = command.FullName,
					Email = command.Email,
					Gender = command.Gender,
					PhoneNumber = command.PhoneNumber,
					PasswordHash = _passwordHasher.Hash(command.Password),
					CreatedAtUtc = DateTime.UtcNow
				};
				await _userRepository.AddAsync(user);
				var patientRole = await _roleRepository.GetByNameAsync("Patient");
				if (patientRole != null)
				{
					var userRole = new UserRole
					{
						UserId = user.Id,
						RoleId = patientRole.Id
					};
					await _userRoleRepository.AddAsync(userRole);
					await _userRoleRepository.SaveChangesAsync();
				}
				return ApiResponse<object>.CreatedResponse(new { userId = user.Id }, "User registered successfully");
			}
			catch (Exception ex)
			{
				return ApiResponse<object>.InternalServerErrorResponse($"Registration failed: {ex.Message}");
			}
		}
		public async Task<ApiResponse<object>> LoginAsync(LoginCommand command)
		{
			try
			{
				var user = await _userRepository.GetByEmailWithRolesAsync(command.Email);
				if (user == null)
				{
					return ApiResponse<object>.UnauthorizedResponse("Invalid credentials");
				}
				if (!_passwordVerifier.Verify(command.Password, user.PasswordHash))
				{
					return ApiResponse<object>.UnauthorizedResponse("Invalid credentials");
				}
				var roles = user.UserRoles.Select(ur => ur.Role.Name).ToArray();
				var permissions = user.UserRoles
					.SelectMany(ur => ur.Role.RolePermissions)
					.Select(rp => rp.Permission.Name)
					.Distinct()
					.ToArray();
				var accessToken = _tokenService.CreateAccessToken(user.Id, roles, permissions);
				var refreshToken = _tokenService.GenerateRefreshToken();
				var refreshTokenEntity = new RefreshToken
				{
					Id = Guid.NewGuid(),
					Token = refreshToken,
					UserId = user.Id,
					ExpiryDateUtc = DateTime.UtcNow.AddDays(7), // 7 days expiry
					CreatedAtUtc = DateTime.UtcNow
				};
				await _refreshTokenRepository.AddAsync(refreshTokenEntity);
				await _refreshTokenRepository.SaveChangesAsync();
				var loginResponse = new 
				{ 
					accessToken, 
					refreshToken,
					user = new
					{
						id = user.Id,
						fullName = user.FullName,
						email = user.Email,
						roles = roles,
						permissions = permissions
					}
				};
				return ApiResponse<object>.SuccessResponse(loginResponse, "Login successful");
			}
			catch (Exception ex)
			{
				return ApiResponse<object>.InternalServerErrorResponse($"Login failed: {ex.Message}");
			}
		}
		public async Task<ApiResponse<object>> RefreshTokenAsync(string refreshToken)
		{
			try
			{
				var refreshTokenEntity = await _refreshTokenRepository.GetByTokenAsync(refreshToken);
				if (refreshTokenEntity == null || !refreshTokenEntity.IsActive)
				{
					return ApiResponse<object>.UnauthorizedResponse("Invalid refresh token");
				}
				// Generate new tokens
				var user = refreshTokenEntity.User;
				var roles = user.UserRoles.Select(ur => ur.Role.Name).ToArray();
				var permissions = user.UserRoles
					.SelectMany(ur => ur.Role.RolePermissions)
					.Select(rp => rp.Permission.Name)
					.Distinct()
					.ToArray();
				var newAccessToken = _tokenService.CreateAccessToken(user.Id, roles, permissions);
				var newRefreshToken = _tokenService.GenerateRefreshToken();
				// Revoke old refresh token
				refreshTokenEntity.IsRevoked = true;
				refreshTokenEntity.RevokedReason = "Replaced by new token";
				// Create new refresh token
				var newRefreshTokenEntity = new RefreshToken
				{
					Id = Guid.NewGuid(),
					Token = newRefreshToken,
					UserId = user.Id,
					ExpiryDateUtc = DateTime.UtcNow.AddDays(7),
					CreatedAtUtc = DateTime.UtcNow
				};
				// Update old token and add new one
				await _refreshTokenRepository.UpdateAsync(refreshTokenEntity);
				await _refreshTokenRepository.AddAsync(newRefreshTokenEntity);
				await _refreshTokenRepository.SaveChangesAsync();
				var tokenResponse = new 
				{ 
					accessToken = newAccessToken, 
					refreshToken = newRefreshToken
				};
				return ApiResponse<object>.SuccessResponse(tokenResponse, "Token refreshed successfully");
			}
			catch (Exception ex)
			{
				return ApiResponse<object>.InternalServerErrorResponse($"Token refresh failed: {ex.Message}");
			}
		}
		public async Task<ApiResponse<object>> AdminCreateUserAsync(AdminCreateUserCommand command)
		{
			try
			{
				// Check if email already exists
				if (await _userRepository.EmailExistsAsync(command.Email))
				{
					return ApiResponse<object>.BadRequestResponse("Email already exists");
				}
				// Create new user
				var user = new User
				{
					Id = Guid.NewGuid(),
					FullName = command.FullName,
					Email = command.Email,
					Gender = command.Gender,
					PhoneNumber = command.PhoneNumber,
					PasswordHash = _passwordHasher.Hash(command.DefaultPassword),
					IsActive = command.IsActive,
					RequirePasswordChange = true, // Force password change on first login
					CreatedAtUtc = DateTime.UtcNow
				};
				// Add user to database
				await _userRepository.AddAsync(user);
				// Assign roles to the new user
				if (command.Roles != null && command.Roles.Any())
				{
					var roleIds = await _roleRepository.GetRoleIdsByNamesAsync(command.Roles);
					if (roleIds.Any())
					{
						var userRoles = roleIds.Select(roleId => new UserRole
						{
							UserId = user.Id,
							RoleId = roleId
						}).ToArray();
						await _userRoleRepository.AddRangeAsync(userRoles);
						await _userRoleRepository.SaveChangesAsync();
					}
				}
				return ApiResponse<object>.CreatedResponse(new { userId = user.Id }, "User created successfully by admin");
			}
			catch (Exception ex)
			{
				return ApiResponse<object>.InternalServerErrorResponse($"User creation failed: {ex.Message}");
			}
		}
		public async Task<ApiResponse> LogoutAsync(string refreshToken)
		{
			try
			{
				var refreshTokenEntity = await _refreshTokenRepository.GetByTokenAsync(refreshToken);
				if (refreshTokenEntity != null && refreshTokenEntity.IsActive)
				{
					refreshTokenEntity.IsRevoked = true;
					refreshTokenEntity.RevokedReason = "Logged out by user";
					await _refreshTokenRepository.UpdateAsync(refreshTokenEntity);
					await _refreshTokenRepository.SaveChangesAsync();
				}
				return ApiResponse.SuccessResponse("Logged out successfully");
			}
			catch (Exception ex)
			{
				return ApiResponse.InternalServerErrorResponse($"Logout failed: {ex.Message}");
			}
		}
		public async Task<ApiResponse> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword)
		{
			try
			{
				var user = await _userRepository.GetByIdAsync(userId);
				if (user == null)
				{
					return ApiResponse.NotFoundResponse("User not found");
				}
				// Verify current password
				if (!_passwordVerifier.Verify(currentPassword, user.PasswordHash))
				{
					return ApiResponse.BadRequestResponse("Current password is incorrect");
				}
				// Validate new password is different
				if (currentPassword == newPassword)
				{
					return ApiResponse.BadRequestResponse("New password must be different from current password");
				}
				// Update password
				user.PasswordHash = _passwordHasher.Hash(newPassword);
				user.RequirePasswordChange = false;
				user.LastPasswordChangeUtc = DateTime.UtcNow;
				await _userRepository.UpdateAsync(user);
				await _userRepository.SaveChangesAsync();
				return ApiResponse.SuccessResponse("Password changed successfully");
			}
			catch (Exception ex)
			{
				return ApiResponse.InternalServerErrorResponse($"Password change failed: {ex.Message}");
			}
		}
		public async Task<ApiResponse<object>> ResetPasswordRequestAsync(string email)
		{
			try
			{
				var user = await _userRepository.GetByEmailAsync(email);
				if (user == null)
				{
					return ApiResponse<object>.NotFoundResponse("User not found");
				}
				// Generate reset token (8-character alphanumeric code)
				var resetToken = Guid.NewGuid().ToString("N")[..8].ToUpper();
				var expiryDate = DateTime.UtcNow.AddHours(1);
				var passwordResetToken = new PasswordResetToken
				{
					Id = Guid.NewGuid(),
					UserId = user.Id,
					Token = resetToken,
					ExpiryDateUtc = expiryDate,
					CreatedAtUtc = DateTime.UtcNow
				};
				await _passwordResetTokenRepository.AddAsync(passwordResetToken);
				await _passwordResetTokenRepository.SaveChangesAsync();
				// Return token in response (in production, this would be sent via email)
				var response = new
				{
					token = resetToken,
					expiresAt = expiryDate,
					message = "Password reset token generated successfully. In production, this would be sent via email."
				};
				return ApiResponse<object>.SuccessResponse(response, "Password reset token generated successfully");
			}
			catch (Exception ex)
			{
				return ApiResponse<object>.InternalServerErrorResponse($"Password reset request failed: {ex.Message}");
			}
		}
		public async Task<ApiResponse> ResetPasswordAsync(string token, string newPassword)
		{
			try
			{
				var resetToken = await _passwordResetTokenRepository.GetByTokenAsync(token);
				if (resetToken == null)
				{
					return ApiResponse.BadRequestResponse("Invalid reset token");
				}
				if (resetToken.IsUsed)
				{
					return ApiResponse.BadRequestResponse("Reset token has already been used");
				}
				if (resetToken.ExpiryDateUtc < DateTime.UtcNow)
				{
					return ApiResponse.BadRequestResponse("Reset token has expired");
				}
				// Get user
				var user = resetToken.User;
				if (user == null)
				{
					return ApiResponse.NotFoundResponse("User not found");
				}
				// Update password
				user.PasswordHash = _passwordHasher.Hash(newPassword);
				user.RequirePasswordChange = false;
				user.LastPasswordChangeUtc = DateTime.UtcNow;
				// Mark token as used
				resetToken.IsUsed = true;
				resetToken.UsedAtUtc = DateTime.UtcNow;
				await _userRepository.UpdateAsync(user);
				await _passwordResetTokenRepository.UpdateAsync(resetToken);
				await _userRepository.SaveChangesAsync();
				await _passwordResetTokenRepository.SaveChangesAsync();
				return ApiResponse.SuccessResponse("Password reset successfully. You can now login with your new password.");
			}
			catch (Exception ex)
			{
				return ApiResponse.InternalServerErrorResponse($"Password reset failed: {ex.Message}");
			}
		}
	}
}