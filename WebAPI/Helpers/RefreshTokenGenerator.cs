using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;

namespace WebAPI.Helpers
{
    public class RefreshTokenGenerator
    {
        string _secretKey;
        string _audienceToken;
        string _issuerToken;
        string _expireTime;
        public RefreshTokenGenerator(string secretKey, string audienceToken, string issuerToken, string expireTime)
        {
            _secretKey = secretKey;
            _audienceToken = audienceToken;
            _issuerToken = issuerToken;
            _expireTime = expireTime;
        }
        public Tuple<string, DateTime> GenerateRefreshToken()
        {
            string token = string.Empty;
            DateTime expiryTime = DateTime.UtcNow.AddMinutes(double.Parse(_expireTime));

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtSecurityToken = tokenHandler.CreateJwtSecurityToken(
                audience: _audienceToken,
                issuer: _issuerToken,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToInt32(_expireTime)),
                signingCredentials: signingCredentials);

            token = tokenHandler.WriteToken(jwtSecurityToken);
            return Tuple.Create(token, expiryTime);
        }

        public bool Validate(string refreshToken)
        {
            TokenValidationParameters validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _issuerToken,
                ValidAudience = _audienceToken,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("REFRESH_SECRET"))),
                ClockSkew = TimeSpan.Zero,
            };
            JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                tokenHandler.ValidateToken(refreshToken, validationParameters, out SecurityToken validatedToken);
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }
    }
}
