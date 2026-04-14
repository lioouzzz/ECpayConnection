namespace Ecpay.Models
{
    public class EcpaySettingModel
    {
        public string CheckoutUrl { get; set; }
        public string MerchantID { get; set; }
        public string HashKey { get; set; }
        public string HashIV { get; set; }
        public string ReturnURL { get; set; }
        public string OrderResultURL { get; set; }
        public string ClientBackURL { get; set; }
    }
}