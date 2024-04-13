using Project_EComm_App_1035.DataAccess.Data;
using Project_EComm_App_1035.DataAccess.Repositry.IRepositry;
using Project_EComm_App_1035.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project_EComm_App_1035.DataAccess.Repositry
{
    public class CompanyRepositry:Repositry<Company>,ICompanyRepositry
    {
        private readonly ApplicationDbContext _context;
        public CompanyRepositry(ApplicationDbContext context):base(context) 
        {
            _context = context;   
        }
    }
}
