using CommonServiceLocator;
using Explayer.Services;
using Explayer.ViewModels;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Explayer
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MainPage : ContentPage
	{
        private readonly IAppManagerService _appManager;
        public List<string> appNames;

        public MainPage ()
		{
            NavigationPage.SetHasNavigationBar(this, false);
            _appManager = ServiceLocator.Current.GetService<IAppManagerService>();
            InitializeComponent ();

            this.BindingContext = new MainPageViewModel(_appManager);
        }

        async void OnDownloadAppButtonClicked(object sender, EventArgs args)
        {
            await Navigation.PushAsync(new DownloadAppPage());
        }

        async void OnOpenWebViewButtonClicked(object sender, EventArgs args)
        {
            await Navigation.PushAsync(new PlayerPage());
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

   
    }


}