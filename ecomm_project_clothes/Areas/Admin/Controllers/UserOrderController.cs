using ecomm_project_clothes.Dataaccess.Repository.IRepository;
using ecomm_project_clothes.Model.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;
using Microsoft.AspNetCore.Identity;
using ecomm_project_clothes.Dataaccess.Data;
using System.Text;
using Microsoft.AspNetCore.WebUtilities;
using ecomm_project_clothes.Utility;
using System.Text.Encodings.Web;

namespace ecomm_project_clothes.Areas.Customer.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class UserOrderController : Controller
    { private readonly IUnitofwork _unitOfWork;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _context;


        public UserOrderController(IUnitofwork unitOfWork, IEmailSender emailSender, UserManager<IdentityUser> userManager)
        
            {
                _unitOfWork = unitOfWork;
                _emailSender = emailSender;
                _userManager = userManager;
            }
        
        public IActionResult Index()
        {
            return View();
        }
        #region APIs
        [HttpGet]
        public IActionResult GetAll()
        {
            // Fetch all users who have added items to their carts
            var usersWithCarts = _unitOfWork.shoppingCart
                .GetAll(sc => sc.IsRemoveFromCart == false, IncludeProperties: "Product,ApplicationUser")
                .GroupBy(sc => sc.ApplicationUserId)
                .ToList();

            var cartViewModels = new List<ShoppingCartVM>();

            foreach (var userCartGroup in usersWithCarts)
            {
                var userId = userCartGroup.Key;
                var user = _unitOfWork.applicationUser.FirstOrDefault(u => u.Id == userId);

                var cartItems = userCartGroup.ToList();

                var cartViewModel = new ShoppingCartVM
                {
                    User = user,
                    ListCart = cartItems
                };

                cartViewModels.Add(cartViewModel);
            }

            return View(cartViewModels);
        }
        public IActionResult UserDetails(string userId)
        {
            // Fetch user details and cart items based on the provided userId
            var userCartViewModel = GetDetailsForUser(userId); // Implement this method to get user details and cart items

            if (userCartViewModel == null)
            {
                // Handle the case when the user or their cart items are not found
                return NotFound();
            }

            return View(userCartViewModel);
        }

        private ShoppingCartVM GetDetailsForUser(string userId)
        {
            // Fetch user details
            var user = _unitOfWork.applicationUser.FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                return null; // User not found
            }

            // Fetch cart items for the user
            var cartItems = _unitOfWork.shoppingCart
                .GetAll(sc => sc.ApplicationUserId == userId, IncludeProperties: "Product")
                .ToList();

            // Create a ShoppingCartVM instance to hold the user and cart item information
            var cartViewModel = new ShoppingCartVM
            {
                User = user,
                ListCart = cartItems
            };

            return cartViewModel;
        }

        public async Task<IActionResult> SendOffer(int id)
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


                await _emailSender.SendEmailAsync(user.Email, "Discount",
                     $"Hi Sanmukh, You have offer on this product by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
                //****

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
                        body: "Hi Sanmukh, You have offer on this product  http://bit.ly/p1we1 ",
                        from: new PhoneNumber(fromPhoneNumber),
                        to: new PhoneNumber(toPhoneNumber)
                    );

                    Console.WriteLine("Message SID: " + message.Sid);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("An error occurred: " + ex.Message);
                }



                return View(id);
            }
            return View(id);
        }








        #endregion
    }
}
