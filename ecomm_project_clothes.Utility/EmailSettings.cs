using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ecomm_project_clothes.Utility
{
    public class EmailSettings
    {
        public string PrimaryDomain { get; set; }
        public int primaryPort { get; set; }
        public string secondaryDomain { get; set; }
        public int secondaryPort { get; set; }
        public string UsernameEmail { get; set; }
        public string UsernamePassword { get; set; }
        public string FromEmail { get; set; }
        public string ToEmail { get; set; }
        public string CcEmail { get; set; }
    }
}
