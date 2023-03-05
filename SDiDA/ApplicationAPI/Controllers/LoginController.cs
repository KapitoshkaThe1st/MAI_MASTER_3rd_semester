using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;

using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

using BC = BCrypt.Net.BCrypt;

using ApplicationAPI.Repository;
using ApplicationAPI.DTOs;
using ApplicationAPI.Entities;


namespace WebApplication.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IRepositoryManager _repositoryManager;

        public LoginController(IConfiguration config, IRepositoryManager repositoryManager)
        {
            _config = config;
            _repositoryManager = repositoryManager;
        }

        [AllowAnonymous]
        [HttpPost("")]
        public async Task<IActionResult> Login([FromBody] UserAccountLoginDto userLoginDto)
        {
            var user = await Authenticate(userLoginDto);
            if(user != null)
            {
                var token = GenerateJWT(user);
                return Ok(new { jwtToken = token });
            }
            return NotFound("User not found");
        }

        private string GenerateJWT(UserProfile user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Username),
            };

            var token = new JwtSecurityToken(
                _config["Jwt:Issuer"],
                _config["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(15),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<UserProfile> Authenticate(UserAccountLoginDto userAccountDto)
        {
            var userAccount = await _repositoryManager.UserAccountsRepository.GetUserAccountByLogin(userAccountDto.Login);
            if (!BC.Verify(userAccountDto.Password, userAccount.PasswordHash))
            {
                return null;
            }

            return await _repositoryManager.UserProfilesRepository.GetUserProfileById(userAccount.ProfileId);
        }
    }
}
