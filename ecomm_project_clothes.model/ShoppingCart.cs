﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ecomm_project_clothes.Model
{
    public class ShoppingCart
    {
        public ShoppingCart() {
            Count = 1;
        }
        public int Id { get; set; }
       public string ApplicationUserId { get; set; }
        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; }
        public int ProductId { get; set; }
        [ForeignKey("ProductId")]
        public Product Product { get; set; }
        public int Count { get;set; }
        [NotMapped]
        public Double Price { get; set; }
       
        public bool IsSelected { get; set; }
        public bool IsRemoveFromCart { get; set; }
        
        
    }
}
