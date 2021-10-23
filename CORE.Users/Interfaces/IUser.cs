using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CORE.Users.Models;

namespace CORE.Users.Interfaces
{
    public  interface IUser: IDisposable
    {
        List<UserModel> GetUsers();
        UserModel GetUser(int ID);
        long AddUser(UserModel model);
        bool UpdateUser(UserModel model);
        void DeleteUser(int ID);
    }
}
