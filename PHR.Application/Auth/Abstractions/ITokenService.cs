using System;
namespace PHR.Application.Auth.Abstractions
{
	public interface ITokenService
	{
		string CreateAccessToken(Guid userId, string[] roles, string[] permissions);
		string GenerateRefreshToken();
	}
}