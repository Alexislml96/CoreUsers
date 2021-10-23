using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CORE.Users.Models
{
    public class BaseModel
    {
        private int user_id;
        private string user_nick;
        private string user_password;

        public int Identificador { get => user_id; set => user_id = value; }

        public string Nick { get => user_nick; set => user_nick = value; }

        public string Password { get => user_password; set => user_password = value; }

    }
}
