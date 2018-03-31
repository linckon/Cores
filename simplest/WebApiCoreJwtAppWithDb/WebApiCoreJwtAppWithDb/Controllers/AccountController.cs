using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiCoreJwtAppWithDb.Models;

namespace WebApiCoreJwtAppWithDb.Controllers
{
    public class RegisterBindingModel
    {
        public string Phone { get; set; }
        public string Email { get;  set; }
        public string Name { get; set; }
        public string Password { get; set; }
    }


    [Produces("application/json")]
    [Route("api/Account")]
    public class AccountController : Controller
    {
        private UserManager<ApplicationUser> userManager;

        public AccountController(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        [AllowAnonymous]
        [Route("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var isPhoneDuplicated = await this.userManager.Users.AnyAsync(x => x.PhoneNumber == model.Phone);
            if (isPhoneDuplicated)
            {
                ModelState.AddModelError("model.Phone", "Duplicate Phone number");
                return BadRequest(ModelState);
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                Name = model.Name,
                PhoneNumber = model.Phone
            };

            IdentityResult result = await userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                return this.BadRequest();
            }

            return Ok();
        }
    }
}