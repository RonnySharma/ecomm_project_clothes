using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Project_EComm_App_1035.DataAccess.Repositry;
using Project_EComm_App_1035.DataAccess.Repositry.IRepositry;
using Project_EComm_App_1035.Model.ViewModels;
using Project_EComm_App_1035.Model;
using System.Diagnostics;
using ShoppingCart = Project_EComm_App_1035.Model.ShoppingCart;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Prooject_EComm_App_1035.Utility;
using Microsoft.AspNetCore.Http;

namespace Project_EComm_App_1035.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitofwork;

        public HomeController(ILogger<HomeController> logger,IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitofwork = unitOfWork;
        }

        public IActionResult Index()
        {
            var claimsIdentity=(ClaimsIdentity) User.Identity;
            var claim=claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if(claim!=null)
            {
                var count = _unitofwork.shoppingCart.GetAll(SC => SC.ApplicationUserId == claim.Value).ToList().Count;
                HttpContext.Session.SetInt32(SD.ss_CartSessionCount,count);
            }
            var productlist = _unitofwork.product.GetAll(includeproperties: "Category,CoverType");
            return View(productlist);
        }
        [HttpPost]
        public IActionResult Index(string search)
        {
            var productlist = _unitofwork.product.GetAll(includeproperties: "Category,CoverType").ToList();

            if (search != null)
            {
                productlist = productlist.Where(m => m.Title.Contains(search, StringComparison.OrdinalIgnoreCase) || m.Author.Contains(search, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            return View(productlist);
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
            var productInDb = _unitofwork.product.FirstorDefault(p => p.Id == id, includeproperties:"Category,CoverType");
            if (productInDb == null) return NotFound();
            var ShoppingCart = new ShoppingCart()
            {
                Product = productInDb,
                ProductId = productInDb.Id
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
                var ClaimIdentity =(ClaimsIdentity) User.Identity;
                var claim = ClaimIdentity.FindFirst(ClaimTypes.NameIdentifier);
                shoppingCart.ApplicationUserId = claim.Value;

                var ShoppingCartfromDb = _unitofwork.shoppingCart.FirstorDefault(sc => sc.ApplicationUserId == claim.Value && sc.ProductId == shoppingCart.ProductId);
                if (ShoppingCartfromDb == null)
                    _unitofwork.shoppingCart.Add(shoppingCart);
                else     
                    ShoppingCartfromDb.Count += shoppingCart.Count;        
                _unitofwork.save();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                var productInDb = _unitofwork.product.FirstorDefault(p => p.Id ==shoppingCart.ProductId,includeproperties: "Category,CoverType");
                if (productInDb == null) return NotFound();
                var ShoppingCart = new ShoppingCart()
                {
                    Product = productInDb,
                    ProductId = productInDb.Id
                };
                return View(ShoppingCart);
            }
        }
    }
}