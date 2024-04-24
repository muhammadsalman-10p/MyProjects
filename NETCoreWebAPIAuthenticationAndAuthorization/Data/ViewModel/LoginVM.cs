using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.ComponentModel.DataAnnotations;

namespace NETCoreWebAPI.Data.ViewModel
{
    public class LoginVM
    {
        [Required]
        public string EmailAdress { get; set; }
        [Required]
        public string Password { get; set; }

    }
}
