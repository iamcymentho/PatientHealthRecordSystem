using System;
namespace PHR.Application.Auth.Abstractions
{
	public interface ITokenIssuer
	{
		string Issue(Guid userId);
	}
}