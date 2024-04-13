using ecomm_project_clothes.Dataaccess.Repository.IRepository;
using ecomm_project_clothes.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ecomm_project_clothes.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class BrandController : Controller
    {
        private readonly IUnitofwork _unitofwork;
        public BrandController(IUnitofwork unitofwork)
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
            var BrandInDb = _unitofwork.brand.Get(id);
            if (BrandInDb == null)
                return Json(new { success = false, Message = "Something Went Wrong While Deleting Data!!" });
            _unitofwork.brand.Remove(BrandInDb);
            _unitofwork.Save();
            return Json(new { success = true, Message = "Data Sucessfully Deleted!!" });

        }
        #region APIs
        [HttpGet]
        public IActionResult GetAll()
        {
            var brandList = _unitofwork.brand.GetAll();
            return Json(new { data = brandList });
        }
        #endregion
        public IActionResult Upsert(int? id)
        {
            Brand brand = new Brand();
            if (id == null) return View(brand);//Create
                                                  //Edit
            brand = _unitofwork.brand.Get(id.GetValueOrDefault());
            if (brand == null) return NotFound();
            return View(brand);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Upsert(Brand brand)
        {
            if (brand == null) return NotFound();
            if (!ModelState.IsValid) return View(brand);
            if (brand.id == 0) _unitofwork.brand.Add(brand);
            else
                _unitofwork.brand.Update(brand);
            _unitofwork.Save();
            return RedirectToAction(nameof(Index));
        }
    }

}
