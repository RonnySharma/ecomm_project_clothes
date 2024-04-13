using ecomm_project_clothes.Model;

namespace ecomm_project_clothes.Dataaccess.Repository.IRepository
{
    public interface IOrderDetailRepository : IRepository<OrderDetails>
    {
        IEnumerable<object> GroupBy(Func<object, object> value);
        IEnumerable<object> where(Func<object, bool> value);
    }
}
