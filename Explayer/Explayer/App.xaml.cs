using CommonServiceLocator;
using Explayer.Server;
using Explayer.Services;
using Explayer.ViewModels;
using Explayer.Views;
using Unity;
using Unity.ServiceLocation;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace Explayer
{
    public partial class App : Application
    {

        private readonly ILocalServer _localServer;

        /// <summary>
        /// App consturctor
        /// </summary>
        public App()
        {
            InitializeComponent();

            // Register Services
            RegisterServices();

            // Inject needed Services
            _localServer = ServiceLocator.Current.GetService<ILocalServer>();

            MainPage = new NavigationPage(new MainPage());
        }

        /// <summary>
        /// Start point of the application
        /// </summary>
        protected override async void OnStart()
        {
            // Start local web server
            await _localServer.Start();
        }


        /// <summary>
        /// Registers services in the service container
        /// </summary>
        private void RegisterServices()
        {
            var unityContainer = new UnityContainer();

            // register services
            unityContainer.RegisterType<ILocalServer, LocalServer>();
            unityContainer.RegisterInstance<IHandleStaticFilesService>(
                Xamarin.Forms.DependencyService.Get<IHandleStaticFilesService>());
            unityContainer.RegisterType<IAppManagerService, AppManagerService>();

            // register viewmodels
            unityContainer.RegisterSingleton<AppLaunchViewModel>();

            ServiceLocator.SetLocatorProvider(() =>
                new UnityServiceLocator(unityContainer));
        }


        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
