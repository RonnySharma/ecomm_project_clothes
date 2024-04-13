using ecomm_project_clothes.Model;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ecomm_project_clothes.Areas.Customer.Controllers
{
    internal class SearchViewModel
    {
        public List<Product> Products { get; set; }
        public string SearchTerm { get; set; }
        public int? SelectedBrandId { get; set; }
        public int? SelectedClothesTypeId { get; set; }
        public IEnumerable<SelectListItem> BrandList { get; set; }
        public IEnumerable<SelectListItem> ClothesTypeList { get; set; }
    }
}