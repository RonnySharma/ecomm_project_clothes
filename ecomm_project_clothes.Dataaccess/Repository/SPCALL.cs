using Dapper;
using ecomm_project_clothes.Dataaccess.Data;
using ecomm_project_clothes.Dataaccess.Repository.IRepository;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ecomm_project_clothes.Dataaccess.Repository
{
    public class SPCALL : ISPCALL
    {
        private readonly ApplicationDbContext _context;
        private static string ConnectionString = "";
        public SPCALL(ApplicationDbContext context)
        {
            _context = context;
            ConnectionString = _context.Database.GetDbConnection().ConnectionString;
        }
        public void Dispose()
        {
           _context.Dispose();
        }

        public void Execute(string procedurename, DynamicParameters param = null)
        {
           using (SqlConnection sqlCon = new SqlConnection(ConnectionString)) 
            {
                sqlCon.Open();
                sqlCon.Execute(procedurename, param, commandType: CommandType.StoredProcedure);
           
                    }
        }

        public IEnumerable<T> List<T>(string procedurename, DynamicParameters param = null)
        {
            using (SqlConnection sqlCon = new SqlConnection(ConnectionString))
            {
                sqlCon.Open(); 
                    return sqlCon.Query<T>(procedurename, param,commandType:CommandType.StoredProcedure);
            }
        }

        public Tuple<IEnumerable<T1>, IEnumerable<T2>> List<T1, T2>(string procedurename, DynamicParameters param = null)
        {
            using(SqlConnection sqlCon= new SqlConnection(ConnectionString))
            {
                sqlCon.Open();
                var result=sqlCon.QueryMultiple(procedurename, param,commandType:CommandType.StoredProcedure);
                var item1=result.Read<T1>();    
                var item2=result.Read<T2>();
                if (item1 != null && item2 != null) 
                    return new Tuple<IEnumerable<T1>, IEnumerable<T2>>(item1, item2);
                return new Tuple<IEnumerable<T1>,IEnumerable<T2>>(new List<T1>(),new List<T2>());
            }
        }

        public T OneRecord<T>(string procedurename, DynamicParameters param = null)
        {
             using (SqlConnection sqlCon=new SqlConnection(ConnectionString)) 
            {
                sqlCon.Open();
                var value=sqlCon.Query<T>(procedurename, param,commandType:CommandType.StoredProcedure);
                return value.FirstOrDefault();
            }
        }

        public T Single<T>(string procedurename, DynamicParameters param = null)
        {
            using (SqlConnection sqlCon=new SqlConnection(ConnectionString)) { sqlCon.Open();
            return sqlCon.ExecuteScalar<T>(procedurename, param,commandType:CommandType.StoredProcedure);}
        }
    }
}
