using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace WCFServer
{
    class Program
    {
        public static DBManager Db { get; set; }
        public static IDGeneratore IdGen { get; set; }
        public static ConsoleWriter ConsoleManager { get; set; }
        static void Main(string[] args)
        {
            try
            {
                //imposto il titolo della finestra
                Console.Title = "e-commerce Federico Canali & Francesco Marchi";
                //Intestazione grafica per la console lato server
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Progetto e-commerce Federico Canali & Francesco Marchi");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Benvenuti nel lato server! Qui potrete vedere le varie operazione che vengono eseguite per soddisfare richieste dell' utente.");
                Console.ForegroundColor = ConsoleColor.White;
                //**************************************************************************************************************************************************

                ServiceHost svcServer = new ServiceHost(typeof(ServiceServer));
                Db = new DBManager(); //istanzio il mio DBManager
                IdGen = new IDGeneratore(); //istanzio il mio Generatore di ID
                ConsoleManager = new ConsoleWriter(); //istanzio il manager della mia console nei casi in cui non posso avere feedback
                //Console.WriteLine(IdGen.IDUtente());
                svcServer.Open();

                //database.OpenConnection(); //test

                Console.WriteLine("Servizio WCF del Server online!");

                Console.ReadLine(); //per mantenerlo in esecuzione fino alla pressione di un pulsante
                svcServer.Close();
                Console.WriteLine("Servizio WCF del Server interrotto!");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Errore => {0}", ex.ToString());
                //mantiente aperta la console in caso di eccezione
                Console.ReadLine();
            }
        }
    }
}