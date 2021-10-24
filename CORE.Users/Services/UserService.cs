using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alexis.CORE.Connection.Interfaces;
using Alexis.CORE.Connection.Models;
using CORE.Users.Interfaces;
using CORE.Users.Models;
using CORE.Users.Tools;
using Dapper;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CORE.Users.Services
{
    public class UserService : IUser, IDisposable
    {
        bool disposedValue;
        IConnectionDB<UserModel> _conn;
        private readonly HashTool _hashTool;
        DynamicParameters _parameters = new DynamicParameters();
        public UserService(IConnectionDB<UserModel> conn)
        {
            _conn = conn;
            _hashTool = new HashTool();
        }
        public List<Models.UserModel> GetUsers()
        {
            List<UserModel> list = new List<UserModel>();

            try
            {
                _conn.PrepararProcedimiento("dbo.[USERS.Get_All]", null);
                var Json = (string)_conn.QueryFirstOrDefaultDapper(TipoDato.Cadena);
                if (Json != string.Empty)
                {
                    JArray arr = JArray.Parse(Json);
                    foreach (JObject jsonOperaciones in arr.Children<JObject>())
                    {
                        list.Add(new UserModel()
                        {
                            Identificador = Convert.ToInt32(jsonOperaciones["Id"].ToString()),
                            Name = jsonOperaciones["Name"].ToString(),
                            LastName = jsonOperaciones["LastName"].ToString(),
                            Nick = jsonOperaciones["Nick"].ToString(),
                            CreateDate = DateTime.Parse(jsonOperaciones["CreateDate"].ToString())
                        });

                    }
                }

                return list;
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
        public UserModel GetUser(int ID)
        {
            UserModel UsuarioResp = null;
            try
            {
                _parameters.Add("@Id", ID, DbType.Int32, ParameterDirection.Input);
                _conn.PrepararProcedimiento("dbo.[USERS.Get_Id]", _parameters);
                UsuarioResp = _conn.QueryFirstOrDefaultDapper();
                return UsuarioResp;
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
        public long AddUser(UserModel model)
        {
            long id = 0;
            model.Password = _hashTool.Hash(model.Password);

            try
            {
                _parameters.Add("@p_user_json", JsonConvert.SerializeObject(model), DbType.String, ParameterDirection.Input);
                _conn.PrepararProcedimiento("dbo.[USERS.Set]", _parameters);
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
        public bool UpdateUser(UserModel model)
        {
            try
            {
                bool reply = false;
                _parameters.Add("@p_user_json", JsonConvert.SerializeObject(model), DbType.String, ParameterDirection.Input);
                _conn.PrepararProcedimiento("dbo.[USERS.Update]", _parameters);
                var affectedRows = (long)_conn.QueryFirstOrDefaultDapper(TipoDato.Numerico);
                reply = affectedRows < 1 ? false : true;
                return reply;
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
        public void DeleteUser(int ID)
        {
            try
            {
                _parameters.Add("@Id", ID, DbType.Int32, ParameterDirection.Input);
                _conn.PrepararProcedimiento("dbo.[USERS.Delete]", _parameters);
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
        #endregion
    }

}
