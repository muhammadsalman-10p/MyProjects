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

        private readonly IServiceWrapper _serviceWrapper;

        private readonly ILoggerManager _logger;
        public UserController(IServiceWrapper serviceWrapper, ILoggerManager logger)
        {
            _serviceWrapper = serviceWrapper;
            _logger = logger;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate([FromBody] AuthenticateModel model)
        {

            if (model == null)
            {
                _logger.LogError("AuthenticateModel object sent from client is null.");
                return BadRequest("object is null");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogError("for Authenticate, Invalid object sent from client.");
                return BadRequest("Invalid model object");
            }

            var user = await _serviceWrapper.UserService.Authenticate(model.Username, model.Password);
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

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (model == null)
            {
                _logger.LogError("RegisterModel object sent from client is null.");
                return BadRequest("object is null");
            }

            if (!ModelState.IsValid)
            {
                _logger.LogError("for Authenticate, Invalid object sent from client.");
                return BadRequest("Invalid model object");
            }

            await _serviceWrapper.UserService.Create(model, model.Password);
            return Ok();
        }
    }
}
