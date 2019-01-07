using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace Moments
{
	public partial class SignInPage : ContentPage
	{
		TapGestureRecognizer cancelButtonTapped;

		public SignInPage ()
		{
			BindingContext = new SignInViewModel ();

			InitializeComponent ();
			SetupEventHandlers ();
		}

		private void SetupEventHandlers ()
		{
			usernameEntry.Completed += (sender, e) => {
				passwordEntry.Focus ();
			};

			cancelButtonTapped = new TapGestureRecognizer ();
			cancelButtonTapped.Tapped += (object sender, EventArgs e) => {
				Navigation.PopModalAsync ();
			};
			cancelButton.GestureRecognizers.Add (cancelButtonTapped);
		}
	}
}

