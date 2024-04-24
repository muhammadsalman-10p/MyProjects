using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NETCoreWebAPI.Data;
using NETCoreWebAPI.Data.Helper;
using NETCoreWebAPI.Data.Models;
using NETCoreWebAPI.Data.ViewModel;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Text;

namespace NETCoreWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly TokenValidationParameters _tokenValidationParameters;

        public AuthenticationController(UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            AppDbContext context,
            IConfiguration configuration,
            TokenValidationParameters tokenValidationParameters
            )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
            _configuration = configuration;
            _tokenValidationParameters = tokenValidationParameters;
        }

        [HttpPost("register-user")]
        public async Task<IActionResult> Register([FromBody] RegisterVM registerVM)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Please provide values for all required fields.");
            }

            var userExists = await _userManager.FindByEmailAsync(registerVM.EmailAdress);

            if (userExists != null)
            {
                return BadRequest($"User {registerVM.EmailAdress} already exists.");
            }

            ApplicationUser newUser = new ApplicationUser()
            {
                FirstName = registerVM.FirstName,
                LastName = registerVM.LastName,
                UserName = registerVM.UserName,
                Email = registerVM.EmailAdress,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var userCreated = await _userManager.CreateAsync(newUser, registerVM.Password);

            if (userCreated.Succeeded)
            {
                switch (registerVM.Role)
                {
                    case UserRoles.Manager:
                    case UserRoles.Student:
                       await _userManager.AddToRoleAsync(newUser, registerVM.Role);
                        break;
                    default:
                        break;
                }
                return Ok("User Created.");
            }
            return BadRequest("User could not be created.");
        }

        [HttpPost("login-user")]
        public async Task<IActionResult> Login([FromBody] LoginVM loginVM)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Please provide for all required fields.");
            }

            var userExists = await _userManager.FindByEmailAsync(loginVM.EmailAdress);

            if (userExists != null && await _userManager.CheckPasswordAsync(userExists, loginVM.Password))
            {
                var tokenValue = await GenerateJWTToken(userExists, null);
                return Ok(tokenValue);
                //return Ok($"User {loginVM.EmailAdress} login successful.");
            }

            return Unauthorized();
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] TokenRequestVM tokenRequestVM)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Please provide for all required fields.");
            }

            var result = await VerifyAndGenerateToken(tokenRequestVM);

            return Ok(result);

        }

        private async Task<AuthResultVM> VerifyAndGenerateToken(TokenRequestVM tokenRequestVM)
        {
            var JwtTokenHandler = new JwtSecurityTokenHandler();

            var storedToken = await _context.RefreshTokens.FirstOrDefaultAsync(x => x.Token == tokenRequestVM.RefreshToken);
            var dbuser = await _userManager.FindByIdAsync(storedToken.UserId);
            try
            {
                var checkTokenValidated = JwtTokenHandler.ValidateToken(tokenRequestVM.Token, _tokenValidationParameters, out var validatedToken);
                var result = await GenerateJWTToken(dbuser, storedToken);
                return result;

            }
            catch (SecurityTokenExpiredException)
            {
                if (storedToken.DateExpire >= DateTime.UtcNow) //if refresh token is still valid then generate an access token again
                {
                    var result = await GenerateJWTToken(dbuser, storedToken);
                    return result;
                }
                else //generate a new refresh token and access token
                {
                    var result = await GenerateJWTToken(dbuser, null);
                    return result;
                }
            }
            

        }

        private async Task<AuthResultVM> GenerateJWTToken(ApplicationUser user, RefreshToken rToken)
        {
            var authClaims = new List<Claim>()
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            //Add user role claims
            var userRoles  = await _userManager.GetRolesAsync(user);
            foreach (var role in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role));
            }
            var authSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JWT:Secret"]));


            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                expires: DateTime.UtcNow.AddMinutes(1),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256));

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            if (rToken != null)
            {
                var onlyAccessTokenUpdateResponse = new AuthResultVM()
                {
                    Token = jwtToken,
                    RefreshToken = rToken.Token,
                    ExpiresAt = token.ValidTo
                };

                return onlyAccessTokenUpdateResponse;
            }

            var refreshToken = new RefreshToken()
            {
                JwtId = token.Id,
                IsRevoked = false,
                UserId = user.Id,
                DateAdded = DateTime.UtcNow,
                DateExpire = DateTime.UtcNow.AddMonths(2),
                Token = Guid.NewGuid().ToString() + Guid.NewGuid().ToString()
            };

            _context.RefreshTokens.Add(refreshToken);
            _context.SaveChanges();

            var response = new AuthResultVM()
            {
                Token = jwtToken,
                RefreshToken = refreshToken.Token,
                ExpiresAt = token.ValidTo
            };

            return response;

        }
    }
}
