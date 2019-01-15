using Acr.UserDialogs;
using Explayer.Models;
using Explayer.Services;
using Explayer.Views;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Explayer.ViewModels
{
    public class AppLaunchViewModel : BaseViewModel
    {
        private readonly IAppManagerService _appManager;

        public AppLaunchViewModel(IAppManagerService appManager)
        {
            _appManager = appManager;
            _webApps = new ObservableCollection<WebApp>(_appManager.GetInstalledApps());
            InstalledVersions = new ObservableCollection<string>();
            LaunchAppMessage = "Please select an app (and version)";
        }

        public void Refresh()
        {
            _webApps.Clear();
            _appManager.GetInstalledApps().ForEach(app => _webApps.Add(app));
            // app preferred version is lost here!
       
            LaunchAppMessage = "Please select an app (and version)";

            if (_appManager.GetCurrentWebApp() != null)
            {
                SelectedWebApp = _webApps.FirstOrDefault(
                    app => app.Name == _appManager.GetCurrentWebApp()?.Name );
            }
            
        }

        private ObservableCollection<WebApp> _webApps;
        public ObservableCollection<WebApp> WebApps
        {
            get => _webApps;
            set
            {
                _webApps = value;
                OnPropertyChanged();
            }
        }

        private WebApp _selectedWebApp;
        public WebApp SelectedWebApp
        {
            get => _selectedWebApp;
            set
            {
                if (_selectedWebApp == value) return;
                _selectedWebApp = value;

                if (_selectedWebApp != null)
                {
                    InstalledVersions.Clear();
                    SelectedWebApp.InstalledVersions.ForEach(version => InstalledVersions.Add(version));
//                    SelectedWebAppVersion = InstalledVersions.FirstOrDefault(
//                        version => version == SelectedWebApp.PreferredVersion);
                    SelectedWebAppVersion = InstalledVersions.First(); // TODO: select preferred version instead
                    LaunchAppMessage = $"Launch {SelectedWebApp.Name} v{SelectedWebApp.PreferredVersion}";
                }

                OnPropertyChanged();
            }
        }

        private ObservableCollection<string> _installedVersions;
        public ObservableCollection<string> InstalledVersions
        {
            get => _installedVersions;
            set
            {
                _installedVersions = value;
                OnPropertyChanged();
            }
        }

        private string _selectedWebAppVersion;
        public string SelectedWebAppVersion
        {
            get => _selectedWebAppVersion;
            set
            {
                if (_selectedWebAppVersion == value) return;
                _selectedWebAppVersion = value;
                if (_selectedWebApp != null)
                {
                    LaunchAppMessage = $"Launch {_selectedWebApp.Name} v{value}";
                }
                OnPropertyChanged();
            }
        }

        private string _launchAppMessage;
        public string LaunchAppMessage
        {
            get => _launchAppMessage;
            set
            {
                if (_launchAppMessage == value) return;
                _launchAppMessage = value;
                OnPropertyChanged();
            }
        }


        private Command _launchCommand;
        public Command LaunchAppCommand
        {
            get
            {
                return _launchCommand ?? (_launchCommand = new Command(
                    async () => await LaunchApp()));
            }
        }

        private async Task LaunchApp()
        {
            if (SelectedWebApp == null || string.IsNullOrEmpty(SelectedWebAppVersion))
            {
                // Show validation error toast
                var toastConfig = new ToastConfig("Please choose the app (and version) to run!")
                    .SetDuration(4000);
                UserDialogs.Instance.Toast(toastConfig);
            }
            else
            {
                // Remember app version and set it as current app
                SelectedWebApp.PreferredVersion = SelectedWebAppVersion;
                _appManager.SetCurrentWebApp(SelectedWebApp);
                
                // Navigate to web app player
                await Navigation.PushAsync(new AppPlayerPage(
                    SelectedWebApp.Name, SelectedWebApp.PreferredVersion));
            }
        }
    }
}
