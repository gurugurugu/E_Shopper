using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using E_Shopper.OrderCheckoutWebService;


namespace E_Shopper.Controllers
{
    public class OrderCheckoutController : Controller
    {
        private readonly OrderCheckoutServiceSoapClient _client;


        public OrderCheckoutController()
        {
            _client = new OrderCheckoutServiceSoapClient();
        }



        [HttpPost]
        public ActionResult Index(string selectedItems)
        {
            //因為使用WebService，必須將List<>轉換成Arrary。

            // 將逗號分隔的字符串轉換為數據
            string[] selectedItemIds = selectedItems.Split(',');

            // 創建 ArrayOfString 實例並附值
            var arrayOfString = new ArrayOfString();
            arrayOfString.AddRange(selectedItemIds);

            // 調用 Web 服務方法，傳遞 ArrayOfString 類型的參數
            var orderItems = _client.GetOrderDetails(arrayOfString);

            return View(orderItems);
        }



        [HttpPost]
        public ActionResult Save(string totalAmount, string paymentMethod, string deliveryMethod, string shippingAddress, string orderNotes, List<OrderItem> orderItems)
        {
            int userId = (int)Session["UserID"];

            //因為使用WebService，必須將List<>轉換成Arrary。
            // 調用方法保存訂單訊息和訂單詳細到資料庫
            int newOrderId = _client.SaveOrderToDatabase(userId, totalAmount, paymentMethod, deliveryMethod, shippingAddress, orderNotes);

            // 將 List<OrderItem> 轉換為 OrderItem 數組
            OrderItem[] orderItemsArray = orderItems.ToArray();

            // 調用 Web 服務方法，傳遞 OrderItem 數組類型參數
            _client.SaveOrderDetailsToDatabase(orderItemsArray, newOrderId);

            return Json(new { newOrderId = newOrderId });
        }
    }


}
