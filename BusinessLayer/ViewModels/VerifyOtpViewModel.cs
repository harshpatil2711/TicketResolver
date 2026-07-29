using System.ComponentModel.DataAnnotations;

namespace TicketResolver.ViewModels
{
    public class VerifyOtpViewModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string OtpCode { get; set; }

        public string Purpose { get; set; }
    }
}
