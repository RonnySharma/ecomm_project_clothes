using ecomm_project_clothes.Dataaccess.Repository;
using ecomm_project_clothes.Dataaccess.Repository.IRepository;
using ecomm_project_clothes.Model;
using ecomm_project_clothes.Model.ViewModels;
using ecomm_project_clothes.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace ecomm_project_clothes.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitofwork _unitofwork;
        public HomeController(ILogger<HomeController> logger, IUnitofwork unitofwork)
        {
            _logger = logger;
            _unitofwork = unitofwork;
        }
        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim != null)
            {
                var count = _unitofwork.shoppingCart.GetAll(sc => sc.ApplicationUserId == claim.Value).ToList().Count;
                HttpContext.Session.SetInt32(SD.Ss_CartsessionCount, count);
            }

            var productsWithOrderCounts = _unitofwork.product.GetAll(IncludeProperties: "Category,ClothesType,Brand")
                .Select(p => new ProductWithOrderCountViewModel
                {
                    ProductId = p.Id,
                    Name = p.Name,
                    Brand = p.Brand.Name,
                    ImgUrl = p.ImgUrl,
                    ListPrice = p.ListPrice,
                    Price75 = p.Price75,

                    OrderCount = _unitofwork.orderDetail
                        .GetAll(od => od.ProductId == p.Id)
                        .Sum(od => od.Count), // Use Sum() to calculate the total count

                    // Add a Quantity property to display the available quantity
                    Quantity = p.Quantity
                })
                .OrderByDescending(p => p.OrderCount)
                .ToList();

            return View(productsWithOrderCounts);
        }

        //public IActionResult Index()
        //{
        //    var claimsIdentity = (ClaimsIdentity)User.Identity;
        //    var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

        //    if (claim != null)
        //    {
        //        var count = _unitofwork.shoppingCart.GetAll(sc => sc.ApplicationUserId == claim.Value).ToList().Count;
        //        HttpContext.Session.SetInt32(SD.Ss_CartsessionCount, count);
        //    }

        //    var productsWithOrderCounts = _unitofwork.product.GetAll(IncludeProperties: "Category,ClothesType,Brand")
        //        .Select(p => new ProductWithOrderCountViewModel
        //        {
        //            ProductId = p.Id,
        //            Name = p.Name,
        //            Brand = p.Brand.Name,
        //            ImgUrl = p.ImgUrl,
        //            ListPrice = p.ListPrice,
        //            Price75 = p.Price75,

        //            OrderCount = _unitofwork.orderDetail
        //                .GetAll(od => od.ProductId == p.Id)
        //                .Sum(od => od.Count) // Use Sum() to calculate the total count
        //        })
        //        .OrderByDescending(p => p.OrderCount)
        //        .ToList();

        //    return View(productsWithOrderCounts);
        //}


        //public IActionResult Index()
        //{
        //    var claimsIdentity = (ClaimsIdentity)User.Identity;
        //    var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
        //    if (claim != null)
        //    {
        //        var count = _unitofwork.shoppingCart.GetAll(sc => sc.ApplicationUserId == claim.Value).ToList().Count;
        //        HttpContext.Session.SetInt32(SD.Ss_CartsessionCount, count);
        //    }
        //    var ProductList = _unitofwork.product.GetAll(IncludeProperties: "Category,ClothesType,Brand");
        //    return View(ProductList);
        //}

        //public IActionResult Index()
        //{
        //    var claimsIdentity=(ClaimsIdentity)User.Identity;
        //    var claim=claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
        //    if (claim!= null)
        //    {
        //        var count=_unitofwork.shoppingCart.GetAll(sc=>sc.ApplicationUserId==claim.Value).ToList().Count;
        //        HttpContext.Session.SetInt32(SD.Ss_CartsessionCount, count);
        //    }
        //    var ProductList = _unitofwork.product.GetAll(IncludeProperties: "Category,ClothesType,Brand");
        //    return View(ProductList);
        //}
        [HttpPost]
        public IActionResult Index(string search)
        {
            var ProductList = _unitofwork.product.GetAll(IncludeProperties: "Category,ClothesType,Brand").ToList();
            if(search!=null)
            {
                // ProductList = ProductList.Where(p => p.Brand.Name.ToString() == search).ToList();
                //ProductList = ProductList.Where(p => p.Brand.Name.Contains(search)).ToList();
                //ProductList = ProductList.Where(p => p.Brand.Name.Contains(search) || p.ClothesType.Name.Contains(search)).ToList();
                ProductList = ProductList.Where(p => p.Brand.Name.Contains(search, StringComparison.OrdinalIgnoreCase) || p.ClothesType.Name.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();

            }


            return View(ProductList);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult Details(int id)
        {
            var ProductinDb = _unitofwork.product.FirstOrDefault(
                p => p.Id == id, IncludeProperties: "Category,ClothesType,Brand");
            if (ProductinDb == null) return NotFound();
            var ShoppingCart = new ShoppingCart()
            {
                Product = ProductinDb,
                ProductId = ProductinDb.Id,
            };
            return View(ShoppingCart);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            shoppingCart.Id = 0;
            if(ModelState.IsValid)
            {
                var ClaimIdentity=(ClaimsIdentity)User.Identity;
                var Claim=ClaimIdentity.FindFirst(ClaimTypes.NameIdentifier);
                if (Claim==null) return NotFound();
                shoppingCart.ApplicationUserId = Claim.Value;
                var ShoppingCartFromDb=_unitofwork.shoppingCart.FirstOrDefault(
                    Sc=>Sc.ApplicationUserId==Claim.Value && Sc.ProductId==shoppingCart.ProductId);
                if (ShoppingCartFromDb == null) _unitofwork.shoppingCart.Add(shoppingCart);
                else
                    ShoppingCartFromDb.Count += shoppingCart.Count;
                _unitofwork.Save();
                return RedirectToAction(nameof(Index));

            }
            else
            {
                var ProductinDb = _unitofwork.product.FirstOrDefault(
               p => p.Id == shoppingCart.ProductId,
               IncludeProperties: "Category,ClothesType,Brand");
                if (ProductinDb == null) return NotFound();
                var ShoppingCartEdit = new ShoppingCart()
                {
                    Product = ProductinDb,
                    ProductId = ProductinDb.Id,
                };
                return View(ShoppingCartEdit);
            }
        }
        public IActionResult Trending()
        {
            var productByCounts = _unitofwork.orderDetail.GetAll(IncludeProperties: "Product").Where(x => x.Product.Active == true).OrderByDescending(x => x.Count).GroupBy(x => x.ProductId);
            List<Product> products = new List<Product>();
            foreach (var laptop in productByCounts)
            {
                var any = _unitofwork.product.FirstOrDefault(x => x.Id == laptop.Key);
                products.Add(any);
            }
            return View("Index", products);
        }
    }
}