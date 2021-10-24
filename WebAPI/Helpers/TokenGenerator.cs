using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace WebApiUsers.Helpers
{
    internal static class TokenGenerator
    {
        public static string GenerateTokenJwt(
            string username,
            long id,
            string _secretKey,
            string _audienceToken,
            string _issuerToken,
            string _expireTime
            )
        {
            var secretKey = _secretKey;
            var audienceToken = _audienceToken;
            var issuerToken = _issuerToken;
            var expireTime = _expireTime;

            // appsetting for Token JWT
            /*var secretKey = _configuration["JWT:SECRET_KEY"];
            var audienceToken = _configuration["JWT:AUDIENCE_TOKEN"];
            var issuerToken = _configuration["JWT:ISSUER_TOKEN"]; //editor
            var expireTime = _configuration["JWT:EXPIRE_MINUTES"];*/

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            // create a claimsIdentity
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, username), new Claim("UserId", id.ToString()) });

            // create token to the user
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = tokenHandler.CreateJwtSecurityToken(
                audience: audienceToken,
                issuer: issuerToken,
                subject: claimsIdentity,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToInt32(expireTime)),
                signingCredentials: signingCredentials);

            var jwtTokenString = tokenHandler.WriteToken(jwtSecurityToken);
            return jwtTokenString;
        }
    }
}

