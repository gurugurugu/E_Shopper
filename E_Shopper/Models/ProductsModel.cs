using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using E_Shopper.ViewModels;
using System.Web.Mvc;
using System.Diagnostics;

namespace E_Shopper.Models
{
    public class ProductsModel
    {
        private readonly ConnectionModel connectionModel;

        public ProductsModel()
        {
            connectionModel = new ConnectionModel();
        }


        public List<ProductsViewModel> GetProducts()
        {
            List<ProductsViewModel> products = new List<ProductsViewModel>();

            string connectionString = connectionModel.DBTEST3con();

            using (OracleConnection con = new OracleConnection(connectionString))
            {
                using (OracleCommand cmd = new OracleCommand("SELECT * FROM PRODUCTS", con))
                {
                    con.Open();
                    using (OracleDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ProductsViewModel product = new ProductsViewModel();
                            product.VCHRPRODUCTID = Convert.ToInt32(reader["VCHRPRODUCTID"]);
                            product.VCHRNAME = reader["VCHRNAME"].ToString();
                            product.VCHRDESCRIPTION = reader["VCHRDESCRIPTION"].ToString();
                            product.VCHRCATEGORY = reader["VCHRCATEGORY"].ToString();
                            product.VCHRBRAND = reader["VCHRBRAND"].ToString();
                            product.VCHRPRICE = reader["VCHRPRICE"].ToString();
                            product.VCHRDISCOUNTPRICE = reader["VCHRDISCOUNTPRICE"].ToString();
                            product.VCHRSTOCKQUANTITY = reader["VCHRSTOCKQUANTITY"].ToString();
                            product.VCHRCOLOR = reader["VCHRCOLOR"].ToString();
                            product.VCHRSIZE = reader["VCHRSIZE"].ToString();
                            product.VCHRMATERIAL = reader["VCHRMATERIAL"].ToString();
                            product.VCHRMAINIMAGEURL = reader["VCHRMAINIMAGEURL"].ToString();
                            product.VCHRSTATUS = reader["VCHRSTATUS"].ToString();
                            product.VCHRTAGS = reader["VCHRTAGS"].ToString();
                            // 繼續填充其他屬性
                            products.Add(product);
                        }
                    }
                }
            }

            return products;
        }


