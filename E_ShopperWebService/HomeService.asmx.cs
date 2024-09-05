using E_ShopperWebService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using E_ShopperWebService.ViewModels;
using Oracle.ManagedDataAccess.Client;

namespace E_ShopperWebService
{
    /// <summary>
    ///HomeService 的摘要描述
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允許使用 ASP.NET AJAX 從指令碼呼叫此 Web 服務，請取消註解下列一行。
    // [System.Web.Script.Services.ScriptService]
    public class HomeService : System.Web.Services.WebService
    {
            
        private readonly ConnectionModel connectionModel;

        public HomeService()
        {
            connectionModel = new ConnectionModel();
        }

        [WebMethod]
        public List<HomeViewModel> GetProducts()
        {
            string connectionString = connectionModel.DBTEST3con();

            var products = new List<HomeViewModel>();

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
                            var product = new HomeViewModel
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

        [WebMethod]
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

        [WebMethod]
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


        [WebMethod]
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

        [WebMethod]
        public List<HomeViewModel> GetProductsByCategory(string category)
        {
            string connectionString = connectionModel.DBTEST3con();

            List<HomeViewModel> products = new List<HomeViewModel>();

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
                        HomeViewModel product = new HomeViewModel
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
