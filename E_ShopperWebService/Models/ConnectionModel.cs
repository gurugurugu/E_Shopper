﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace E_ShopperWebService.Models
{
    public class ConnectionModel
    {
        public string DBTEST3con()
        {
            //System.Configuration.ConfigurationManager.ConnectionStrings["OracleDbContext"].ConnectionString
            return "Data Source =; User ID = ; Password =;";
        }
    }
}