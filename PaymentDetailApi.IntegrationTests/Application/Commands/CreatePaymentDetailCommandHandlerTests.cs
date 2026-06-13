using Xunit;
using Microsoft.EntityFrameworkCore;
using PaymentDetailApi.Application.PaymentDetail.Commands;
using PaymentDetailApi.Infrastructure.Persistence;

namespace PaymentDetailApi.IntegrationTests.Application.Commands;

// Integration test: verifies the handler works correctly with a real EF Core context
// backed by an in-memory database (no SQL Server needed).
public class CreatePaymentDetailCommandHandlerTests : IDisposable
{
    // ── Valid test data ──────────────────────────────────────────────────────
    private const string ValidName         = "John Doe";
    private const string ValidCardNumber   = "1234567890123456";
    private const string ValidExpiration   = "12/25";
    private const string ValidSecurityCode = "123";

    // ── Infrastructure shared across all tests in this class ─────────────────
    private readonly PaymentDetailsContext _context;
    private readonly CreatePaymentDetailCommandHandler _handler;

    // This constructor runs BEFORE every single test.
    // A fresh in-memory database is created for each test so they never interfere.
    public CreatePaymentDetailCommandHandlerTests()
    {
        var options = new DbContextOptionsBuilder<PaymentDetailsContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()) // unique DB per test
            .Options;

        _context = new PaymentDetailsContext(options);
        _handler = new CreatePaymentDetailCommandHandler(_context);
    }

    // This runs AFTER every test to release the DbContext.
    public void Dispose() => _context.Dispose();

    // ── Helper: builds a valid command using the shared constants ─────────────
    private static CreatePaymentDetailCommand ValidCommand() =>
        new(ValidName, ValidCardNumber, ValidExpiration, ValidSecurityCode);

    // ── Test 1 ───────────────────────────────────────────────────────────────
    // The most basic check: did the handler run without errors and give us an ID?
    // EF Core sets Id after SaveChangesAsync, so a value > 0 confirms a DB insert happened.
    [Fact]
    public async Task Handle_ValidCommand_ReturnsPositiveId()
    {
        // Arrange: the command is already built by ValidCommand()

        // Act: send the command to the handler
        var id = await _handler.Handle(ValidCommand(), CancellationToken.None);

        // Assert: EF Core auto-increments the Id, so anything > 0 means it was saved
        Assert.True(id > 0);
    }

    // ── Test 2 ───────────────────────────────────────────────────────────────
    // Test 1 told us an ID came back. Test 2 goes further:
    // it reads the record straight from the database using that ID and checks
    // every field was stored with the exact values we sent in the command.
    [Fact]
    public async Task Handle_ValidCommand_PersistsCorrectDataToDatabase()
    {
        // Arrange
        var command = ValidCommand();

        // Act
        var id = await _handler.Handle(command, CancellationToken.None);

        // Assert: fetch the record back from the in-memory DB using the returned ID
        var saved = await _context.PaymentDetails.FindAsync(id);

        // Confirm the record actually exists (not null)
        Assert.NotNull(saved);

        // Confirm each field matches what the command carried
        Assert.Equal(ValidName, saved.CardOwnerName);
        Assert.Equal(ValidCardNumber, saved.CardNumber);
        Assert.Equal(ValidExpiration, saved.ExpirationDate);
        Assert.Equal(ValidSecurityCode, saved.SecurityCode);

        // Confirm the record is Active = true (new payments are always active)
        Assert.True(saved.Active);
    }

    // ── Test 3 ───────────────────────────────────────────────────────────────
    // Proves that inserting two separate payments gives each one a unique ID.
    // This catches bugs where the handler might return the same ID twice,
    // or where the DB accidentally overwrites the previous record.
    [Fact]
    public async Task Handle_TwoValidCommands_EachGetsDifferentId()
    {
        // Arrange: two different commands with different card owners
        var firstCommand  = new CreatePaymentDetailCommand("Alice", "1111111111111111", "01/26", "111");
        var secondCommand = new CreatePaymentDetailCommand("Bob",   "2222222222222222", "02/27", "222");

        // Act: send both commands one after the other against the same DB
        var firstId  = await _handler.Handle(firstCommand,  CancellationToken.None);
        var secondId = await _handler.Handle(secondCommand, CancellationToken.None);

        // Assert: IDs must not be the same — EF Core auto-increments so 1 and 2
        // are expected, but what matters is they are never equal
        Assert.NotEqual(firstId, secondId);

        // Extra check: both records exist independently in the database
        var firstSaved  = await _context.PaymentDetails.FindAsync(firstId);
        var secondSaved = await _context.PaymentDetails.FindAsync(secondId);

        Assert.NotNull(firstSaved);
        Assert.NotNull(secondSaved);

        // Each record holds its own owner — no data was overwritten
        Assert.Equal("Alice", firstSaved.CardOwnerName);
        Assert.Equal("Bob",   secondSaved.CardOwnerName);
    }
}
