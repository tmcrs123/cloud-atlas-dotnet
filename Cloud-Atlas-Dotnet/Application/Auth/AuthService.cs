using Cloud_Atlas_Dotnet.Application.Configuration;
using Cloud_Atlas_Dotnet.Domain;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Cloud_Atlas_Dotnet.Application.Auth
{
    public interface IAuthService
    {
        string GenerateJwtToken(string username, string email, string id);
    }

    public class AuthService : IAuthService
    {
        private readonly IOptions<AppSettings> _settings;

        public AuthService(IOptions<AppSettings> settings)
        {
            _settings = settings;
        }

        string IAuthService.GenerateJwtToken(string username, string email, string id)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.PrimarySid, id),
                new Claim(ClaimTypes.Role, Roles.Owner.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.Value.JwtSecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMinutes(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
