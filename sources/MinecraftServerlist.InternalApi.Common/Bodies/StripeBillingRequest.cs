using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MinecraftServerlist.InternalApi.Common.Bodies;

public record StripeBillingRequest
{
    [JsonPropertyName("BillingName")]
    [Required(ErrorMessage = "Please enter cardholders full name")]
    public string BillingName { get; set; }
    [JsonPropertyName("BillingEmail")]
    [Required(ErrorMessage = "Please enter a valid email address")]
    [EmailAddress(ErrorMessage = "Please enter a valid email address")]
    public string BillingEmail { get; set; }
    [JsonPropertyName("paymentMethodId")]
    public string PaymentMethod { get; set; }

    [JsonPropertyName("priceId")]
    public string Price { get; set; } = "price_1Hu1TdBkXnJ98OJrezQwPn5l";
}