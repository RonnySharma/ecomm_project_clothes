using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ecomm_project_clothes.Model
{
    public class OrderHeader
    {
        public int Id { get; set; }
        public string ApplicationId { get; set; }
        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; }
        [Required]

        public DateTime ShoppingDate { get; set; }
        [Required]
        public Double OrderTotal { get; set; }
        public string TrackingNumber { get; set; }
        public string Carrier { get; set; }
        public string OrderStatus { get; set; }
        public DateTime OrderDate { get; set; }
        public string PaymentStatus { get; set; }
        public DateTime paymentDate { get; set; }
        public DateTime paymentDueDate { get; set; }
        public string TransationId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [Display(Name = "Street Address")]
        public string StreetAddress { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public String State { get; set; }
        [Required]
        [Display(Name ="Postal Code")]
        public string PostalCode { get; set; }
        [Required]
        [Display(Name ="Phone Number")]
        public string PhoneNumber { get; set; }
        [NotMapped]
        public IEnumerable<object> OrderDetails { get; set; }
    }
}
