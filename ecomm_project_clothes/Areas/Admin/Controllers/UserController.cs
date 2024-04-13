using Amazon.Runtime.Internal.Util;
using ecomm_project_clothes.Dataaccess.Data;
using ecomm_project_clothes.Dataaccess.Repository.IRepository;
using ecomm_project_clothes.Model;
using ecomm_project_clothes.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.VisualBasic;

namespace ecomm_project_clothes.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =SD.role_Admin +",")]

    public class UserController : Controller
    {
        private readonly IUnitofwork _unitofwork;
        private readonly ApplicationDbContext _context;
        public UserController(IUnitofwork unitofwork, ApplicationDbContext context)
        {
            _context = context;
            _unitofwork = unitofwork;
        }

        public IActionResult Index()
        {
            return View();
        }
        #region APIs
        [HttpGet]
        public IActionResult GetAll()
        {
            var UserList = _context.ApplicationUsers.ToList();//ASPnetUsers
            var roles = _context.Roles.ToList();
            var UserRoles = _context.UserRoles.ToList(); //ASPNET User Roles
            foreach (var user in UserList)
            {
                var roleId = UserRoles.FirstOrDefault(r => r.UserId == user.Id).RoleId;
                user.Role = roles.FirstOrDefault(r => r.Id == roleId).Name;
                if (user.CompanyId != null)
                {
                    user.Company = new Company()
                    {
                        Name = _unitofwork.company.Get(Convert.ToInt32(user.CompanyId)).Name
                    };
                }
                if (user.Company == null)
                {
                    user.Company = new Company()
                    {
                        Name = ""
                    };
                }

            }
            //remove admin user from list
            var adminuser = UserList.FirstOrDefault(u => u.Role == SD.role_Admin);
            UserList.Remove(adminuser);
            return Json(new { data = UserList });
        }
        [HttpPost]
        public IActionResult LockUnLock ([FromBody]string id)
        {
            bool islocked=false;
            var UserInDb=_context.ApplicationUsers.FirstOrDefault(u =>u.Id==id);
            if (UserInDb == null)
                return Json(new { success = false, message = "Something went wrong" });
            if (UserInDb!= null && UserInDb.LockoutEnd>DateAndTime.Now)
            {
                UserInDb.LockoutEnd= DateAndTime.Now;
                islocked = false;
            }
            else
            {
                UserInDb.LockoutEnd=DateAndTime.Now.AddYears(100);
                islocked = true;
            }
            _context.SaveChanges();
            return Json(new { success = true, message = islocked == true ? "user Successfull locked":"user Successfully Unlocked"});
        }
        #endregion
    }
}
