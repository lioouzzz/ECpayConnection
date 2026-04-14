using Ecpay.Models;
using System.Data;
using System.Data.SqlClient;

namespace Ecpay.Services
{
    public class OrderService
    {
        private readonly string _connStr;

        public OrderService(string connStr)
        {
            _connStr = connStr;
        }
        public int CreateOrder(OrderModel orderModel)
        {
            using (SqlConnection sqlCon = new SqlConnection(_connStr))
            {
                sqlCon.Open();
                string createSql =
                """
                INSERT INTO EcpayOrders
                (
                    OrderNo,
                    TotalAmount,
                    TradeDesc,
                    ItemName,
                    Status,
                    MerchantTradeNo,
                    TradeNo,
                    PaymentType,
                    PaymentDate                   
                )
                VALUES
                (
                    @OrderNo,
                    @TotalAmount,
                    @TradeDesc,
                    @ItemName,
                    @Status,
                    @MerchantTradeNo,
                    @TradeNo,
                    @PaymentType,
                    @PaymentDate                                
                )
                    SELECT CAST(SCOPE_IDENTITY() AS INT);
                """;

                using (SqlCommand sqlCmd = new SqlCommand(createSql, sqlCon))
                {
                    sqlCmd.Parameters.AddWithValue("@OrderNo", Guid.NewGuid().ToString());
                    sqlCmd.Parameters.AddWithValue("@TotalAmount", orderModel.TotalAmount);
                    sqlCmd.Parameters.AddWithValue("@TradeDesc", orderModel.TradeDesc);
                    sqlCmd.Parameters.AddWithValue("@ItemName", orderModel.ItemName);
                    sqlCmd.Parameters.AddWithValue("@Status", "Pending");
                    sqlCmd.Parameters.AddWithValue("@MerchantTradeNo", Guid.NewGuid().ToString("N").Substring(0, 20));
                    sqlCmd.Parameters.AddWithValue("@TradeNo", orderModel.TradeNo ?? "");
                    sqlCmd.Parameters.AddWithValue("@PaymentType", "aio");
                    sqlCmd.Parameters.AddWithValue("@PaymentDate", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));

                    return (int)sqlCmd.ExecuteScalar();
                }
            }
        }

        public OrderModel? GetOrderId(int orderId)
        {
            using (SqlConnection sqlCon = new SqlConnection(_connStr))
            {
                sqlCon.Open();

                string queryOrderSql =
                    """
                      SELECT 
                            Id,
                            OrderNo,
                            TotalAmount,
                            TradeDesc,
                            ItemName,
                            Status,
                            MerchantTradeNo,
                            TradeNo,
                            PaymentType,
                            PaymentDate
                    FROM EcpayOrders
                    WHERE Id = @Id; 
                    """;
                using (SqlCommand sqlCmd = new SqlCommand(queryOrderSql, sqlCon))
                {
                    sqlCmd.Parameters.AddWithValue("@Id", orderId);

                    using (SqlDataReader reader = sqlCmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new OrderModel
                            {
                                Id = reader.GetInt32(0),
                                OrderNo = reader.GetString(1),
                                TotalAmount = reader.GetInt32(2),
                                TradeDesc = reader.GetString(3) ?? "",
                                ItemName = reader.GetString(4),
                                Status = reader.GetString(5),
                                MerchantTradeNo = reader.GetString(6),
                                TradeNo = reader.GetString(7) ?? "",
                                PaymentType = reader.GetString(8),
                                PaymentDate = reader.GetString(9)
                            };
                        }
                        else
                        {
                            return null;
                        }
                    }
                }


            }
        }

        public OrderModel? GetMerchantTradeNo(string merchantTradeNo)
        {
            using (SqlConnection sqlCon = new SqlConnection(_connStr))
            {
                sqlCon.Open();

                string querySql =
                   """
                      SELECT 
                            Id,
                            OrderNo,
                            TotalAmount,
                            TradeDesc,
                            ItemName,
                            Status,
                            MerchantTradeNo,
                            TradeNo,
                            PaymentType,
                            PaymentDate
                    FROM EcpayOrders
                    WHERE MerchantTradeNo = @MerchantTradeNo; 
                    """;

                using (SqlCommand sqlCmd = new SqlCommand(querySql, sqlCon))
                {
                    sqlCmd.Parameters.AddWithValue("@MerchantTradeNo", merchantTradeNo);
                    using var reader = sqlCmd.ExecuteReader();


                    if (!reader.Read())
                        return null;

                    return new OrderModel
                    {
                        Id = reader.GetInt32(0),
                        OrderNo = reader.GetString(1),
                        TotalAmount = reader.GetInt32(2),
                        TradeDesc = reader.GetString(3) ?? "",
                        ItemName = reader.GetString(4),
                        Status = reader.GetString(5),
                        MerchantTradeNo = reader.GetString(6),
                        TradeNo = reader.GetString(7) ?? "",
                        PaymentType = reader.GetString(8),
                        PaymentDate = reader.GetString(9)
                    };
                }
            }
        }

        public void UpdatePaid(int orderId)
        {
            using (SqlConnection sqlCon = new SqlConnection(_connStr))
            {
                sqlCon.Open();

                string updateSql =
                """
                    UPDATE EcpayOrders
                    SET Status =@Status
                    WHERE Id=@Id
                """;
                using (SqlCommand sqlCmd = new SqlCommand(updateSql, sqlCon))
                {
                    sqlCmd.Parameters.AddWithValue("@Status", "Paid");
                    sqlCmd.Parameters.AddWithValue("@Id", orderId);
                    sqlCmd.ExecuteNonQuery();
                }

            }
        }
    }
}