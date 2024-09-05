using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using E_ShopperWebService.Models;
using Oracle.ManagedDataAccess.Client;
using E_ShopperWebService.ViewModels;

namespace E_ShopperWebService
{
    /// <summary>
    ///ProductsDetailService 的摘要描述
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允許使用 ASP.NET AJAX 從指令碼呼叫此 Web 服務，請取消註解下列一行。
    // [System.Web.Script.Services.ScriptService]
    public class ProductsDetailService : System.Web.Services.WebService
    {

        private readonly ConnectionModel connectionModel;

        public ProductsDetailService()
        {
            connectionModel = new ConnectionModel();
        }

        [WebMethod]
        public ProductsDetailViewModel GetProductById(int id)
        {
            string connectionString = connectionModel.DBTEST3con();

            ProductsDetailViewModel product = null;

            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                string query = "SELECT VCHRPRODUCTID, VCHRNAME, VCHRDESCRIPTION, VCHRPRICE, VCHRDISCOUNTPRICE, VCHRSTOCKQUANTITY, VCHRMAINIMAGEURL FROM PRODUCTS WHERE VCHRPRODUCTID = :id";

                using (OracleCommand command = new OracleCommand(query, conn))
                {
                    command.Parameters.Add(new OracleParameter("id", id));
                    conn.Open();
                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            product = new ProductsDetailViewModel
                            {
                                VCHRPRODUCTID = Convert.ToInt32(reader["VCHRPRODUCTID"]),
                                VCHRNAME = reader["VCHRNAME"].ToString(),
                                VCHRDESCRIPTION = reader["VCHRDESCRIPTION"].ToString(),
                                VCHRPRICE = reader["VCHRPRICE"].ToString(),
                                VCHRSTOCKQUANTITY = reader["VCHRSTOCKQUANTITY"].ToString(),
                                VCHRDISCOUNTPRICE = reader["VCHRDISCOUNTPRICE"].ToString(),
                                VCHRMAINIMAGEURL = reader["VCHRMAINIMAGEURL"].ToString()
                            };
                        }
                    }
                }
            }

            return product;
        }


        [WebMethod]
        // 將商品添加到購物車的方法
        public void AddToCartInDatabase(int userId, int productId, string quantity)
        {
            // 使用 OracleDB 將商品添加到 CartItems 表中
            string connectionString = connectionModel.DBTEST3con();

            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                string query = "INSERT INTO CARTITEM (VCHRPRODUCTID, VCHQUANTITY, VCHUSERID) VALUES (:productId, :quantity, :userId)";
                OracleCommand command = new OracleCommand(query, connection);
                command.Parameters.Add(":productId", OracleDbType.Int32).Value = productId;
                command.Parameters.Add(":quantity", OracleDbType.Int32).Value = quantity;
                command.Parameters.Add(":userId", OracleDbType.Int32).Value = userId;

                connection.Open();
                command.ExecuteNonQuery();
            }
        }

        [WebMethod]
        //檢查物品是否已在購物車內
        public bool IsProductInCart(int userId, int productId)
        {
            string connectionString = connectionModel.DBTEST3con();

            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                conn.Open();
                using (OracleCommand command = new OracleCommand("SELECT COUNT(*) FROM CARTITEM WHERE VCHUSERID = :userId AND VCHRPRODUCTID = :productId", conn))
                {
                    command.Parameters.Add(new OracleParameter("userId", userId));
                    command.Parameters.Add(new OracleParameter("productId", productId));

                    int count = Convert.ToInt32(command.ExecuteScalar());
                    return count > 0;
                }
            }
        }
    }
}
