using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
        private BindingList<UserModel> _users = new();

        [ObservableProperty]
        private UserModel? _selectedUser;

        [ObservableProperty]
        private BindingList<string> _UserRoles = new();

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(RemoveSelectedRoleCommand))]
        private string? _roleToRemove;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AddSelectedRoleCommand))]
        private string? _roleToAdd;

        [ObservableProperty]
        private BindingList<string> _availableRoles = new();


        partial void OnSelectedUserChanged(UserModel? value)
        {
            if (SelectedUser is null)
            {
                UserRoles = new();
            }
            else
            {
                UserRoles = new(SelectedUser.Roles.Select(x => x.Value).ToList());
                _ = LoadRoles();
            }
            AddSelectedRoleCommand.NotifyCanExecuteChanged();
            RemoveSelectedRoleCommand.NotifyCanExecuteChanged();
        }

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

        private bool CanAddSelectedRole => SelectedUser is not null && RoleToAdd is not null;

        [RelayCommand(CanExecute = nameof(CanAddSelectedRole))]
        private async void AddSelectedRole()
        {
            await _userEndpoint.AddUserToRole(SelectedUser!.Id, RoleToAdd!);

            UserRoles.Add(RoleToAdd!);
            AvailableRoles.Remove(RoleToAdd!);
            OnPropertyChanged(nameof(Users));
        }
        private bool CanRemoveSelectedRole => SelectedUser is not null && RoleToRemove is not null;

        [RelayCommand(CanExecute = nameof(CanRemoveSelectedRole))]
        private async void RemoveSelectedRole()
        {
            await _userEndpoint.RemoveUserFromRole(SelectedUser!.Id, RoleToRemove!);

            AvailableRoles.Add(RoleToRemove!);

            UserRoles.Remove(RoleToRemove!);
            OnPropertyChanged(nameof(Users));
        }

        private async Task LoadRoles()
        {
            RoleToAdd = null;
            AvailableRoles.Clear();
            var roles = await _userEndpoint.GetAllRoles();
            foreach (var role in roles)
            {
                if (UserRoles.IndexOf(role.Value) == -1)
                {
                    AvailableRoles.Add(role.Value);
                }
            }
        }
    }
}
