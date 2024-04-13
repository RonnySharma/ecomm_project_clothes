using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_EComm_App_1035.DataAccess.Repositry.IRepositry
{
    public interface IUnitOfWork
    {
        ICategoryRepositry category { get; }//readonly
        ICoverTypeRepositry coverType { get; }
        IProductRepositry product { get; }
        ICompanyRepositry company { get; }
        IShoppingCartRepositry shoppingCart { get; }
        ISPCALL SPCALL { get; }
        void save();//save method

    }
}
