using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using E_Shopper.CartWebService;

namespace E_Shopper.Controllers
{
    public class CartController : Controller
    {
        private readonly CartServiceSoapClient _client;
        

        public CartController()
        {
            _client = new CartServiceSoapClient();


        }

        public ActionResult Index()
        {

            if (Session["UserID"] == null)
            {
                TempData["MessageType"] = "warning";
                TempData["MessageContent"] = "現在還未登入";

                return RedirectToAction("Index", "Home");

            }
            int userId = (int)Session["UserID"];
            var cartItems = _client.GetCartbyUserId(userId);
            return View(cartItems);
        }

        [HttpPost]
        public ActionResult UpdateCartItem(int cartItemId, string quantity)
        {

            _client.UpdateCartItemQuantity(cartItemId, quantity);
            return Json(new { success = true });
        }

        // 在控制器中的某个操作方法中调用删除方法
        [HttpPost]
        public ActionResult DeleteCartItem(int cartitemid)
        {

            _client.DeletebyCartItemId(cartitemid);

   
            return Json(new { success = true });
        }

    }
}