using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using E_ShopperWebService.ViewModels;
using Oracle.ManagedDataAccess.Client;
using E_ShopperWebService.Models;
using System.Security.Cryptography;
using System.Text;

namespace E_ShopperWebService
{
    /// <summary>
    /// AccountService 的摘要說明
    /// </summary>
    [WebService(Namespace = "http://yourwebsite.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    public class AccountService : WebService
    {
        private readonly ConnectionModel connectionModel;
        public AccountService()
        {
            connectionModel = new ConnectionModel();
        }


        [WebMethod]
        public AccountViewModel GetUserByUsername(string username)
        {

            try
            {
                string connectionString = connectionModel.DBTEST3con();

                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();

                    OracleCommand command = new OracleCommand
                        (
                        "SELECT " +
                        "VCHUSERID, VCHUSERNAME, VCHPASSWORD, VCHEMAIL, VCHROLE, VCHACTIVE " +
                        "FROM ACCOUNT " +
                        "WHERE VCHUSERNAME = :username",
                        connection);

                    command.BindByName = true;
                    command.Parameters.Add(new OracleParameter("username", username));

                    using (OracleDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new AccountViewModel
                            {
                                VCHUSERID = Convert.ToInt32(reader["VCHUSERID"]),
                                VCHUSERNAME = reader["VCHUSERNAME"].ToString(),
                                VCHPASSWORD = reader["VCHPASSWORD"].ToString(),
                                VCHEMAIL = reader["VCHEMAIL"].ToString(),
                                VCHROLE = reader["VCHROLE"].ToString(),
                                VCHACTIVE = reader["VCHACTIVE"].ToString(),
                            };
                        }
                    }
                }
            }
            catch (OracleException ex)
            {
                Console.Error.WriteLine("Oracle Error: " + ex.Message);
                throw new ApplicationException("Error querying records", ex); 
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("General Error: " + ex.Message);
                throw new ApplicationException("An error occurred while retrieving records", ex); 
            }

            return null;
        }

        [WebMethod]
        public bool ValidatePassword(AccountViewModel userinfo, string password)
        {
            string[] storedPasswordHash = userinfo.VCHPASSWORD.Split(':');
            byte[] salt = Convert.FromBase64String(storedPasswordHash[0]);
            byte[] expectedHash = Convert.FromBase64String(storedPasswordHash[1]);

            byte[] passwordBytes = ConcatenateSaltAndPassword(Encoding.UTF8.GetBytes(password), salt);

            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] computedHash = sha256.ComputeHash(passwordBytes);

                if (computedHash.SequenceEqual(expectedHash))
                {
                    return true;

                }
                else
                {
                    return false;
                }
            }

          
        }
        [WebMethod]
        public void RegisterUser(AccountViewModel user)
        {
            try
            {
                string connectionString = connectionModel.DBTEST3con();
                using (OracleConnection connection = new OracleConnection(connectionString))
                {
                    connection.Open();

                    user.VCHPASSWORD = HashPassword(user.VCHPASSWORD); // 加密密碼

                    string insertsql = @"INSERT INTO ACCOUNT (VCHUSERNAME, VCHPASSWORD, VCHEMAIL, VCHROLE) VALUES (:username, :password, :email, :role)";
                    using (OracleCommand command = new OracleCommand(insertsql, connection))
                    {

                        command.BindByName = true; //SQL 參數绑定是根據參數名稱而不是順序進行

                        command.Parameters.Add(new OracleParameter("username", user.VCHUSERNAME));
                        command.Parameters.Add(new OracleParameter("password", user.VCHPASSWORD));
                        command.Parameters.Add(new OracleParameter("email", user.VCHEMAIL));
                        command.Parameters.Add(new OracleParameter("role", user.VCHROLE));

                        command.ExecuteNonQuery(); // 執行插入操作



                    }


                }

            }
            catch (OracleException ex)
            {
                Console.Error.WriteLine("Oracle Error: " + ex.Message);
                // 您可以返回空列表，抛出异常，或紀錄錯誤
                throw new ApplicationException("Error querying records", ex); // 重新抛出异常
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("General Error: " + ex.Message);
                // 一般異常處理
                throw new ApplicationException("An error occurred while retrieving records", ex); // 重新抛出異常
            }

        }


        public static byte[] ConcatenateSaltAndPassword(byte[] password, byte[] salt)
        {
            try
            {
                byte[] result = new byte[salt.Length + password.Length];
                Buffer.BlockCopy(salt, 0, result, 0, salt.Length); // 複製鹽
                Buffer.BlockCopy(password, 0, result, salt.Length, password.Length); // 複製密碼
                return result;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Concatenation Error: " + ex.Message);
                throw new ApplicationException("An error occurred while concatenating salt and password.", ex); // 抛出異常
            }
        }

        public static byte[] GenerateSalt(int length)
        {
            try
            {
                RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                byte[] salt = new byte[length];
                rng.GetBytes(salt); // 生成隨機數
                return salt;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine("Salt Generation Error: " + ex.Message);
                throw new ApplicationException("An error occurred while generating salt.", ex); // 拋出異常
            }
        }

        public static string HashPassword(string password)
        {
            try
            {
                using (SHA256 sha256 = SHA256.Create())
                {
                    byte[] salt = GenerateSalt(16); // 生成隨機鹽
                    byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                    byte[] saltAndPassword = ConcatenateSaltAndPassword(passwordBytes, salt);

                    byte[] hash = sha256.ComputeHash(saltAndPassword); // 計算哈希值

                    return Convert.ToBase64String(salt) + ":" + Convert.ToBase64String(hash); // 返回鹽和哈希值
                }
            }
            catch (ArgumentException ex)
            {
                // 處理無效输入或參數錯誤
                Console.Error.WriteLine("Argument Error: " + ex.Message);
                throw new ApplicationException("An error occurred while hashing the password.", ex); // 抛出異常或返回特定的錯誤消息
            }
            catch (Exception ex)
            {
                // 處理其他通用異常
                Console.Error.WriteLine("General Error: " + ex.Message);
                throw new ApplicationException("An unexpected error occurred.", ex); // 拋出異常或返回特定的錯誤消息
            }
        }
    }
}
