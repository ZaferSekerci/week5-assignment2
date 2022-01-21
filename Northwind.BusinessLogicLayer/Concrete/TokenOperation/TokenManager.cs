using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Northwind.EntityLayer.Concrete.Dtos.DtoTokenOperations;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Northwind.BusinessLogicLayer.Concrete.TokenOperation
{
    public class TokenManager
    {
        private readonly IConfiguration _configuration;

        public TokenManager(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string CreateAccessToken(DtoLoginUser dtoLoginUser)
        {
            var claims = new[]
            {

                new Claim(JwtRegisteredClaimNames.Sub, dtoLoginUser.Email),

                new Claim(JwtRegisteredClaimNames.Jti, dtoLoginUser.UserId.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, "Token");

            var claimsRoleList = new List<Claim>
            {   
                
                new Claim("role", "a")
            };

           
            var t = Encoding.UTF8.GetBytes(_configuration["Tokens:Key"]);
            var key = new SymmetricSecurityKey(t);

            
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

       
            var token = new JwtSecurityToken
            (
                issuer: _configuration["Tokens:Issuer"], 
                audience: _configuration["Tokens:Issuer"], 
                expires: DateTime.Now.AddMinutes(5), 
                notBefore: DateTime.Now, 
                signingCredentials: credentials, 
                claims: claimsIdentity.Claims 
            );

            var tokenHandler = new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token)
            };
            return tokenHandler.token;
        }
    }
}
