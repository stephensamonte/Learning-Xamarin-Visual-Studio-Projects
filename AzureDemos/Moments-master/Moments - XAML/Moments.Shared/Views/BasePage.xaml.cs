using System;
using System.Collections.ObjectModel;

using Xamarin.Forms;

namespace Moments
{
	public partial class BasePage : ContentPage
	{
		public ObservableCollection<ToolbarItem> LeftToolbarItems { get; set; }

		public BasePage ()
		{
			InitializeComponent ();

			LeftToolbarItems = new ObservableCollection<ToolbarItem> ();

			SetBinding (Page.TitleProperty, new Binding (BaseViewModel.TitlePropertyName));
			SetBinding (Page.IconProperty, new Binding (BaseViewModel.IconPropertyName));
			SetBinding (Page.IsBusyProperty, new Binding (BaseViewModel.IsBusyPropertyName));
		}
	}
}

