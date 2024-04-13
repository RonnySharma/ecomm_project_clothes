using ecomm_project_clothes.Dataaccess.Repository;
using ecomm_project_clothes.Dataaccess.Repository.IRepository;
using ecomm_project_clothes.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;

namespace ecomm_project_clothes.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class CompanyController : Controller
    {
        private readonly IUnitofwork _unitofwork;
            public CompanyController(IUnitofwork unitofwork)
        {
            _unitofwork = unitofwork;
        }
        public IActionResult Index()
        {
            return View();
        }
        #region
        [HttpGet]
        public IActionResult GetAll() 
        {
            return Json(new { data = _unitofwork.company.GetAll() });
        }
        [HttpDelete] public IActionResult Delete(int id)
        {
            var CompanyinDb = _unitofwork.company.Get(id);
            if (CompanyinDb != null)
                return Json(new { success = false, message = "Something went wrong while deleting data!!" });
            _unitofwork.company.Remove(CompanyinDb);
            _unitofwork.Save();
            return Json(new { success = true, Message = "Data Deleted Successfully" });
            
        }
        #endregion
        public IActionResult Upsert(int? id)
        {
            Company company=new Company();
            if (id==null) return View(company);
            company=_unitofwork.company.Get(id.GetValueOrDefault());
            if (company == null) return NotFound();
            return View(company);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Upsert(Company company) 
        {
            if (company == null)return BadRequest();
            if (!ModelState.IsValid)return View(company);
            if (company.Id==0)
                _unitofwork.company.Add(company);
            else
                _unitofwork.company.Update(company);
            _unitofwork.Save();
            return RedirectToAction(nameof(Index));
        }
    }
}