        public void AddProduct(ProductsViewModel product)
        {
            string connectionString = connectionModel.DBTEST3con();

            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                conn.Open();

                string insertQuery = @"INSERT INTO PRODUCTS (VCHRNAME, VCHRDESCRIPTION, VCHRCATEGORY, VCHRBRAND, VCHRPRICE, VCHRDISCOUNTPRICE, VCHRSTOCKQUANTITY, VCHRCOLOR, VCHRSIZE, VCHRMATERIAL, VCHRMAINIMAGEURL, VCHRSTATUS, VCHRTAGS)" +
                    " VALUES (:VCHRNAME, :VCHRDESCRIPTION, :VCHRCATEGORY, :VCHRBRAND, :VCHRPRICE, :VCHRDISCOUNTPRICE, :VCHRSTOCKQUANTITY, :VCHRCOLOR, :VCHRSIZE, :VCHRMATERIAL, :VCHRMAINIMAGEURL, :VCHRSTATUS, :VCHRTAGS)";

                using (OracleCommand command = new OracleCommand(insertQuery, conn))
                {


                    command.Parameters.Add(new OracleParameter("VCHRNAME", product.VCHRNAME));
                    command.Parameters.Add(new OracleParameter("VCHRDESCRIPTION", product.VCHRDESCRIPTION));
                    command.Parameters.Add(new OracleParameter("VCHRCATEGORY", product.VCHRCATEGORY));
                    command.Parameters.Add(new OracleParameter("VCHRBRAND", product.VCHRBRAND));
                    command.Parameters.Add(new OracleParameter("VCHRPRICE", product.VCHRPRICE ));
                    command.Parameters.Add(new OracleParameter("VCHRDISCOUNTPRICE", product.VCHRDISCOUNTPRICE));
                    command.Parameters.Add(new OracleParameter("VCHRSTOCKQUANTITY", product.VCHRSTOCKQUANTITY));
                    command.Parameters.Add(new OracleParameter("VCHRCOLOR", product.VCHRCOLOR));
                    command.Parameters.Add(new OracleParameter("VCHRSIZE", product.VCHRSIZE));
                    command.Parameters.Add(new OracleParameter("VCHRMATERIAL ", product.VCHRMATERIAL ));
                    command.Parameters.Add(new OracleParameter("VCHRMAINIMAGEURL", product.VCHRMAINIMAGEURL ));
                    command.Parameters.Add(new OracleParameter("VCHRSTATUS", product.VCHRSTATUS ));
                    command.Parameters.Add(new OracleParameter("VCHRTAGS", product.VCHRTAGS ));


                    command.ExecuteNonQuery();
                }
            }
        }



        public void UpdateStatus(string productId, string status)
        {

            try
            {
                // 数据库更新逻辑
                string connectionString = connectionModel.DBTEST3con();

                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();

                    string sql = @"
                        UPDATE PRODUCTS
                        SET VCHRSTATUS = :status
                        WHERE VCHRPRODUCTID = :productId";

                    using (OracleCommand command = new OracleCommand(sql, connection))
                    {
                        command.Parameters.Add(new OracleParameter("status", status));
                        command.Parameters.Add(new OracleParameter("productId", productId));

                        command.ExecuteNonQuery();
                    }
                }
     }
            catch (Exception ex)
            {

            }
        }


        public void UpdateProduct(ProductsViewModel product)
        {

            try
            {
                string connectionString = connectionModel.DBTEST3con();
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();

                    string sql = @"
                    UPDATE PRODUCTS 
                    SET VCHRDESCRIPTION = :description,
                        VCHRCATEGORY = :category,
                        VCHRBRAND = :brand,
                        VCHRPRICE = :price,
                        VCHRDISCOUNTPRICE = :discountPrice,
                        VCHRSTOCKQUANTITY = :stockQuantity,
                        VCHRCOLOR = :color,
                        VCHRMATERIAL = :material,
                        VCHRSTATUS = :status,
                        VCHRTAGS = :tags
                    WHERE VCHRPRODUCTID = :productId";

                    using (OracleCommand command = new OracleCommand(sql, connection))
                    {

                        command.Parameters.Add(new OracleParameter("description", product.VCHRDESCRIPTION ));
                        command.Parameters.Add(new OracleParameter("category", product.VCHRCATEGORY ));
                        command.Parameters.Add(new OracleParameter("brand", product.VCHRBRAND ));
                        command.Parameters.Add(new OracleParameter("price",  product.VCHRPRICE ));
                        command.Parameters.Add(new OracleParameter("discountPrice", product.VCHRDISCOUNTPRICE));
                        command.Parameters.Add(new OracleParameter("stockQuantity", product.VCHRSTOCKQUANTITY ));
                        command.Parameters.Add(new OracleParameter("color", product.VCHRCOLOR ));
                        command.Parameters.Add(new OracleParameter("material", product.VCHRMATERIAL ));
                        command.Parameters.Add(new OracleParameter("status", product.VCHRSTATUS ));
                        command.Parameters.Add(new OracleParameter("tags", product.VCHRTAGS ));
                        command.Parameters.Add(new OracleParameter("productId", product.VCHRPRODUCTID ));




                        command.ExecuteNonQuery();

                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception details for debugging
                Debug.WriteLine("Error updating product: " + ex.Message);
                throw; // Optionally re-throw the exception if you want it to propagate further
            }

        }


        public void UpdateProductImage(ProductsViewModel product)
        {

            try
            {
                string connectionString = connectionModel.DBTEST3con();
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();

                    string sql = @"
                    UPDATE PRODUCTS 
                    SET VCHRMAINIMAGEURL = :mainImageUrl
                    WHERE VCHRPRODUCTID = :productId";

                    using (OracleCommand command = new OracleCommand(sql, connection))
                    {

                        command.Parameters.Add(new OracleParameter("mainImageUrl", product.VCHRMAINIMAGEURL));
                        command.Parameters.Add(new OracleParameter("productId", product.VCHRPRODUCTID));

                        command.ExecuteNonQuery();

                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception details for debugging
                Debug.WriteLine("Error updating product: " + ex.Message);
                throw; // Optionally re-throw the exception if you want it to propagate further
            }

        }



        public void DeleteProduct(int productId)
        {
            string connectionString = connectionModel.DBTEST3con();

            using (OracleConnection connection = new OracleConnection(connectionString))
            {
                string sql = @"
                DELETE FROM PRODUCTS
                WHERE VCHRPRODUCTID = :productId";

                OracleCommand command = new OracleCommand(sql, connection);
                command.Parameters.Add(":productId", OracleDbType.Int32).Value = productId;

                try
                {
                    connection.Open();
                    int rowsAffected = command.ExecuteNonQuery();
                    if (rowsAffected == 0)
                    {
                        throw new Exception("商品未找到或已被删除。");
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("删除商品时发生错误：" + ex.Message);
                }
                finally
                {
                    connection.Close();
                }
            }

        }



    }
}