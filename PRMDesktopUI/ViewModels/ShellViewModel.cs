using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using CommunityToolkit.Mvvm.Messaging.Messages;
using Microsoft.Extensions.DependencyInjection;
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
    public partial class ShellViewModel : IRecipient<LogOnMessage>
    {
        private readonly SalesViewModel _salesViewModel;

        [ObservableProperty]
        private object _selectedViewModel;

        [ObservableProperty]
        string _title = "Get ready to sell, sell, sell!";

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public ShellViewModel(SalesViewModel salesViewModel)
        {
            _salesViewModel = salesViewModel;

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
            SelectedViewModel = _salesViewModel;
        }
    }
}
