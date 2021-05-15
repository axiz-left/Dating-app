using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using API.Entities;
using API.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace API.Services
{
    public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey _key;
        public TokenService(IConfiguration config)
        {
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["TokenKey"]));
        }

        public string CreateToken(AppUser appUser)
        {
            var claims = new List<Claim> //creates the claim data
            {
                new Claim(JwtRegisteredClaimNames.NameId,appUser.UserName)
            };

            //Signing credentials used to sign the token
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            //Token descriptor describing the payload and signing credentials to sign the token raw data
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds
            };

            var tokenHandler =  new JwtSecurityTokenHandler();

            //Creates a JWT token from tokendescriptor by encrypting it using the signing credentials in token descriptor
            var token = tokenHandler.CreateToken(tokenDescriptor);
            
            //Serializes the token to string and returns the token to calling method
            return tokenHandler.WriteToken(token);

        }
    }
}