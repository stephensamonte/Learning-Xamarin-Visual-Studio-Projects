using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace _2017._10._27_CrossPlatform1.Droid
{
    [Activity(Label = "_2017._10._27_CrossPlatform1.Android", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        MySharedClass shared = new MySharedClass();

		protected override void OnCreate (Bundle bundle)
		{
			base.OnCreate (bundle);

			// Set our view from the "main" layout resource
			SetContentView (Resource.Layout.Main);

			// Get our button from the layout resource,
			// and attach an event to it
			Button button = FindViewById<Button> (Resource.Id.myCountButton);
			
			button.Click += delegate {
                button.Text = shared.IncreaseCount();
			};
		}

    }
}


