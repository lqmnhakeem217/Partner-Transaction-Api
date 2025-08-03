using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace PartnerTransactionAPI.DTOs
{
    public class ItemDetailDTO
    {
        [Required(ErrorMessage = "partneritemref is required.")]
        [StringLength(50)]
        [JsonPropertyName("partneritemref")]
        public string PartnerItemRef { get; set; } = string.Empty;

        [Required(ErrorMessage = "name is required.")]
        [StringLength(100)]
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "qty is required.")]
        [Range(1, 5, ErrorMessage = "Quantity must be between 1 and 5.")]
        [JsonPropertyName("qty")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "unitprice is required.")]
        [Range(1, long.MaxValue, ErrorMessage = "Unit price must be a positive value")]
        [JsonPropertyName("unitprice")]
        public long UnitPrice { get; set; }
    }
}
