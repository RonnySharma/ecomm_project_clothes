using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Project_EComm_App_1035.DataAccess.Repositry;
using Project_EComm_App_1035.DataAccess.Repositry.IRepositry;
using Project_EComm_App_1035.Model;
using Prooject_EComm_App_1035.Utility;

namespace Project_EComm_App_1035.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin+"," +(SD.Role_Employee))]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CompanyController(IUnitOfWork unitOfWork)
        {
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
                return Json(new { data = _unitOfWork.company.GetAll()});
                
            }
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var companyInDb = _unitOfWork.company.Get(id);
            if (companyInDb != null)
                return Json(new { success = false, message = "Something went wrong whilw data deleted" });
            _unitOfWork.company.Remove(companyInDb);
            _unitOfWork.save();
            return Json(new { success = true, message = "Data Deleted Successfully" });
        }
        #endregion
        public IActionResult Upsert(int? id)
        {
            Company company= new Company();
            if(id==null) return View(company);
            company=_unitOfWork.company.Get(id.GetValueOrDefault());
            if(company==null)return NotFound();
            return View(company);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Upsert(Company company)
        {
            if(company==null) return BadRequest();
            if(!ModelState.IsValid)return View (company);
            if(company.Id==0)
                _unitOfWork.company.Add(company);
            else
                _unitOfWork.company.update(company);
            _unitOfWork.save();
            return RedirectToAction(nameof(Index));
        } 
     }
 }

