import { useState } from "react";

//從網址路徑拆出orderId的路徑
//呼叫後端付款Api,取得綠界需要的表單資料
//建立form元素 準備送到綠界
//把form加到document.body中
//送出form，導向綠界付款頁面

function PaymentPage() {
  const [message, setMessage] = useState("");

  const handlePay = async () => {
    const path = window.location.pathname;
    const parts = path.split("/").filter(Boolean);
    const orderId = parts[parts.length - 1];

    if (!orderId) {
      setMessage("找不到訂單編號");
      return;
    }

    try {
      setMessage("導向付款中");
      const response = await fetch(
        `http://localhost:5170/api/payment/checkout/${orderId}`,
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

  return (
    <div className="container mt-5">
      <button className="btn btm-primary" onClick={handlePay}>
        前往付款
      </button>

      <p className="mt-3 text-danger">{message}</p>
    </div>
  );
}

export default PaymentPage;
