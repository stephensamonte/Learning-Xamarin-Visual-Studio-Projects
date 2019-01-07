using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Linq;
using System.Diagnostics;

namespace Sport.Mobile.Shared
{
	public partial class RankStripView : ContentView
	{
		public RankStripView()
		{
			InitializeComponent();
			root.BindingContext = this;
		}

		public static readonly BindableProperty MembershipProperty =
			BindableProperty.Create(nameof(Membership), typeof(Membership), typeof(RankStripView), null);

		public Membership Membership
		{
			get
			{
				return (Membership)GetValue(MembershipProperty);
			}
			set
			{
				SetValue(MembershipProperty, value);
				LocalRefresh();
			}
		}

		public Color DarkColor
		{
			get
			{
				return Membership?.League?.Theme?.Dark ?? Color.Transparent;
				//return Membership == null || Membership.League == null || Membership.League.Theme == null ?
    //               Color.Transparent : Membership.League.Theme.Light.AddLuminosity(-.07);
			}
		}

		public Color LightColor
		{
			get
			{
				return Membership?.League?.Theme?.Light ?? Color.Transparent;
				//return Membership == null || Membership.League == null || Membership.League.Theme == null ?
    //               Color.Transparent : Membership.League.Theme.Light.AddLuminosity(.07);
			}
		}

		public Membership UpperMembership
		{
			get;
			private set;
		}

		public Membership LowerMembership
		{
			get;
			private set;
		}

		public Action<Membership> OnAthleteClicked
		{
			get;
			set;
		}

		public Action<Athlete> OnChallengeClicked
		{
			get;
			set;
		}

		void HandleChallengeClicked(object sender, EventArgs e)
		{
			var btn = sender as Button;
			OnChallengeClicked?.Invoke((btn.BindingContext as Membership)?.Athlete);
		}

		void HandleAthleteClicked(object sender, EventArgs e)
		{
			var btn = sender as RoundedRectangleView;
			OnAthleteClicked?.Invoke(btn.CommandParameter as Membership);
		}

		void LocalRefresh()
		{
			OnPropertyChanged(nameof(Membership));
			OnPropertyChanged(nameof(DarkColor));
			OnPropertyChanged(nameof(LightColor));

			if(Membership != null)
			{
				UpperMembership = Membership.League.Memberships.SingleOrDefault(m => m.CurrentRank == Membership.CurrentRank - 1);
				LowerMembership = Membership.League.Memberships.SingleOrDefault(m => m.CurrentRank == Membership.CurrentRank + 1);
			}
			else
			{
				UpperMembership = null;
				LowerMembership = null;
			}

			OnPropertyChanged(nameof(UpperMembership));
			OnPropertyChanged(nameof(LowerMembership));
		}
	}
}