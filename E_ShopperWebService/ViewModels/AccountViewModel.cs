using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E_ShopperWebService.ViewModels
{
    public class AccountViewModel
    {
        public int VCHUSERID { get; set; }

        public string VCHUSERNAME { get; set; }
        public string VCHPASSWORD { get; set; }
        public string VCHEMAIL { get; set; }
        public string VCHROLE { get; set; }
        public string VCHACTIVE { get; set; }
        public DateTime DCREATEDATE { get; set; }
    }
}