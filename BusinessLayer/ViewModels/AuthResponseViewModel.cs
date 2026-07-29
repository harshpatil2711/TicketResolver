namespace TicketResolver.ViewModels
{
    public class AuthResponseViewModel
    {
        public bool Success { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
        public string Message { get; set; }
    }
}
