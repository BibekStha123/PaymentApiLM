using PaymentDetailApi.Application.Common.Interfaces;

namespace PaymentDetailApi.Infrastructure.Notification
{
    public class EmailService : IEmailService
    {
        public async Task SendAsync()
        {
            await Task.CompletedTask;
        }
    }
}
