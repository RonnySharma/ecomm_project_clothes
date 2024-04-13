using ecomm_project_clothes.Dataaccess.Data;
using ecomm_project_clothes.Dataaccess.Migrations;
using ecomm_project_clothes.Dataaccess.Repository.IRepository;
using ecomm_project_clothes.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ecomm_project_clothes.Dataaccess.Repository
{
    public class Unitofwork : IUnitofwork
    {
        public readonly ApplicationDbContext _context;
        public Unitofwork (ApplicationDbContext context)
        {
            _context = context;
            category = new CatagoryRepository(_context);
            clothesType=new ClothesTypeRepository(_context);
            SPCALL = new SPCALL(_context);
            brand=new BrandRepository(_context);
            product= new ProductRepository(_context);
            company = new CompanyRepository(_context);
            shoppingCart=new ShoppingCartRepository(_context);
            orderHeader=new OrderHeaderRepository(_context);
            orderDetail=new OrderDetailRepository(_context);
            applicationUser = new ApplicationUserRepository(_context);
        }
        public IcategoryRepository category { private set; get; }
        public ISPCALL SPCALL { private set; get; }
       public IClothesTypeRepostiry clothesType { private set; get; }  
        public IProductRepostry product {  private set; get; }
        public IBrandRepository brand { private set; get; }
        public ICompanyRepository company { private set; get; }
        
        public IShoppingCartRepository shoppingCart { private set; get; }
        public IOrderHeaderRepository orderHeader { private set; get; }
        public IOrderDetailRepository orderDetail { private set; get; }
        public IApplicationUserRepository applicationUser { private set; get; }


        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
