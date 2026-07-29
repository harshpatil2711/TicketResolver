using System.ComponentModel.DataAnnotations;

namespace TicketResolver.ViewModels
{
    public class ProfileIndexViewModel
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Mobile { get; set; }
        public string RoleName { get; set; }
        public System.DateTime CreatedDate { get; set; }
    }

    public class ProfileEditViewModel
    {
        public int UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }

        [Required]
        [MaxLength(200)]
        [EmailAddress]
        public string Email { get; set; }

        [MaxLength(20)]
        public string Mobile { get; set; }
    }
}
