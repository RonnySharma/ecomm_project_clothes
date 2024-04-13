using ecomm_project_clothes.Dataaccess.Data;
using ecomm_project_clothes.Dataaccess.Repository.IRepository;
using ecomm_project_clothes.Model;
using ecomm_project_clothes.Model.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using System.Security.Claims;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;



namespace ecomm_project_clothes.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderManagementController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _context;
        private readonly IUnitofwork _unitOfWork;
        public OrderManagementController(IUnitofwork unitofwork, ApplicationDbContext context, IEmailSender emailSender, UserManager<IdentityUser> userManager)
        {
            _unitOfWork = unitofwork;
        }


        public IActionResult Index()
        {
            return View();
        }
        public IActionResult ByDateTime(DateTime? datetime1, DateTime? datetime2)
        {
            if (datetime1 == null && datetime2 == null)
            {
                return RedirectToAction("Index");
            }
            if (datetime2 == null)
            {
                var date = _unitOfWork.orderHeader.GetAll(u => u.OrderDate.Date == datetime1);
                return View(date);
            }
            if (datetime1 == null)
            {
                var date = _unitOfWork.orderHeader.GetAll(u => u.OrderDate.Date == datetime2);
                return View(date);
            }
            IQueryable<OrderHeader> query = from o in _context.OrderHeaders select o;
            if (datetime1 != null && datetime2 != null)
            {
                query = from o in _context.OrderHeaders where o.OrderDate.Date > datetime1 && o.OrderDate.Date <= datetime2 select o;
            }
            return View(query);
        }
        public IActionResult Searching(DateTime StartDate, DateTime endDate)
        {
            var Filter = _unitOfWork.orderHeader.GetAll(or => or.OrderDate >= StartDate && or.OrderDate <= endDate).ToList();
            return Json(Filter);
        }



        [HttpGet]
        public IActionResult GetAll()
        {
            var orderlist = _unitOfWork.orderHeader.GetAll(IncludeProperties: "ApplicationUser");
            return Json(new { Data = orderlist });
            //return View(orderlist);
        }

        //    public IActionResult AllOrder(int id)
        //{
        //    var order = _unitOfWork.orderDetail.FirstOrDefault(s => s.Id == id);
        //    return View(order);
        //}

        public IActionResult Details(int? id)
        {
            OrderDetails orderDetails = new OrderDetails();
            if (id == null) return View(orderDetails);
            orderDetails = _unitOfWork.orderDetail.FirstOrDefault(or => or.Id == id, IncludeProperties: "Product,OrderHeader.ApplicationUser,OrderHeader,Product.Brand,Product.Category");
            if (orderDetails == null) return NotFound();
            return View(orderDetails);

        }
        [HttpGet]
        public IActionResult PendingOrders()
        {
            var pendingOrders = _unitOfWork.orderHeader.GetAll(
             or => or.OrderStatus == "Pending", // Assuming the order status value is "Pending"
             IncludeProperties: "ApplicationUser"
         ).ToList();

            return View(pendingOrders);
        }
        [HttpGet]
        public IActionResult ApprovedOrders()
        {
            var approvedOrders = _unitOfWork.orderHeader.GetAll(
                or => or.OrderStatus == "Approved", IncludeProperties: "ApplicationUser").ToList();
            return View(approvedOrders);
        }

        [HttpPost]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            var order = _unitOfWork.orderHeader.FirstOrDefault(
                or => or.Id == orderId, IncludeProperties: "ApplicationUser");

            if (order == null)
            {
                // Handle the case where the order is not found.
                return NotFound();
            }

            // Check if the ApplicationUser associated with the order is null.
            if (order.ApplicationUser == null)
            {
                // Handle the case where ApplicationUser is null.
                // You can return an error message or take appropriate action.
                return BadRequest("User information not available for this order.");
            }

            // Update the order status to "Cancelled."
            order.OrderStatus = "Cancelled";
            order.PaymentStatus = "Refunded";
            _unitOfWork.Save();

            // Redirect back to the list of approved orders.
            return RedirectToAction("Sendmail", new { id = orderId });
            return View(ApprovedOrders());
        }




        public IActionResult statuscancelled()
        {

            var approved = _unitOfWork.orderHeader.GetAll().Where(os => os.OrderStatus == "Cancelled");
            if (approved.ToList().Count == 0)
            {
                return View("_NoRecord");
            }
            return View(approved);
        }
        public IActionResult StatusRefunded()
        {
            var approved = _unitOfWork.orderHeader.GetAll().Where(os => os.OrderStatus == "Refunded");
            if (approved.ToList().Count == 0)
            {
                return View("_NoRecord");
            }
            return View(approved);
        }
        public IActionResult StatusProcessing()
        {
            var approved = _unitOfWork.orderHeader.GetAll().Where(os => os.OrderStatus == "Processing");
            if (approved.ToList().Count == 0)
            {
                return View("_NoRecord");
            }
            return View(approved);
        }
        public IActionResult StatusShipped()
        {
            var approved = _unitOfWork.orderHeader.GetAll().Where(os => os.OrderStatus == "Shipped");
            if (approved.ToList().Count == 0)
            {
                return View("_NoRecord");
            }
            return View(approved);
        }
        public IActionResult OrderHistory()
        {
            // Retrieve the orders for the currently logged-in user
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var orders = _unitOfWork.orderHeader.GetAll(o => o.ApplicationId == userId);

            return View(orders);
        }
        public IActionResult OrderDetails(int id)
        {
            // Retrieve the order details for the specified order ID
            var order = _unitOfWork.orderHeader.FirstOrDefault(o => o.Id == id);
            var orderDetails = _unitOfWork.orderDetail.GetAll(od => od.OrderHeaderId == id);

            // Pass the order and its details to the view
            var viewModel = new OrderDetailsViewModel
            {
                Order = order,
                OrderDetails = orderDetails.ToList() // Ensure orderDetails is converted to a list
            };

            return View(viewModel);
        }
        
        [HttpPost]
        public IActionResult ApproveSelectedOrders(int[] selectedOrders)
        {
            if (selectedOrders != null && selectedOrders.Length > 0)
            {
                foreach (int orderId in selectedOrders)
                {
                    // Fetch the order by orderId from your repository
                    var order = _unitOfWork.orderHeader.FirstOrDefault(o => o.Id == orderId);

                    if (order != null && order.OrderStatus == "Pending")
                    {
                        // Update the order status to "Approved"
                        order.OrderStatus = "Approved";
                        _unitOfWork.Save();

                        // Call the ApprovedOrderMail action to send an email or perform other actions
                        ApprovedOrderMail(order.Id);
                    }
                }
            }

            // Redirect back to the "PendingOrders" view or another appropriate page
            return RedirectToAction("PendingOrders");
        }

      
        private void SendEmailToUser(string email, string v1, string v2)
        {
            throw new NotImplementedException();
        }

        private void SendTwilioMessage(string phoneNumber, string v)
        {
            throw new NotImplementedException();
        }

        public async Task<IActionResult> Sendmail(int id)
        {
            var order = _unitOfWork.orderHeader.FirstOrDefault(
                or => or.Id == id, IncludeProperties: "ApplicationUser");

            if (order == null)
            {
                // Handle the case where the order is not found.
                return NotFound();
            }

            if (order.ApplicationUser == null)
            {
                // Handle the case where ApplicationUser is null.
                // You can return an error message or take appropriate action.
                return BadRequest("User information not available for this order.");
            }

            try
            {
                var user = order.ApplicationUser;
                var userEmail = user.Email;

                if (string.IsNullOrEmpty(userEmail))
                {
                    // Handle the case where the user's email is empty.
                    return BadRequest("User's email is empty.");
                }

                // Send an email to the user.
                var emailSubject = "Your Order Has Been Cancelled";
                var emailMessage = "Your order has been cancelled. We apologize for any inconvenience.";

                //await _emailSender.SendEmailAsync(userEmail, emailSubject, emailMessage);

                // Initialize Twilio and send SMS
                string accountSid = "ACfe7401725960fac0160c9e92c185cbcf";
                string authToken = "a2a5c2babfd63c174ca4428b9a025e59";
                TwilioClient.Init(accountSid, authToken);

                string fromPhoneNumber = "+12059645502"; // Replace with your Twilio phone number
                string toPhoneNumber = "+919541879475"; // Replace with the destination phone number

                var smsMessage = MessageResource.Create(
                    body: "Hi Sanmukh, Your order has been cancelled. We apologize for any inconvenience. http://bit.ly/p1we1",
                    from: new PhoneNumber(fromPhoneNumber),
                    to: new PhoneNumber(toPhoneNumber)
                );

                Console.WriteLine("SMS Message SID: " + smsMessage.Sid);

                // Redirect to a relevant view or take further action as needed.
                //return View(id);
                return RedirectToAction("Refund");
            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during email or SMS sending.
                // You can log the error or return an error view.
                return View("Error", new ErrorViewModel { ErrorMessage = ex.Message });
            }
        }
        private async Task<string> Refund(OrderHeader order)
        {
            try
            {
                // Retrieve the Stripe charge ID associated with this order.
                string chargeId = order.TransationId; // Replace with the actual property name

                if (!string.IsNullOrEmpty(chargeId))
                {
                    // Configure Stripe with your Secret Key
                    StripeConfiguration.ApiKey = "sk_test_51NYOktCFScgGj36O6cl71Xqd5FG4M2XoHDaFX1OMRaEtHkKjAUM12V5alZdXj3oUQ98oTlwsbzTjS1Y4exRE7yxH00MEfLGOJB";

                    // Create a refund using the Stripe Charge ID
                    var refundService = new RefundService();
                    var refundOptions = new RefundCreateOptions
                    {
                        Charge = chargeId,
                        // Optionally, you can set the refund amount
                        Amount = 100, // Refund amount in cents (e.g., $1.00)
                        Reason = RefundReasons.RequestedByCustomer,
                    };

                    var refund = refundService.Create(refundOptions);

                    // Return the refund status
                    return refund.Status;
                }
                else
                {
                    // Handle the case where the chargeId is not found
                    // Log an error and return an appropriate message
                    return "Charge ID not found";
                }
            }
            catch (StripeException ex)
            {
                // Handle Stripe exceptions (e.g., invalid parameters, authentication issues)
                // Log the exception and return an appropriate message
                return ex.Message;
            }
        }
        public async Task<IActionResult> ApprovedOrderMail(int id)
        {
            var order = _unitOfWork.orderHeader.FirstOrDefault(
                or => or.Id == id, IncludeProperties: "ApplicationUser");

            if (order == null)
            {
                // Handle the case where the order is not found.
                return NotFound();
            }

            if (order.ApplicationUser == null)
            {
                // Handle the case where ApplicationUser is null.
                // You can return an error message or take appropriate action.
                return BadRequest("User information not available for this order.");
            }

            try
            {
                var user = order.ApplicationUser;
                var userEmail = user.Email;

                if (string.IsNullOrEmpty(userEmail))
                {
                    // Handle the case where the user's email is empty.
                    return BadRequest("User's email is empty.");
                }

                // Send an email to the user.
                var emailSubject = "Your Order Has Been Approved";
                var emailMessage = "Your order has been Approved.";

               //await _emailSender.SendEmailAsync(userEmail, emailSubject, emailMessage);

                // Initialize Twilio and send SMS
                string accountSid = "ACfe7401725960fac0160c9e92c185cbcf";
                string authToken = "a2a5c2babfd63c174ca4428b9a025e59";
                TwilioClient.Init(accountSid, authToken);

                string fromPhoneNumber = "+12059645502"; // Replace with your Twilio phone number
                string toPhoneNumber = "+919541879475"; // Replace with the destination phone number

                var smsMessage = MessageResource.Create(
                    body: "Hi Sanmukh, Your order has been cancelled. We apologize for any inconvenience. http://bit.ly/p1we1",
                    from: new PhoneNumber(fromPhoneNumber),
                    to: new PhoneNumber(toPhoneNumber)
                );

                Console.WriteLine("SMS Message SID: " + smsMessage.Sid);

                // Redirect to a relevant view or take further action as needed.
                return View(id);

            }
            catch (Exception ex)
            {
                // Handle any exceptions that occur during email or SMS sending.
                // You can log the error or return an error view.
                return View("Error", new ErrorViewModel { ErrorMessage = ex.Message });
            }
        }
        [HttpPost]
        [Route("Refund")]
        public async Task<IActionResult> Refund(int orderId)
        {
            var order = _unitOfWork.orderHeader.FirstOrDefault(
                or => or.Id == orderId, IncludeProperties: "ApplicationUser");

            if (order == null)
            {
                // Handle the case where the order is not found.
                return NotFound();
            }

            // Check if the ApplicationUser associated with the order is null.
            if (order.ApplicationUser == null)
            {
                // Handle the case where ApplicationUser is null.
                // You can return an error message or take appropriate action.
                return BadRequest("User information not available for this order.");
            }

            // Update the order status to "Cancelled."
            order.OrderStatus = "Cancelled";
            _unitOfWork.Save();

            try
            {
                // Retrieve the Stripe charge ID associated with this order.
                string chargeId = order.TransationId; // Replace with the actual property name

                if (!string.IsNullOrEmpty(chargeId))
                {
                    // Configure Stripe with your Secret Key
                    StripeConfiguration.ApiKey = "sk_test_51NYOktCFScgGj36O6cl71Xqd5FG4M2XoHDaFX1OMRaEtHkKjAUM12V5alZdXj3oUQ98oTlwsbzTjS1Y4exRE7yxH00MEfLGOJB";

                    // Create a refund using the Stripe Charge ID
                    var refundService = new RefundService();
                    var refundOptions = new RefundCreateOptions
                    {
                        Charge = chargeId,
                        // Optionally, you can set the refund amount
                        Amount = 100, // Refund amount in cents (e.g., $1.00)
                        Reason = RefundReasons.RequestedByCustomer,
                    };

                    var refund = refundService.Create(refundOptions);

                    // Check if the refund was successful
                    if (refund.Status == "succeeded")
                    {
                        // Redirect to the Sendmail action, passing the orderId
                        return RedirectToAction("Sendmail", new { id = orderId });
                    }
                    else
                    {
                        // Handle the case where the refund failed
                        // Log an error and display an error message to the user
                        // ...

                        return RedirectToAction("ErrorView");
                    }
                }
                else
                {
                    // Handle the case where the chargeId is not found
                    // Log an error and display an error message to the user
                    // ...

                    return RedirectToAction("ErrorView");
                }
            }
            catch (StripeException ex)
            {
                // Handle Stripe exceptions (e.g., invalid parameters, authentication issues)
                // Log the exception and display an error message to the user
                // ...

                return RedirectToAction("ErrorView");
            }
        }



    }
}


