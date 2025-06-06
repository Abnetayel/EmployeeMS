﻿using System.ComponentModel.DataAnnotations;

namespace WebApplication3.Models
{
    public class ResetPasswordViewModel
    {
        public string? Token { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string? NewPassword { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Passwords do not match.")]
        public string? ConfirmPassword { get; set; }
    }
}
