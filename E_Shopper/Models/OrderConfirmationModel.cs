using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using E_Shopper.ViewModels;
using Oracle.ManagedDataAccess.Client;


namespace E_Shopper.Models
{
    public class OrderConfirmationModel
    {
        private readonly ConnectionModel connectionModel;

        public OrderConfirmationModel()
        {
            connectionModel = new ConnectionModel();
        }
        public OrderConfirmationViewModel GetOrderDetails(int orderId)
        {
            OrderConfirmationViewModel orderDetails = new OrderConfirmationViewModel();
            string connectionString = connectionModel.DBTEST3con();

            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();

                    // 获取订单的基本信息
                    string orderSql = @"
                SELECT 
                    VCHRORDERID AS OrderId,
                    VCHRTOTALAMOUNT AS TotalAmount,
                    VCHRPAYMENTMETHOD AS PaymentMethod,
                    VCHRSHIPPINGADDRESS AS ShippingAddress
                FROM 
                    ORDERS 
                WHERE 
                    VCHRORDERID = :orderId";

                    using (OracleCommand orderCommand = new OracleCommand(orderSql, connection))
                    {
                        orderCommand.Parameters.Add(new OracleParameter("orderId", orderId));

                        using (OracleDataReader orderReader = orderCommand.ExecuteReader())
                        {
                            if (orderReader.Read())
                            {
                                orderDetails.OrderId = orderReader.GetInt32(orderReader.GetOrdinal("OrderId"));
                                orderDetails.TotalAmount = orderReader.GetString(orderReader.GetOrdinal("TotalAmount"));
                                orderDetails.PaymentMethod = orderReader.GetString(orderReader.GetOrdinal("PaymentMethod"));
                                orderDetails.ShippingAddress = orderReader.GetString(orderReader.GetOrdinal("ShippingAddress"));
                            }
                        }
                    }

                    string orderDetailsSql = @"
                SELECT 
                    OD.VCHRPRODUCTID AS ProductId,
                    OD.VCHRQUANTITY AS Quantity,
                    OD.VCHRPRICE AS Price,
                    P.VCHRNAME AS ProductName,
                    P.VCHRPRICE AS ProductPrice,
                    P.VCHRMAINIMAGEURL AS ProductImageURL
                FROM 
                    ORDER_DETAILS OD
                JOIN 
                    PRODUCTS P ON OD.VCHRPRODUCTID = P.VCHRPRODUCTID
                WHERE 
                    OD.VCHRORDERID = :orderId";

                    using (OracleCommand orderDetailsCommand = new OracleCommand(orderDetailsSql, connection))
                    {
                        orderDetailsCommand.Parameters.Add(new OracleParameter("orderId", orderId));

                        using (OracleDataReader orderDetailsReader = orderDetailsCommand.ExecuteReader())
                        {
                            orderDetails.OrderItem_OrderConfirmations = new List<OrderItem_OrderConfirmation>();

                            while (orderDetailsReader.Read())
                            {
                                OrderItem_OrderConfirmation orderItem = new OrderItem_OrderConfirmation
                                {
                                    ProductId = orderDetailsReader.GetInt32(orderDetailsReader.GetOrdinal("ProductId")),
                                    Quantity = orderDetailsReader.GetString(orderDetailsReader.GetOrdinal("Quantity")),
                                    Price = orderDetailsReader.GetString(orderDetailsReader.GetOrdinal("Price")),
                                    ProductName = orderDetailsReader.GetString(orderDetailsReader.GetOrdinal("ProductName")),
                                    ProductPrice = orderDetailsReader.GetString(orderDetailsReader.GetOrdinal("ProductPrice")),
                                    ProductImageURL = orderDetailsReader.GetString(orderDetailsReader.GetOrdinal("ProductImageURL"))
                                };
                                orderDetails.OrderItem_OrderConfirmations.Add(orderItem);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error retrieving order details: " + ex.Message);
            }

            return orderDetails;
        }
    }
}
