using Xunit;
using PaymentDetailApi.Domain.Payment.Entities;
using PaymentDetailApi.Domain.Payment.Events;

namespace PaymentDetailApi.UnitTests.Domain;

public class PaymentDetailTests
{
    private static readonly Guid ValidUserId      = Guid.NewGuid();
    private const string ValidCardNumber    = "1234567890123456";
    private const string ValidExpiration    = "12/25";
    private const string ValidSecurityCode3 = "123";
    private const string ValidSecurityCode4 = "1234";

    private static PaymentDetail CreateValid() =>
        new(ValidUserId, ValidCardNumber, ValidExpiration, ValidSecurityCode3);

    // ── Constructor: happy path ──────────────────────────────────────────────

    [Fact]
    public void Constructor_ValidData_SetsPropertiesCorrectly()
    {
        var payment = new PaymentDetail(ValidUserId, ValidCardNumber, ValidExpiration, ValidSecurityCode3);

        Assert.Equal(ValidUserId, payment.UserId);
        Assert.Equal(ValidCardNumber, payment.CardNumber);
        Assert.Equal(ValidExpiration, payment.ExpirationDate);
        Assert.Equal(ValidSecurityCode3, payment.SecurityCode);
        Assert.True(payment.Active);
    }

    [Fact]
    public void Constructor_ValidData_RaisesPaymentCreatedEvent()
    {
        var payment = CreateValid();

        var domainEvent = Assert.Single(payment.DomainEvents);
        Assert.IsType<PaymentCreatedDomainEvent>(domainEvent);
    }

    [Theory]
    [InlineData(ValidSecurityCode3)]
    [InlineData(ValidSecurityCode4)]
    public void Constructor_ValidSecurityCode_Succeeds(string code)
    {
        var payment = new PaymentDetail(ValidUserId, ValidCardNumber, ValidExpiration, code);
        Assert.Equal(code, payment.SecurityCode);
    }

    [Theory]
    [InlineData("01/25")]
    [InlineData("12/99")]
    [InlineData("06/30")]
    public void Constructor_ValidExpirationDate_Succeeds(string date)
    {
        var payment = new PaymentDetail(ValidUserId, ValidCardNumber, date, ValidSecurityCode3);
        Assert.Equal(date, payment.ExpirationDate);
    }

    // ── Constructor: UserId validation ───────────────────────────────────────

    [Fact]
    public void Constructor_EmptyUserId_Throws()
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            new PaymentDetail(Guid.Empty, ValidCardNumber, ValidExpiration, ValidSecurityCode3));

        Assert.Contains("UserId is required", ex.Message);
    }

    // ── Constructor: CardNumber validation ───────────────────────────────────

    [Theory]
    [InlineData("123456789012345")]    // 15 digits
    [InlineData("12345678901234567")]  // 17 digits
    [InlineData("123456789012345A")]   // non-numeric
    [InlineData("")]
    public void Constructor_InvalidCardNumber_Throws(string cardNumber)
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            new PaymentDetail(ValidUserId, cardNumber, ValidExpiration, ValidSecurityCode3));

        Assert.Contains("Card number must be exactly 16 digits", ex.Message);
    }

    // ── Constructor: ExpirationDate validation ───────────────────────────────

    [Theory]
    [InlineData("13/25")]    // month out of range
    [InlineData("00/25")]    // month 00 invalid
    [InlineData("1225")]     // missing slash
    [InlineData("12/2025")]  // 4-digit year
    [InlineData("")]
    public void Constructor_InvalidExpirationDate_Throws(string date)
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            new PaymentDetail(ValidUserId, ValidCardNumber, date, ValidSecurityCode3));

        Assert.Contains("Expiration date must be in MM/YY format", ex.Message);
    }

    // ── Constructor: SecurityCode validation ─────────────────────────────────

    [Theory]
    [InlineData("12")]      // too short
    [InlineData("12345")]   // too long
    [InlineData("12A")]     // non-numeric
    [InlineData("")]
    public void Constructor_InvalidSecurityCode_Throws(string code)
    {
        var ex = Assert.Throws<ArgumentException>(() =>
            new PaymentDetail(ValidUserId, ValidCardNumber, ValidExpiration, code));

        Assert.Contains("Security code must be 3 or 4 digits", ex.Message);
    }

    // ── Delete ───────────────────────────────────────────────────────────────

    [Fact]
    public void Delete_SetsActiveFalse()
    {
        var payment = CreateValid();
        payment.Delete();
        Assert.False(payment.Active);
    }

    [Fact]
    public void Delete_RaisesPaymentDeletedEvent()
    {
        var payment = CreateValid();
        payment.ClearEvents();
        payment.Delete();

        var domainEvent = Assert.Single(payment.DomainEvents);
        Assert.IsType<PaymentDeletedDomainEvent>(domainEvent);
    }

    // ── ClearEvents ──────────────────────────────────────────────────────────

    [Fact]
    public void ClearEvents_RemovesAllDomainEvents()
    {
        var payment = CreateValid();
        Assert.NotEmpty(payment.DomainEvents);

        payment.ClearEvents();

        Assert.Empty(payment.DomainEvents);
    }
}
