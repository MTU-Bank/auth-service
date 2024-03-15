using Microsoft.IdentityModel.Tokens;
using MTUModelContainer.Database.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace MTUBankBase.Auth
{
    public class JwtService
    {
        private readonly SecurityKey _secretKey;
        private readonly string _issuer;

        public JwtService(SecurityKey secretKey, string issuer)
        {
            _secretKey = secretKey;
            _issuer = issuer;
        }

        public string GenerateToken(User u)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(BuildClaims(u)),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = _issuer,
                SigningCredentials = new SigningCredentials(_secretKey, SecurityAlgorithms.RsaSha256)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private Claim[] BuildClaims(User u)
        {
            var claims = new List<Claim>();

            // get all public properties that are NON-foreign keys
            var allProperties = u.GetType().GetProperties(BindingFlags.Public);
            var nonForeignProps = allProperties.Where((z) => z.GetCustomAttribute(typeof(ForeignKeyAttribute)) is null)
                                               .ToList();

            foreach (var prop in nonForeignProps)
            {
                // get value
                var propValue = prop.GetValue(u);

                // attempt to get string value
                var strVal = propValue as string;
                if (strVal is null && propValue is not null) continue; // cannot be casted to string

                claims.Add(new Claim(prop.Name, strVal));
            }

            return claims.ToArray();
        }
    }
}
