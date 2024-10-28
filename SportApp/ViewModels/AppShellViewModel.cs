using SportApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

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
        private bool isAdmin;
        public bool IsAdmin { get => isAdmin; set { isAdmin = value; OnPropertyChanged(nameof(IsAdmin)); Refresh(); } }
        public bool IsntLoggedIn { get { return !isLoggedIn; }}
        public AppShellViewModel()
        {
            IsLoggedIn = false;
        }
        public void Refresh()
        {
            OnPropertyChanged(nameof(IsLoggedIn));
            OnPropertyChanged(nameof(IsntLoggedIn));
            OnPropertyChanged("IsAdmin");
        }
    }
}
