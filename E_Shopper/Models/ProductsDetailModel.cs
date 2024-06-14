using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using E_Shopper.ViewModels;
using Oracle.ManagedDataAccess.Client;

namespace E_Shopper.Models
{
    public class ProductsDetailModel
    {
        private readonly ConnectionModel connectionModel;

        public ProductsDetailModel()
        {
            connectionModel = new ConnectionModel();
        }


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
                                VCHRNAME =          reader["VCHRNAME"].ToString(),
                                VCHRDESCRIPTION =   reader["VCHRDESCRIPTION"].ToString(),
                                VCHRPRICE =         reader["VCHRPRICE"].ToString(),
                                VCHRSTOCKQUANTITY = reader["VCHRSTOCKQUANTITY"].ToString(),
                                VCHRDISCOUNTPRICE = reader["VCHRDISCOUNTPRICE"].ToString(),
                                VCHRMAINIMAGEURL =  reader["VCHRMAINIMAGEURL"].ToString()
                            };
                        }
                    }
                }
            }

            return product;
        }



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