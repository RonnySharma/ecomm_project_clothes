using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ecomm_project_clothes.Model
{
    public class Product
    {
        public int Id { get; set; }
        [Required] public string Name { get; set; }
        [Required] public string Description { get; set; }
        [Required] public string Size { get; set; }
        
        [Required]
        [Range(1,1000)]
        public double ListPrice { get; set; }//600
        [Required]
        public double Price { get; set; }//590
        [Required][Range(1,1000)]
        public double Price50 { get; set; }//500
        [Required][Range (1,1000)]
        public double Price75 { get; set;}//450
        [Display(Name ="image Url")]
        public string ImgUrl { get; set; }
        [Display(Name ="Category")]
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        [Display(Name = "ClothesType")]
        public int ClothesTypeId { get; set; }
        public clothesType ClothesType { get; set; }
        [Display(Name ="Brands")]
        public int BrandId { get; set; }
        public Brand Brand { get; set; }
        public bool Active { get; set; }
        public int OrderCount { get; set; }
        public int Quantity { get; set; }
        public bool IsOutOfStock => Quantity <= 0;

        public ICollection<OrderDetails> OrderDetails { get; set; }
        
    }
}
