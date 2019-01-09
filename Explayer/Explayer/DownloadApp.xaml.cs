using Acr.UserDialogs;
using Explayer.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Explayer
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DownloadApp : ContentPage
    {
        private readonly IAppManagerService _appManager;

        public DownloadApp()
        {
            InitializeComponent();
        }

        public DownloadApp(IAppManagerService appManager)
        {
            _appManager = appManager;
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