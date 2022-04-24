namespace WebApi.Models;

public record CreatePaymentResponse
{
    public string? PaymentId { get; init; }

    public DateTime? CreateDate { get; init; }

    public PaymentStatus? Status { get; init; }
}

public enum PaymentStatus
{
    Created,
    Cancelled
}
