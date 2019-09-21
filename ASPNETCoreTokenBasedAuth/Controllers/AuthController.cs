using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ASPNETCoreTokenBasedAuth.Helpers;
using ASPNETCoreTokenBasedAuth.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ASPNETCoreTokenBasedAuth.Controllers
{
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly ApplicationModelContext db;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private AppSettings AppSettings { get; set; }
        public AuthController(ApplicationModelContext applicationModelContext,
                                UserManager<ApplicationUser> userManager,
                                SignInManager<ApplicationUser> signInManager,
                                IOptions<AppSettings> settings)
        {
            db = applicationModelContext;
            _userManager = userManager;
            _signInManager = signInManager;
            AppSettings = settings.Value;
        }
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginVM loginVM)
        {
            var user = await _userManager.FindByNameAsync(loginVM.UserName);
            if (user!=null && await _userManager.CheckPasswordAsync(user,loginVM.Password))
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(AppSettings.Secret);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                    new Claim(ClaimTypes.Name, user.Id.ToString())
                    }),
                    Expires = DateTime.UtcNow.AddDays(7),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                string Token = tokenHandler.WriteToken(token);              

                return Ok(new  { Token , token.ValidTo });
            }
            return Unauthorized();
        }

        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterVM model)
        {
            var ErrorMessage = string.Empty;
            if (!ModelState.IsValid)
            {
               ErrorMessage = string.Join(" | ", ModelState.Values
                                          .SelectMany(v => v.Errors)
                                          .Select(e => e.ErrorMessage));



                return BadRequest(ErrorMessage);
            }
            ApplicationUser user = new ApplicationUser()
            {
                Email = model.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                FullName = model.FullName,
                UserName = model.UserName
            };
            var result = await _userManager.CreateAsync(user,model.Password);
            if (result.Succeeded)
            {
                return Ok("User Create Succefully.");
            }else
            {
                ErrorMessage = string.Join(" | ", result.Errors.Select(v => v.Description));
                return BadRequest(ErrorMessage);
            }
            
            


           

            
           
        }
    }
}