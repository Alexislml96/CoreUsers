using Alexis.CORE.Connection.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alexis.CORE.Connection;
using Alexis.CORE.Connection.Models;
using CORE.Users.Tools;

namespace CORE.Users.Configuration
{
    public class BridgeDBConnection<T>
    {
        public static IConnectionDB<T> Create(string ConnectionString, DbEnum DB)
        {
            return Factorizer<T>.Create(EncryptTool.Decrypt(ConnectionString), DB);
        }
    }
}
