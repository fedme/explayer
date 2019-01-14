using Acr.UserDialogs;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Explayer.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AppPlayerPage : ContentPage
	{

        public AppPlayerPage ()
		{
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent ();
		}

        public AppPlayerPage(string appName, string preferredVersion)
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();
            webview.Source = $"http://127.0.0.1:8787/{appName}/{preferredVersion}/index.html";

            // Show toast
            var toastConfig = new ToastConfig($"Loading {appName} v{preferredVersion}...").SetDuration(4000);
            UserDialogs.Instance.Toast(toastConfig);
        }

        //private void OnGoButtonClicked(object sender, EventArgs e)
        //{
        //    webview.Source = addressEntry.Text;
        //}

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