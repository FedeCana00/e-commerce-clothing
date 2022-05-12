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
    public class Valutazione
    {
        public Valutazione() { }
        [DataMember]
        public string ID { get; set; }
        [DataMember]
        public Utente Utente { get; set; }
        [DataMember]
        public int Stelle { get; set; }
        [DataMember]
        public string Recensione { get; set; }
        [DataMember]
        public DateTime Data { get; set; }
        [DataMember]
        public Prodotto Prodotto { get; set; }
    }
}