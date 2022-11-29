using Microsoft.IdentityModel.Tokens;
using SharedPublic.DTOs;
using SharedPublic.DTOs.Enums;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MCWebAPI.Controllers.Utils
{
    /// <summary>
    /// Contains util methods for authentication.
    /// </summary>
    public class AuthUtils
    {
        /// <summary>
        /// Generates a list of claims for the user.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="platform"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static List<Claim> GenerateClaims(DataUser user, Platform platform, IConfiguration config)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, config["Jwt:Subject"]),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Sid, user.Id.ToString()),
                new Claim(ClaimTypes.Role, user.UserType.ToString()),
                new Claim("Platform", platform.ToString())
            };

            return claims.ToList();
        }

        /// <summary>
        /// Generates a JWT token for the user.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="platform"></param>
        /// <param name="config"></param>
        /// <returns></returns>
        public static string GenerateJwt(DataUser user, Platform platform, IConfiguration config)
        {
            List<Claim> claims = GenerateClaims(user, platform, config);

            SymmetricSecurityKey key = new (Encoding.UTF8.GetBytes(config["Jwt:Key"]));
            SigningCredentials signIn = new (key, SecurityAlgorithms.HmacSha512);

            JwtHeader header = new (signIn);

            JwtPayload payload = new (
                config["Jwt:Issuer"],
                config["Jwt:Audience"],
                claims,
                null,
                DateTime.UtcNow.AddDays(10));

            JwtSecurityToken token = new (header, payload);

            string serializedToken = new JwtSecurityTokenHandler().WriteToken(token);
            return serializedToken;
        }
    }
}
