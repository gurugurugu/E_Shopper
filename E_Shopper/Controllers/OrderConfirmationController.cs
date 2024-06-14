using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using E_Shopper.Models;

namespace E_Shopper.Controllers
{
    public class OrderConfirmationController : Controller
    {
        private readonly OrderConfirmationModel _Model;

        public OrderConfirmationController()
        {
            _Model = new OrderConfirmationModel();

        }
        // GET: OrderConfirmation
        public ActionResult Index(int id)
        {
            // 根据订单ID获取订单详细信息并传递到视图
            var order = _Model.GetOrderDetails(id);
            return View(order);
        }
    }
}