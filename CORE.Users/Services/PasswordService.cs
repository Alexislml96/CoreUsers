using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alexis.CORE.Connection.Interfaces;
using CORE.Users.Interfaces;
using CORE.Users.Models;
using Dapper;

namespace CORE.Users.Services
{
    public class PasswordService : IPasswordService, IDisposable
    {
        private bool disposedValue;
        private IConnectionDB<LoginMinModel> _conn;
        DynamicParameters _parameters = new DynamicParameters();

        public PasswordService(IConnectionDB<LoginMinModel> conn)
        {
            _conn = conn;
        }

        public LoginMinModel CheckUser(LoginMinModel login)
        {
            try
            {
                LoginMinModel model = new LoginMinModel();
                _parameters.Add("@Nick", login.Nick, DbType.String, ParameterDirection.Input);
                _conn.PrepararProcedimiento("dbo.[CheckUser]", _parameters);
                model = _conn.QueryFirstOrDefaultDapper();
                return model;
            }
            catch (SqlException sqlEx)
            {
                throw new Exception(sqlEx.Message);
            }
            catch (MySql.Data.MySqlClient.MySqlException mysqlEx)
            {
                throw new Exception(mysqlEx.Message);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                _conn.Dispose();
            }


        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _conn.Dispose();// TODO: eliminar el estado administrado (objetos administrados)
                }

                // TODO: liberar los recursos no administrados (objetos no administrados) y reemplazar el finalizador
                // TODO: establecer los campos grandes como NULL
                disposedValue = true;
            }
        }

        // // TODO: reemplazar el finalizador solo si "Dispose(bool disposing)" tiene código para liberar los recursos no administrados
        // ~MinervaService()
        // {
        //     // No cambie este código. Coloque el código de limpieza en el método "Dispose(bool disposing)".
        //     Dispose(disposing: false);
        // }

        void IDisposable.Dispose()
        {
            // No cambie este código. Coloque el código de limpieza en el método "Dispose(bool disposing)".
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
