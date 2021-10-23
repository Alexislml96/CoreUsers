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
using CORE.Users.Tools;
using Dapper;
using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace CORE.Users.Services
{
    public class UserService : IUser, IDisposable
    {
        private bool disposedValue;
        private IConnectionDB<UserModel> _conn;
        private List<Tuple<string, object, int>> _parameters = new List<Tuple<string, object, int>>();

        string _connectionString = string.Empty;
        public UserService(IConnectionDB<UserModel> conn)
        {
            _conn = conn;
        }
        public UserService(IConnectionDB<UserModel> conn, string connectionString)
        {
            _conn = conn;
            _connectionString = EncryptTool.Decrypt(connectionString);
        }

        public List<UserModel> GetUsers()
        {
            List<UserModel> list = new List<UserModel>();

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var Json = connection.QueryFirstOrDefault<string>("dbo.[USERS.Get_All]", null, commandType: CommandType.StoredProcedure);

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
                _parameters.Clear();
            }
        }
        public UserModel GetUser(int ID)
        {
            UserModel UsuarioResp = null;
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    UsuarioResp = (UserModel)connection.QueryFirst<UserModel>("dbo.[USERS.Get_Id]", new { Id = ID }, commandType: CommandType.StoredProcedure);
                }
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
                _parameters.Clear();
            }
        }
        public long AddUser(UserModel model)
        {
            long id = 0;

            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    id = connection.QueryFirstOrDefault<long>("dbo.[USERS.Set]", new { p_user_json = JsonConvert.SerializeObject(model) }, commandType: CommandType.StoredProcedure);
                }

                return id;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            finally
            {
                _parameters.Clear();
            }
        }
        public bool UpdateUser(UserModel model)
        {
            try
            {
                bool reply = false;
                using (var connection = new SqlConnection(_connectionString))
                {
                    var affectedRows = connection.QueryFirstOrDefault<long>("dbo.[USERS.Update]", new { p_user_json = JsonConvert.SerializeObject(model) }, commandType: CommandType.StoredProcedure);

                    reply = affectedRows < 1 ? false : true;
                }

                return reply;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            finally
            {
                _parameters.Clear();
            }
        }
        public void DeleteUser(int ID)
        {
            try
            {
                using (var connection = new SqlConnection(_connectionString))
                {
                    var affectedRows = connection.Execute("dbo.[USERS.Delete]", new { Id = ID }, commandType: CommandType.StoredProcedure);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
            finally
            {
                _parameters.Clear();
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
