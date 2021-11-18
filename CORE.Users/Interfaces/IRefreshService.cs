using CORE.Users.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CORE.Users.Interfaces
{
    public interface IRefreshService: IDisposable
    {
        RefreshToken GetByToken(string token);
        long Create(RefreshToken refreshToken);

        void Delete(long id);
    }
}
