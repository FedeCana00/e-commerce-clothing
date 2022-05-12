using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVCWebSite.Models
{
    public class CredenzialiModel
    {
        public CredenzialiModel()
        {
            this.ListOfUsers = new List<ServiceReferenceServer.Credenziali>();
        }
        public List<ServiceReferenceServer.Credenziali> ListOfUsers { get; set; }
    }

    public class Credenziali
    {
        public Credenziali() { }
        public Credenziali(string username, string password, string email, string ruolo, Utente utente) //costruttore
        {
            this.Username = username;
            this.Password = password;
            this.Email = email;
            this.Ruolo = ruolo;
            this.Utente = utente;
        }

        [Required]
        [StringLength(20, MinimumLength = 3)]
        public string Username { get; set; }
        [Required]
        [RegularExpression(@"^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[a-zA-Z]).{8,}$")]
        public string Password { get; set; }
        [Required]
        [Display(Name = "e-mail")]
        [RegularExpression(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$")]
        public string Email { get; set; }
        public string Ruolo { get; set; }
        public Utente Utente { get; set; }
        [Required]
        [Display(Name = "Inserisci vecchia password")]
        public string OldPassword { get; set; } //utilizato solo nel caso di aggiornamento dati utente
    }
}