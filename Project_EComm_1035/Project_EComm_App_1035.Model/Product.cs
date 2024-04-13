using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_EComm_App_1035.Model
{
   public class Product
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string ISBN { get; set; }
        public  string Author { get; set; }
        [Required]
        [Range(1,1000)]
        public double ListPrice { get; set; }//800
        [Required]
        [Range(1,1000)]
        public double Price { get; set; }//710
        [Required]
        [Range(1, 1000)]
        public double Price50 { get; set; }//600
        [Required]
        [Range(1, 1000)]
        public double Price100 { get; set;}//450
        [Display(Name = "Image Url")]
        public string ImageUrl { get; set; }
        [Display(Name ="Category")]
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        [Display(Name ="Cover Type")]
        public int CoverTypeId { get; set; }
        public CoverType CoverType { get; set; }



    }
}
