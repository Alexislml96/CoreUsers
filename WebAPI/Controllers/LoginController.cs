using CORE.Users.Interfaces;
using CORE.Users.Models;
using CORE.Users.Services;
using CORE.Users.Tools;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApiUsers.Helpers;

namespace WebApiUsers.Controllers
{
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly HashTool _hashTool;
        string ConnectionStringAzure = string.Empty;
        string _secretKey;
        string _audienceToken;
        string _issuerToken;
        string _expireTime;
        public LoginController(IConfiguration configuration)
        {
            _configuration = configuration;
            _hashTool = new HashTool();
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production")
            {
                ConnectionStringAzure = _configuration.GetConnectionString("CloudServer");
                _secretKey = Environment.GetEnvironmentVariable("SECRET_KEY");
                _audienceToken = Environment.GetEnvironmentVariable("AUDIENCE_TOKEN");
                _issuerToken = Environment.GetEnvironmentVariable("ISSUER_TOKEN");
                _expireTime = Environment.GetEnvironmentVariable("EXPIRE_MINUTES");
            }
            else
            {
                _secretKey = _configuration["JWT:SECRET_KEY"];
                _audienceToken = _configuration["JWT:AUDIENCE_TOKEN"];
                _issuerToken = _configuration["JWT:ISSUER_TOKEN"];
                _expireTime = _configuration["JWT:EXPIRE_MINUTES"];
            }
        }

        //https://localhost:5001/Login
        [AllowAnonymous]
        [HttpPost("Login")]
        public ActionResult<LoginMinModel> Post([FromBody] LoginMinModel login)
        {
            if (string.IsNullOrEmpty(login.Nick))
                throw new NullReferenceException("Nick vacío, el campo es necesario");
            if (string.IsNullOrEmpty(login.Password))
                throw new NullReferenceException("Password vacío, el campo es necesario");
            var validation = UsuarioValido(login);

            if (validation.Item1)
            {
                LoginModel model = new LoginModel();
                using (ILogin User = FactorizerService.Login(ConnectionStringAzure == string.Empty ? EServer.LOCAL : EServer.CLOUD))
                {
                    model = User.Login(validation.Item2);
                }

                model.Token = TokenGenerator.GenerateTokenJwt(model.Name, model.Id, _secretKey, _audienceToken, _issuerToken, _expireTime);
                return Ok(model);
            }

            return NotFound();
        }

        private Tuple<bool, LoginMinModel> UsuarioValido(LoginMinModel login)
        {
            LoginMinModel user = new LoginMinModel();
            using (IPasswordService service = FactorizerService.Password(ConnectionStringAzure == string.Empty ? EServer.LOCAL : EServer.CLOUD))
            {
                user = service.CheckUser(login);
            }

            var isValid = _hashTool.Check(user.Password, login.Password);
            var res = Tuple.Create(isValid, user);
            return res;
        }

    }
}

