using CORE.Users;
using CORE.Users.Interfaces;
using CORE.Users.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        string ConnectionStringAzure = string.Empty;

        public UsersController(IConfiguration configuration)
        {
            _configuration = configuration;
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production")
                ConnectionStringAzure = _configuration.GetConnectionString("CloudServer");
        }

        ///https://localhost:5001/api/User/GetUsers
        [HttpGet("GetUsers")]
        public IEnumerable<UserModel> GetUsers()
        {

            List<UserModel> model = new List<UserModel>();
            using (IUser User = CORE.Users.Services.FactorizerService.Inicializar(ConnectionStringAzure == string.Empty ? EServer.LOCAL : EServer.CLOUD))
                //using (IUser User = Users_CORE.Services.FactorizerService.Inicializar(ConnectionStringAzure == string.Empty ? Users_CORE.Models.EServer.CLOUD : Users_CORE.Models.EServer.CLOUD))
            {
                model = User.GetUsers();
            }

            return model;

        }


    }
}
