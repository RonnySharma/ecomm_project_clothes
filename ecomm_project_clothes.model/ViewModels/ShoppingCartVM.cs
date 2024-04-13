using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ecomm_project_clothes.Model.ViewModels
{
    public class ShoppingCartVM
    {
        //public IEnumerable<ShoppingCart> ListCart { get; set; }
        public List<ShoppingCart> ListCart { get; set; }
        public OrderHeader orderHeader { get; set; }
        public List<int> selectedOrderIds { get; set; }
        public ApplicationUser User { get; set; } // Replace with your user model
        public IEnumerable<ShoppingCart> CartItems { get; set; }

    }

}
