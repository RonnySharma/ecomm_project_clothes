using ecomm_project_clothes.Dataaccess.Data;
using ecomm_project_clothes.Dataaccess.Repository.IRepository;
using ecomm_project_clothes.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ecomm_project_clothes.Dataaccess.Repository
{
    public class ProductRepository:Repository<Product>, IProductRepostry
    {
        private readonly ApplicationDbContext _context;
        public ProductRepository(ApplicationDbContext context):base(context)
        {
            _context = context;
        }

        public List<Product> GetAll(string searchTerm, int? brandId, int? clothesTypeId)
        {
            throw new NotImplementedException();
        }

        public object OrderByDescending(Func<object, object> value)
        {
            throw new NotImplementedException();
        }
    }
    //{
    //    private readonly ApplicationDbContext _context;
    //    public ProductRepository (ApplicationDbContext context):base(context)
    //    {
    //        _context = context;
    //    }
    //}
}
