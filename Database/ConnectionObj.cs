using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace e_commerce2._0.Database
{
    //Thread Safety Singleton using Double-Check Locking
    public sealed class ConnectionObj
    {
        //Se true, l'applicazione può gestire la funzionalità MARS (Multiple Active Result Set). 
        //Se false, l'applicazione deve elaborare o annullare tutti i set di risultati di un batch prima di poter eseguire qualsiasi altro batch nella connessione.
        private static readonly string connetionString = @"Server=(LocalDb)\e-commerce;Database=e-commerce;Integrated Security=SSPI;MultipleActiveResultSets=False";
        private static readonly object _lock = new object ();  
        private static ConnectionObj _instance = null; 
        public SqlConnection Connection { get; set; }
        private ConnectionObj() { }
        //Creo l'istanza di oggetto però assicurandomi che solo un thread istanzi l'oggetto se più thread
        //vi accedono in contemporanea al momento dell'istanza in cui è ancora null
        public static ConnectionObj Instance
        {
            get
            {
                if (_instance == null) //primo controllo di istanza
                {
                    lock (_lock)
                    {
                        if (_instance == null) //secondo controllo di istanza
                        {
                            _instance = new ConnectionObj
                            {
                                Connection = new SqlConnection(connetionString)
                            };
                            _instance.Connection.Open();
                        }
                    }
                }
                return _instance;
            }
        }
    }
}
