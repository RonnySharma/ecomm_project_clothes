using ecomm_project_clothes.Dataaccess.Data;
using ecomm_project_clothes.Dataaccess.Repository.IRepository;
using ecomm_project_clothes.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ecomm_project_clothes.Dataaccess.Repository
{
    public class ClothesTypeRepository : Repository<clothesType>, IClothesTypeRepostiry

    {
        private readonly ApplicationDbContext _context;
        public ClothesTypeRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
