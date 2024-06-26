﻿using System.ComponentModel.DataAnnotations;

namespace NETCoreWebAPI.Data.ViewModel
{
    public class TokenRequestVM
    {
        [Required]
        public string Token { get; set; }
        
        [Required]
        public string RefreshToken { get; set; }

    }
}
