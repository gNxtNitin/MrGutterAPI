using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using MrQuote.Services.IServices;

namespace MrQuote.Services.Services
{
    //New
    public class JwtAuthService : IJwtAuthService
    {
        private readonly IConfiguration _configuration;

        public JwtAuthService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task<string> GenerateJwtToken(string name, string role)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(role))
            {
                return "Invalid parameters"; // Return a clear error message for invalid input
            }

            try
            {
                var claims = new[]
                {
                new Claim(ClaimTypes.Name, name),
                new Claim(ClaimTypes.Role, role)
            };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    //Expires = DateTime.UtcNow.AddMinutes(10),
                    Expires = DateTime.UtcNow.AddDays(10),
                    Issuer = _configuration["Jwt:Issuer"],
                    Audience = _configuration["Jwt:Audience"],
                    SigningCredentials = credentials
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                return await Task.FromResult(tokenString);
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }









        //Old
        //public class JwtAuthService : IJwtAuthService
        //{
        //    public IConfiguration _configuration;
        //    public JwtAuthService(IConfiguration configuration)
        //    {
        //        _configuration = configuration;
        //    }
        //    public async Task<string> GenerateJwtToken()//Name, Role
        //    {
        //        string user = "Test";
        //        string result = string.Empty;
        //        try
        //        {
        //            if (user != null)
        //            {
        //                //create claims details based on the user information
        //                var claims = new[] {
        //                    //new Claim(JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
        //                    //new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        //                    //new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
        //                    //new Claim("UserId", user.ToString()),
        //                    //new Claim("DisplayName", "DisplayName"),
        //                    //new Claim("UserName", "UserName"),
        //                    //new Claim("Email","Email"),
        //                    new Claim(ClaimTypes.NameIdentifier,user),
        //                    new Claim(ClaimTypes.Role,user)
        //                };

        //                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
        //                var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        //                var token = new JwtSecurityToken(
        //                    _configuration["Jwt:Issuer"],
        //                    _configuration["Jwt:Audience"],
        //                    claims,
        //                    expires: DateTime.UtcNow.AddMinutes(10),
        //                    signingCredentials: signIn);

        //                result = new JwtSecurityTokenHandler().WriteToken(token);
        //            }
        //            else
        //            {
        //                result = "Invalid credentials";
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            result = ex.Message;
        //        }
        //        return await Task.FromResult(result);
        //    }
        //}

    }
