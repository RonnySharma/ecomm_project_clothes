using ecomm_project_clothes.Dataaccess.Repository.IRepository;
using ecomm_project_clothes.Model;
using ecomm_project_clothes.Model.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ecomm_project_clothes.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class ProductController : Controller
    {
        private readonly IUnitofwork _unitofwork;
        public readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitofwork unitofwork, IWebHostEnvironment webHostEnvironment)

        {
            _unitofwork = unitofwork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {

            return View();
        }
        //Delete
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var ProductInDb = _unitofwork.product.Get(id);
            if (ProductInDb == null)
                return Json(new { success = false, Message = "Something Went Wrong While Deleting Data!!" });

            //Image Delete Code
            var webrootpath=_webHostEnvironment.WebRootPath;
            var imgpath = Path.Combine(webrootpath, ProductInDb.ImgUrl.Trim('\\'));
            if(System.IO.File.Exists(imgpath))
            {
                System.IO.File.Delete(imgpath);
            }
            _unitofwork.product.Remove(ProductInDb);
            _unitofwork.Save();
            return Json(new { success = true, Message = "data deleted sucessfully" });
        }
        #region APIs
        [HttpGet]
        public IActionResult GetAll()
        {
            var ProductList = _unitofwork.product.GetAll(IncludeProperties: "Category,ClothesType");
            return Json(new { data = ProductList });

        }
        #endregion
        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new ProductVM()
            {
                Product = new Product(),
                CategoryList = _unitofwork.category.GetAll().Select(cl => new SelectListItem()
                {
                    Text = cl.Name,
                    Value = cl.id.ToString()
                }),
                ClothesTypeList = _unitofwork.clothesType.GetAll().Select(ct => new SelectListItem()
                {
                    Text = ct.Name,
                    Value = ct.id.ToString()
                }),
                BrandsTypeList = _unitofwork.brand.GetAll().Select(bt => new SelectListItem()
                {
                    Text = bt.Name,
                    Value = bt.id.ToString()
                })
            };
            if (id == null) return View(productVM);//create
                                                   //edit
            
            productVM.Quantity = productVM.Product.Quantity; // Set the quantity from the Product to the view model

            productVM.Product = _unitofwork.product.Get(id.GetValueOrDefault());
            return View(productVM);
        }
        [HttpPost]
       // [AutoValidateAntiforgeryToken]

        //public IActionResult Upsert(ProductVM productVM)
        //{
        //    if (ModelState.IsValid)
        //    {

        //        if (productVM.Product.Id == 0)
        //        {
        //            // Create a new product
        //            productVM.Product.Quantity = productVM.Quantity; // Set the quantity from the view model
        //            _unitofwork.product.Add(productVM.Product);
        //        }
        //        else
        //        {
        //            // Edit an existing product
        //            var existingProduct = _unitofwork.product.Get(productVM.Product.Id);
        //            if (existingProduct != null)
        //            {
        //                existingProduct.Name = productVM.Product.Name;
        //                // Update other properties as needed
        //                existingProduct.Quantity = productVM.Quantity; // Update the quantity
        //            }
        //        }

        //        _unitofwork.Save(); // Save changes to the database
        //        return RedirectToAction(nameof(Index));
        //    }

        //    // If ModelState is not valid, return to the Upsert view with validation errors
        //    productVM.CategoryList = _unitofwork.category.GetAll().Select(cl => new SelectListItem()
        //    {
        //        Text = cl.Name,
        //        Value = cl.id.ToString()
        //    });
        //    productVM.ClothesTypeList = _unitofwork.clothesType.GetAll().Select(ct => new SelectListItem()
        //    {
        //        Text = ct.Name,
        //        Value = ct.id.ToString()
        //    });
        //    productVM.BrandsTypeList = _unitofwork.brand.GetAll().Select(bt => new SelectListItem()
        //    {
        //        Text = bt.Name,
        //        Value = bt.id.ToString()
        //    });

        //    return View(productVM);
        //}


        public IActionResult Upsert(ProductVM productVM)
        {
            if (ModelState.IsValid)
            {
                var webRootPath = _webHostEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;
                if (files.Count() > 0)
                {
                    var fileName = Guid.NewGuid().ToString();
                    var extension = Path.GetExtension(files[0].FileName);
                    var upload = Path.Combine(webRootPath, @"Images\Product");
                    if (productVM.Product.Id != 0)
                    {
                        var imageExists = _unitofwork.product.Get(productVM.Product.Id).ImgUrl;
                        productVM.Product.ImgUrl = imageExists;
                    }
                    if (productVM.Product.ImgUrl != null)
                    {
                        var imagepath = Path.Combine(webRootPath, productVM.Product.ImgUrl.Trim('\\'));
                        if (System.IO.File.Exists(imagepath))
                        {
                            System.IO.File.Delete(imagepath);
                        }
                    }
                    using (var filestream = new FileStream(Path.Combine(upload, fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(filestream);
                    }
                    productVM.Product.ImgUrl = @"\Images\Product\" + fileName + extension;
                }
                else
                {
                    if (productVM.Product.Id != 0)
                    {
                        var imageexists = _unitofwork.product.Get(productVM.Product.Id).ImgUrl;
                        productVM.Product.ImgUrl = imageexists;
                    }
                }
                
                if (productVM.Product.Id == 0)
                    _unitofwork.product.Add(productVM.Product);
                else
                    _unitofwork.product.Update(productVM.Product);
                _unitofwork.Save();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                productVM = new ProductVM()
                {
                    Product = new Product(),
                    CategoryList = _unitofwork.category.GetAll().Select(cl => new SelectListItem()
                    {
                        Text = cl.Name,
                        Value = cl.id.ToString(),
                    }),
                    ClothesTypeList = _unitofwork.clothesType.GetAll().Select(ct => new SelectListItem()
                    {
                        Text = ct.Name,
                        Value = ct.id.ToString(),
                    }),
                    BrandsTypeList = _unitofwork.brand.GetAll().Select(bt => new SelectListItem()
                    {
                        Text = bt.Name,
                        Value = bt.id.ToString(),
                    })
                };
                if (productVM.Product.Id != 0)
                {
                    productVM.Product = _unitofwork.product.Get(productVM.Product.Id);
                }
                return View(productVM);
            }
        }
    }
}

