using PRMDataManager.Library.Models;
using System.Collections.Generic;

namespace PRMDataManager.Library.DataAccess
{
    public interface IUserData
    {
        void CreateUser(UserModel user);
        List<UserModel> GetUserById(string Id);
    }
}