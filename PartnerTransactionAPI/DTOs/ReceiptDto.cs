using System.ComponentModel.DataAnnotations;

namespace PartnerTransactionAPI.DTOs
{
    public class ReceiptDto
    {
        [Required]
        public int Result { get; set; }

        public long? TotalAmount { get; set; }

        public long? TotalDiscount { get; set; }

        public long? FinalAmount { get; set; }

        public string? ResultMessage { get; set; }
    }
}
