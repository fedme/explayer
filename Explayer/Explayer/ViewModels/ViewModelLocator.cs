using CommonServiceLocator;
using System;
using System.Collections.Generic;
using System.Text;

namespace Explayer.ViewModels
{
    public class ViewModelLocator
    {
        public AppLaunchViewModel AppLaunchViewModel
        {
            get { return ServiceLocator.Current.GetInstance<AppLaunchViewModel>(); }
        }
    }
}
