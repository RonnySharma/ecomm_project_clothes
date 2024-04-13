using ecomm_project_clothes.Dataaccess.Data;
using ecomm_project_clothes.Dataaccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ecomm_project_clothes.Dataaccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _Context;
        internal DbSet<T> DbSet;
        public Repository (ApplicationDbContext context)
        {
            _Context = context;
            DbSet = _Context.Set<T>();
        }
        public void Add(T entity)
        {
            DbSet.Add(entity);
        }

        public T FirstOrDefault(Expression<Func<T, bool>> Filter = null, string IncludeProperties = null)
        {
            IQueryable<T> query = DbSet;
            if (Filter!=null) query=query.Where(Filter);
            if (IncludeProperties != null)
            {
                foreach (var includeproperties  in IncludeProperties.Split(new[] {','},StringSplitOptions.RemoveEmptyEntries))
                {
                    query=query.Include(includeproperties);
                }
            }
            return query.FirstOrDefault();
        }

        public T Get(int id)
        {
            return DbSet.Find(id);
        }
       
        public IEnumerable<T> GetAll(Expression<Func<T, bool>> Filter = null, Func<IEnumerable<T>, IOrderedQueryable<T>> orderby = null, string IncludeProperties = null)
        {
            IQueryable<T> query = DbSet;
            if (Filter != null)
                query = query.Where(Filter);
            if (IncludeProperties != null)
            {
                foreach (var includeprop in IncludeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeprop);
                }
            }
            if (orderby != null)

                return orderby(query).ToList();
            return query.ToList();
        }

        public void Remove(T entity)
        {
            DbSet.Remove(entity);
        }

        public void Remove(int id)
        {
           //T entity = DbSet.Find(id);
            //DbSet.Remove(entity);
            DbSet.Remove(Get(id));
        }

        public void RemoveRange(IEnumerable<T> values)
        {
            DbSet.RemoveRange(values);
        }

        public void Update(T entity)
        {
            _Context.ChangeTracker.Clear();
            DbSet.Update(entity);
        }
        
    }
}
