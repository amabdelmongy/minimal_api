namespace WebApi.Models;

public record CreatePaymentRequest
{
    public string PaymentId { get; init; } = string.Empty;

    public string? PaymentData { get; init; }
}
