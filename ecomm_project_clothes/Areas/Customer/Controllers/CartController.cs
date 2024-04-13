using ecomm_project_clothes.Dataaccess.Repository.IRepository;
using ecomm_project_clothes.Model;
using ecomm_project_clothes.Model.ViewModels;
using ecomm_project_clothes.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Collections.Generic;
using Stripe;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using ecomm_project_clothes.Dataaccess.Repository;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Encodings.Web;
using System.Text;
using System.Xml.Linq;
using Newtonsoft.Json;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace ecomm_project_clothes.Areas.Admin.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitofwork _unitOfWork;
        private static bool isEmailConfirm = false;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<IdentityUser> _userManager;
        public CartController(IUnitofwork unitOfWork, IEmailSender emailSender, UserManager<IdentityUser> userManager)
        {
            _unitOfWork = unitOfWork;
            _emailSender = emailSender;
            _userManager = userManager;
        }
        [BindProperty]
        public ShoppingCartVM ShoppingCartVM { get; set; }
        public IActionResult Index()
        {
            var clamisIdentity = (ClaimsIdentity)User.Identity;
            var claim = clamisIdentity.FindFirst(ClaimTypes.NameIdentifier);
            if (claim == null)
            {
                ShoppingCartVM = new ShoppingCartVM()
                {
                    ListCart = new List<ShoppingCart>()
                };
                return View(ShoppingCartVM);
            }
            var allCartItems = _unitOfWork.shoppingCart.GetAll(sc => sc.ApplicationUserId == claim.Value && !sc.IsRemoveFromCart, IncludeProperties: "Product").ToList();
            var itemsToDisplay = allCartItems.Where(item => !item.IsSelected).ToList();

            ShoppingCartVM = new ShoppingCartVM()
            {
                ListCart = itemsToDisplay,
                orderHeader = new OrderHeader()
            };

            //******
            //ShoppingCartVM = new ShoppingCartVM()
            //{
            //    //ListCart = _unitOfWork.shoppingCart.GetAll
            //    //(sc => sc.ApplicationUserId == claim.Value, IncludeProperties: "Product"),
            //    ListCart = _unitOfWork.shoppingCart.GetAll
            //    (sc => sc.ApplicationUserId == claim.Value, IncludeProperties: "Product").ToList(),
            //    orderHeader = new OrderHeader()
            //};
            ShoppingCartVM.orderHeader.OrderTotal = 0;
            ShoppingCartVM.orderHeader.ApplicationUser = _unitOfWork.applicationUser.FirstOrDefault(u => u.Id == claim.Value);
            foreach (var list in ShoppingCartVM.ListCart)
            {
                list.Price = SD.GetPriceBasedonQuantity(list.Count,
                    list.Product.Price, list.Product.Price50, list.Product.Price75);
                ShoppingCartVM.orderHeader.OrderTotal += (list.Price * list.Count);
                if (list.Product.Description.Length > 100)
                {
                    list.Product.Description = list.Product.Description.Substring(0, 99) + "...";
                }
            }
            //Email
            if (!isEmailConfirm)
            {
                ViewBag.EmailMessage = "Email has been sent kindly verify your email !";
                ViewBag.EmailCSS = "text-success";
                isEmailConfirm = false;
            }
            else
            {
                ViewBag.EmailMessage = "Email Must be confirm for authorize customer !";
                ViewBag.EmailCSS = "text-danger";
            }
            return View(ShoppingCartVM);
        }
        [HttpPost]
        [AutoValidateAntiforgeryToken]
        [ActionName("Index")]
        public async Task<IActionResult> IndexPost()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var user = _unitOfWork.applicationUser.FirstOrDefault(au => au.Id == claims.Value);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Email is Empty");
            }
            else
            {
                // Email
                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                   "/Account/ConfirmEmail",
                   pageHandler: null,
                   values: new { area = "Identity", userId = userId, code = code },
                    protocol: Request.Scheme);

                await _emailSender.SendEmailAsync(user.Email, "Confirm your email",
                    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                //**
                isEmailConfirm = true;
            }
            return RedirectToAction(nameof(Index));
        }
        public IActionResult plus(int id)
        {
            var cart = _unitOfWork.shoppingCart.FirstOrDefault(sc => sc.Id == id);
            cart.Count += 1;
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult minus(int id)
        {
            var cart = _unitOfWork.shoppingCart.FirstOrDefault(sc => sc.Id == id);
            if (cart.Count == 1)
                cart.Count = 1;
            else
                cart.Count -= 1;
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult delete(int id)
        {
            var cart = _unitOfWork.shoppingCart.FirstOrDefault(sc => sc.Id == id);
            _unitOfWork.shoppingCart.Remove(cart);
            _unitOfWork.Save();
            //Session
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var count = _unitOfWork.shoppingCart.GetAll(sc => sc.ApplicationUserId == claims.Value).ToList().Count;
            HttpContext.Session.SetInt32(SD.Ss_CartsessionCount, count);
            //*******
            return RedirectToAction(nameof(Index));

        }
       // [HttpPost]
        //[ActionName("Summary")]
        public IActionResult Summary(ShoppingCartVM shoppingCart, int[] iselected)
        {
            if (shoppingCart.ListCart?.Count() > 0)
            {
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
                var userId = claims.Value;
                ShoppingCartVM = new ShoppingCartVM()
                {
                    //ListCart = _unitOfWork.shoppingCart.GetAll(sc => sc.ApplicationUserId ==
                    //claims.Value, IncludeProperties: "Product"),
                    ListCart = _unitOfWork.shoppingCart.GetAll(sc => sc.ApplicationUserId ==
                    claims.Value, IncludeProperties: "Product").ToList(),
                    orderHeader = new OrderHeader()
                };
                ShoppingCartVM.orderHeader.ApplicationUser = _unitOfWork.
                    applicationUser.FirstOrDefault(au => au.Id == userId);
                ShoppingCartVM.orderHeader.OrderTotal = 0;

                var selectedIds = shoppingCart?.ListCart?.Where(x => x.IsSelected).Select(x => x.Id).ToList();
                ShoppingCartVM.ListCart = selectedIds?.Count() > 0 ? ShoppingCartVM?.ListCart.Where(x => selectedIds.Any(y => y == x.Id)).ToList() : ShoppingCartVM.ListCart;

                foreach (var list in ShoppingCartVM.ListCart)
                {
                    list.Price = SD.GetPriceBasedonQuantity(list.Count, list.Product.Price, list.Product.Price50, list.Product.Price75);

                    ShoppingCartVM.orderHeader.OrderTotal += (list.Price * list.Count);
                    if (list.Product.Description.Length > 100)
                    {
                        list.Product.Description = list.Product.Description.Substring(0, 99) + "...";
                    }
                }
                ShoppingCartVM.orderHeader.Name = ShoppingCartVM.orderHeader.ApplicationUser.Name;
                ShoppingCartVM.orderHeader.StreetAddress = ShoppingCartVM.orderHeader.ApplicationUser.StreetAddress;
                ShoppingCartVM.orderHeader.City = ShoppingCartVM.orderHeader.ApplicationUser.City;
                ShoppingCartVM.orderHeader.State = ShoppingCartVM.orderHeader.ApplicationUser.State;
                ShoppingCartVM.orderHeader.PostalCode = ShoppingCartVM.orderHeader.ApplicationUser.PostalCode;
                ShoppingCartVM.orderHeader.PhoneNumber = ShoppingCartVM.orderHeader.ApplicationUser.PhoneNumber;

                TempData["ShoppingCart"] = JsonConvert.SerializeObject(ShoppingCartVM);
                //TempData["ShoppingCart"] =ShoppingCartVM;
                return View(ShoppingCartVM);
            }
            else
            {
                
                return RedirectToAction("SummaryPost", "Cart");
            }
        }
        //[HttpPost]
        [AutoValidateAntiforgeryToken]
        [ActionName("SummaryPost")]
        public IActionResult SummaryPost(string stripeToken)
        {

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            
            _unitOfWork.Save();
            
            if (TempData.ContainsKey("ShoppingCart"))

            {
                var test = TempData["ShoppingCart"];
                ShoppingCartVM = JsonConvert.DeserializeObject<ShoppingCartVM>((string)test);
            }
            // Identify items that are not selected for ordering
            var itemsToKeep = ShoppingCartVM.ListCart.Where(item => !item.IsSelected).ToList();

            // Identify items to remove
            var itemsToRemove = ShoppingCartVM.ListCart.Where(item => item.IsSelected).ToList();

            // Remove selected items from the cart
            foreach(var item in itemsToRemove)
            {
                item.IsRemoveFromCart = true;
                _unitOfWork.shoppingCart.Update(item);
            }
            //_unitOfWork.shoppingCart.RemoveRange(itemsToRemove);
            _unitOfWork.Save();
            

            ShoppingCartVM.ListCart = itemsToKeep;
            ShoppingCartVM.orderHeader.ApplicationUser = _unitOfWork.applicationUser.FirstOrDefault(au => au.Id == claims.Value);
            ShoppingCartVM.orderHeader.OrderStatus = SD.OrderStatusApproved;
            ShoppingCartVM.orderHeader.PaymentStatus = SD.OrderStatusApproved;
            ShoppingCartVM.orderHeader.OrderDate = DateTime.Now;
            ShoppingCartVM.orderHeader.ApplicationId = claims.Value;
            ShoppingCartVM.orderHeader.OrderTotal = itemsToKeep.Sum(item => item.Price * item.Count); // Update order total
            _unitOfWork.orderHeader.Add(ShoppingCartVM.orderHeader);
            _unitOfWork.Save();


            foreach (var list in ShoppingCartVM.ListCart)
            {
                list.Price = SD.GetPriceBasedonQuantity(list.Count, list.Product.Price, list.Product.Price50, list.Product.Price75);
                OrderDetails orderDetail = new OrderDetails()
                {
                    ProductId = list.ProductId,
                    OrderHeaderId = ShoppingCartVM.orderHeader.Id,
                    Price = list.Price,
                    Count = list.Count,
                };

                ShoppingCartVM.orderHeader.OrderTotal += (list.Price * list.Count);

                _unitOfWork.orderDetail.Add(orderDetail);
                _unitOfWork.Save();
            }
            







            //Session
            HttpContext.Session.SetInt32(SD.Ss_CartsessionCount, itemsToKeep.Count);
            #region Stripe
            if (stripeToken == null)
            {
                ShoppingCartVM.orderHeader.paymentDueDate = DateTime.Today.AddDays(30);
                ShoppingCartVM.orderHeader.PaymentStatus = SD.PaymentStatusDelayed;
                ShoppingCartVM.orderHeader.OrderStatus = SD.OrderStatusApproved;
            }
            else
            {
                //Payment Process
                var options = new ChargeCreateOptions()
                {
                    Amount = Convert.ToInt32(ShoppingCartVM.orderHeader.OrderTotal),
                    Currency = "usd",
                    Description = "Order Id: " + ShoppingCartVM.orderHeader.Id,
                    Source = stripeToken
                };
                //Payment
                var service = new ChargeService();
                Charge charge = service.Create(options);
                if (charge.BalanceTransactionId == null)
                    ShoppingCartVM.orderHeader.PaymentStatus = SD.PaymentStatusRejected;
                else
                    ShoppingCartVM.orderHeader.TransationId = charge.BalanceTransactionId;
                if (charge.Status.ToLower() == "succeeded")
                {
                    ShoppingCartVM.orderHeader.PaymentStatus = SD.PaymentStatusApproved;
                    ShoppingCartVM.orderHeader.OrderStatus = SD.OrderStatusApproved;
                    ShoppingCartVM.orderHeader.OrderDate = DateTime.Now;
                }
                _unitOfWork.Save();

            }

            #endregion




            return RedirectToAction("OrderConfirmation","Cart",
                new { id = ShoppingCartVM.orderHeader.Id });
        }
        public async Task<IActionResult> OrderConfirmation(int id)
        {
            var ClaimsIdentity = (ClaimsIdentity)User.Identity;
            var Claims = ClaimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            var user = _unitOfWork.applicationUser.FirstOrDefault(au => au.Id == Claims.Value);

            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Email is empty");
            }

            else
            {
                //    //email

                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new { area = "Identity", userId = userId, code = code },
                    protocol: Request.Scheme);


                await _emailSender.SendEmailAsync(user.Email, "Order Confirmation",
                    $"Your order has been placed successfully. Thank you for shopping with us by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
                //****
                //isEmailConfirm = true;
                ////Twilio
                //string accountSid = "ACfe7401725960fac0160c9e92c185cbcf";
                //string authToken = "a2a5c2babfd63c174ca4428b9a025e59";
                //var phoneNumber1 = _unitOfWork.applicationUser.FirstOrDefault(x => x.Id == Claims.Value);
                //string phoneNumber = phoneNumber1.PhoneNumber;
                //TwilioClient.Init(accountSid, authToken);

                //var message = MessageResource.Create(
                //    body: "Your Order is Confirmed And Your Order's id is:" + id,
                //    from: new Twilio.Types.PhoneNumber("+12059645502"),
                //    to: phoneNumber
                //    );
                //Twilio
                string accountSid = "ACfe7401725960fac0160c9e92c185cbcf";
                string authToken = "a2a5c2babfd63c174ca4428b9a025e59";


                // Initialize the Twilio client
                TwilioClient.Init(accountSid, authToken);

                // The phone number to send the message from (Twilio phone number)
                string fromPhoneNumber = "+12059645502"; // Replace with your Twilio phone number

                // The phone number to send the message to (destination phone number)
                string toPhoneNumber = "+919541879475"; // Replace with the destination phone number

                try
                {
                    var message = MessageResource.Create(
                        body: "Mubarka Bhaji Ho Gya Order Confirm !",
                        from: new PhoneNumber(fromPhoneNumber),
                        to: new PhoneNumber(toPhoneNumber)
                    );

                    Console.WriteLine("Message SID: " + message.Sid);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred: " + ex.Message);
                }
            }
            return View(id);
        }

        private double CalculateUpdatedOrderTotal(int orderId)
        {
            var orderDetails = _unitOfWork.orderDetail.GetAll(od => od.OrderHeaderId == orderId);
            double total = 0.0;

            foreach (var detail in orderDetails)
            {
                total += detail.Price * detail.Count;
            }

            return total;
        }


    }
}
//public async Task<IActionResult> OrderConfirmation(int id)
//{
//    var ClaimsIdentity = (ClaimsIdentity)User.Identity;
//    var Claims = ClaimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
//    var user = _unitOfWork.applicationUser.FirstOrDefault(au => au.Id == Claims.Value);

