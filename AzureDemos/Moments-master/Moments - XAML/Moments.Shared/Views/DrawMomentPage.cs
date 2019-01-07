﻿using System;
using System.Collections.Generic;
using System.IO;
using Xamarin.Forms;

namespace Moments
{
	public class DrawMomentPage : BasePage
	{
		MomentsPicker picker;
		Image cancelButton, clockButton, colorButton, sendButton;
		TapGestureRecognizer cancelButtonTapped, clockButtonTapped, colorButtonTapped, sendButtonTapped;
		RelativeLayout mainLayout;

		public DrawMomentPage (byte[] image)
		{
			BindingContext = new DrawMomentViewModel (image);

			SetupUserInterface ();
			SetupEventHandlers ();
		}

		private DrawMomentViewModel ViewModel
		{
			get { return BindingContext as DrawMomentViewModel; }
		}

		protected override void OnDisappearing ()
		{
			base.OnDisappearing ();

			cancelButton.IsVisible = true;
			clockButton.IsVisible = true;
			colorButton.IsVisible = true;
			sendButton.IsVisible = true;
		}

		private void SetupUserInterface ()
		{
			BackgroundColor = Color.Black;
			NavigationPage.SetHasNavigationBar (this, false);

			colorButton = new Image {
				Source = Images.Paintbrush
			};

			clockButton = new Image {
				Source = Images.ClockButton
			};

			sendButton = new Image {
				Source = Images.SendMomentButton
			};

			cancelButton = new Image {
				Source = Images.CancelButton,
				HorizontalOptions = LayoutOptions.Start,
				HeightRequest = 22,
				WidthRequest = 22
			};

			var moment = new Image {
				HorizontalOptions = LayoutOptions.FillAndExpand,
				VerticalOptions = LayoutOptions.FillAndExpand,
				Source = ImageSource.FromStream(() => new MemoryStream (ViewModel.Image))
			};

			var drawableMoment = new DrawableImage {
				BackgroundColor = Color.Transparent
			};
			drawableMoment.SetBinding (DrawableImage.CurrentLineColorProperty, "PenColor");

			mainLayout = new RelativeLayout () {
				Padding = new Thickness (0)
			};

			mainLayout.Children.Add (moment, 
				Constraint.Constant (0), 
				Constraint.Constant (0), 
				Constraint.RelativeToParent ((parent) => { return parent.Width; }),
				Constraint.RelativeToParent ((parent) => { return parent.Height; }));

			mainLayout.Children.Add (drawableMoment, 
				Constraint.Constant (0), 
				Constraint.Constant (0), 
				Constraint.RelativeToParent ((parent) => { return parent.Width; }),
				Constraint.RelativeToParent ((parent) => { return parent.Height; }));

			mainLayout.Children.Add (colorButton, 
				Constraint.RelativeToParent ((parent) => { return parent.Width - 40; }),
				Constraint.Constant (15),
				Constraint.Constant (30), 
				Constraint.Constant (30));

			mainLayout.Children.Add (cancelButton, 
				Constraint.RelativeToParent ((parent) => { return 15; }),
				Constraint.Constant (15),
				Constraint.Constant (30), 
				Constraint.Constant (30));

			mainLayout.Children.Add (clockButton, 
				Constraint.Constant (15),
				Constraint.RelativeToParent ((parent) => { return parent.Height - 40; }),
				Constraint.Constant (30), 
				Constraint.Constant (30));

			mainLayout.Children.Add (sendButton, 
				Constraint.RelativeToParent ((parent) => { return parent.Width - 40; }),
				Constraint.RelativeToParent ((parent) => { return parent.Height - 40; }),
				Constraint.Constant (30), 
				Constraint.Constant (30));

			picker = new MomentsPicker ();
			GetTimePickerItems ();
			SetupBindings ();
			mainLayout.Children.Add (picker, 				
				Constraint.Constant (15),
				Constraint.RelativeToParent ((parent) => { return parent.Height - 40; }),
				Constraint.Constant (30), 
				Constraint.Constant (30));

			Content = mainLayout;
		}

		private void SetupEventHandlers ()
		{
			cancelButtonTapped = new TapGestureRecognizer ();
			cancelButtonTapped.Tapped += async (object sender, EventArgs e) => {
				await App.Current.MainPage.Navigation.PopModalAsync ();
			};
			cancelButton.GestureRecognizers.Add (cancelButtonTapped);

			colorButtonTapped = new TapGestureRecognizer ();
			colorButtonTapped.Tapped += async (object sender, EventArgs e) => {
				await Navigation.PushAsync (new ColorPickerPage (ViewModel));
			};
			colorButton.GestureRecognizers.Add (colorButtonTapped);

			sendButtonTapped = new TapGestureRecognizer ();
			sendButtonTapped.Tapped += async (object sender, EventArgs e) => {
				cancelButton.IsVisible = false;
				clockButton.IsVisible = false;
				colorButton.IsVisible = false;
				sendButton.IsVisible = false;

				var image = DependencyService.Get<ScreenshotService> ().CaptureScreen ();
				await Navigation.PushAsync (new SendMomentPage (new MemoryStream (image), ViewModel.DisplayTime));
			};
			sendButton.GestureRecognizers.Add (sendButtonTapped);
		}

		private void SetupBindings ()
		{
			picker.SetBinding (Picker.SelectedIndexProperty, "SelectedIndex");
		}

		private void GetTimePickerItems ()
		{
			var items = new List<string> {
				Strings.ThreeSeconds,
				Strings.FourSeconds,
				Strings.FiveSeconds,
				Strings.SixSeconds,
				Strings.SevenSeconds,
				Strings.EightSeconds,
				Strings.NineSeconds,
				Strings.TenSeconds
			};

			foreach (var item in items) {
				picker.Items.Add (item);
			}
		}
	}
}