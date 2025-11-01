using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PHR.Application.Auth.Abstractions;
using ApplicationITokenService = PHR.Application.Auth.Abstractions.ITokenService;
namespace PHR.Infrastructure.Auth
{
	public class TokenService : ITokenService, ApplicationITokenService, ITokenIssuer
	{
		private readonly JwtOptions _options;
		public TokenService(IOptions<JwtOptions> options)
		{
			_options = options.Value;
		}
		public string CreateAccessToken(Guid userId, string[] roles, string[] permissions)
		{
			return CreateAccessToken(userId, (IEnumerable<string>)roles, (IEnumerable<string>)permissions);
		}
		public string CreateAccessToken(Guid userId, IEnumerable<string> roles, IEnumerable<string> permissions)
		{
			var claims = new List<Claim>
			{
				new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
			};
			foreach (var role in roles)
			{
				claims.Add(new Claim(ClaimTypes.Role, role));
			}
			foreach (var perm in permissions)
			{
				claims.Add(new Claim("permission", perm));
			}
			return GenerateJwtToken(claims, _options.ExpirationMinutes);
		}
		public string Issue(Guid userId)
		{
			var claims = new List<Claim>
			{
				new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
			};
			return GenerateJwtToken(claims, _options.ExpirationMinutes);
		}
		public string GenerateRefreshToken()
		{
			var randomNumber = new byte[32];
			using var rng = RandomNumberGenerator.Create();
			rng.GetBytes(randomNumber);
			return Convert.ToBase64String(randomNumber);
		}
		public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
		{
			var tokenValidationParameters = new TokenValidationParameters
			{
				ValidateAudience = true,
				ValidateIssuer = true,
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SigningKey)),
				ValidateLifetime = false, // Don't validate lifetime for refresh
				ValidIssuer = _options.Issuer,
				ValidAudience = _options.Audience
			};
			var tokenHandler = new JwtSecurityTokenHandler();
			var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
			if (securityToken is not JwtSecurityToken jwtSecurityToken || 
			    !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
			{
				throw new SecurityTokenException("Invalid token");
			}
			return principal;
		}
		private string GenerateJwtToken(IEnumerable<Claim> claims, int expirationMinutes)
		{
			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SigningKey));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
			var token = new JwtSecurityToken(
				issuer: _options.Issuer,
				audience: _options.Audience,
				claims: claims,
				expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
				signingCredentials: creds
			);
			return new JwtSecurityTokenHandler().WriteToken(token);
		}
	}
}