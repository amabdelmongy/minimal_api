namespace WebApi.Services;

public class PaymentsPost
{
    private readonly ILogger _logger;

    public PaymentsPost(ILogger logger)
    {
        _logger = logger;
    }

    public async Task<IResult> Post(
        HttpRequest request,
        string paymentMethodIdentifier,
        string? authorization,
        string? ckoMerchantData,
        string? ckoPaymentSignature)
    {
        //Read Body with Serialization as SnakeCase
        CreatePaymentRequest createPaymentRequest =
            await request.ReadFromJsonAsync<CreatePaymentRequest>(options: SerializationExtensions.Options) ??
            throw new InvalidOperationException();

        if (createPaymentRequest is null)
            throw new ArgumentNullException(nameof(createPaymentRequest));

        if (string.IsNullOrWhiteSpace(authorization) ||
            string.IsNullOrWhiteSpace(ckoMerchantData) ||
            string.IsNullOrWhiteSpace(ckoPaymentSignature))
            return Results.Json(
                statusCode: StatusCodes.Status401Unauthorized,
                data: null,
                options: SerializationExtensions.Options);

        return Results.Json(
            statusCode: StatusCodes.Status201Created,
            data: GetAcceptedResponse(),
            options: SerializationExtensions.Options);

    CreatePaymentResponse GetAcceptedResponse()
        {
            _logger.LogInformation("Payment was successfully created. PaymentId: {@ExternalPaymentId}",
                createPaymentRequest.PaymentId);

            return new CreatePaymentResponse() {CreateDate = DateTime.Now,PaymentId = createPaymentRequest.PaymentId,Status = PaymentStatus.Created};
        }
    }
}
