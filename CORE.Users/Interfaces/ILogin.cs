using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CORE.Users.Models;

namespace CORE.Users.Interfaces
{
    public interface ILogin: IDisposable
    {
        LoginModel Login(LoginMinModel user);
    }
}
