using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using E_Shopper.Models;
using E_Shopper.ViewModels;

namespace E_Shopper.Controllers
{
    public class HomeController : Controller
    {
        private HomeModel _Model;

        public HomeController()
        {
            _Model = new HomeModel();
        }

        public ActionResult Index(string category)
        {



            var categories = _Model.GetCategoriesWithCount();

            ViewBag.Categories = categories;


            List<ProductsViewModel> products;

            if (!string.IsNullOrEmpty(category))
            {
                products = _Model.GetProductsByCategory(category); // 根据类别获取商品
            }
            else
            {
                products = _Model.GetProducts(); // 获取所有商品示例
            }



            return View(products);
        }










        // 處理將商品添加到購物車的動作
        [HttpPost]
        public ActionResult AddToCart(int productId)
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
                // 如果商品不在购物车中，添加商品到购物车
                int quantity = 1; // 默认数量为1
                _Model.AddToCartInDatabase(userId, productId, quantity);
                TempData["MessageType"] = "success";
                TempData["MessageContent"] = "商品已成功添加到購物車中！";
            }

            // 返回到原始的 Home 页面或任何您希望返回的地方
            return RedirectToAction("Index", "Home");
        }










    }
}