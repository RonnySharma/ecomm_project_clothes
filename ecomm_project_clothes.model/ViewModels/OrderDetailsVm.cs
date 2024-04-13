using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ecomm_project_clothes.Model.ViewModels
{
    public class OrderDetailsViewModel
    {
        public OrderHeader Order { get; set; }
        public List<OrderDetails> OrderDetails { get; set; }
    }

}
