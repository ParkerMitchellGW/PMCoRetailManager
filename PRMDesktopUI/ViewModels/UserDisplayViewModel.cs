using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using PRMDesktopUI.Library.Api;
using PRMDesktopUI.Library.Models;
using PRMDesktopUI.Messages;
using PRMDesktopUI.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRMDesktopUI.ViewModels
{
    [ObservableObject]
    [ObservableRecipient]
    public partial class UserDisplayViewModel : IReceiveViewEvents
    {
        private readonly IStatusInfoDisplay _statusInfo;
        private readonly IUserEndpoint _userEndpoint;

        [ObservableProperty]
        public BindingList<UserModel> _users;

        public UserDisplayViewModel(IStatusInfoDisplay statusInfo, IUserEndpoint userEndpoint)
        {
            _statusInfo = statusInfo;
            _userEndpoint = userEndpoint;
            Messenger = WeakReferenceMessenger.Default;
        }

        public async void OnViewLoaded(object view)
        {
            try
            {
                await LoadUsers();
            }
            catch (Exception ex)
            {
                if (ex.Message == "Unauthorized")
                {
                    _statusInfo.ShowMessage("You do not have permission to access this.", "Unauthorized", "System Error");
                }
                else
                {
                    _statusInfo.ShowMessage(ex.Message, "Fatal Exception");
                }
                Messenger.Send<ClosePageMessage>();
            }
        }

        private async Task LoadUsers()
        {
            var userList = await _userEndpoint.GetAll();
            Users = new(userList);
        }
    }
}
