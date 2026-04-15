using Ecpay.Models;
using Ecpay.Services;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Ecpay.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]

    public class PaymentController : Controller
    {
        private readonly CreditCardService _creditCardService;
        private readonly OrderService _orderService;
        private readonly EcpaySettingModel _ecpaySetting;


        public PaymentController(CreditCardService creditCardService, OrderService orderService, EcpaySettingModel ecpaySetting)
        {
            _creditCardService = creditCardService;
            _orderService = orderService;
            _ecpaySetting = ecpaySetting;
        }

        [HttpPost("checkout/{orderId}")]
        public IActionResult Checkout(int orderId)
        {
            var order = _orderService.GetOrderId(orderId);

            if (order == null)
            {
                return NotFound(new { message = "找不到訂單" });
            }
            if (order.Status == "Paid")
            {
                return BadRequest("此訂單已付款");
            }

            var model = _creditCardService.CreateCardOrder(order);

            return Ok(new
            {
                actionUrl = _ecpaySetting.CheckoutUrl,
                fields = new Dictionary<string, string>
                {
                { "MerchantID", model.MerchantID },
                { "MerchantTradeNo", model.MerchantTradeNo },
                { "MerchantTradeDate", model.MerchantTradeDate },
                { "PaymentType", model.PaymentType },
                { "TotalAmount", model.TotalAmount.ToString() },
                { "TradeDesc", model.TradeDesc },
                { "ItemName", model.ItemName },
                { "ReturnURL", model.ReturnURL },
                { "OrderResultURL", model.OrderResultURL },
                { "ClientBackURL", model.ClientBackURL },
                { "ChoosePayment", model.ChoosePayment },
                { "CheckMacValue", model.CheckMacValue },
                { "EncryptType", model.EncryptType },
                { "NeedExtraPaidInfo", model.NeedExtraPaidInfo }

                }
            });
        }

        [HttpPost("return")]
        public IActionResult Return([FromForm] EcpayCallbackModel callback)
        {
            if (!_creditCardService.CheckMacValueCb(Request.Form))
            {
                return Content("0|Error");
            }

            var order = _orderService.GetMerchantTradeNo(callback.MerchantTradeNo);
            if (order == null)
            {
                return Content("0|Error");
            }

            if (callback.RtnCode == "1")
            {
                _orderService.UpdatePaid(order.Id);
            }

            return Content("1|OK");

        }

        [HttpPost("Result")]

        public IActionResult Result([FromForm] EcpayCallbackModel callback)
        {
            var orderNo = Uri.EscapeDataString(callback.MerchantTradeNo ?? string.Empty);
            var rtnCode = Uri.EscapeDataString(callback.RtnCode ?? string.Empty);
            var rtnMsg = Uri.EscapeDataString(callback.RtnMsg ?? string.Empty);


            return Redirect($"/Payment/Result?orderNo={orderNo}&rtnCode={rtnCode}&rtnMsg={rtnMsg}");
        }

    }
}