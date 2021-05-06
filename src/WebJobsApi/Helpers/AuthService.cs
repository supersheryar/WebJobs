using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace WebJobsApi.Helpers
{
    public interface IAuthService
    {
        public string Authenticate([FromBody] string apiholekey);
    }

    public class AuthService: IAuthService
    {
        private readonly WJaSettings _wjaSettings;

        public AuthService(IOptions<WJaSettings> wjaSettings)
        {
            _wjaSettings = wjaSettings.Value;
        }

        public string Authenticate([FromBody] string apiholekey)
        {
            if (_wjaSettings.ApiHoleKey != apiholekey) return null;

            return generateJwtToken(_wjaSettings.ApiHoleEmail);
        }

        private string generateJwtToken(string email)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_wjaSettings.SecurityKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("LoginEmail", email) }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}