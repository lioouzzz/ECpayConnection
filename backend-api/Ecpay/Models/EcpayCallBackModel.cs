
namespace Ecpay.Models
{
    public class EcpayCallbackModel
    {
        public string MerchantTradeNo { get; set; } = string.Empty;
        public string RtnCode { get; set; } = string.Empty;
        public string RtnMsg { get; set; } = string.Empty;
        public string TradeNo { get; set; } = string.Empty;
        public string PaymentDate { get; set; } = string.Empty;
        public string PaymentType { get; set; } = string.Empty;
        public string SimulatePaid { get; set; } = string.Empty;
        public string CheckMacValue { get; set; } = string.Empty;

    }
}