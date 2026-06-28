using Xunit;
using Microsoft.EntityFrameworkCore;
using PaymentDetailApi.Application.PaymentDetail.Commands;
using PaymentDetailApi.Infrastructure.Persistence;

namespace PaymentDetailApi.IntegrationTests.Application.Commands;

public class CreatePaymentDetailCommandHandlerTests : IDisposable
{
    private static readonly Guid ValidUserId      = Guid.NewGuid();
    private const string ValidCardNumber   = "1234567890123456";
    private const string ValidExpiration   = "12/25";
    private const string ValidSecurityCode = "123";

    private readonly PaymentDetailsContext _context;
    private readonly CreatePaymentDetailCommandHandler _handler;

    public CreatePaymentDetailCommandHandlerTests()
    {
        var options = new DbContextOptionsBuilder<PaymentDetailsContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new PaymentDetailsContext(options);
        _handler = new CreatePaymentDetailCommandHandler(_context);
    }

    public void Dispose() => _context.Dispose();

    private static CreatePaymentDetailCommand ValidCommand() =>
        new(ValidUserId, ValidCardNumber, ValidExpiration, ValidSecurityCode);

    [Fact]
    public async Task Handle_ValidCommand_ReturnsNonEmptyId()
    {
        var id = await _handler.Handle(ValidCommand(), CancellationToken.None);

        Assert.NotEqual(Guid.Empty, id);
    }

    [Fact]
    public async Task Handle_ValidCommand_PersistsCorrectDataToDatabase()
    {
        var command = ValidCommand();

        var id = await _handler.Handle(command, CancellationToken.None);

        var saved = await _context.PaymentDetails.FindAsync(id);

        Assert.NotNull(saved);
        Assert.Equal(ValidUserId, saved.UserId);
        Assert.Equal(ValidCardNumber, saved.CardNumber);
        Assert.Equal(ValidExpiration, saved.ExpirationDate);
        Assert.Equal(ValidSecurityCode, saved.SecurityCode);
        Assert.True(saved.Active);
    }

    [Fact]
    public async Task Handle_TwoValidCommands_EachGetsDifferentId()
    {
        var firstUserId  = Guid.NewGuid();
        var secondUserId = Guid.NewGuid();

        var firstCommand  = new CreatePaymentDetailCommand(firstUserId,  "1111111111111111", "01/26", "111");
        var secondCommand = new CreatePaymentDetailCommand(secondUserId, "2222222222222222", "02/27", "222");

        var firstId  = await _handler.Handle(firstCommand,  CancellationToken.None);
        var secondId = await _handler.Handle(secondCommand, CancellationToken.None);

        Assert.NotEqual(firstId, secondId);

        var firstSaved  = await _context.PaymentDetails.FindAsync(firstId);
        var secondSaved = await _context.PaymentDetails.FindAsync(secondId);

        Assert.NotNull(firstSaved);
        Assert.NotNull(secondSaved);

        Assert.Equal(firstUserId,  firstSaved.UserId);
        Assert.Equal(secondUserId, secondSaved.UserId);
    }
}
