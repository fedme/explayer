using CommonServiceLocator;
using Explayer.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Explayer.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class AppLaunchPage : ContentPage
	{

        public AppLaunchPage ()
		{
            InitializeComponent ();
            //BindingContext = ServiceLocator.Current.GetInstance<AppLaunchViewModel>();
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            // Refresh list of installed apps
            var vm = BindingContext as AppLaunchViewModel;
            vm.Refresh();
        }


    }


}