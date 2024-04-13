using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_EComm_App_1035.DataAccess.Repositry.IRepositry
{
    public interface ISPCALL:IDisposable// to use dispose member
    {
        void Execute(string ProcedureName, DynamicParameters param=null);//save,update,delete
        T single<T>(string ProcedureName,DynamicParameters param=null);//Find
        T OneRecord<T>( string ProcedureName, DynamicParameters param = null);//FirstOrDefault
        IEnumerable<T> List<T>(string ProcedureName, DynamicParameters param = null);//display
        Tuple<IEnumerable<T1>, IEnumerable<T2>> List<T1, T2>(string ProcedureName, DynamicParameters param = null);//Execute Multiple Queries

    }
}
