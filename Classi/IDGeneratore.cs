using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCFServer
{
    class IDGeneratore
    {
        private static readonly string[] LETTERE = {"a","b","c","d","e","f","g","h","i","j","k","l","m","n","o","p","q","r","s","t","u","v","w","x","y","z"};
        private string id;
        public string IDUtente()
        {
            Random rand = new Random();
            bool returnOK = true;
            int randSize, randLetter, randNumber;
            //eseguo il ciclo finchè non ottengo un ID non già presente
            id = "";
            while (returnOK)
            {
                id = "";
                //genero due lettere
                for (int l = 0; l < 2; l++)
                {
                    randSize = rand.Next(0, 2);
                    switch (randSize)
                    {
                        //Lettera minuscola
                        case 0:
                            randLetter = rand.Next(0, LETTERE.Length);
                            id += LETTERE[randLetter];
                            break;
                        //Lettera maiuscola
                        case 1:
                            randLetter = rand.Next(0, LETTERE.Length);
                            id += LETTERE[randLetter].ToUpper();
                            break;
                        default:
                            Console.WriteLine("Errore in fase di generazione dimensione lettere!");
                            return "Error";
                    }
                }
                //genero 3 numeri
                for (int n = 0; n < 3; n++)
                {
                    randNumber = rand.Next(0, 10);
                    id += randNumber.ToString();
                }
                returnOK = Program.Db.CheckIfExistIdUtente(id);
            }
            return id;
        }

        public string ASINProdotto()
        {
            Random rand = new Random();
            bool returnOK = true;
            int randSize, randLetter, randNumber;
            //eseguo il ciclo finchè non ottengo un ID non già presente
            while (returnOK)
            {
                id = "";
                //genero tre lettere
                for (int l = 0; l < 3; l++)
                {
                    randSize = rand.Next(0, 2);
                    switch (randSize)
                    {
                        //Lettera minuscola
                        case 0:
                            randLetter = rand.Next(0, LETTERE.Length);
                            id += LETTERE[randLetter];
                            break;
                        //Lettera maiuscola
                        case 1:
                            randLetter = rand.Next(0, LETTERE.Length);
                            id += LETTERE[randLetter].ToUpper();
                            break;
                        default:
                            Console.WriteLine("Errore in fase di generazione dimensione lettere!");
                            return "Error";
                    }
                }
                //genero 2 numeri
                for (int n = 0; n < 2; n++)
                {
                    randNumber = rand.Next(0, 10);
                    id += randNumber.ToString();
                }
                returnOK = Program.Db.CheckIfExistASINProdotto(id);
            }
            return id;
        }

        public string[] IDCompraVendi(int i)
        {
            try
            {
                Random rand = new Random();
                bool returnOK = true;
                int randSize, randLetter, randNumber;
                string[] outputId = new string[i];
                for (int j = 0; j < i; j++)
                {
                    //eseguo il ciclo finchè non ottengo un ID non già presente
                    returnOK = true;
                    while (returnOK)
                    {
                        id = "";
                        //genero 4 lettere
                        for (int l = 0; l < 4; l++)
                        {
                            randSize = rand.Next(0, 2);
                            switch (randSize)
                            {
                                //Lettera minuscola
                                case 0:
                                    randLetter = rand.Next(0, LETTERE.Length);
                                    id += LETTERE[randLetter];
                                    break;
                                //Lettera maiuscola
                                case 1:
                                    randLetter = rand.Next(0, LETTERE.Length);
                                    id += LETTERE[randLetter].ToUpper();
                                    break;
                                default:
                                    Console.WriteLine("Errore in fase di generazione dimensione lettere!");
                                    return null;
                            }
                        }
                        //genero 4 numeri
                        for (int n = 0; n < 4; n++)
                        {
                            randNumber = rand.Next(0, 10);
                            id += randNumber.ToString();
                        }
                        returnOK = Program.Db.CheckIfExistIDCompraVendi(id) && !outputId.Contains(id);
                    }
                    outputId[j] = id;
                }
                //Console.WriteLine("Dimensione array ID in ID: {0}", outputId.Length); //test debug
                return outputId;
            } catch(Exception ex)
            {
                var st = new StackTrace();
                var sf = st.GetFrame(1);
                Console.WriteLine("{1} => Commit Exception Type : {0}", ex.GetType(), sf.GetMethod());
                Console.WriteLine("  Message: {0}", ex.Message);
            }
            return null;
        }

        public string IDValutazione()
        {
            Random rand = new Random();
            bool returnOK = true;
            int randSize, randLetter, randNumber;
            //eseguo il ciclo finchè non ottengo un ID non già presente
            id = "";
            while (returnOK)
            {
                id = "";
                //genero 3 lettere
                for (int l = 0; l < 3; l++)
                {
                    randSize = rand.Next(0, 2);
                    switch (randSize)
                    {
                        //Lettera minuscola
                        case 0:
                            randLetter = rand.Next(0, LETTERE.Length);
                            id += LETTERE[randLetter];
                            break;
                        //Lettera maiuscola
                        case 1:
                            randLetter = rand.Next(0, LETTERE.Length);
                            id += LETTERE[randLetter].ToUpper();
                            break;
                        default:
                            Console.WriteLine("Errore in fase di generazione dimensione lettere!");
                            return "Error";
                    }
                }
                //genero 3 numeri
                for (int n = 0; n < 3; n++)
                {
                    randNumber = rand.Next(0, 10);
                    id += randNumber.ToString();
                }
                returnOK = Program.Db.CheckIfExistIdValutazione(id);
            }
            return id;
        }
    }
}
