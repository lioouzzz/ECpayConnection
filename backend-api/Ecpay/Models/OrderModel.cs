namespace Ecpay.Models
{
    public class OrderModel
    {
        public int Id { get; set; }
        public string OrderNo { get; set; } = string.Empty;
        public int TotalAmount { get; set; }
        public string TradeDesc { get; set; } = string.Empty;
        public string ItemName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? TradeNo { get; set; }
        public string? MerchantTradeNo { get; set; }
        public string? PaymentType { get; set; }
        public string? PaymentDate { get; set; }
    }
}
