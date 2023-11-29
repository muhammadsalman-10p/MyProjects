using AssignmentProject.Application.Interfaces;
using AssignmentProject.Application.Models;
using AssignmentProject.Core.Entities;
using AssignmentProject.Helpers;
using LoggingService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentProject.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        
        private readonly IUserService _userService;
        private readonly ILoggerManager _logger;
        public UserController(IUserService userService, ILoggerManager logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Authenticate([FromBody] AuthenticateModel model)
        {
            try
            {
                var user = await _userService.Authenticate(model.Username, model.Password);
                _logger.LogInfo($"authenticating user: {model.Username}");
                if (user == null)
                {
                    _logger.LogError($"authenticating failed for user:  {model.Username}");
                    return BadRequest(new { message = "Username or password is incorrect" });

                }

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(AppSettings.SecretKey);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, user.Id.ToString()),
                    }),
                    Expires = DateTime.UtcNow.AddMinutes(AppSettings.TokenExpireTimeMinutes),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                    Audience = AppSettings.Issuer,
                    Issuer = AppSettings.Issuer,
                    
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                // return basic user info and authentication token
                return Ok(new
                {
                    Id = user.Id,
                    Username = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Token = tokenString
                });
            }
            catch (Exception ex)
            {
                _logger.LogError($"authenticating failed for user:  {model.Username} with detail: {ex.ToString()}");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error occured while validating user, please try again." } );
            }
        }

        [AllowAnonymous]
        [HttpPost("register")]
        [ValidateAntiForgeryToken]
        public IActionResult Register([FromBody] RegisterModel model)
        {
            // map model to entity
            //var user = _mapper.Map<User>(model);

            try
            {
                // create user
                _userService.Create(model, model.Password);
                return Ok();
            }
            catch (Exception ex)
            {
                // return error message if there was an exception
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
