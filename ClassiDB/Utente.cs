using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace WCFServer
{
    [Serializable]
    [DataContract]
    public class Utente
    {
        public Utente()
        {
            this.IndirizzoSpedizione = new Luogo();
        }

        public Utente(string cod, string n, string c, DateTime d, string e, Luogo p, bool u, double s)
        {
            this.ID = cod;
            this.Nome = n;
            this.Cognome = c;
            this.DataNascita = d;
            this.Societa = e;
            this.IndirizzoSpedizione = p;
            this.Attivo = u;
            this.Portafoglio = s;
        }

        [DataMember]
        public string ID { get; set; }
        [DataMember]
        public string Nome { get; set; }
        [DataMember]
        public string Cognome { get; set; }
        [DataMember]
        public DateTime DataNascita { get; set; }
        [DataMember]
        public string Societa { get; set; }
        [DataMember]
        public Luogo IndirizzoSpedizione { get; set; }
        [DataMember]
        public bool Attivo { get; set; }
        [DataMember]
        public double Portafoglio { get; set; }
    }
}
