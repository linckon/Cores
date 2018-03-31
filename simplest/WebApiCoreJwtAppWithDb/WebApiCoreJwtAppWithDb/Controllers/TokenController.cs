using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using WebApiCoreJwtAppWithDb.Models;

namespace WebApiCoreJwtAppWithDb.Controllers
{
    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class UserModel
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime Birthdate { get; set; }
    }


    [Produces("application/json")]
    [Route("api/Token")]
    public class TokenController : Controller
    {
        private UserManager<ApplicationUser> userManager;

        public TokenController(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult CreateToken([FromBody]LoginModel login)
        {
            IActionResult response = Unauthorized();
            var user = Authenticate(login);

            if (user != null)
            {
                var tokenString = BuildToken(user);
                response = Ok(new { token = tokenString });
            }

            return response;
        }

        private string BuildToken(ApplicationUser user)
        {
            var claims = new[] {
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
                new Claim(JwtRegisteredClaimNames.Sub, user.Name),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            string bizbook365 = "http://bizbook365.com";
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(bizbook365));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                bizbook365,
                bizbook365,
                claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            JwtSecurityTokenHandler securityTokenHandler = new JwtSecurityTokenHandler();
            string token = securityTokenHandler.WriteToken(jwtSecurityToken);
            return token;
        }


        private ApplicationUser Authenticate(LoginModel login)
        {
            UserModel user = null;

            if (login.Username == "mario" && login.Password == "secret")
            {
                user = new UserModel { Name = "Mario Rossi", Email = "mario.rossi@domain.com" };
            }
            //       var userToVerify = await _userManager.FindByNameAsync(userName);
            //  await _userManager.CheckPasswordAsync(userToVerify, password)
            // _jwtFactory.GenerateClaimsIdentity(userName, userToVerify.Id)
            /*
             *
             *      string jwt = await Tokens.GenerateJwt(
                     identity,
                     _jwtFactory,
                     loginViewModel.Username,
                     _jwtOptions,
                     new JsonSerializerSettings { Formatting = Formatting.Indented });
             */
            ApplicationUser applicationUser = userManager.FindByNameAsync(login.Username).GetAwaiter().GetResult();
            bool result = userManager.CheckPasswordAsync(applicationUser, login.Password).GetAwaiter().GetResult();


            return applicationUser;
        }        
    }
}