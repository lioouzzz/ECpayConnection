using Ecpay.Models;
using Ecpay.Services;
using Microsoft.AspNetCore.Mvc;

namespace Ecpay.Controllers.Api
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : Controller
    {
        private readonly OrderService _orderservice;

        public OrderController(OrderService orderService)
        {
            _orderservice = orderService;
        }

        [HttpPost("create")]
        public IActionResult CreateOrder([FromBody] CreateOrderInputModel input)
        {
            var order = new OrderModel
            {
                OrderNo = Guid.NewGuid().ToString(),
                TotalAmount = input.TotalAmount,
                TradeDesc = input.TradeDesc,
                ItemName = input.ItemName,
                Status = "Pending",
                MerchantTradeNo = Guid.NewGuid().ToString("N").Substring(0, 20),
                TradeNo = null,
                PaymentType = "aio",
                PaymentDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")
            };
            int newOrderId = _orderservice.CreateOrder(order);

            return Json(new
            {
                success = true,
                orderId = newOrderId,
                redirectUrl = Url.Action("OrderId", "Order", new { id = newOrderId })
            });

        }

        [HttpGet("Detail/{id}")]
        public IActionResult GetOrder(int id)
        {
            var order = _orderservice.GetOrderId(id);
            if (order == null) return NotFound();

            return Json(new { order.Id, order.OrderNo, order.TotalAmount, order.Status, order.ItemName });
        }

    }
}