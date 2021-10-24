using CORE.Users.Interfaces;
using CORE.Users.Models;
using Alexis.CORE.Connection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alexis.CORE.Connection.Models;
using CORE.Users.Configuration;

namespace CORE.Users.Services
{
    public  class FactorizerService
    {
        public static IUser Inicializar(EServer typeServer)
        {
            return typeServer switch
            {
                EServer.UDEFINED => throw new NullReferenceException(),
                EServer.LOCAL => new UserService(BridgeDBConnection<UserModel>.Create(ConnectionStrings.LocalServer, DbEnum.Sql)),
                EServer.CLOUD => new UserService(BridgeDBConnection<UserModel>.Create(ConnectionStrings.CloudServer, DbEnum.Sql)),
                _ => throw new NullReferenceException(),
            };

        }

        public static ILogin Login(EServer typeServer)
        {
            return typeServer switch
            {
                EServer.UDEFINED => throw new NullReferenceException(),
                EServer.LOCAL => new LoginService(BridgeDBConnection<LoginModel>.Create(ConnectionStrings.LocalServer, DbEnum.Sql)),
                EServer.CLOUD => new LoginService(BridgeDBConnection<LoginModel>.Create(ConnectionStrings.CloudServer, DbEnum.Sql)),
                _ => throw new NullReferenceException(),
            };

        }

        public static IPasswordService Password(EServer typerServer)
        {
            return typerServer switch
            {
                EServer.UDEFINED => throw new NullReferenceException(),
                EServer.LOCAL => new PasswordService(BridgeDBConnection<LoginMinModel>.Create(ConnectionStrings.LocalServer, DbEnum.Sql)),
                EServer.CLOUD => new PasswordService(BridgeDBConnection<LoginMinModel>.Create(ConnectionStrings.CloudServer, DbEnum.Sql)),
                _ => throw new NullReferenceException(),
            };
        }
    }
}
