using Project_EComm_App_1035.DataAccess.Data;
using Project_EComm_App_1035.DataAccess.Repositry.IRepositry;
using Project_EComm_App_1035.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_EComm_App_1035.DataAccess.Repositry
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            category = new CategoryRepositry(_context);
            coverType=new CoverTypeRepositry(_context);
            product=new ProductRepositry(_context); 
            company=new CompanyRepositry(_context);
            SPCALL = new SPCALL(_context);
            shoppingCart = new SHOPINGCARTR(_context);

        }
        public ICategoryRepositry category  { private set; get;}

        public ICoverTypeRepositry coverType { private set; get;}
        public IProductRepositry product { private set; get;}
        public ICompanyRepositry company { private set; get;}
        public IShoppingCartRepositry shoppingCart { private set; get;}

        public IProductRepositry Product => throw new NotImplementedException();

        public ISPCALL SPCALL { private set; get;}

        public void save()
        {
            _context.SaveChanges();
        }
    }
}
