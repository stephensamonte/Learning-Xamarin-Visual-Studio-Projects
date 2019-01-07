using System.Threading.Tasks;

namespace FreeTimeApplication
{
	public interface IAuthenticate
	{
		Task<bool> AuthenticateAsync ();

		Task<bool> LogoutAsync ();
	}
}
