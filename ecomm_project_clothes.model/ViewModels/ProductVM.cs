using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ecomm_project_clothes.Model.ViewModels
{
    public class ProductVM
    {
        public Product Product { get; set; }
        public IEnumerable<SelectListItem> CategoryList { get; set; }
        public IEnumerable<SelectListItem> ClothesTypeList { get; set; }
        public IEnumerable<SelectListItem>BrandsTypeList { get; set; }
        public List<Product> AvailableProducts { get; set; }
        public List<Product> OutOfStockProducts { get; set; }
        public int Quantity { get; set; }
    }
}
