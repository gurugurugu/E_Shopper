using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E_ShopperWebService.ViewModels
{
    public class CartViewModel
    {

        public int VCHCARTITEMID { get; set; }
        public int VCHRPRODUCTID { get; set; }
        public string VCHQUANTITY { get; set; }

        public string VCHRNAME { get; set; }
        public string VCHRPRICE { get; set; }
        public string VCHRSTOCKQUANTITY { get; set; }
        public string VCHRMAINIMAGEURL { get; set; }

    }
}