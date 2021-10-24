using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CORE.Users.Models
{
    public class PasswordOptions
    {
        public PasswordOptions()
        {
            Iterations = 10000;
            SaltSize = 16;
            KeySize = 32;

        }
        public int SaltSize { get; private set; }
        public int KeySize { get; private set; }
        public int Iterations { get; private set; }
    }
}
