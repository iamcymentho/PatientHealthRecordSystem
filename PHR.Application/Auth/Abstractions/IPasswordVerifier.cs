namespace PHR.Application.Auth.Abstractions
{
	public interface IPasswordVerifier
	{
		bool Verify(string password, string hash);
	}
}