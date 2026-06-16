namespace PaymentDetailApi.API.Controllers.Users
{
    public class AddUserRequest
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string? DisplayName { get; set; }
    }
}
