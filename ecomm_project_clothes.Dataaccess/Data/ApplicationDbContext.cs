﻿using ecomm_project_clothes.Dataaccess.Migrations;
using ecomm_project_clothes.Model;
using ecomm_project_clothes.Model.ViewModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ecomm_project_clothes.Dataaccess.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Category>Categories { get; set; }
        public DbSet<clothesType>ClothesTypes { get; set; }
        public DbSet<Brand>Brands { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Company> Company { get; set; }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set;}
        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<OrderDetails> OrderDetails { get; set; }
    }
}