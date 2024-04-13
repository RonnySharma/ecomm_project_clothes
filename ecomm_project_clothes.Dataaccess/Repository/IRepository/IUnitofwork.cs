using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ecomm_project_clothes.Dataaccess.Repository.IRepository
{
    public interface IUnitofwork
    {
        IcategoryRepository category { get; }
        IClothesTypeRepostiry clothesType { get; }
        IBrandRepository brand { get; }
        ISPCALL SPCALL { get; }
        IProductRepostry product { get; }
        ICompanyRepository company { get; }
        IShoppingCartRepository shoppingCart { get; }
        IOrderHeaderRepository orderHeader { get; }
        IOrderDetailRepository orderDetail { get; }
        IApplicationUserRepository applicationUser { get; }

        void Save();
    }
}
