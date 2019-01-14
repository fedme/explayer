using CommonServiceLocator;
using Explayer.Services;
using Explayer.ViewModels;
using System;
using System.Collections.Generic;
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
            //this.BindingContext = ServiceLocator.Current.GetInstance<MainViewModel>();
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

   
    }


}