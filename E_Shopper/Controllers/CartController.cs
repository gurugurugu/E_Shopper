using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using E_Shopper.Models;

namespace E_Shopper.Controllers
{
    public class CartController : Controller
    {
        // GET: Cart
        private readonly CartModel _Model;

        public CartController()
        {
            _Model = new CartModel();
        }

        public ActionResult Index()
        {
            int userId = (int)Session["UserID"];
            var cartItems = _Model.GetCartbyUserId(userId);
            return View(cartItems);
        }

        [HttpPost]
        public ActionResult UpdateCartItem(int cartItemId, string quantity)
        {
            // 更新购物车项数量到数据库
            _Model.UpdateCartItemQuantity(cartItemId, quantity);
            return Json(new { success = true });
        }


        // 在控制器中的某个操作方法中调用删除方法
        [HttpPost]
        public ActionResult DeleteCartItem(int cartitemid)
        {

            _Model.DeletebyCartItemId(cartitemid);

   
            return Json(new { success = true });
        }

    }
}