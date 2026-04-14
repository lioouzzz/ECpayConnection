namespace Ecpay.Models
{
    public class CreditCardModel
    {
        public string MerchantID { get; set; } = string.Empty;
        public string MerchantTradeNo { get; set; } = string.Empty;
        public string MerchantTradeDate { get; set; } = string.Empty;
        public string PaymentType { get { return "aio"; } }

        public int TotalAmount { get; set; }
        public string TradeDesc { get; set; } = string.Empty;
        public string ItemName { get; set; } = string.Empty;

        public string ReturnURL { get; set; } = string.Empty;

        public string OrderResultURL { get; set; } = string.Empty;
        public string ClientBackURL { get; set; } = string.Empty;
        public string ChoosePayment { get { return "Credit"; } }
        public string CheckMacValue { get; set; } = string.Empty;

        public string EncryptType { get { return "1"; } }

        public string NeedExtraPaidInfo { get { return "Y"; } }


        public Dictionary<string, string> ToDictionary()
        {
            return new Dictionary<string, string>
            {
                { "MerchantID", MerchantID },
                { "MerchantTradeNo", MerchantTradeNo },
                { "MerchantTradeDate", MerchantTradeDate },
                { "PaymentType", PaymentType },
                { "TotalAmount", TotalAmount.ToString() },
                { "TradeDesc", TradeDesc },
                { "ItemName", ItemName },
                { "ReturnURL", ReturnURL },
                { "OrderResultURL", OrderResultURL },
                { "ClientBackURL", ClientBackURL },
                { "ChoosePayment", ChoosePayment },
                { "EncryptType", EncryptType },
                { "NeedExtraPaidInfo", NeedExtraPaidInfo }
            };
        }

    }
}