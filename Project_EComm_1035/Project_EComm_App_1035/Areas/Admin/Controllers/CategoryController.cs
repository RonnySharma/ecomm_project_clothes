using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Project_EComm_App_1035.DataAccess.Repositry.IRepositry;
using Project_EComm_App_1035.Model;
using Prooject_EComm_App_1035.Utility;

namespace Project_EComm_App_1035.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =SD.Role_Admin)]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _UnitOfWork;
        public CategoryController(IUnitOfWork unitOfWork)
        {
            _UnitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            return View();
        }
        #region APIs
        [HttpGet]
        public IActionResult GetAll() 
        {
        var CategoryList=_UnitOfWork.category.GetAll();
            return Json(new {data=CategoryList});
        }
        [HttpDelete]

        public IActionResult Delete(int id)
        {
            var CategoryInDb = _UnitOfWork.category.Get(id);
            if (CategoryInDb == null)
                return Json (new { success = false, message = "Something went wrong Delete Data" });
            _UnitOfWork.category.Remove(CategoryInDb);
            _UnitOfWork.save();
            return Json(new { success = true, message = "Data Successfully Deleted" });
        }
        #endregion
        public IActionResult upsert(int? id)
        {
            Category category=new Category();
            if(id==null)return View(category);//create
            //edit
            category=_UnitOfWork.category.Get(id.GetValueOrDefault());
            if(category==null) return NotFound();
            return View(category);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult upsert(Category category)
        {
            if(category==null) return NotFound();
            if(!ModelState.IsValid) return View(category);
            if (category.Id == 0)
                _UnitOfWork.category.Add(category);
            else
                _UnitOfWork.category.update(category);
            _UnitOfWork.save();
            return RedirectToAction(nameof(Index));
        }
        
    }
}