//    if (user == null)
//    {
//        ModelState.AddModelError(string.Empty, "Email is empty");
//    }

//    else
//    {
//        //    //email

//        var userId = await _userManager.GetUserIdAsync(user);
//        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
//        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
//        var callbackUrl = Url.Page(
//            "/Account/ConfirmEmail",
//            pageHandler: null,
//            values: new { area = "Identity", userId = userId, code = code },
//            protocol: Request.Scheme);


//        await _emailSender.SendEmailAsync(user.Email, "Order Confirmation",
//            $"Your order has been placed successfully. Thank you for shopping with us by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
//        //****
//        //isEmailConfirm = true;
//        ////Twilio
//        //string accountSid = "ACfe7401725960fac0160c9e92c185cbcf";
//        //string authToken = "a2a5c2babfd63c174ca4428b9a025e59";
//        //var phoneNumber1 = _unitOfWork.applicationUser.FirstOrDefault(x => x.Id == Claims.Value);
//        //string phoneNumber = phoneNumber1.PhoneNumber;
//        //TwilioClient.Init(accountSid, authToken);

//        //var message = MessageResource.Create(
//        //    body: "Your Order is Confirmed And Your Order's id is:" + id,
//        //    from: new Twilio.Types.PhoneNumber("+12059645502"),
//        //    to: phoneNumber
//        //    );

//        //Twilio
//        string accountSid = "ACfe7401725960fac0160c9e92c185cbcf";
//        string authToken = "a2a5c2babfd63c174ca4428b9a025e59";


//        // Initialize the Twilio client
//        TwilioClient.Init(accountSid, authToken);

//        // The phone number to send the message from (Twilio phone number)
//        string fromPhoneNumber = "+12059645502"; // Replace with your Twilio phone number

//        // The phone number to send the message to (destination phone number)
//        string toPhoneNumber = "+919541879475"; // Replace with the destination phone number

//        try
//        {
//            var message = MessageResource.Create(
//                body: "Mubarka Bhaji Ho Gya Order Confirm !",
//                from: new PhoneNumber(fromPhoneNumber),
//                to: new PhoneNumber(toPhoneNumber)
//            );

//            Console.WriteLine("Message SID: " + message.Sid);
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine("An error occurred: " + ex.Message);
//        }
//    }
//    return View(id);
//}

