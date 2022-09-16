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
using PRMDesktopUI.Library;
using PRMDesktopUI.Library.Models;
using System.ComponentModel.DataAnnotations;
using PRMDesktopUI.Services;

namespace PRMDesktopUI.ViewModels
{
    [ObservableObject]
    [ObservableRecipient]
    public partial class RegisterUserViewModel
    {
        private readonly IStatusInfoDisplay _statusInfo;
        private readonly IUserEndpoint _userEndpoint;

        //[ObservableProperty]
        //[NotifyCanExecuteChangedFor(nameof(SubmitCommand))]
        //private string _username = "";

        //[ObservableProperty]
        //[NotifyCanExecuteChangedFor(nameof(SubmitCommand))]
        //private SecureString _securePassword = new();

        //[ObservableProperty]
        //[NotifyPropertyChangedFor(nameof(IsErrorVisible))]
        //private string _errorMessage = "";

        //public bool IsErrorVisible => !string.IsNullOrEmpty(ErrorMessage);

        //[RelayCommand(CanExecute = nameof(CanSubmit))]
        //private async Task Submit()
        //{
        //    ErrorMessage = "";
        //    try
        //    {
        //        var result = await _apiHelper.Authenticate(Username, SecurePassword);

        //        //Capture more information about the user
        //        await _apiHelper.GetLoggedInUserInfo(result.Access_Token);
        //        Messenger.Send(new LogOnMessage());
        //    }
        //    catch(Exception ex) 
        //    { 
        //        ErrorMessage = ex.Message;
        //        Trace.WriteLine(ex.Message);
        //    }
        //}

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SubmitCommand))]
        private SecureString _securePassword = new();

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SubmitCommand))]
        private SecureString _confirmPassword = new();

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SubmitCommand))]
        private string _firstName = "";

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SubmitCommand))]
        private string _lastName = "";

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SubmitCommand))]
        private string _emailAddress = "";


        public RegisterUserViewModel(IStatusInfoDisplay statusInfo, IUserEndpoint userEndpoint)
        {
            Messenger = WeakReferenceMessenger.Default;
            _statusInfo = statusInfo;
            _userEndpoint = userEndpoint;
        }
        private bool CanSubmit => true;
            //!string.IsNullOrEmpty(FirstName) &&
            //!string.IsNullOrEmpty(LastName) &&
            //!string.IsNullOrEmpty(EmailAddress) &&
            //_securePassword.Length > 0 &&
            //_confirmPassword.Length > 0;

        [RelayCommand(CanExecute = nameof(CanSubmit))]
        private async Task Submit()
        {
            var user = new CreateUserModel
            {
                FirstName = FirstName,
                LastName = LastName,
                EmailAddress = EmailAddress,
                Password = SecurePassword.ToPlainString(),
                ConfirmPassword = ConfirmPassword.ToPlainString()
            };
            if (!Validate(user, out var results))
            {
                _statusInfo.ShowMessage(string.Join('\n', results), "User Registration Failed", "User Registration Failed");
            }
            else
            {
                try
                {
                    await _userEndpoint.CreateUser(user);
                }
                catch(Exception ex)
                {
                    _statusInfo.ShowMessage(ex.Message, "Failed to create user", "User Registration Failed");
                }
                
            }
        }

        static bool Validate<T>(T obj, out ICollection<ValidationResult> results) where T : notnull
        {
            results = new List<ValidationResult>();

            return Validator.TryValidateObject(obj, new ValidationContext(obj), results, true);
        }
    }
}
