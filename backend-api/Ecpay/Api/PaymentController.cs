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
        public IActionResult Checkout([FromRoute] int orderId)
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


        // 給綠界伺服器呼叫
        [HttpPost("Return")]
        public IActionResult Return([FromForm] EcpayCallbackModel callback)
        {
            var checkMac = _creditCardService.CheckMacValueCb(Request.Form);
            var merchantTradeNo = callback.MerchantTradeNo;
            var rtnCode = callback.RtnCode;

            var order = _orderService.GetMerchantTradeNo(callback.MerchantTradeNo);

            if (!checkMac)
            {
                return Content("checkMac error");
            }


            if (order == null)
            {
                return Content("order not found");
            }

            if (!_creditCardService.CheckMacValueCb(Request.Form))
            {
                return Content("??");
            }


            if (callback.RtnCode == "1")
            {
                _orderService.UpdatePaid(order.Id);
            }

            return Content("1|OK");

        }

        [HttpPost("Result")]

        //綠界瀏覽器導向這支

        public IActionResult Result([FromForm] EcpayCallbackModel callback)
        {
            var merchantTradeNo = Uri.EscapeDataString(callback.MerchantTradeNo ?? string.Empty);
            var rtnCode = Uri.EscapeDataString(callback.RtnCode ?? string.Empty);
            var rtnMsg = Uri.EscapeDataString(callback.RtnMsg ?? string.Empty);


            return Redirect($"http://localhost:5173/Payment/Result?merchantTradeNo={merchantTradeNo}&rtnCode={rtnCode}&rtnMsg={rtnMsg}");
        }


        //查詢資料庫OrderStatus,可以確保綠界是否付款成功

        [HttpGet("OrderStatus")]
        public IActionResult GetOrderStatus([FromQuery] string merchantTradeNo)
        {
            var order = _orderService.GetMerchantTradeNo(merchantTradeNo);

            if (order == null)
            {
                return NotFound(new { message = "找不到訂單" });
            }

            return Json(new
            {
                MerchantTradeNo = order.MerchantTradeNo,
                Status = order.Status
            });

        }

    }
}