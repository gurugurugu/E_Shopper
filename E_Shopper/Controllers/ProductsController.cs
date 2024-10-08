﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using E_Shopper.ProductsWebService;

namespace E_Shopper.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ProductsServiceSoapClient _client;

        public ProductsController()
        {
            _client = new ProductsServiceSoapClient();

        }
        // GET: Products
        public ActionResult Index()
        {
            ProductsViewModel[] productsArray = _client.GetProducts();

            // 將數組轉換為 List<ProductsViewModel>
            List<ProductsViewModel> products = productsArray.ToList();

            return View(products);
        }


        // GET: Product/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Product/Create
        [HttpPost]


        public ActionResult Create(ProductsViewModel product, HttpPostedFileBase MainImageFile)
        {

            if (MainImageFile != null && MainImageFile.ContentLength > 0)
            {
                // 生成唯一文件名以避免冲突
                var fileName = Path.GetFileName(MainImageFile.FileName);
                var path = Path.Combine(Server.MapPath("~/images/Products/"), fileName);
                MainImageFile.SaveAs(path);

                // 将文件路径保存到产品的VCHRMAINIMAGEURL字段中
                product.VCHRMAINIMAGEURL = "/images/Products/" + fileName;
            }


            if (ModelState.IsValid)
            {
                _client.AddProduct(product);

                return RedirectToAction("Index");
            }
            return View(product);
        }



        [HttpPost]
        public ActionResult UpdateProductStatus(string productId, string status)
        {
            _client.UpdateStatus(productId, status);

            return Json(new { success = true });



        }

        [HttpPost]
        public ActionResult UpdateProduct (ProductsViewModel product, HttpPostedFileBase MainImageFile)
        {
            try
            {
                if (MainImageFile != null && MainImageFile.ContentLength > 0)
                {
                    // 生成唯一文件名以避免冲突
                    var fileName = Path.GetFileName(MainImageFile.FileName);
                    var path = Path.Combine(Server.MapPath("~/images/Products/"), fileName);
                    MainImageFile.SaveAs(path);

                    // 将文件路径保存到产品的VCHRMAINIMAGEURL字段中
                    product.VCHRMAINIMAGEURL = "/images/Products/" + fileName;

                    _client.UpdateProductImage(product);



                }

                // 更新产品信息
                _client.UpdateProduct(product);

                return Json(new { success = true, message = "Product updated successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }




        [HttpPost]
        public ActionResult DeleteProduct(int productId)
        {
            try
            {
                // 根据 productId 删除商品的逻辑，这里假设使用 ProductService 来处理删除操作
                _client.DeleteProduct(productId);

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
    
}
