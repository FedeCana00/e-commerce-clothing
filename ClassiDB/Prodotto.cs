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
    public class Prodotto
    {
        public Prodotto()
        {
            this.Venditore = new Utente();
        }

        [DataMember]
        public string ASIN { get; set; }
        [DataMember]
        public string NomeProdotto { get; set; }
        [DataMember]
        public int Quantita { get; set; }
        [DataMember]
        public string Taglie { get; set; }
        [DataMember]
        public string Materiale { get; set; }
        [DataMember]
        public double CostoReso { get; set; }
        [DataMember]
        public double CostoProdotto { get; set; }
        [DataMember]
        public string TempoSpedizione{ get; set; }
        [DataMember]
        public Utente Venditore { get; set; }
        [DataMember]
        public string Categoria { get; set; }
        [DataMember]
        public string DescrizioneProdotto { get; set; }
        [DataMember]
        public string ImmagineProdotto { get; set; }
    }
}
