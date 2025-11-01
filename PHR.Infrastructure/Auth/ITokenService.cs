using System;
using System.Collections.Generic;
namespace PHR.Infrastructure.Auth
{
	public interface ITokenService
	{
		string CreateAccessToken(Guid userId, IEnumerable<string> roles, IEnumerable<string> permissions);
	}
}