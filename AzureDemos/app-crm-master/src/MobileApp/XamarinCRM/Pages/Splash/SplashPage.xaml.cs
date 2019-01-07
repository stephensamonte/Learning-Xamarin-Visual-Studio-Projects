﻿
using System;
using Xamarin.Forms;
using System.Threading.Tasks;
using XamarinCRM.Services;
using XamarinCRM.Statics;
using XamarinCRM.Pages.Base;
using XamarinCRM.ViewModels.Splash;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Xamarin;

namespace XamarinCRM.Pages.Splash 
{
    public partial class SplashPage : SplashPageXaml
    {
        readonly IAuthenticationService _AuthenticationService;

        public SplashPage()
        {
            InitializeComponent();

            BindingContext = new SplashViewModel();
            _AuthenticationService = DependencyService.Get<IAuthenticationService>();

            SignInButton.GestureRecognizers.Add(
                new TapGestureRecognizer()
                { 
                    NumberOfTapsRequired = 1, 
                    Command = new Command(SignInButtonTapped) 
                });

            SkipSignInButton.GestureRecognizers.Add(
                new TapGestureRecognizer()
                { 
                    NumberOfTapsRequired = 1, 
                    Command = new Command(SkipSignInButtonTapped) 
                });
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            // fetch the demo credentials
            await ViewModel.LoadDemoCredentials();

            // pause for a moment before animations
            await Task.Delay(App.AnimationSpeed);

            // Sequentially animate the login buttons. ScaleTo() makes them "grow" from a singularity to the full button size.
            await SignInButton.ScaleTo(1, (uint)App.AnimationSpeed, Easing.SinIn);
            await SkipSignInButton.ScaleTo(1, (uint)App.AnimationSpeed, Easing.SinIn);
        }

        async Task<bool> Authenticate()
        {
            bool success = false;
            try
            {
                // The underlying call behind App.Authenticate() calls the ADAL library, which presents the login UI and awaits success.
                success = await _AuthenticationService.AuthenticateAsync();
            }
            catch (Exception ex)
            {
                if (ex is AdalException && (ex as AdalException).ErrorCode == "authentication_canceled")
                {
                    // Do nothing, just duck the exception. The user just cancelled out of the login UI.
                }
                else
                    await DisplayAlert("Login error", "An unknown login error has occurred. Please try again.", "OK");
            }
            finally
            {
                // When the App.Authenticate() returns, the login UI is hidden, regardless of success (for example, if the user taps "Cancel" in iOS).
                // This means the SplashPage will be visible again, so we need to make the sign in button clickable again by hiding the activity indicator (via the IsPresentingLoginUI property).
                ViewModel.IsPresentingLoginUI = false;
            }

            return success;
        }

        async void SignInButtonTapped()
        {
            await App.ExecuteIfConnected(async () =>
                {
                    // trigger the activity indicator through the IsPresentingLoginUI property on the ViewModel
                    ViewModel.IsPresentingLoginUI = true;

                    if (await Authenticate())
                    {
                        // Pop off the modally presented SplashPage.
                        // Note that we're not popping the ADAL auth UI here; that's done automatcially by the ADAL library when the Authenticate() method returns.
                        App.GoToRoot();

                        // Broadcast a message that we have sucessdully authenticated.
                        // This is mostly just for Android. We need to trigger Android to call the SalesDashboardPage.OnAppearing() method,
                        // because unlike iOS, Android does not call the OnAppearing() method each time that the Page actually appears on screen.
                        MessagingCenter.Send(this, MessagingServiceConstants.AUTHENTICATED);
                    }
                });
        }

        async void SkipSignInButtonTapped()
        {
            _AuthenticationService.BypassAuthentication();

            // Broadcast a message that we have sucessdully authenticated.
            // This is mostly just for Android. We need to trigger Android to call the SalesDashboardPage.OnAppearing() method,
            // because unlike iOS, Android does not call the OnAppearing() method each time that the Page actually appears on screen.
            MessagingCenter.Send(this, MessagingServiceConstants.AUTHENTICATED);

            App.GoToRoot();
        }
    }

    /// <summary>
    /// This class definition just gives us a way to reference ModelBoundContentPage<T> in the XAML of this Page.
    /// </summary>
    public abstract class SplashPageXaml : ModelBoundContentPage<SplashViewModel>
    {
    }
}

