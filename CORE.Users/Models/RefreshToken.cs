using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CORE.Users.Models
{
    public  class RefreshToken
    {
        public long token_id { get; set; }
        public long user_id { get; set; }
        public string token { get; set; }
    }
}
