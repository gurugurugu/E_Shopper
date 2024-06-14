using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using E_Shopper.Models;
using E_Shopper.ViewModels;


namespace E_Shopper.Controllers
{
    public class OrderCheckoutController : Controller
    {
        private readonly OrderCheckoutModel _Model;

        public OrderCheckoutController()
        {
            _Model = new OrderCheckoutModel();
        }



        [HttpPost]
        public ActionResult Index(string selectedItems)
        {

            // 将逗号分隔的字符串转换为 List<string>
            var selectedItemIds = selectedItems.Split(',').ToList();

            var orderItems = _Model.GetOrderDetails(selectedItemIds);

            return View(orderItems);
        }



        // POST: /OrderCheckout/Save
        [HttpPost]
        public ActionResult Save(string totalAmount, string paymentMethod, string deliveryMethod, string shippingAddress, string orderNotes,List<OrderItem> orderItems)
        {

            int userId = (int)Session["UserID"];
            // 调用方法保存订单信息和订单详情到数据库
            int newOrderId = _Model.SaveOrderToDatabase(userId, totalAmount, paymentMethod, deliveryMethod, shippingAddress, orderNotes);

            _Model.SaveOrderDetailsToDatabase(orderItems, newOrderId);


            return Json(new { newOrderId = newOrderId });
        }   


    }
}