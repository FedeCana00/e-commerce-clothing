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
    public class Luogo
    {
        public Luogo()
        {

        }

        Luogo(string via, string citta, string civico, string nazione)
        {
            this.Via = via;
            this.Citta = citta;
            this.Civico = civico;
            this.Nazione = nazione;
        }
        [DataMember]
        public string Via { get; set; }
        [DataMember]
        public string Citta { get; set; }
        [DataMember]
        public string Civico { get; set; }
        [DataMember]
        public string Nazione { get; set; }
    }
}
