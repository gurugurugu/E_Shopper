using E_ShopperWebService.Models;
using E_ShopperWebService.ViewModels;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace E_ShopperWebService
{
    /// <summary>
    ///OrderConfirmationService 的摘要描述
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允許使用 ASP.NET AJAX 從指令碼呼叫此 Web 服務，請取消註解下列一行。
    // [System.Web.Script.Services.ScriptService]
    public class OrderConfirmationService : System.Web.Services.WebService
    {
        private readonly ConnectionModel connectionModel;

        public OrderConfirmationService()
        {
            connectionModel = new ConnectionModel();
        }

        [WebMethod]
        public OrderConfirmationViewModel GetOrderDetails(int orderId)
        {
            OrderConfirmationViewModel orderDetails = new OrderConfirmationViewModel();
            string connectionString = connectionModel.DBTEST3con();

            try
            {
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();

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
