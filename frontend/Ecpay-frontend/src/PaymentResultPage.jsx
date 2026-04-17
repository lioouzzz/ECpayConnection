import { useEffect, useState } from "react";
import { useSearchParams } from "react-router-dom";
function PaymentResult() {
  const [searchParams] = useSearchParams();
  const [loading, setLoading] = useState(true);
  const merchantTradeNo = searchParams.get("merchantTradeNo");
  const rtnCode = searchParams.get("rtnCode");
  const rtnMsg = searchParams.get("rtnMsg");

  const [orderStatus, setOrderStatus] = useState({});
  useEffect(() => {
    const fetchOrderStatus = async () => {
      try {
        const response = await fetch(
          `http://localhost:5170/api/Payment/OrderStatus?merchantTradeNo=${merchantTradeNo}`,
        );

        const fetchData = await response.json();

        setOrderStatus(fetchData);
      } catch (error) {
        setOrderStatus({ status: error });
      } finally {
        setLoading(false);
      }
    };
    fetchOrderStatus();
  }, [merchantTradeNo]);

  const renderResultText = () => {
    if (loading) return "查詢中...";
    return orderStatus?.status === "Paid" ? "付款成功" : "交易失敗";
  };

  return (
    <div className="container py-5">
      <div className="row justify-content-center">
        <div className="col-md-6">
          <div className="card shadow-sm">
            <div className="card-body p-4">
              <h3 className="text-center mb-4">
                付款結果：
                {renderResultText()}
              </h3>

              <div className="mb-3">
                <label className="form-label fw-bold">交易編號</label>
                <div className="form-control">{merchantTradeNo}</div>
              </div>

              <div className="mb-3">
                <label className="form-label fw-bold">狀態碼</label>
                <div className="form-control">{rtnCode}</div>
              </div>

              <div className="mb-3">
                <label className="form-label fw-bold">訊息</label>
                <div className="form-control">{rtnMsg}</div>
              </div>

              <div className="d-grid gap-2">
                <button
                  className="btn btn-primary"
                  onClick={() => window.location.reload()}
                >
                  重新查詢
                </button>

                <button
                  className="btn btn-outline-secondary"
                  onClick={() => (window.location.href = "/order/create")}
                >
                  返回首頁
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}

export default PaymentResult;
