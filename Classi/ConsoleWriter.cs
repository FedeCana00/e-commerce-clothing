using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCFServer
{
    //Oggetto usato per poche cose
    class ConsoleWriter
    {
        //METODO per scrivere a console quando viene eseguito il logout da parte dell'user (da pulsante)
        public void WriteLogout(string user)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Logout eseguito da {0}", user);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
