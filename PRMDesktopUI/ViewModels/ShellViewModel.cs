using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Microsoft.Extensions.DependencyInjection;
using PRMDesktopUI.Library.Api;
using PRMDesktopUI.Library.Models;
using PRMDesktopUI.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace PRMDesktopUI.ViewModels
{
    [ObservableObject]
    [ObservableRecipient]
    public partial class ShellViewModel : IRecipient<LogOnMessage>, IRecipient<ClosePageMessage>
    {
        private readonly ILoggedInUserModel _user;
        private readonly IAPIHelper _apiHelper;
        [ObservableProperty]
        private object? _selectedViewModel;

        [ObservableProperty]
        string _title = "Get ready to sell, sell, sell!";

        public ShellViewModel(ILoggedInUserModel loggedInUserModel, IAPIHelper apiHelper)
        {
            _user = loggedInUserModel;
            _apiHelper = apiHelper;

            // Ask for a new login viewmodel
            // We do not store the VM in any other location to prevent
            // credentials from being stored
            SelectedViewModel = App.GetRequiredService<LoginViewModel>();

            // Enable the Messenger
            Messenger = WeakReferenceMessenger.Default;

            // Begin listening for messages
            IsActive = true;
        }

        public void Receive(LogOnMessage message)
        {
            SelectedViewModel = App.GetRequiredService<SalesViewModel>();
            LogOutCommand.NotifyCanExecuteChanged();
            OnPropertyChanged(nameof(IsLoggedIn));
        }

        public void Receive(ClosePageMessage message)
        {
            SelectedViewModel = null;
        }

        [RelayCommand]
        private void ExitApplication()
        {
            Application.Current.Shutdown();
        }

        public bool IsLoggedIn => !string.IsNullOrEmpty(_user.Token);
        [RelayCommand(CanExecute = nameof(IsLoggedIn))]
        private void LogOut()
        {
            _user.ResetOffUser();
            _apiHelper.LogOffUser();
            SelectedViewModel = App.GetRequiredService<LoginViewModel>();
            LogOutCommand.NotifyCanExecuteChanged();
            OnPropertyChanged(nameof(IsLoggedIn));
        }

        [RelayCommand]
        private void RegisterUser()
        {
            SelectedViewModel = App.GetRequiredService<RegisterUserViewModel>();
        }

        [RelayCommand]
        private void UserManagement()
        {
            SelectedViewModel = App.GetRequiredService<UserDisplayViewModel>();
        }

        //private bool CanAccessUserManagement => _user.R
    }
}
