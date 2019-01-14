using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Explayer.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MainPage : ContentPage
	{
		public MainPage ()
		{
			InitializeComponent ();
		}

        async void OnAppLaunchButtonClicked(object sender, EventArgs args)
        {
            await Navigation.PushAsync(new AppLaunchPage());
        }

        async void OnAppInstallButtonClicked(object sender, EventArgs args)
        {
            await Navigation.PushAsync(new DownloadAppPage());
        }


    }
}