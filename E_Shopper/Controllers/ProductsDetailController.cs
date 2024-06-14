using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using E_Shopper.Models;
using Oracle.ManagedDataAccess.Client;


namespace E_Shopper.Controllers
{
    public class ProductsDetailController : Controller
    {
        private readonly ProductsDetailModel _Model;
        public ProductsDetailController()
        {
            _Model = new ProductsDetailModel();
        }
        public ActionResult Index(int id=0)
        {
            if (id == 0)
            {
                ViewBag.ErrorMessage = "不存在该商品。";
                return View("Error");
            }
            var product = _Model.GetProductById(id);

            if (product == null)
            {
                return HttpNotFound();
            }

            return View(product);
        }


        // 處理將商品添加到購物車的動作
        [HttpPost]
        public ActionResult AddToCartAndQuantity(int productId, string quantity)
        {
            // 获取当前登录用户的ID
            int userId = (int)Session["UserID"];

            // 检查购物车中是否已有该商品
            bool isProductInCart = _Model.IsProductInCart(userId, productId);

            if (isProductInCart)
            {
                // 如果商品已经在购物车中，设置提示信息
                TempData["MessageType"] = "warning";
                TempData["MessageContent"] = "該商品已經在購物車中！";
            }
            else
            {
                _Model.AddToCartInDatabase(userId, productId, quantity);
                TempData["MessageType"] = "success";
                TempData["MessageContent"] = "商品已成功添加到購物車中！";
            }

            // 返回到原始的 Home 页面或任何您希望返回的地方
            return RedirectToAction("Index", "Home");
        }
    }
}