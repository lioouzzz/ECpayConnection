import { BrowserRouter, Routes, Route, Navigate } from "react-router-dom";
import CreateOrder from "./CreateOrderPage";
import OrderDetail from "./OrderDetailPage";
import PaymentResult from "./PaymentResultPage";
function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route path="/Order/Create" element={<CreateOrder />}></Route>
        <Route path="/Order/Detail/:id" element={<OrderDetail />}></Route>
        <Route path="Payment/Result" element={<PaymentResult />}></Route>
      </Routes>
    </BrowserRouter>
  );
}

export default App;
