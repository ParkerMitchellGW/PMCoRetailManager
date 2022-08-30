using PRMDesktopUI.ViewModels;
using PRMDesktopUI.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PRMDesktopUI.Services
{
    public class StatusInfoDisplay : IStatusInfoDisplay
    {
        public void ShowMessage(string message, string? header = "", string? title = "")
        {
            StatusInfoViewModel model = new(header, message);

            StatusInfoView messageBox = new() { DataContext = model, Owner = App.Current.MainWindow, Title = title };

            messageBox.ShowDialog();
        }
    }
}
