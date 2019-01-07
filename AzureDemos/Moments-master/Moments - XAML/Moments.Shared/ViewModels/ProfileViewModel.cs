using System;
using System.Net;
using Xamarin.Forms;
using Refractored.Xam.Settings;

namespace Moments
{
	public class ProfileViewModel : BaseViewModel
	{
		public string ProfileName
		{
			get { return CrossSettings.Current.GetValueOrDefault<string> ("profileName"); }
		}

		public string ProfileImageUrl
		{
			get { return CrossSettings.Current.GetValueOrDefault<string> ("profileImage");; }
		}
	}
}