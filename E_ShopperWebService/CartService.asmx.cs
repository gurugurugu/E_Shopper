﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using Oracle.ManagedDataAccess.Client;
using E_ShopperWebService.Models;
using E_ShopperWebService.ViewModels;


namespace E_ShopperWebService
{
    /// <summary>
    ///CartService 的摘要描述
    /// </summary>
    [WebService(Namespace = "http://yourwebsite.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允許使用 ASP.NET AJAX 從指令碼呼叫此 Web 服務，請取消註解下列一行。
    // [System.Web.Script.Services.ScriptService]
    public class CartService : System.Web.Services.WebService
    {
        private readonly ConnectionModel connectionModel;
        public CartService()
        {
            connectionModel = new ConnectionModel();
        }

        [WebMethod]
        public List<CartViewModel> GetCartbyUserId(int userid)
        {
            string connectionString = connectionModel.DBTEST3con();

            var carts = new List<CartViewModel>();

            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                // 使用 JOIN 查询
                string query = @"
            SELECT 
                ci.VCHCARTITEMID, 
                ci.VCHRPRODUCTID, 
                ci.VCHQUANTITY,
                p.VCHRNAME,
                p.VCHRPRICE,
                p.VCHRSTOCKQUANTITY,
                p.VCHRMAINIMAGEURL
            FROM 
                CARTITEM ci
            JOIN 
                PRODUCTS p ON ci.VCHRPRODUCTID = p.VCHRPRODUCTID
            WHERE 
                ci.VCHUSERID = :userid";

                using (OracleCommand command = new OracleCommand(query, conn))
                {
                    command.Parameters.Add(new OracleParameter("userid", userid));

                    conn.Open();
                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var cart = new CartViewModel
                            {
                                VCHCARTITEMID = Convert.ToInt32(reader["VCHCARTITEMID"]),
                                VCHRPRODUCTID = Convert.ToInt32(reader["VCHRPRODUCTID"]),
                                VCHQUANTITY = reader["VCHQUANTITY"].ToString(),
                                VCHRNAME = reader["VCHRNAME"].ToString(),
                                VCHRPRICE = reader["VCHRPRICE"].ToString(),
                                VCHRSTOCKQUANTITY = reader["VCHRSTOCKQUANTITY"].ToString(),
                                VCHRMAINIMAGEURL = reader["VCHRMAINIMAGEURL"].ToString()
                            };
                            carts.Add(cart);
                        }
                    }
                }
            }
            return carts;
        }
        [WebMethod]
        public void DeletebyCartItemId(int cartitemid)
        {
            string connectionString = connectionModel.DBTEST3con();

            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                string sql = "DELETE FROM CARTITEM WHERE VCHCARTITEMID = :cartitemid";

                using (OracleCommand command = new OracleCommand(sql, conn))
                {
                    command.Parameters.Add(new OracleParameter("cartitemid", cartitemid));

                    conn.Open();


                    command.ExecuteNonQuery();
                }
            }
        }
        [WebMethod]
        public void UpdateCartItemQuantity(int cartItemId, string quantity)
        {
            string connectionString = connectionModel.DBTEST3con();
            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                conn.Open();
                using (OracleCommand command = new OracleCommand("UPDATE CARTITEM SET VCHQUANTITY = :quantity WHERE VCHCARTITEMID = :cartItemId", conn))
                {
                    command.Parameters.Add(new OracleParameter("quantity", quantity));
                    command.Parameters.Add(new OracleParameter("cartItemId", cartItemId));
                    command.ExecuteNonQuery();
                }
            }
        }

    }
}
