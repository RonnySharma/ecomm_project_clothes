using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ecomm_project_clothes.Utility
{
    public static class SD //standerd distonary
    {
        //Cover type store ptocedure
        public const string Proc_GetClothesTypes="GetClothesTypes";
        public const string Proc_GetClothesType = "GetClothesType";
        public const string Proc_CreateClothesTypes = "CreateClothesTypes";
        public const string proc_UpdateClothesTypes = "UpdateClothesTypes";
        public const string proc_DeleteClothesTypes = "DeleteClothesTypes";
        //order status
        public const string OrderStatusPending = "Pending";
        public const string OrderStatusApproved = "Approved";
        public const string OrderStatusProcessing = "Processing";
        public const string OrderStatusShipped = "Shipped";
        public const string OrderStatusCancelled= "Cancelled";
        public const string OrderStatusRefunded = "Refunded";
        //Payment Status
        public const string PaymentStatusPending = "Pending";
        public const string PaymentStatusApproved = "Approved";
        public const string PaymentStatusDelayed = "Delayed";
        public const string PaymentStatusRejected = "Rejected";
        //Role
        public const string role_Admin = "Admin";
        public const string role_Employee = "EmployeeUser";
        public const string role_Individual = "IndividualUser";
        public const string role_Company = "CompanyUser";
        //session
        public const string Ss_CartsessionCount = "CountCartSession";
        public static double GetPriceBasedonQuantity(double quantity, double price, double price50, double price75)
        {
            if (quantity < 50)
                return price;
            else if (quantity < 100)
                return price50;
            else return price75;
        }
        //ConvertToRawHtml
        public static string ConverToRawHtml(string source)
        {
            char[] array = new char[source.Length];
            int arrayIndex = 0;
            bool inside = false;
            for (int i = 0; i < source.Length; i++)
            {
                char let = source[i];
                if (let == '<')
                {
                    inside = true;
                    continue;
                }
                if (let == '>')
                {
                    inside = false;
                    continue;
                }
                if (!inside)
                {
                    array[arrayIndex] = let;
                    arrayIndex++;
                }
            }
            return new string(array, 0, arrayIndex);
        }

    }
}
