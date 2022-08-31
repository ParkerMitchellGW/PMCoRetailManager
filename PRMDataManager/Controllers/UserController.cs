using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using PRMDataManager.Library.DataAccess;
using PRMDataManager.Library.Models;
using PRMDataManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace PRMDataManager.Controllers
{
    [Authorize]
    public class UserController : ApiController
    {
        [HttpGet]
        public UserModel GetById()
        {
            string userId = RequestContext.Principal.Identity.GetUserId();

            UserData data = new UserData();

            return data.GetUserById(userId).First();
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("Admin/GetAllUsers")]
        public void GetAllUsers()
        {
            using (var context = new ApplicationDbContext())
            {
                var userStore = new UserStore<ApplicationUser>(context);
                var userManager = new UserManager<ApplicationUser>(userStore);

                var users = userManager.Users.ToList();

                var roles = context.Roles.ToList();

                foreach(var user in users)
                {
                    var u = new ApplicationUserModel
                    {
                        Id = user.Id,
                        Email = user.Email
                    };
                }
            }
        }

    }
}
