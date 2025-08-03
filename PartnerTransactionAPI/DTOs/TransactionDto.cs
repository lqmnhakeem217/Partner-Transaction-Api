using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PartnerTransactionAPI.DTOs
{
    public class TransactionDto
    {
        [Required(ErrorMessage = "partnerkey is required.")]
        [StringLength(50, ErrorMessage = "PartnerKey cannot exceed 50 characters.")]
        [JsonPropertyName("partnerkey")]
        public string PartnerKey { get; set; } = string.Empty;

        [Required(ErrorMessage = "partnerrefno is required.")]
        [StringLength(50, ErrorMessage = "PartnerRefNo cannot exceed 50 characters.")]
        [JsonPropertyName("partnerrefno")]
        public string PartnerRefNo { get; set; } = string.Empty;

        [Required(ErrorMessage = "partnerpassword is required.")]
        [StringLength(50, ErrorMessage = "PartnerPassword cannot exceed 50 characters.")]
        [JsonPropertyName("partnerpassword")]
        public string PartnerPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "totalamount is required.")]
        [Range(1, long.MaxValue, ErrorMessage = "Amount must be a positive value")]
        [JsonPropertyName("totalamount")]
        public long TotalAmount { get; set; }

        [JsonPropertyName("items")]
        public List<ItemDetailDTO>? Items { get; set; }

        [Required(ErrorMessage = "timestamp is required.")]
        [JsonPropertyName("timestamp")]
        public string Timestamp { get; set; } = string.Empty;

        [Required(ErrorMessage = "sig is required.")]
        [JsonPropertyName("sig")]
        public string Signature { get; set; } = string.Empty;
    }
}
