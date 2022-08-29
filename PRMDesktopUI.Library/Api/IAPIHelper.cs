using PRMDesktopUI.Library.Models;
using System.Net.Http;
using System.Security;
using System.Threading.Tasks;

namespace PRMDesktopUI.Library.Api
{
    public interface IAPIHelper
    {
        HttpClient ApiClient { get; }
        Task<AuthenticatedUser> Authenticate(string username, SecureString password);
        Task GetLoggedInUserInfo(string token);
        public void LogOffUser();
    }
}