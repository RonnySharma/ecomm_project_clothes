using ecomm_project_clothes.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ecomm_project_clothes.Dataaccess.Repository.IRepository
{
    public interface IProductRepostry : IRepository<Product>
    {
        List<Product> GetAll(string searchTerm, int? brandId, int? clothesTypeId);
        object OrderByDescending(Func<object, object> value);
    }
}
