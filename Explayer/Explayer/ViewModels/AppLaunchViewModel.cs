using Acr.UserDialogs;
using Explayer.Models;
using Explayer.Services;
using Explayer.Views;
using System.Collections.ObjectModel;
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
            LaunchAppMessage = "Please select an app (and version)";
        }

        public void Refresh()
        {
            _webApps = new ObservableCollection<WebApp>(_appManager.GetInstalledApps());
            LaunchAppMessage = "Please select an app (and version)";
            InstalledVersions = new ObservableCollection<string>();
        }

        ObservableCollection<WebApp> _webApps;
        public ObservableCollection<WebApp> WebApps
        {
            get { return _webApps; }
            set
            {
                _webApps = value;
                OnPropertyChanged();
            }
        }

        WebApp _selectedWebApp;
        public WebApp SelectedWebApp
        {
            get { return _selectedWebApp; }
            set
            {
                if (_selectedWebApp != value)
                {
                    // TODO: check selected app not null
                    _selectedWebApp = value;
                    InstalledVersions = new ObservableCollection<string>(_selectedWebApp.InstalledVersions);
                    SelectedWebAppVersion = _selectedWebApp.PreferredVersion;
                    LaunchAppMessage = $"Launch {SelectedWebApp.Name} v{SelectedWebApp.PreferredVersion}";
                    OnPropertyChanged();
                }
            }
        }

        ObservableCollection<string> _installedVersions;
        public ObservableCollection<string> InstalledVersions
        {
            get { return _installedVersions; }
            set
            {
                _installedVersions = value;
                OnPropertyChanged();
            }
        }

        string _selectedWebAppVersion;
        public string SelectedWebAppVersion
        {
            get { return _selectedWebAppVersion; }
            set
            {
                if (_selectedWebAppVersion != value)
                {
                    _selectedWebAppVersion = value;
                    SelectedWebApp.PreferredVersion = value;
                    LaunchAppMessage = $"Launch {SelectedWebApp.Name} v{SelectedWebApp.PreferredVersion}";
                    OnPropertyChanged();
                }
            }
        }

        string _launchAppMessage;
        public string LaunchAppMessage
        {
            get { return _launchAppMessage; }
            set
            {
                if (_launchAppMessage != value)
                {
                    _launchAppMessage = value;
                    OnPropertyChanged();
                }
            }
        }


        Command _launchCommand;
        public Command LaunchAppCommand
        {
            get
            {
                return _launchCommand ?? (_launchCommand = new Command(async () => await LaunchApp()));
            }
        }

        private async Task LaunchApp()
        {
            if (SelectedWebApp == null || string.IsNullOrEmpty(SelectedWebAppVersion))
            {
                // Show toast
                var toastConfig = new ToastConfig("Please choose the app (and version) to run!").SetDuration(4000);
                UserDialogs.Instance.Toast(toastConfig);
            }
            else
            {
                await Navigation.PushAsync(new AppPlayerPage(SelectedWebApp.Name, SelectedWebApp.PreferredVersion));
            }
        }
    }
}
