namespace PaymentDetailApi.Application.Common.Interfaces
{
    public interface IJwtTokenService
    {
        string GenerateToken(Guid id, string userName, string email, string role);
    }
}
