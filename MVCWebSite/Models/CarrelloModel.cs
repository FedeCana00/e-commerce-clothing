using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCWebSite.Models
{
    public class CarrelloModel
    {
        public CarrelloModel()
        {
            this.ListaElementiCarrello = new List<ServiceReferenceServer.Carrello>();
        }
        public List<ServiceReferenceServer.Carrello> ListaElementiCarrello { get; set; }
    }
}