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
    public class Carrello
    {
        [DataMember]
        public Utente Utente { get; set; }
        [DataMember]
        public Prodotto Prodotto { get; set; }
        [DataMember]
        public int Quantita { get; set; }
        [DataMember]
        public double CostoTot { get; set; }

    }
}
