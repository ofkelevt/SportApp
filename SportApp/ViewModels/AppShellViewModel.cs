using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportApp.ViewModels
{
    class AppShellViewModel: ViewModel
    {
        private bool isLoggedIn;
        public bool IsLoggedIn 
        { get { return isLoggedIn; } 
            set 
            {
                isLoggedIn = value;
               OnPropertyChanged(nameof(IsLoggedIn)); 
               OnPropertyChanged(nameof(IsntLoggedIn));
            } 
        }
        public bool IsntLoggedIn { get { return !isLoggedIn; }}
        public AppShellViewModel()
        {
            IsLoggedIn = false;
        }
    }
}
