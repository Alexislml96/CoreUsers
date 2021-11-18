using CORE.Users.Interfaces;
using CORE.Users.Models;
using CORE.Users.Services;
using CORE.Users.Tools;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Helpers;
using WebApiUsers.Helpers;

namespace WebApiUsers.Controllers
{
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly HashTool _hashTool;
        private readonly RefreshTokenGenerator _tokenGenerator;
        string ConnectionStringAzure = string.Empty;
        string _secretKey;
        string _audienceToken;
        string _issuerToken;
        string _expireTime;
        string _refreshKey;
        string _refreshExpire;
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
                _refreshKey = Environment.GetEnvironmentVariable("REFRESH_SECRET");
                _refreshExpire = Environment.GetEnvironmentVariable("REFRESH_EXPIRE");
            }
            else
            {
                _secretKey = _configuration["JWT:SECRET_KEY"];
                _audienceToken = _configuration["JWT:AUDIENCE_TOKEN"];
                _issuerToken = _configuration["JWT:ISSUER_TOKEN"];
                _expireTime = _configuration["JWT:EXPIRE_MINUTES"];
                _refreshKey = _configuration["JWT:REFRESH_SECRET"];
                _refreshExpire = _configuration["JWT:REFRESH_EXPIRE"]; ;
            }
            _tokenGenerator = new RefreshTokenGenerator(_refreshKey, _audienceToken, _issuerToken, _refreshExpire);
        }

        //https://localhost:5001/Login
        [AllowAnonymous]
        [HttpPost("Login")]
        public ActionResult<LoginMinModel> Post([FromBody] LoginMinModel login)
        {
            long idRefresh = 0;
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
                var refresh = _tokenGenerator.GenerateRefreshToken();
                model.RefreshToken = refresh.Item1;
                model.RefreshTokenExpiryTime = refresh.Item2;
                RefreshToken refreshToken = new RefreshToken
                {
                    token = model.RefreshToken,
                    user_id = model.Id,
                };
                using (IRefreshService service = FactorizerService.Refresh(ConnectionStringAzure == string.Empty ? EServer.LOCAL : EServer.CLOUD))
                {
                    idRefresh = service.Create(refreshToken);
                }

                if (!(idRefresh > 0))
                    return BadRequest();

                return Ok(model);
            }

            return NotFound();
        }

        [HttpPost("refresh")]
        public ActionResult<LoginMinModel> Refresh([FromBody] RefreshRequest refreshRequest)
        {
            long idRefresh = 0;
            RefreshToken tokenModel = new RefreshToken();
            UserModel userModel = new UserModel();
            LoginModel model = new LoginModel();
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var isValid = _tokenGenerator.Validate(refreshRequest.refreshToken);

            if (!isValid)
            {
                return BadRequest();
            }
            //Get token data by the token itself
            using (IRefreshService service = FactorizerService.Refresh(ConnectionStringAzure == string.Empty ? EServer.LOCAL : EServer.CLOUD))
            {
                tokenModel = service.GetByToken(refreshRequest.refreshToken);
            }
            if (tokenModel == null)
            {
                return NotFound();
            }

            //Delete refresh token from DB
            using (IRefreshService service = FactorizerService.Refresh(ConnectionStringAzure == string.Empty ? EServer.LOCAL : EServer.CLOUD))
            {
                service.Delete(tokenModel.token_id);
            }

            //Get user and retrieve refresh token and JWT
            using (IUser userService = FactorizerService.Inicializar(ConnectionStringAzure == string.Empty ? EServer.LOCAL : EServer.CLOUD))
            {
                userModel = userService.GetUser(Convert.ToInt32(tokenModel.user_id));
            }
            if (userModel == null)
            {
                return NotFound();
            }

            model.Token = TokenGenerator.GenerateTokenJwt(userModel.Name, userModel.Identificador, _secretKey, _audienceToken, _issuerToken, _expireTime);
            var refresh = _tokenGenerator.GenerateRefreshToken();
            model.RefreshToken = refresh.Item1;
            model.RefreshTokenExpiryTime = refresh.Item2;
            RefreshToken refreshToken = new RefreshToken
            {
                token = model.RefreshToken,
                user_id = tokenModel.user_id,
            };
            using (IRefreshService service = FactorizerService.Refresh(ConnectionStringAzure == string.Empty ? EServer.LOCAL : EServer.CLOUD))
            {
                idRefresh = service.Create(refreshToken);
            }

            if (!(idRefresh > 0))
                return BadRequest();

            return Ok(model);


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

