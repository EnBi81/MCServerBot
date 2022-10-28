using Microsoft.IdentityModel.Tokens;
using Shared.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MCWebAPI.Controllers.Utils
{
    public class AuthUtils
    {
        public static List<Claim> GenerateClaims(DataUser user, IConfiguration config)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, config["Jwt:Subject"]),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Sid, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.UserType.ToString())
            };

            return claims.ToList();
        }

        public static string GenerateJwt(DataUser user, IConfiguration config)
        {
            List<Claim> claims = GenerateClaims(user, config);

            SymmetricSecurityKey key = new (Encoding.UTF8.GetBytes(config["Jwt:Key"]));
            SigningCredentials signIn = new (key, SecurityAlgorithms.HmacSha512);

            JwtHeader header = new (signIn);

            JwtPayload payload = new (
                config["Jwt:Issuer"],
                config["Jwt:Audience"],
                claims,
                null,
                DateTime.UtcNow.AddMinutes(60));

            JwtSecurityToken token = new (header, payload);

            string serializedToken = new JwtSecurityTokenHandler().WriteToken(token);
            return serializedToken;
        }
    }
}
