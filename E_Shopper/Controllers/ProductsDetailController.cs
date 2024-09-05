using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Oracle.ManagedDataAccess.Client;
using E_Shopper.ProductsDetailWebService;


namespace E_Shopper.Controllers
{
    public class ProductsDetailController : Controller
    {
        private readonly ProductsDetailServiceSoapClient _client;

        public ProductsDetailController()
        {
            _client = new ProductsDetailServiceSoapClient();

        }
        public ActionResult Index(int id=0)
        {
            if (id == 0)
            {
                ViewBag.ErrorMessage = "不存在该商品。";
                return View("Error");
            }
            var product = _client.GetProductById(id);

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

            int userId = (int)Session["UserID"];

            // 檢查購物車是否已經有該商品
            bool isProductInCart = _client.IsProductInCart(userId, productId);

            if (isProductInCart)
            {

                TempData["MessageType"] = "warning";
                TempData["MessageContent"] = "該商品已經在購物車中！";
            }
            else
            {
                _client.AddToCartInDatabase(userId, productId, quantity);
                TempData["MessageType"] = "success";
                TempData["MessageContent"] = "商品已成功添加到購物車中！";
            }

            return RedirectToAction("Index", "Home");
        }
    }
}