using Acr.UserDialogs;
using CommonServiceLocator;
using Explayer.Services;
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
    public partial class DownloadAppPage : ContentPage
    {
        private readonly IAppManagerService _appManager;

        public DownloadAppPage()
        {
            _appManager = ServiceLocator.Current.GetService<IAppManagerService>();
            InitializeComponent();
        }

        async void OnDownloadButtonClicked(object sender, EventArgs args)
        {
            var appServerUrl = serverUrlEntry.Text;
            var appName = appNameEntry.Text;
            var appVersion = appVersionEntry.Text;

            using (var loading = UserDialogs.Instance.Loading("Downloading app..."))
            {
                loading.Show();
                // Download app
                await _appManager.DownloadApp(appServerUrl, appName, appVersion);
            }
        }
    }
}