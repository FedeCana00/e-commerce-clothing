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
    public class CompraVendita
    {
        [DataMember]
        public string ID { get; set; }
        [DataMember]
        public DateTime Data { get; set; }
        [DataMember]
        public Prodotto Prodotto { get; set; }
        [DataMember]
        public Utente Utente { get; set; }
        [DataMember]
        public char Tipo { get; set; }
        [DataMember]
        public int Qt { get; set; }

    }
}
