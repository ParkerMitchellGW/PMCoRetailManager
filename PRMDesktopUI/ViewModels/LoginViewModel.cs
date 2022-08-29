using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using PRMDesktopUI.Messages;
using PRMDesktopUI.Library.Api;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace PRMDesktopUI.ViewModels
{
    [ObservableObject]
    [ObservableRecipient]
    public partial class LoginViewModel
    {
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SubmitCommand))]
        private string _username = "";

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SubmitCommand))]
        private SecureString _securePassword = new();

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(IsErrorVisible))]
        private string _errorMessage = "";

        public bool IsErrorVisible => !string.IsNullOrEmpty(ErrorMessage);
        
        private readonly IAPIHelper _apiHelper;

        [RelayCommand(CanExecute = nameof(CanSubmit))]
        private async Task Submit()
        {
            ErrorMessage = "";
            try
            {
                var result = await _apiHelper.Authenticate(Username, SecurePassword);

                //Capture more information about the user
                await _apiHelper.GetLoggedInUserInfo(result.Access_Token);
                Messenger.Send(new LogOnMessage());
            }
            catch(Exception ex) 
            { 
                ErrorMessage = ex.Message;
                Trace.WriteLine(ex.Message);
            }
        }

        public LoginViewModel(IAPIHelper apiHelper)
        {
            _apiHelper = apiHelper;
            Messenger = WeakReferenceMessenger.Default;
            InitCredentials();
        }

        [Conditional("DEBUG")]
        private void InitCredentials()
        {
            Username = "Parker@PMCo.com";
            SecurePassword = new NetworkCredential("", "Pwd123!").SecurePassword;
        }

        private bool CanSubmit => Username.Length > 0 && SecurePassword?.Length > 0;
    }
}
