using Explayer.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace Explayer.ViewModels
{
    class MainPageViewModel : INotifyPropertyChanged
    {
        private readonly IAppManagerService _appManager;

        public MainPageViewModel(IAppManagerService appManager)
        {
            _appManager = appManager;
            AppList = _appManager.GetInstalledAppNames();
        }

        List<string> appList;
        public List<string> AppList
        {
            get { return appList; }
            set
            {
                if (appList != value)
                {
                    appList = value;
                    OnPropertyChanged();
                }
            }
        }

        string selectedApp;
        public string SelectedApp
        {
            get { return selectedApp; }
            set
            {
                if (selectedApp != value)
                {
                    selectedApp = value;
                    OnPropertyChanged();
                }
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
