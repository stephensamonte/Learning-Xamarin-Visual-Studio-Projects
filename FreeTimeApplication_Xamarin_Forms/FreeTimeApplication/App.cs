using System;

using Xamarin.Forms;

using System.Threading.Tasks; // This is for user authentication 

namespace FreeTimeApplication
{
	public class App : Application
	{
		public App ()
		{
			// The root page of your application
			MainPage = new TodoList();
		}

        // This is for user authentication 
        public interface IAuthenticate
        {
            Task<bool> Authenticate();
        }

        protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}


        // This is for user authentication 
        public static IAuthenticate Authenticator { get; private set; }

        // This is for user authentication 
        public static void Init(IAuthenticate authenticator)
        {
            Authenticator = authenticator;
        }
    }
}

