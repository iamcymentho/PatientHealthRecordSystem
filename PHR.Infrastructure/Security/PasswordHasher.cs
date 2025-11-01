using BCrypt.Net;
using PHR.Application.Auth.Abstractions;
namespace PHR.Infrastructure.Security
{
	public class BcryptPasswordHasher : IPasswordHasher, IPasswordHasherAbstraction, IPasswordVerifier
	{
		public string Hash(string password)
		{
			return BCrypt.Net.BCrypt.HashPassword(password);
		}
		public bool Verify(string password, string hash)
		{
			return BCrypt.Net.BCrypt.Verify(password, hash);
		}
	}
}