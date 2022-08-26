using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRMDesktopUI.ViewModels
{
    [ObservableObject]
    public partial class ShellViewModel
    {
        private readonly LoginViewModel _loginViewModel;

        [ObservableProperty]
        private object _selectedViewModel;

        [ObservableProperty]
        string _title = "WE HERE MAYNE";

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public ShellViewModel(LoginViewModel loginViewModel)
        {
            SelectedViewModel = _loginViewModel = loginViewModel;
        }
    }
}
