namespace PartnerTransactionAPI.DTOs
{
    public class DiscountItemDto
    {
        public long OriginalAmount { get; set; }
        public long DiscountAmount { get; set; }
        public long FinalAmount { get; set; }
        public decimal DiscountPercentage { get; set; }
    }
}
