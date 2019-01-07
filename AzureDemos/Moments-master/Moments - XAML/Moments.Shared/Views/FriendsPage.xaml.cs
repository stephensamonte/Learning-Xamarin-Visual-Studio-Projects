using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace Moments
{
	public partial class FriendsPage : BasePage
	{
		public FriendsPage ()
		{
			BindingContext = new FriendsViewModel ();

			InitializeComponent ();

			SetupToolbar ();
			SetupEventHandlers ();
		}

		private FriendsViewModel ViewModel
		{
			get { return BindingContext as FriendsViewModel; }
		}

		private void SetupToolbar ()
		{
			ToolbarItems.Add (new ToolbarItem {
				Icon = Images.AddFriendButton,
				Command = new Command (() => Navigation.PushModalAsync (new AddFriendPage (), true))
			});

			if (Device.OS != TargetPlatform.iOS) {
				ToolbarItems.Add (new ToolbarItem {
					Icon = Images.FriendRequestsButton,
					Command = new Command (() => Navigation.PushModalAsync (new NavigationPage (new FriendRequestsPage ()) {
						BarBackgroundColor = Colors.NavigationBarColor,
						BarTextColor = Colors.NavigationBarTextColor
					}, true)),
					Priority = 1
				});
			}
		}

		private void SetupEventHandlers ()
		{
			friendsListView.ItemSelected += (s, e) => {
				friendsListView.SelectedItem = null;
			};

			friendsListView.Refreshing += (sender, e) => {
				friendsListView.IsRefreshing = true;
				ViewModel.FetchFriendsCommand.Execute (null);
				friendsListView.IsRefreshing = false;
			};
		}
	}
}

