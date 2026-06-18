namespace PaymentDetailApi.API.Controllers.Users
{
    public sealed record RegisterUserRequest(
        string UserName,
        string Email,
        string Password,
        string ConfirmPassword,
        string? DisplayName);
}
