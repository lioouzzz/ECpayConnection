import { useState, useEffect } from "react";

//先從網址拿出OrderId
//利用useEffect 呼叫後端/OrderId API
function OrderDetail() {
  const [order, setOrder] = useState(null);
  const [message, setMessage] = usestate("");
  const [loading, setLoading] = useState(true);

  const handlePay = () => {
    const path = window.location.pathname;
    const parts = path.split("/").filter(Boolean);
    const orderId = parts[parts.length - 1];
    return orderId;
  };

  useEffect(() => {
    const fetchOrder = async () => {
      const orderId = handlePay();

      if (!orderId) {
        setMessage("找不到訂單");
        return;
      }

      try {
        const response = await fetch(
          `http://localhost:5170/api/order/${orderId}`,
        );

        if (!response.ok) {
          const text = await response.text();
          throw new Error(text + "取得訂單資料失敗");
        }
        const data = await response.json();

        setOrder(data);
        setMessage("");
      } catch (error) {
        setMessage(error.message || "載入訂單失敗");
      }
    };
    fetchOrder();
  }, []);

  //抽成function、還有防呆
  const handleToPay = () => {
    if (!order?.id) {
      setMessage("找不到訂單資料");
      return;
    }

    window.location.href = `/Order/Pay/${order.id}`;
  };

  if (loading) {
    return (
      <div className="container mt-5">
        <div className="alert alert-info">載入中...</div>
      </div>
    );
  }

  if (message && !order) {
    return (
      <div className="container mt-5">
        <div className="alert alert-danger">{message}</div>
      </div>
    );
  }
  return (
    <div className="container mt-5">
      <div className="card shadow-sm">
        <div className="card-header d-flex justify-content-between align-items-center">
          <h3 className="mb-0">訂單詳細頁</h3>
          <span
            className={`badge ${order.status == "Paid" ? "bg-success" : "bg-warning text-dark"}`}
          >
            {order.status}
          </span>
        </div>

        <div className="card-body">
          {message && <div className="alert alert-info">{message}</div>}

          <div className="row mb-3">
            <div className="col-md-3 fw-bold">訂單Id</div>
            <div className="col-md-9">{order.id}</div>
          </div>

          <div className="row mb-3">
            <div className="col-md-3 fw-bold">訂單編號</div>
            <div className="col-md-9">{order.orderNo}</div>
          </div>

          <div className="row mb-3">
            <div className="col-md-3 fw-bold">訂單金額</div>
            <div className="col-md-9">{order.totalAmount}</div>
          </div>

          <div className="row mb-3">
            <div className="col-md-3 fw-bold">交易描述</div>
            <div className="col-md-9">{order.tradeDesc}</div>
          </div>

          <div className="row mb-3">
            <div className="col-md-3 fw-bold">商品名稱</div>
            <div className="col-md-9">{order.itemName}</div>
          </div>

          <div className="row mb-3">
            <div className="col-md-3 fw-bold">MerchantTradeNo</div>
            <div className="col-md-9">{order.merchantTradeNo}</div>
          </div>

          <div className="row mb-3">
            <div className="col-md-3 fw-bold">付款方式</div>
            <div className="col-md-9">{order.paymentType}</div>
          </div>

          <div className="row mb-3">
            <div className="col-md-3 fw-bold">付款時間</div>
            <div className="col-md-9">{order.paymentDate}</div>
          </div>

          <div className="d-flex gap-2">
            <button
              type="button"
              className="btn btn-primary"
              onClick={handleToPay}
              disabled={order.status === "Paid"}
            >
              {order.status === "Paid" ? "已付款" : "前往付款"}
            </button>

            <button
              type="button"
              className="btn btn-outline-secondary"
              onClick={() => {
                window.location.href = "/Order/Create";
              }}
            >
              返回建立訂單
            </button>
          </div>
        </div>
      </div>
    </div>
  );
}

export default OrderDetail;
