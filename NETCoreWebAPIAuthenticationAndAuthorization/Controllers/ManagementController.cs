using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NETCoreWebAPI.Data.Helper;

namespace NETCoreWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = UserRoles.Manager)]
    public class ManagementController : ControllerBase
    {

        public ManagementController()
        {
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Welcome to ManagementController");
        }
    }
}
