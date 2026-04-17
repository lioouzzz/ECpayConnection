import { useState, useEffect } from "react";
import { useParams } from "react-router-dom";
//先從網址拿出OrderId
//利用useEffect 呼叫後端/OrderId API

function OrderDetail() {
  const { id } = useParams();
  const [order, setOrder] = useState(null);
  const [message, setMessage] = useState("");
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchOrder = async () => {
      if (!id) {
        setMessage("找不到訂單");
        setLoading(false);
        return;
      }

      try {
        const response = await fetch(
          `http://localhost:5170/api/order/Detail/${id}`,
        );

        if (!response.ok) {
          const text = await response.text();
          throw new Error(text + "取得訂單資料失敗");
        }
        const data = await response.json();

        setOrder(data);
        setMessage("");
        setLoading(false);
      } catch (error) {
        setMessage(error.message || "載入訂單失敗");
      }
    };
    fetchOrder();
  }, [id]);

  //抽成function、還有防呆

  const ClickToPay = async () => {
    if (!id) {
      setMessage("找不到訂單編號");
      return;
    }

    try {
      setMessage("導向付款中");
      const response = await fetch(
        `http://localhost:5170/api/payment/checkout/${id}`,
        {
          method: "POST",
        },
      );

      if (!response.ok) {
        const errorText = response.text();
        throw new Error(errorText);
      }

      const data = await response.json();

      const form = document.createElement("form");
      form.method = "POST";
      form.action = data.actionUrl;

      Object.entries(data.fields).forEach(([key, value]) => {
        const input = document.createElement("input");
        input.name = key;
        input.value = value ?? "";
        input.type = "hidden";
        form.appendChild(input);
      });

      document.body.appendChild(form);
      form.submit();
    } catch (error) {
      setMessage(error + "付款失敗");
    }
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
        <div className="card-header position-relative d-flex align-items-center">
          <h3 className="mb-0 text-center w-100">訂單詳細頁</h3>
          <span
            className={`badge position-absolute end-0 me-3 ${order.status == "Paid" ? "bg-success" : "bg-warning text-dark"}`}
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
            <div className="col-md-3 fw-bold">交易時間</div>
            <div className="col-md-9">{order.paymentDate}</div>
          </div>

          <div className="d-flex gap-2 justify-content-center pt-3 pb-2">
            <button
              type="button"
              className="btn btn-primary"
              onClick={ClickToPay}
              disabled={order.status === "Paid"}
            >
              {order.status === "Paid" ? "已付款" : "前往付款"}
            </button>

            <button
              type="button"
              className="btn btn-outline-secondary d-flex justify-content-center"
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
