using System.Security.Cryptography;
using System.Text;
using Ecpay.Models;

namespace Ecpay.Services
{
    public class CreditCardService
    {
        //DI注入(低耦合不綁死類別 好測試(可搭配interface) 也方便替換實作)
        private EcpaySettingModel _ecpaySettings;

        public CreditCardService(EcpaySettingModel ecpaySetting)
        {
            _ecpaySettings = ecpaySetting;
        }

        public CreditCardModel CreateCardOrder(OrderModel orderRequest)
        {
            var creditModel = new CreditCardModel
            {
                MerchantID = _ecpaySettings.MerchantID,
                MerchantTradeNo = orderRequest.MerchantTradeNo,
                MerchantTradeDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"),
                TotalAmount = orderRequest.TotalAmount,
                TradeDesc = orderRequest.TradeDesc,
                ItemName = orderRequest.ItemName,
                ReturnURL = _ecpaySettings.ReturnURL,
                OrderResultURL = _ecpaySettings.OrderResultURL,
                ClientBackURL = _ecpaySettings.ClientBackURL
            };

            if (creditModel.MerchantTradeNo.Length > 20)
            {
                //只取20個字元
                creditModel.MerchantTradeNo = creditModel.MerchantTradeNo.Substring(0, 20);
            }

            creditModel.CheckMacValue = GenerateCheckMacValue(creditModel.ToDictionary());

            return creditModel;
        }


        public bool CheckMacValueCb(IFormCollection form)
        {
            var data = new Dictionary<string, string>();

            foreach (var item in form)
            {
                data[item.Key] = item.Value.ToString();
            }

            if (!data.ContainsKey("CheckMacValue"))
            {
                return false;
            }

            string checkMacValue = data["CheckMacValue"];

            string newCheckMacValue = GenerateCheckMacValue(data);
            return string.Equals(checkMacValue, newCheckMacValue, StringComparison.OrdinalIgnoreCase);

        }

        public string GenerateCheckMacValue(Dictionary<string, string> data)
        {
            var hashKey = _ecpaySettings.HashKey;
            var hashIV = _ecpaySettings.HashIV;

            var sortData = data
                            .OrderBy(x => x.Key)
                            .Select(x => $"{x.Key}={x.Value}");

            var rawString = $"HashKey={hashKey}&{string.Join("&", sortData)}&HashIV={hashIV}";

            //字串進行URL編碼
            var encoded = Uri.EscapeDataString(rawString)
                .Replace("%20", "+")
                .Replace("%21", "!")
                .Replace("%28", "(")
                .Replace("%29", ")")
                .Replace("%2A", "*")
                .Replace("%2D", "-")
                .Replace("%2E", ".")
                .Replace("%5F", "_")
                .ToLower();

            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(encoded));
            return BitConverter.ToString(bytes).Replace("-", "").ToUpper();
        }
    }
}