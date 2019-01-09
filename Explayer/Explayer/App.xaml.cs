using Acr.UserDialogs;
using CommonServiceLocator;
using Explayer.Server;
using Explayer.Services;
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
        private readonly IHandleStaticFilesService _staticFilesService;
        private readonly IAppManagerService _appManagerService;

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
            _staticFilesService = ServiceLocator.Current.GetService<IHandleStaticFilesService>();
            _appManagerService = ServiceLocator.Current.GetService<IAppManagerService>();

            MainPage = new NavigationPage(new MainPage());
        }

        /// <summary>
        /// Start point of the application
        /// </summary>
        protected override async void OnStart()
        {
            // Start local web server
            _localServer.Start();
        }


        /// <summary>
        /// Registers services in the service container
        /// </summary>
        private void RegisterServices()
        {
            var unityContainer = new UnityContainer();

            // register any service you like with the scheme interface, service
            unityContainer.RegisterType<ILocalServer, LocalServer>();
            unityContainer.RegisterType<IAppManagerService, AppManagerService>();
            unityContainer.RegisterInstance<IHandleStaticFilesService>(Xamarin.Forms.DependencyService.Get<IHandleStaticFilesService>());          

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
