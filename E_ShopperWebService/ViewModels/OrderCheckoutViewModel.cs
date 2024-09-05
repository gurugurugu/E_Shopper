using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E_ShopperWebService.ViewModels
{
    public class OrderCheckoutViewModel
    {
        public int VCHCARTITEMID { get; set; }
        public int VCHRPRODUCTID { get; set; }
        public string VCHQUANTITY { get; set; }
        public int VCHUSERID { get; set; }
        public string VCHRNAME { get; set; }
        public string VCHRPRICE { get; set; }
        public string VCHRMAINIMAGEURL { get; set; }

    }

    public class OrderItem
    {
        public string name { get; set; }
        public string price { get; set; }
        public string quantity { get; set; }
        public string total { get; set; }
        public int productid { get; set; }


    }
}