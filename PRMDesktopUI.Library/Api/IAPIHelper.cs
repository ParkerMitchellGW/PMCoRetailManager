using PRMDesktopUI.Library.Models;
using System.Security;
using System.Threading.Tasks;

namespace PRMDesktopUI.Library.Api
{
    public interface IAPIHelper
    {
        Task<AuthenticatedUser> Authenticate(string username, SecureString password);
        Task GetLoggedInUserInfo(string token);
    }
}