using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using E_Shopper.ViewModels;
using Oracle.ManagedDataAccess.Client;

namespace E_Shopper.Models
{
    public class HomeModel
    {
        private readonly ConnectionModel connectionModel;

        public HomeModel()
        {
            connectionModel = new ConnectionModel();
        }




        public List<ProductsViewModel> GetProducts()
        {
            string connectionString = connectionModel.DBTEST3con();

            var products = new List<ProductsViewModel>();

            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                string query = "SELECT VCHRPRODUCTID, VCHRNAME, VCHRDESCRIPTION, VCHRCATEGORY, VCHRBRAND, VCHRPRICE, VCHRDISCOUNTPRICE, VCHRSTOCKQUANTITY, VCHRCOLOR, VCHRSIZE, VCHRMATERIAL, VCHRMAINIMAGEURL, VCHRSTATUS, VCHRTAGS FROM PRODUCTS";

                using (OracleCommand cmd = new OracleCommand(query, conn))
                {
                    conn.Open();
                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var product = new ProductsViewModel
                            {
                                VCHRPRODUCTID = Convert.ToInt32(reader["VCHRPRODUCTID"]),
                                VCHRNAME = reader["VCHRNAME"].ToString(),
                                VCHRDESCRIPTION = reader["VCHRDESCRIPTION"].ToString(),
                                VCHRCATEGORY = reader["VCHRCATEGORY"].ToString(),
                                VCHRBRAND = reader["VCHRBRAND"].ToString(),
                                VCHRPRICE = reader["VCHRPRICE"].ToString(),
                                VCHRDISCOUNTPRICE = reader["VCHRDISCOUNTPRICE"].ToString(),
                                VCHRSTOCKQUANTITY = reader["VCHRSTOCKQUANTITY"].ToString(),
                                VCHRCOLOR = reader["VCHRCOLOR"].ToString(),
                                VCHRSIZE = reader["VCHRSIZE"].ToString(),
                                VCHRMATERIAL = reader["VCHRMATERIAL"].ToString(),
                                VCHRMAINIMAGEURL = reader["VCHRMAINIMAGEURL"].ToString(),
                                VCHRSTATUS = reader["VCHRSTATUS"].ToString(),
                                VCHRTAGS = reader["VCHRTAGS"].ToString()
                            };
                            products.Add(product);
                        }
                    }
                }
            }

            return products;
        }




        // 將商品添加到購物車的方法
        public void AddToCartInDatabase(int userId, int productId, int quantity)
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



        public List<CategoryCountViewModel> GetCategoriesWithCount()
        {
            string connectionString = connectionModel.DBTEST3con();

            List<CategoryCountViewModel> categories = new List<CategoryCountViewModel>();

            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                string query = @"
                SELECT VCHRCATEGORY, COUNT(*) AS Count
                FROM Products
                GROUP BY VCHRCATEGORY
            ";

                using (OracleCommand command = new OracleCommand(query, connection))
                {
                    connection.Open();
                    OracleDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        string category = reader["VCHRCATEGORY"].ToString();
                        int count = Convert.ToInt32(reader["Count"]);

                        categories.Add(new CategoryCountViewModel
                        {
                            Category = category,
                            Count = count
                        });
                    }
                }
            }

            return categories;
        }



        public List<ProductsViewModel> GetProductsByCategory(string category)
        {
            string connectionString = connectionModel.DBTEST3con();

            List<ProductsViewModel> products = new List<ProductsViewModel>();

            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                string query = @"
                SELECT 
                    VCHRPRODUCTID,
                    VCHRDESCRIPTION,
                    VCHRCATEGORY,
                    VCHRBRAND,
                    VCHRPRICE,
                    VCHRDISCOUNTPRICE,
                    VCHRSTOCKQUANTITY,
                    VCHRCOLOR,
                    VCHRSIZE,
                    VCHRMATERIAL,
                    VCHRMAINIMAGEURL,
                    VCHRSTATUS,
                    VCHRTAGS
                FROM Products
                WHERE VCHRCATEGORY = :category
            ";

                using (OracleCommand command = new OracleCommand(query, connection))
                {
                    command.Parameters.Add(new OracleParameter("category", OracleDbType.Varchar2)).Value = category;

                    connection.Open();
                    OracleDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        ProductsViewModel product = new ProductsViewModel
                        {
                            VCHRPRODUCTID = Convert.ToInt32(reader["VCHRPRODUCTID"]),
                            VCHRDESCRIPTION = reader["VCHRDESCRIPTION"].ToString(),
                            VCHRCATEGORY = reader["VCHRCATEGORY"].ToString(),
                            VCHRBRAND = reader["VCHRBRAND"].ToString(),
                            VCHRPRICE = reader["VCHRPRICE"].ToString(),
                            VCHRDISCOUNTPRICE = reader["VCHRDISCOUNTPRICE"].ToString(),
                            VCHRSTOCKQUANTITY = reader["VCHRSTOCKQUANTITY"].ToString(),
                            VCHRCOLOR = reader["VCHRCOLOR"].ToString(),
                            VCHRSIZE = reader["VCHRSIZE"].ToString(),
                            VCHRMATERIAL = reader["VCHRMATERIAL"].ToString(),
                            VCHRMAINIMAGEURL = reader["VCHRMAINIMAGEURL"].ToString(),
                            VCHRSTATUS = reader["VCHRSTATUS"].ToString(),
                            VCHRTAGS = reader["VCHRTAGS"].ToString()
                        };
                        products.Add(product);
                    }
                }
            }

            return products;
        }
    }
}