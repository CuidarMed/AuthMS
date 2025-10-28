using Application.Interfaces.IServices.IAuthServices;
using Domain.Entities;
using System.Security.Cryptography;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Service
{
    public class JwtService : IAuthTokenService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> GenerateAccessToken(User user)
        {

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:key"]!));

            var signingCredentials = new SigningCredentials(
                key: securityKey,
                algorithm: SecurityAlgorithms.HmacSha256Signature
            );

            var claims = new ClaimsIdentity();
            claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()));
            claims.AddClaim(new Claim(JwtRegisteredClaimNames.Sub, user.UserId.ToString()));
            claims.AddClaim(new Claim(ClaimTypes.Role, user.Role));
            claims.AddClaim(new Claim("IsActive", user.IsActive.ToString()));
            claims.AddClaim(new Claim("UserId", user.UserId.ToString()));
            claims.AddClaim(new Claim("FirstName", user.FirstName.ToString()));
            claims.AddClaim(new Claim("LastName", user.LastName.ToString()));
            //claims.AddClaim(new Claim("Email", user.Email.ToString()));
            //claims.AddClaim(new Claim("Dni", user.Dni.ToString()));


            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claims,
                Expires = DateTime.UtcNow.AddMinutes(int.Parse(_configuration["JwtSettings:TokenExpirationMinutes"]!)),
                IssuedAt = DateTime.UtcNow,
                SigningCredentials = signingCredentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();            
            var tokenConfig = tokenHandler.CreateToken(tokenDescriptor);
            var serializedJwt = tokenHandler.WriteToken(tokenConfig);

            return serializedJwt;
        }

        public Task<string> GenerateRefreshToken()
        {
            var size = int.Parse(_configuration["RefreshTokenSettings:Lenght"]!);
            var buffer = new byte[size];
            using var rn = RandomNumberGenerator.Create();
            rn.GetBytes(buffer);

            return Task.FromResult(Convert.ToBase64String(buffer));
        }
        public Task<int> GetRefreshTokenLifetimeInMinutes()
        {
            return Task.FromResult(int.Parse(_configuration["RefreshTokenSettings:LifeTimeInMinutes"]!));
        }
        
    }
    
}
