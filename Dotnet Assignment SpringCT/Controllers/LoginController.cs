using Dotnet_Assignment_SpringCT.Helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Dotnet_Assignment_SpringCT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public LoginController(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult Login([FromBody] UserLogin loginInfo)
        {
            var user = AuthenticateUser(loginInfo);

            if (user != null)
            {
                var token = GenerateToken(user);
                return Ok(new { token });
            }
            return BadRequest();
        }
        private UserLogin AuthenticateUser(UserLogin loginInfo)
        {
            if (loginInfo.UserName == "Admin" && loginInfo.Password == "1234")
            {
                return new UserLogin { UserName = "Admin", Password = "1234" };
            }
            return null;
        }
        private string GenerateToken(UserLogin loginInfo)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: credentials
                );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
