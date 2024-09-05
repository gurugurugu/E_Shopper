using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E_ShopperWebService.ViewModels
{
    public class ProductsDetailViewModel
    {
        public int VCHRPRODUCTID { get; set; }
        public string VCHRNAME { get; set; }
        public string VCHRDESCRIPTION { get; set; }
        public string VCHRCATEGORY { get; set; }
        public string VCHRBRAND { get; set; }
        public string VCHRPRICE { get; set; }
        public string VCHRDISCOUNTPRICE { get; set; }
        public string VCHRSTOCKQUANTITY { get; set; }
        public string VCHRCOLOR { get; set; }
        public string VCHRSIZE { get; set; }
        public string VCHRMATERIAL { get; set; }
        public string VCHRMAINIMAGEURL { get; set; }
        public string VCHRSTATUS { get; set; }
        public string VCHRTAGS { get; set; }
        public DateTime VCHRCREATEDAT { get; set; }
        public DateTime VCHRUPDATEDAT { get; set; }
        public string VCHRAVERAGERATING { get; set; }
        public string VCHRREVIEWCOUNT { get; set; }
    }
}