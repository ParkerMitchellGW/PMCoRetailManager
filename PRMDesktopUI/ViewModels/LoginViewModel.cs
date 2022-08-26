using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PRMDesktopUI.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace PRMDesktopUI.ViewModels
{
    [ObservableObject]
    public partial class LoginViewModel
    {
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SubmitCommand))]
        private string _username = "";

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SubmitCommand))]
        private SecureString _securePassword;
        
        private readonly IAPIHelper _apiHelper;

        [RelayCommand(CanExecute = nameof(CanSubmit))]
        private async Task Submit()
        {
            try
            {
                var result = await _apiHelper.Authenticate(Username, SecurePassword);
            }
            catch(Exception ex)
            {
                Trace.WriteLine(ex.Message);
            }
        }

        public LoginViewModel(IAPIHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }

        private bool CanSubmit => Username.Length > 0 && SecurePassword?.Length > 0;
    }
}
