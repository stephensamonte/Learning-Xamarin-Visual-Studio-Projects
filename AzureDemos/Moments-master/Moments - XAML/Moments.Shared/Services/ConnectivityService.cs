using System;
using System.Threading.Tasks;

using Connectivity.Plugin;

namespace Moments
{
	public class ConnectivityService
	{
		public static async Task<bool> IsConnected ()
		{
			return await CrossConnectivity.Current.IsRemoteReachable (Keys.ApplicationMobileService, 80, 5000);
		}
	}
}

