using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NETCoreWebAPI.Data.Helper;

namespace NETCoreWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = UserRoles.Student)]
    //[Authorize(Roles = UserRoles.Manager + "," + UserRoles.Student)]
    public class StudentController : ControllerBase
    {
        public StudentController()
        {
            
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Welcome to StrudentController");
        }
    }
}
