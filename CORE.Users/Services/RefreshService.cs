using Alexis.CORE.Connection.Interfaces;
using CORE.Users.Interfaces;
using CORE.Users.Models;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data;
using Alexis.CORE.Connection.Models;
using System.Data.SqlClient;

namespace CORE.Users.Services
{
    public class RefreshService : IRefreshService, IDisposable
    {
        private bool disposedValue;
        private IConnectionDB<RefreshToken> _conn;
        DynamicParameters _parameters = new DynamicParameters();
        public RefreshService(IConnectionDB<RefreshToken> conn)
        {
            _conn = conn;
        }

        public long Create(RefreshToken refreshToken)
        {
            long id = 0;
            try
            {
                RefreshToken model = new RefreshToken();
                _parameters.Add("@p_user_json", JsonConvert.SerializeObject(refreshToken), DbType.String, ParameterDirection.Input);
                _conn.PrepararProcedimiento("dbo.[Set_Tokens]", _parameters);
                id = (long)_conn.QueryFirstOrDefaultDapper(TipoDato.Numerico);
                return id;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            finally
            {
                _conn.Dispose();
            }
        }

        public void Delete(long id)
        {
            try
            {
                _parameters.Add("@Id", id, DbType.Int32, ParameterDirection.Input);
                _conn.PrepararProcedimiento("dbo.[Delete_Tokens]", _parameters);
                var affectedRows = _conn.Query();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            finally
            {
                _conn.Dispose();
            }
        }

        public RefreshToken GetByToken(string token)
        {
            RefreshToken model = null;
            try
            {
                _parameters.Add("@Token", token, DbType.String, ParameterDirection.Input);
                _conn.PrepararProcedimiento("dbo.[Get_Tokens]", _parameters);
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

        #region Dispose

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
#endregion