using BusinessObject;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class JwtGenerator
    {
        public IConfiguration _configuration;
        public JwtGenerator(IConfiguration configuration) {
            _configuration = configuration;
        }
        //public string GenerateJwtToken(SystemAccount user)
        //{
        //    var claims = new[]
        //    {
        //        new Claim(JwtRegisteredClaimNames.Sub, user.AccountEmail),
        //        new Claim(ClaimTypes.NameIdentifier, user.AccountId.ToString()),
        //        new Claim(ClaimTypes.Role, user.AccountRole.ToString()) // Adding role claim
        //    };

        //    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        //    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        //    var token = new JwtSecurityToken(
        //        issuer: _configuration["Jwt:Issuer"],
        //        audience: _configuration["Jwt:Audience"],
        //        claims: claims,
        //        expires: DateTime.UtcNow.AddHours(1),
        //        signingCredentials: creds
        //    );

        //    return new JwtSecurityTokenHandler().WriteToken(token);
        //}

        public string GenerateToken(SystemAccount user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                    new Claim("email", user.AccountEmail),
                    new Claim(ClaimTypes.Role, user.AccountRole.ToString()),
                    new Claim(ClaimTypes.NameIdentifier,user.AccountId.ToString())

                     }),
                    Expires = DateTime.UtcNow.AddHours(1),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])), SecurityAlgorithms.HmacSha256Signature)
                };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public string GenerateTokenAdmin(SystemAccount user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                    {
                    new Claim("email", user.AccountEmail),
                    new Claim(ClaimTypes.Role, user.AccountRole.ToString()),         
                    }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"])), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        private class InvalidToken
        {
            public string Token { get; set; }
            public DateTime InvalidatedAt { get; set; }
        }
        private static List<InvalidToken> InvalidTokens = new List<InvalidToken>();
        public static void InvalidateToken(string token)
        {
            InvalidTokens.Add(new InvalidToken { Token = token, InvalidatedAt = DateTime.UtcNow });
        }
        public static bool IsTokenValid(string token)
        {
            // Clear expired tokens before checking validity
            ClearExpiredTokens();

            return !InvalidTokens.Any(t => t.Token == token);
        }

        private static void ClearExpiredTokens()
        {
            var expirationThreshold = DateTime.UtcNow.AddHours(-1); // Set your expiration duration

            InvalidTokens.RemoveAll(t => t.InvalidatedAt < expirationThreshold);
        }

        public static string ExtractUserIdFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

            // Extract the user ID claim
            var uid = jsonToken?.Claims.FirstOrDefault(c => c.Type.Equals("nameid"))?.Value;
            if (uid == null)
            {
                uid = jsonToken?.Claims.FirstOrDefault(c => c.Type.Equals("email"))?.Value;
            }
            return uid;
        }
    }
}
