﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ecomm_project_clothes.Dataaccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        void Add(T entity);
        void Update(T entity);
        void Remove(T entity);
        void Remove(int id);
        void RemoveRange(IEnumerable<T> values);
        T Get(int id);
        IEnumerable<T> GetAll(
            Expression<Func<T,bool>>Filter=null,
            Func<IEnumerable<T>,IOrderedQueryable<T>>orderby=null,
            string IncludeProperties=null);
        T FirstOrDefault(
            Expression<Func<T, bool>> Filter = null,
            string IncludeProperties = null);
    }
}
