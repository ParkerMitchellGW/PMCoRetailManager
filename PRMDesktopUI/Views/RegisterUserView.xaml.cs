using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PRMDesktopUI.Views
{
    /// <summary>
    /// Interaction logic for RegisterUserView.xaml
    /// </summary>
    public partial class RegisterUserView : UserControl
    {
        public RegisterUserView()
        {
            InitializeComponent();
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext == null) return;
            ((dynamic)DataContext).SecurePassword = ((PasswordBox)sender).SecurePassword;
        }

        private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (DataContext == null) return;
            ((dynamic)DataContext).ConfirmPassword = ((PasswordBox)sender).SecurePassword;
        }
    }
}
