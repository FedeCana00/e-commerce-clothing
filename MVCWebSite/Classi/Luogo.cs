using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVCWebSite.Classi
{
    public class Luogo
    {
        public Luogo(){ }
        [Required]
        [Display(Name = "Via")]
        [StringLength(20, MinimumLength = 3)]
        public string Via { get; set; }
        [Required]
        [Display(Name = "Città")]
        [StringLength(20, MinimumLength = 3)]
        public string Citta { get; set; }
        [Required]
        [Display(Name = "Civico")]
        [RegularExpression(@"^[0-9]{1,3}([a-z]?)$")]
        public string Civico { get; set; }
        [Required]
        [Display(Name = "Nazione")]
        [StringLength(20, MinimumLength = 2)]
        public string Nazione { get; set; }

    }
}