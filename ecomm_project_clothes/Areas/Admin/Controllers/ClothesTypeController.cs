using Dapper;
using ecomm_project_clothes.Dataaccess.Repository.IRepository;
using ecomm_project_clothes.Model;
using ecomm_project_clothes.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ecomm_project_clothes.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class ClothesTypeController : Controller
    {
        private readonly IUnitofwork _unitofwork;
        public ClothesTypeController(IUnitofwork unitofwork)
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
            var clothesTypeInDb = _unitofwork.clothesType.Get(id);
            if (clothesTypeInDb == null)
                return Json(new { success = false, Message = "Something Went Wrong While Deleting Data!!" });

            //   _unitofwork.clothesType.Remove(clothesTypeInDb);
            // _unitofwork.Save();
            var param = new DynamicParameters();
            param.Add("id", id);
            _unitofwork.SPCALL.Execute(SD.proc_DeleteClothesTypes, param);

            return Json(new { success = true, Message = "Data Successfully Deleted!!" });
        }

        #region APIs
        [HttpGet]
        public IActionResult GetAll() 
        {
            return Json(new {data=_unitofwork.SPCALL.List<clothesType>(SD.Proc_GetClothesTypes) });
        }
        #endregion
        public IActionResult Upsert(int? id)
        {
            clothesType clothesType = new clothesType();
            if (id == null) return View(clothesType);

            // clothesType = _unitofwork.clothesType.Get(id.GetValueOrDefault());
            var param = new DynamicParameters();
            param.Add("id",id.GetValueOrDefault());
            clothesType = _unitofwork.SPCALL.OneRecord<clothesType>(SD.Proc_GetClothesType,param);
            if (clothesType == null) return NotFound();
            return View(clothesType);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Upsert (clothesType clothesType)
        {
            if (clothesType == null) return NotFound();
            if (!ModelState.IsValid) return View(clothesType);
           
            var param=new DynamicParameters();
            param.Add("name",clothesType.Name);
            if (clothesType.id == 0)// _unitofwork.clothesType.Add(clothesType);
            {
                _unitofwork.SPCALL.Execute(SD.Proc_CreateClothesTypes, param);
            }

            else
            {
                param.Add("id", clothesType.id);
                _unitofwork.SPCALL.Execute(SD.proc_UpdateClothesTypes, param);
            }
            // param.Add("@id", clothesType.id);
            // _unitofwork.SPCALL.Execute(SD.proc_UpdateClothesTypes, param);
            // _unitofwork.clothesType.Update(clothesType);

            _unitofwork.Save();
            return RedirectToAction(nameof(Index));
        }
    }
}
 