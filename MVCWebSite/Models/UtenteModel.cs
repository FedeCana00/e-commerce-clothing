using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using MVCWebSite.Classi;

namespace MVCWebSite.Models
{
    public class Utente
    {
        public string ID { get; set; }
        [Required]
        [Display(Name = "Nome")]
        [StringLength(20, MinimumLength = 3)]
        public string Nome { get; set; }
        [Required]
        [Display(Name = "Cognome")]
        [StringLength(20, MinimumLength = 3)]
        public string Cognome { get; set; }
        [Required]
        [Display(Name = "Data di nascita")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DataNascita { get; set; }
        public string Societa { get; set; }
        public Luogo IndirizzoSpedizione { get; set; }
        public bool Attivo { get; set; }
        public double Portafoglio { get; set; }
    }
}