using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PRMDesktopUI.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRMDesktopUI.ViewModels
{
    [ObservableObject]
    public partial class StatusInfoViewModel
    {
        private string? _header;
        public string? Header { get => _header; private set => SetProperty(ref _header, value); }
        
        private string? _message;
        public string? Message { get => _message; private set => SetProperty(ref _message, value); }
        public void UpdateMessage(string? header, string? message)
        {
            Header = header;
            Message = message;
        }
        public StatusInfoViewModel(string? header = null, string? message = null) 
        {
            _header = header;
            _message = message;
        }
        [RelayCommand]
        public void Close(object view)
        {
            if(view is ICloseable window)
            {
                window.Close();
            }
        }
    }
}
