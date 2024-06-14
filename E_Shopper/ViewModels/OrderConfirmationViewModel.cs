using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E_Shopper.ViewModels
{
    public class OrderConfirmationViewModel
    {
        public int OrderId { get; set; }
        public string TotalAmount { get; set; }
        public string PaymentMethod { get; set; }
        public string ShippingAddress { get; set; }
        public List<OrderItem_OrderConfirmation> OrderItem_OrderConfirmations { get; set; } // 订单项列表
    }

    public class OrderItem_OrderConfirmation
    {
        public int ProductId { get; set; }
        public string Quantity { get; set; }
        public string Price { get; set; }

        public string ProductName { get; set; }
        public string ProductPrice { get; set; }
        public string ProductImageURL { get; set; }


    }
}