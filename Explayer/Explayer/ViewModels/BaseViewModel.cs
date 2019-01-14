using CommonServiceLocator;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using Xamarin.Forms;

namespace Explayer.ViewModels
{
    public abstract class BaseViewModel : INotifyPropertyChanged
    {

        protected INavigation Navigation => App.Current.MainPage.Navigation;

        protected BaseViewModel()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this,
                new PropertyChangedEventArgs(propertyName));
        }
    }

}
