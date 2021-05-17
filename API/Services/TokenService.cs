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

            //Create signing credentials with the secret key and the HMAC alogrithm for encrypting/signing signatures
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            //Token descriptor containing the payload and signing credentials to encrypt/sign the signature
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds
            };

            var tokenHandler =  new JwtSecurityTokenHandler();

            //Creates a JWT token from tokendescriptor and uses the signing credentials to sign the signature
            var token = tokenHandler.CreateToken(tokenDescriptor);
            
            //Serializes the token to string and returns the token to calling method
            return tokenHandler.WriteToken(token);

        }
    }
}