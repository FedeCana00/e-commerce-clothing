using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace WCFServer
{
    [Serializable]
    [DataContract]
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

        [DataMember]
        public virtual string Username { get; set; }
        [DataMember]
        public virtual string Password { get; set; }
        [DataMember]
        public virtual string Email { get; set; }
        [DataMember]
        public virtual string Ruolo { get; set; }
        [DataMember]
        public virtual Utente Utente { get; set; }

    }
}
