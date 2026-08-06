using System.Text.Json.Serialization;

namespace PDEWebAPIS.ViewModel
{
    public class pincodeApiRes
    {
        [JsonPropertyName("Message")]
        public string? Message { get; set; }

        [JsonPropertyName("Status")]
        public string? Status { get; set; }

        [JsonPropertyName("PostOffice")]
        public List<PostOffice>? PostOffice { get; set; }

        public static implicit operator pincodeApiRes(List<pincodeApiRes> v)
        {
            throw new NotImplementedException();
        }

        public static implicit operator pincodeApiRes?(List<PostOffice>? v)
        {
            throw new NotImplementedException();
        }
    }

    public class PostOffice
    {
        [JsonPropertyName("Name")]
        public string? Name { get; set; }
        [JsonPropertyName("Description")]
        public string? Description { get; set; }
        [JsonPropertyName("BranchType")]
        public string? BranchType { get; set; }
        [JsonPropertyName("DeliveryStatus")]
        public string? DeliveryStatus { get; set; }
        [JsonPropertyName("Circle")]
        public string? Circle { get; set; }
        [JsonPropertyName("District")]
        public string? District { get; set; }
        [JsonPropertyName("Division")]
        public string? Division { get; set; }
        [JsonPropertyName("Region")]
        public string? Region { get; set; }
        [JsonPropertyName("Block")]
        public string? Block { get; set; }
        [JsonPropertyName("State")]
        public string? State { get; set; }
        [JsonPropertyName("Country")]
        public string? Country { get; set; }
        [JsonPropertyName("Pincode")]
        public string? Pincode { get; set; }
    }
}
