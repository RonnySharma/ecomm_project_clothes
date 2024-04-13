using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Project_EComm_App_1035.DataAccess.Repositry.IRepositry
{
    public interface IRepositry<T> where T : class
    {
        void Add(T entity);
        void update(T entity);
        void Remove(T entity);
        void Remove(int id);//overloading same name butt diferent parameters
        void RemoveRange(IEnumerable<T> values); // To delete multiple tables
        T Get(int id);
        IEnumerable<T> GetAll(
        Expression<Func<T, bool>> filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,// To sorting
        string includeproperties = null  //category,covertype
            );
        T FirstorDefault( //to retrieve a single record from a collection or database table
            Expression<Func<T, bool>> filter = null,
            string includeproperties = null 
            );
    }
}
