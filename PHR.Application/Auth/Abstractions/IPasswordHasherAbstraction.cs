namespace PHR.Application.Auth.Abstractions
{
	public interface IPasswordHasherAbstraction
	{
		string Hash(string password);
	}
}