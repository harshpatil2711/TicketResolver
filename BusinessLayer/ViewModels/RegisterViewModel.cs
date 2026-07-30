using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using TicketResolver.Models;

namespace TicketResolver.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MaxLength(100)]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "First name can only contain letters and spaces.")]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Last name can only contain letters and spaces.")]
        public string LastName { get; set; }

        [Required]
        public string Mobile { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        [Required]
        public int RoleId { get; set; }

        public List<TicketRole> Roles { get; set; }
    }
}
