using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using E_Shopper.HomeWebService;

namespace E_Shopper.Controllers
{
    public class HomeController : Controller
    {
        private readonly HomeServiceSoapClient _client;

        public HomeController()
        {

            _client = new HomeServiceSoapClient();
        }

        public ActionResult Index(string category)
        {




            var categories = _client.GetCategoriesWithCount();

            ViewBag.Categories = categories;


            List<HomeViewModel> products;

            if (!string.IsNullOrEmpty(category))
            {
                products = _client.GetProductsByCategory(category).ToList(); 
            }
            else
            {
                products = _client.GetProducts().ToList(); 
            }



            return View(products);
        }




        // 處理將商品添加到購物車的動作
        [HttpPost]
        public ActionResult AddToCart(int productId)
        {

            if(Session["UserID"] == null)
            {
                TempData["MessageType"] = "warning";
                TempData["MessageContent"] = "現在還未登入";

                return RedirectToAction("Index", "Home");

            }
            int userId = (int)Session["UserID"];

            // 檢查購物車中是否已經有該商品
            bool isProductInCart = _client.IsProductInCart(userId, productId);

            if (isProductInCart)
            {

                TempData["MessageType"] = "warning";
                TempData["MessageContent"] = "該商品已經在購物車中！";
            }
            else
            {
                int quantity = 1; 
                _client.AddToCartInDatabase(userId, productId, quantity);
                TempData["MessageType"] = "success";
                TempData["MessageContent"] = "商品已成功添加到購物車中！";
            }

            // 返回到原始的 Home 页面或任何您希望返回的地方
            return RedirectToAction("Index", "Home");
        }










    }
}