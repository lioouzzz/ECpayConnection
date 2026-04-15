import { useState } from "react";

//建立使用者所需要的欄位
//更新資料useState

//useState Message

//呼叫後端api /order/create
//呼叫成功後 轉導orderDetail詳細頁面

function CreateOrder() {
  const [data, setData] = useState({
    totalAmount: "",
    tradeDesc: "",
    itemName: "",
  });

  const [message, setMessage] = useState("");

  //更新欄位資料
  const inputHandleChange = (e) => {
    const inputName = e.target.name;
    const inputValue = e.target.value;
    //或是直接簡寫成 const {inputName,inputValue}=e.tartget

    setData((prevData) => ({
      ...prevData,
      [inputName]: inputValue,
    }));
  };

  //送出建立訂單

  const handleSubmit = async (e) => {
    e.preventDefault();

    if (!data.totalAmount || Number(data.totalAmount <= 0)) {
      setMessage("請輸入正確金額");
    }
    if (!data.itemName.trim()) {
      setMessage("請輸入商品名稱");
    }

    if (!data.tradeDesc.trim()) {
      setMessage("請輸入交易描述");
    }

    try {
      setMessage("訂單建立中");
      const response = await fetch(`http://localhost:5170/api/order/create`, {
        method: "POST",
        body: JSON.stringify({
          totalAmount: data.totalAmount,
          tradeDesc: data.tradeDesc,
          itemName: data.itemName,
        }),
      });

      if (!response.ok) {
        const txt = await response.text();
        throw new Error(txt || "建立訂單失敗");
      }
      const data = await response.json();

      //建立成功之後跳轉訂單詳細頁面
      window.location.href = `/Order/Detail/${data.Id}`;
    } catch (error) {
      setMessage(error.message || "建立訂單失敗");
    }
  };

  return (
    <div className="container md-5">
      <div className="card shadow-sm">
        <div className="card-header">
          <h3 className="mb-0">建立訂單</h3>
        </div>

        <div className="card-body">
          {message && <div className="alert alert-info">{message}</div>}

          <form onSubmit={handleSubmit}>
            <div className="mb-3">
              <label className="form-label">商品名稱</label>
              <input
                type="text"
                className="form-control"
                name="itemName"
                value={data.itemName}
                onChange={inputHandleChange}
                placeholder="請輸入商品名稱"
              />
            </div>

            <div className="mb-3">
              <label className="form-label">金額</label>
              <input
                type="text"
                className="form-control"
                name="totalAmount"
                value={data.totalAmount}
                onChange={inputHandleChange}
                placeholder="請輸入金額"
              />
            </div>

            <div className="mb-3">
              <label className="form-label">交易描述</label>
              <input
                type="text"
                className="form-control"
                name="tradeDesc"
                value={data.tradeDesc}
                onChange={inputHandleChange}
                placeholder="請輸入交易描述"
              />
            </div>

            <button type="submit" btn btn-primary>
              送出訂單
            </button>
          </form>
        </div>
      </div>
    </div>
  );
}

export default CreateOrder;
