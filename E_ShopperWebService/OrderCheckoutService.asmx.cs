﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Services;
using E_ShopperWebService.Models;
using E_ShopperWebService.ViewModels;
using Oracle.ManagedDataAccess.Client;
using Oracle.ManagedDataAccess.Types;

namespace E_ShopperWebService
{
    /// <summary>
    ///OrderCheckoutService 的摘要描述
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允許使用 ASP.NET AJAX 從指令碼呼叫此 Web 服務，請取消註解下列一行。
    // [System.Web.Script.Services.ScriptService]
    public class OrderCheckoutService : System.Web.Services.WebService
    {
        private readonly ConnectionModel connectionModel;

        public OrderCheckoutService()
        {
            connectionModel = new ConnectionModel();
        }

        [WebMethod]
        public List<OrderCheckoutViewModel> GetOrderDetails(string[] selectedItems)
        {
            string connectionString = connectionModel.DBTEST3con();

            var orderItems = new List<OrderCheckoutViewModel>();

            using (var connection = new OracleConnection(connectionString))
            {

                connection.Open();

                string query = @"
                SELECT
                    ci.VCHRPRODUCTID, 
                    ci.VCHQUANTITY,
				    ci.VCHUSERID,
                    p.VCHRNAME,
                    p.VCHRPRICE,
                    p.VCHRMAINIMAGEURL
                FROM 
                    CARTITEM ci
                JOIN 
                    PRODUCTS p ON ci.VCHRPRODUCTID = p.VCHRPRODUCTID
                WHERE 
                    ci.VCHCARTITEMID = :itemId";



                foreach (var itemId in selectedItems)
                {
                    using (var command = new OracleCommand(query, connection))
                    {
                        command.Parameters.Add(new OracleParameter("itemId", itemId));

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var orderItem = new OrderCheckoutViewModel
                                {
                                    VCHRPRODUCTID = Convert.ToInt32(reader["VCHRPRODUCTID"]),
                                    VCHQUANTITY = reader["VCHQUANTITY"].ToString(),
                                    VCHUSERID = Convert.ToInt32(reader["VCHUSERID"]),
                                    VCHRNAME = reader["VCHRNAME"].ToString(),
                                    VCHRPRICE = reader["VCHRPRICE"].ToString(),
                                    VCHRMAINIMAGEURL = reader["VCHRMAINIMAGEURL"].ToString()
                                };
                                orderItems.Add(orderItem);
                            }
                        }
                    }
                }
            }

            return orderItems;
        }

        [WebMethod]
        public int SaveOrderToDatabase(int userId, string totalAmount, string paymentMethod, string deliveryMethod, string shippingAddress, string orderNotes)
        {

            string connectionString = connectionModel.DBTEST3con();

            int newOrderId;

            using (var connection = new OracleConnection(connectionString))
            {
                connection.Open();
                var query = "INSERT INTO ORDERS (VCHRUSERID, VCHRTOTALAMOUNT, VCHRPAYMENTMETHOD, VCHRSHIPPINGADDRESS, VCHRNOTES) VALUES (:UserId, :TotalAmount, :PaymentMethod, :ShippingAddress, :OrderNotes) RETURNING VCHRORDERID INTO :newOrderId";
                using (var command = new OracleCommand(query, connection))
                {
                    command.Parameters.Add(new OracleParameter("UserId", userId));
                    command.Parameters.Add(new OracleParameter("TotalAmount", totalAmount));
                    command.Parameters.Add(new OracleParameter("PaymentMethod", paymentMethod));
                    command.Parameters.Add(new OracleParameter("ShippingAddress", shippingAddress));
                    command.Parameters.Add(new OracleParameter("OrderNotes", orderNotes));
                    command.Parameters.Add(new OracleParameter("newOrderId", OracleDbType.Int32, ParameterDirection.Output));
                    command.ExecuteNonQuery();

                    //System.InvalidCastException: '無法將類型 'Oracle.ManagedDataAccess.Types.OracleDecimal' 的物件轉換為類型 'System.IConvertible'。


                    OracleDecimal oracleDecimal = (OracleDecimal)command.Parameters["newOrderId"].Value;
                    newOrderId = oracleDecimal.ToInt32();

                    return newOrderId;
                }
            }
        }


        [WebMethod]
        public void SaveOrderDetailsToDatabase(List<OrderItem> orderItems, int newOrderId)
        {

            string connectionString = connectionModel.DBTEST3con();

            using (var connection = new OracleConnection(connectionString))
            {
                connection.Open();
                foreach (var item in orderItems)
                {
                    var query = "INSERT INTO ORDER_DETAILS (VCHRORDERID, VCHRPRODUCTID, VCHRQUANTITY, VCHRPRICE) VALUES (:OrderId, :ProductId, :Quantity, :Price)";
                    using (var command = new OracleCommand(query, connection))
                    {
                        command.Parameters.Add(new OracleParameter("OrderId", newOrderId));
                        command.Parameters.Add(new OracleParameter("ProductId", item.productid));
                        command.Parameters.Add(new OracleParameter("Quantity", item.quantity));
                        command.Parameters.Add(new OracleParameter("Price", item.price));
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

    }
}
