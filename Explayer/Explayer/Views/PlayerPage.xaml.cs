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
	public partial class PlayerPage : ContentPage
	{
		public PlayerPage ()
		{
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent ();
		}

        private void OnGoButtonClicked(object sender, EventArgs e)
        {
            webview.Source = addressEntry.Text;
        }

        protected override bool OnBackButtonPressed()
        {
            Device.BeginInvokeOnMainThread(async () => {
                var result = await this.DisplayAlert("Leave Session?", "You will lose all unsaved data from this session. Click anywhere to dismiss this message and remain here.", "Yes, exit and lose all data", "No, stay here");
                if (result) await this.Navigation.PopAsync(); // or anything else
            });

            return true;
        }
    }
}