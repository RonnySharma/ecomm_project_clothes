

using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Project_EComm_App_1035.DataAccess.Data;
using Project_EComm_App_1035.DataAccess.Repositry.IRepositry;
using Project_EComm_App_1035.Model;
using Prooject_EComm_App_1035.Utility;

namespace Project_EComm_App_1035.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles =SD.Role_Admin)]
    public class CoverTypeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CoverTypeController(IUnitOfWork unitOfWork)
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
            return Json(new { data = _unitOfWork.SPCALL.List<CoverType>(SD.Proc_GetCoverTypes) });
            //var covertypeList=_unitOfWork.coverType.GetAll();
            //return Json(new { data = covertypeList });
        }
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var param = new DynamicParameters();
            param.Add("id", id);

            //var covertypeInDb=_unitOfWork.coverType.Get(id);
            var covertypeInDb = _unitOfWork.SPCALL.OneRecord<CoverType>(SD.Proc_GetCoverType, param);
            if(covertypeInDb == null)
                return Json(new {success = false,message="something went wrong delete data"});
            //_unitOfWork.coverType.Remove(covertypeInDb);
            //_unitOfWork.save();
            _unitOfWork.SPCALL.Execute(SD.Proc_DeleteCoverType, param);
            return Json(new { success = true, message = "Data Delete Successfully" });
        }

        #endregion
        public IActionResult upsert(int? id) 
        { 
            CoverType coverType=new CoverType();
            if(id==null)return View(coverType);//create
            //edit
           // coverType=_unitOfWork.coverType.Get(id.GetValueOrDefault());
           var param=new DynamicParameters();
            param.Add("id", id);
            coverType=_unitOfWork.SPCALL.OneRecord<CoverType>(SD.Proc_GetCoverType, param); 
            if (coverType == null) return NotFound();
            return View(coverType);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult upsert(CoverType coverType)
        {
            if(coverType==null) return NotFound();
            if(!ModelState.IsValid) return View(coverType);
            var param=new DynamicParameters();
            param.Add("name",coverType.Name); 
            if (coverType.Id == 0)
               // _unitOfWork.coverType.Add(coverType);
               _unitOfWork.SPCALL.Execute(SD.Proc_CreateCoverType, param);
            else
            {
                param.Add("id",coverType.Id);
                _unitOfWork.SPCALL.Execute(SD.Proc_UpdateCoverType, param);
            }
                //_unitOfWork.coverType.update(coverType);
            //_unitOfWork.save();
            return RedirectToAction(nameof(Index));
        }
    }
}
