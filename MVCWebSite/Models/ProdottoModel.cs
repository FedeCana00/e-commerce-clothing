using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVCWebSite.Models
{
    public class ProdottoModel
    {
        public ProdottoModel()
        {
            this.ListaProdotti = new List<ServiceReferenceServer.Prodotto>();
        }
        public List<ServiceReferenceServer.Prodotto> ListaProdotti { get; set; }
    }

    public class Prodotto
    {
        public string ASIN { get; set; }
        [Required]
        [Display(Name = "Nome Prodotto")]
        [StringLength(50, MinimumLength = 3)]
        public string NomeProdotto { get; set; }
        [Required]
        [Display(Name = "Quantità")]
        [Range(1, 10000)] //(min, max)
        public int Quantita { get; set; }
        [Required]
        [Display(Name = "Taglia")]
        [StringLength(3, MinimumLength = 1)]
        public string Taglie { get; set; }
        [Required]
        [Display(Name = "Materiale")]
        [StringLength(40, MinimumLength = 3)]
        public string Materiale { get; set; }
        [Required]
        [Display(Name = "Costo per il reso")]
        [Range(0,30)]
        public double CostoReso { get; set; }
        [Required]
        [Display(Name = "Prezzo Prodotto")]
        [Range(0.20,10000)]
        public double CostoProdotto { get; set; }
        [Required]
        [Display(Name = "Tempo Spedizione")]
        [StringLength(10, MinimumLength = 1)]
        public string TempoSpedizione { get; set; }
        public Utente Venditore { get; set; }
        [Required]
        [Display(Name = "Categoria")]
        public string Categoria { get; set; }
        [Required]
        [Display(Name = "Descrizione")]
        [StringLength(150, MinimumLength = 3)]
        public string DescrizioneProdotto { get; set; }
        [DataType(DataType.Upload)]    
        [Display(Name = "Upload File")]
        public string ImmagineProdotto { get; set; }
    }
}