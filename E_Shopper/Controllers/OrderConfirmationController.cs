using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using E_Shopper.OrderConfirmationWebService;
namespace E_Shopper.Controllers
{
    public class OrderConfirmationController : Controller
    {
        private readonly OrderConfirmationServiceSoapClient _client;


        public OrderConfirmationController()
        {
            _client = new OrderConfirmationServiceSoapClient();


        }
        // GET: OrderConfirmation
        public ActionResult Index(int id)
        {
            // 根據訂單ID獲取訂單詳細訊系並傳遞到視圖
            var order = _client.GetOrderDetails(id);
            return View(order);
        }
    }
}