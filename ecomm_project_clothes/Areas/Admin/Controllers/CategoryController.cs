using ecomm_project_clothes.Dataaccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using ecomm_project_clothes.Model;
using Microsoft.AspNetCore.Authorization;

namespace ecomm_project_clothes.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly IUnitofwork _unitofwork;

        public CategoryController(IUnitofwork unitofwork)
        {
            _unitofwork = unitofwork;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var categoryInDb=_unitofwork.category.Get(id);
            if (categoryInDb == null)
                return Json(new { success = false, Message = "Something Went Wrong While Deleting Data!!" });
            _unitofwork.category.Remove(categoryInDb);
            _unitofwork.Save();
            return Json(new { success = true, Message = "Data Sucessfully Deleted!!" });

        }

        #region APIs
        [HttpGet]
        public IActionResult GetAll()
        {
            var categoryList = _unitofwork.category.GetAll();
            return Json(new { data = categoryList });
        }
        #endregion
        public IActionResult Upsert(int? id)
        {
            Category catagory = new Category();
            if (id == null) return View(catagory);//Create
            //Edit
            catagory = _unitofwork.category.Get(id.GetValueOrDefault());
            if (catagory == null) return NotFound();
            return View(catagory);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Upsert(Category category)
        {
            if (category == null) return NotFound();    
            if(!ModelState.IsValid) return View(category);
            if (category.id==0)_unitofwork.category.Add(category);
            else
                _unitofwork.category.Update(category);
            _unitofwork.Save();
            return RedirectToAction(nameof(Index));
        }
    }
}
