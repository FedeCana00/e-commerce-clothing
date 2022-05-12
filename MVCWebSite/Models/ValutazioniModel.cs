using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVCWebSite.Models
{
    public class ValutazioniModel
    {
        public ValutazioniModel()
        {
            this.ListaValutazioni = new List<ServiceReferenceServer.Valutazione>();
        }
        public List<ServiceReferenceServer.Valutazione> ListaValutazioni { get; set; }
    }
    public class Valutazioni
    {
        public string ID { get; set; }
        public Utente Utente { get; set; }
        [Required]
        public int Stelle { get; set; }
        [Required]
        [StringLength(200, MinimumLength =3)]
        public string Recensione { get; set; }
        public DateTime Data { get; set; }
        public Prodotto Prodotto { get; set; }


    }
}