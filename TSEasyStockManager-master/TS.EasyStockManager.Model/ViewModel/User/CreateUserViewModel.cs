using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using TS.EasyStockManager.Model.ViewModel.Base;

namespace TS.EasyStockManager.Model.ViewModel.User
{
    public class CreateUserViewModel: BaseViewModel
    {
        [Required(ErrorMessage = "Email is required.")]
        [MaxLength(50)]
        [Display]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&]).{8,}$",
        ErrorMessage = "Password must include uppercase, lowercase, number, and special character.")]
        [Display]
        public string Password { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [MaxLength(30)]
        [Display]
        public string Name { get; set; }

        [Required(ErrorMessage = "Surname is required.")]
        [MaxLength(30)]
        [Display]
        public string Surname { get; set; }
    }
}
