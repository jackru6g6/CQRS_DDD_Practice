using System.Text.Json.Serialization;

namespace SampleProject.API.Model.Order.Request
{
    public class CreateRequest
    {
        [JsonPropertyName("amount")]
        public decimal Amount { get; set; }
    }
}