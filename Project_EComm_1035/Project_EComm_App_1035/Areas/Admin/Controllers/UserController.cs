using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Operations;
using Project_EComm_App_1035.DataAccess.Data;
using Project_EComm_App_1035.DataAccess.Repositry.IRepositry;
using Project_EComm_App_1035.Model;
using Prooject_EComm_App_1035.Utility;

namespace Project_EComm_App_1035.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin+"," +(SD.Role_Employee))]
    public class UserController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _context;
        public UserController(IUnitOfWork unitOfWork, ApplicationDbContext context)
        {
                _context = context;
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }
        #region APIs
        [HttpGet]
        public IActionResult GetAll()
        {
            var userList=_context.applicationUsers.ToList();//AspNetUser
            var roles=_context.Roles.ToList();//AspNetRoles
            var userRoles=_context.UserRoles.ToList();//ASpNetRoles
            foreach (var user in userList)
            {
                var roleId = userRoles.FirstOrDefault(r => r.UserId == user.Id).RoleId;
                user.Role = roles.FirstOrDefault(r => r.Id == roleId).Name;
                if(user.CompanyId!=null)
                {
                    user.Company = new Company()
                    {
                        Name = _unitOfWork.company.Get(Convert.ToInt32(user.CompanyId)).Name
                    };
                }
                if(user.Company==null)
                {
                    user.Company = new Company()
                    {
                        Name = ""
                    };
                }
            }
            //Remove AdminUser
            var adminUser = userList.FirstOrDefault(u => u.Role == SD.Role_Admin);
            userList.Remove(adminUser);
            return Json(new {data= userList});
        }
        [HttpPost]
        public IActionResult LockUnLock([FromBody]string id)
        {
            bool isLocked=false;
            var userInDb=_context.applicationUsers.FirstOrDefault(u=>u.Id==id);
            if(userInDb==null) 
                return Json(new {success=false,message="Something Went Wrong While Lock Or UnLock User"});
            if(userInDb!=null && userInDb.LockoutEnd>DateTime.Now)
            {
                userInDb.LockoutEnd = DateTime.Now;
                isLocked = false;
            }
            else
            {
                userInDb.LockoutEnd = DateTime.Now.AddYears(100);
                isLocked = true;
            }
            _context.SaveChanges();
            return Json(new { success = true, message = isLocked == true ? "User Successfully Locked" : "User Successfully UnLock" });
        }

        #endregion
    }
}
