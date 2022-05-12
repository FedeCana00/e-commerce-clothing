using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCWebSite.Models
{

    public class CompraVenditaModel
    {
        public CompraVenditaModel()
        {
            this.ListaProdotti = new List<ServiceReferenceServer.CompraVendita>();
        }
        public List<ServiceReferenceServer.CompraVendita> ListaProdotti { get; set; }
    }
}