using e_commerce2._0.Database;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WCFServer
{
    class DBManager
    {
        public static ConnectionObj connessione; //singleton per la connessione al db
        public DBManager()
        {
            connessione = ConnectionObj.Instance;
            lock (connessione)
            {
                //lock sull'oggetto
            }
        }

        public bool VerifyUserIdentity(Credenziali user)
        {
            String queryLogin = $"SELECT [Username], [Password] FROM [dbo].[Credenziali] WHERE [Username] = '{user.Username}' AND [Password] = '{user.Password}'";
            try
            {
                using SqlCommand command = new SqlCommand(queryLogin, connessione.Connection);
                using (SqlDataReader exe = command.ExecuteReader())
                {
                    //eseguo la query      
                    if (exe.Read())
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("Accesso effettuato user: {0} ", user.Username);
                        Console.ForegroundColor = ConsoleColor.White;
                        exe.Close();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace();
                var sf = st.GetFrame(1);
                Console.WriteLine("{1} => Commit Exception Type : {0}", ex.GetType(), sf.GetMethod());
                Console.WriteLine("  Message: {0}", ex.Message);
            }
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Accesso negato a user: {0}", user.Username);
            Console.ForegroundColor = ConsoleColor.White;
            return false;
        }

        //METODO per verificare la corretta immisione della vecchia password nel DB
        public bool CheckOldPassword(string ID, string passw)
        {
            String query = $"SELECT * FROM [dbo].[Credenziali] WHERE [Utente] = '{ID}' AND [Password] = '{passw}'";
            try
            {
                using SqlCommand command = new SqlCommand(query, connessione.Connection);
                using (SqlDataReader exe = command.ExecuteReader())
                {
                    //eseguo la query      
                    if (exe.Read())
                    {
                        exe.Close();
                        return true; //password vecchia riconosciuta
                    }
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace();
                var sf = st.GetFrame(1);
                Console.WriteLine("{1} => Commit Exception Type : {0}", ex.GetType(), sf.GetMethod());
                Console.WriteLine("  Message: {0}", ex.Message);
            }
            Console.WriteLine("Inserimento vecchia password errato da user ID: {0}", ID);
            return false;
        }

        //METODO per ottenere un il ruolo di un user
        public string GetRoleOfUser(string ID)
        {
            String query = $"SELECT [Ruolo] FROM [dbo].[Credenziali] WHERE [Utente] = '{ID}'";
            try
            {
                using SqlCommand command = new SqlCommand(query, connessione.Connection);
                using (SqlDataReader exe = command.ExecuteReader())
                {
                    //eseguo la query      
                    if (exe.Read())
                    {
                        return exe.GetString(0);
                    }
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace();
                var sf = st.GetFrame(1);
                Console.WriteLine("{1} => Commit Exception Type : {0}", ex.GetType(), sf.GetMethod());
                Console.WriteLine("  Message: {0}", ex.Message);
            }
            return null;
        }

        //METODO per ottenere lo stato di un user => se attivo o disattivo
        public bool GetUserState(string ID)
        {
            String query = $"SELECT [Attivo] FROM [dbo].[Utente] WHERE [ID] = '{ID}'";
            try
            {
                using SqlCommand command = new SqlCommand(query, connessione.Connection);
                using (SqlDataReader exe = command.ExecuteReader())
                {
                    //eseguo la query      
                    if (exe.Read())
                    {
                        return exe.GetBoolean(0);
                    }
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace();
                var sf = st.GetFrame(1);
                Console.WriteLine("{1} => Commit Exception Type : {0}", ex.GetType(), sf.GetMethod());
                Console.WriteLine("  Message: {0}", ex.Message);
            }
            return false;
        }

        //METODO per ottenere una lista di utenti registrati all'e-commerce
        public List<Credenziali> GetAllUsers()
        {
            List<Credenziali> listUsers = new List<Credenziali>();
            //ottengo alcuni dati dell'utente ma non quelli dell'amministratore
            String query = $"SELECT [ID], [Username], [Nome],[Cognome],[Nazione],[Societa],[Attivo],[email] FROM [dbo].[Credenziali],[dbo].[Utente] WHERE [ID] = [Utente] AND [Ruolo] <> 'A' ";
            try
            {
                Credenziali user;
                using SqlCommand command = new SqlCommand(query, connessione.Connection);
                using (SqlDataReader exe = command.ExecuteReader())
                {
                    //eseguo la query      
                    while (exe.Read())
                    {
                        user = new Credenziali();
                        user.Utente = new Utente
                        {
                            ID = exe.GetString(0),
                            Nome = exe.GetString(2),
                            Cognome = exe.GetString(3),
                            IndirizzoSpedizione = new Luogo
                            {
                                Nazione = exe.GetString(4)
                            },
                            Societa = exe.GetString(5),
                            Attivo = exe.GetBoolean(6)
                        };
                        user.Username = exe.GetString(1);
                        user.Email = exe.GetString(7);
                        listUsers.Add(user); //aggiungo l'utente alla lista appena creata
                    }
                    exe.Close();
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace();
                var sf = st.GetFrame(1);
                Console.WriteLine("{1} => Commit Exception Type : {0}", ex.GetType(), sf.GetMethod());
                Console.WriteLine("  Message: {0}", ex.Message);
            }
            return listUsers;
        }

        //METODO per ottenere tutte le informazioni di un profile utente
        public Credenziali GetInformationUser(string ID)
        {
            String query = $"SELECT [Username],[Password],[email],[Nome],[Cognome],[DataNasc],[Via],[Civico],[Citta],[Nazione] FROM [dbo].[Utente],[dbo].[Credenziali] WHERE [Utente] = [ID] AND [ID] = '{ID}'";
            Credenziali user = new Credenziali();
            try
            {
                using SqlCommand command = new SqlCommand(query, connessione.Connection);
                using (SqlDataReader exe = command.ExecuteReader())
                {
                    //eseguo la query      
                    if (exe.Read())
                    {
                        user.Utente = new Utente
                        {
                            ID = ID,
                            Nome = exe.GetString(3),
                            Cognome = exe.GetString(4),
                            DataNascita = exe.GetDateTime(5),
                            IndirizzoSpedizione = new Luogo
                            {
                                Via = exe.GetString(6),
                                Civico = exe.GetString(7),
                                Citta = exe.GetString(8),
                                Nazione = exe.GetString(9),
                            }

                        };
                        user.Username = exe.GetString(0);
                        user.Password = exe.GetString(1);
                        user.Email = exe.GetString(2);
                    }
                    exe.Close();
                }
                return user;
            }
            catch (Exception ex)
            {
                var st = new StackTrace();
                var sf = st.GetFrame(1);
                Console.WriteLine("{1} => Commit Exception Type : {0}", ex.GetType(), sf.GetMethod());
                Console.WriteLine("  Message: {0}", ex.Message);
            }
            return user;
        }

        //METODO per aggiornare le informazioni di un utente
        public bool UpdateInformationUser(Credenziali userinfo)
        {
            using (SqlCommand command = connessione.Connection.CreateCommand())
            {
                SqlTransaction transaction;

                // Start a local transaction.
                transaction = connessione.Connection.BeginTransaction("Transation For userInfo update");

                // Must assign both transaction object and connection
                // to Command object for a pending local transaction
                command.Connection = connessione.Connection;
                command.Transaction = transaction;

                try
                {

                    string dateNascd = userinfo.Utente.DataNascita.Year + "-" + userinfo.Utente.DataNascita.Month + "-" + userinfo.Utente.DataNascita.Day;

                    command.CommandText = //ricordarsi che la data va passata con DATETIME e la variabile .Date !
                        $"UPDATE [dbo].[Utente] SET [ID] = '{userinfo.Utente.ID}',[Nome] = '{userinfo.Utente.Nome}',[Cognome] = '{userinfo.Utente.Cognome}',[DataNasc] = '{dateNascd}',[Via] = '{userinfo.Utente.IndirizzoSpedizione.Via}',[Civico] = '{userinfo.Utente.IndirizzoSpedizione.Civico}',[Citta] = '{userinfo.Utente.IndirizzoSpedizione.Citta}',[Nazione] = '{userinfo.Utente.IndirizzoSpedizione.Nazione}' WHERE [ID] = '{userinfo.Utente.ID}'";
                    command.ExecuteNonQuery();
                    command.CommandText =
                        $"UPDATE [dbo].[Credenziali] SET [Username] = '{userinfo.Username}',[Password] = '{userinfo.Password}',[Email] = '{userinfo.Email}' WHERE [Utente] = '{userinfo.Utente.ID}'";
                    command.ExecuteNonQuery();

                    // Attempt to commit the transaction.
                    transaction.Commit();
                    Console.WriteLine("Aggiornamento informazioni user {0}", userinfo.Utente.ID);
                    return true;
                }
                catch (Exception ex)
                {
                    var st = new StackTrace();
                    var sf = st.GetFrame(1);
                    Console.WriteLine("{1} => Commit Exception Type : {0}", ex.GetType(), sf.GetMethod());
                    Console.WriteLine("  Message: {0}", ex.Message);
                    // Attempt to roll back the transaction.
                    try
                    {
                        transaction.Rollback();
                        return false;
                    }
                    catch (Exception ex2)
                    {
                        // This catch block will handle any errors that may have occurred
                        // on the server that would cause the rollback to fail, such as
                        // a closed connection.
                        var st2 = new StackTrace();
                        var sf2 = st.GetFrame(1);
                        Console.WriteLine("{1} => Commit Exception Type : {0}", ex2.GetType(), sf2.GetMethod());
                        Console.WriteLine("  Message: {0}", ex2.Message);
                        return false;
                    }
                }
            }
        }

        //METODO che abilita o disabilita un utente da AMMINISTRATORE
        public bool EnableDisableUser(Utente user)
        {
            using (SqlCommand command = connessione.Connection.CreateCommand())
            {
                try
                {
                    string consoleMessage;
                    command.CommandText =
                        $"UPDATE [dbo].[Utente] SET [Attivo] = '{user.Attivo}' WHERE [ID] = '{user.ID}'";
                    //Console.WriteLine(command.CommandText); //test debug
                    command.ExecuteNonQuery();
                    if (user.Attivo)
                    {
                        consoleMessage = "User " + user.ID + " abilitato!";
                    } else
                    {
                        consoleMessage = "User " + user.ID + " disabilitato!";
                    }
                    Console.WriteLine(consoleMessage);
                    return true;
                }
                catch (Exception ex)
                {
                    var st = new StackTrace();
                    var sf = st.GetFrame(1);
                    Console.WriteLine("{1} => Commit Exception Type : {0}", ex.GetType(), sf.GetMethod());
                    Console.WriteLine("  Message: {0}", ex.Message);
                }
                return false;
            }
        }

        public string GetIDActiveUser(Credenziali user)
        {
            String queryLogin = $"SELECT [Utente] FROM [dbo].[Credenziali] WHERE [Username] = '{user.Username}' AND [Password] = '{user.Password}'";
            try
            {
                using SqlCommand command = new SqlCommand(queryLogin, connessione.Connection);
                using (SqlDataReader exe = command.ExecuteReader())
                {
                    //eseguo la query      
                    if (exe.Read())
                    {
                        return exe.GetString(0); //restituisco l'ID del utente attualmente attivo
                    }
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace();
                var sf = st.GetFrame(1);
                Console.WriteLine("{1} => Commit Exception Type : {0}", ex.GetType(), sf.GetMethod());
                Console.WriteLine("  Message: {0}", ex.Message);
            }
            return "none";
        }

        //METODO che aggiunge un nuovo user nel db dopo registrazione
        public bool AddUserWithTransaction(Credenziali userInfo)
        {

            //controllo che i dati del nuovo utente non siano già presenti nel DB
            if (CheckIfEmailUserExist(userInfo))
                return false;

            using (SqlCommand command = connessione.Connection.CreateCommand())
            {
                string id = Program.IdGen.IDUtente();
                SqlTransaction transaction;

                // Start a local transaction.
                transaction = connessione.Connection.BeginTransaction("Transation For SignUp");

                // Must assign both transaction object and connection
                // to Command object for a pending local transaction
                command.Connection = connessione.Connection;
                command.Transaction = transaction;

                try
                {
                    if (id == "Error") //controllo che la generazione del ID non mi dia errori
                        return false;

                    string dateNascd = userInfo.Utente.DataNascita.Year + "-" + userInfo.Utente.DataNascita.Month + "-" + userInfo.Utente.DataNascita.Day;

                    command.CommandText = //ricordarsi che la data va passata con DATETIME e la variabile .Date !
                        $"INSERT INTO [dbo].[Utente] ([ID],[Nome],[Cognome],[DataNasc],[Societa],[Via],[Civico],[Citta],[Nazione],[Attivo],[Portafoglio]) VALUES ('{id}','{userInfo.Utente.Nome}','{userInfo.Utente.Cognome}','{dateNascd}','{userInfo.Utente.Societa}','{userInfo.Utente.IndirizzoSpedizione.Via}','{userInfo.Utente.IndirizzoSpedizione.Civico}','{userInfo.Utente.IndirizzoSpedizione.Citta}','{userInfo.Utente.IndirizzoSpedizione.Nazione}','{userInfo.Utente.Attivo}','{userInfo.Utente.Portafoglio}')";
                    command.ExecuteNonQuery();
                    command.CommandText =
                        $"INSERT INTO [dbo].[Credenziali] ([Username],[Password],[Email],[Ruolo],[Utente]) VALUES ('{userInfo.Username}','{userInfo.Password}', '{userInfo.Email}', '{userInfo.Ruolo}', '{id}')";
                    command.ExecuteNonQuery();

                    // Attempt to commit the transaction.
                    transaction.Commit();
                    Console.WriteLine("Inserimento avvenuto correttamente nel DB! Nuovo user {0}", userInfo.Username);
                    return true;
                }
                catch (Exception ex)
                {
                    var st = new StackTrace();
                    var sf = st.GetFrame(1);
                    Console.WriteLine("{1} => Commit Exception Type : {0}", ex.GetType(), sf.GetMethod());
                    Console.WriteLine("  Message: {0}", ex.Message);
                    // Attempt to roll back the transaction.
                    try
                    {
                        transaction.Rollback();
                        return false;
                    }
                    catch (Exception ex2)
                    {
                        // This catch block will handle any errors that may have occurred
                        // on the server that would cause the rollback to fail, such as
                        // a closed connection.
                        var st2 = new StackTrace();
                        var sf2 = st.GetFrame(1);
                        Console.WriteLine("{1} => Commit Exception Type : {0}", ex2.GetType(), sf2.GetMethod());
                        Console.WriteLine("  Message: {0}", ex2.Message);
                        return false;
                    }
                }
            }
        }

        //METODO per ottenere tutti gli elementi del carrello di un utente
        public List<Carrello> GetAllElementOfCarrello(Credenziali filtroRicerca)
        {
            List<Carrello> lista = new List<Carrello>();
            Carrello carrelloAppoggio;
            String queryLogin = $"SELECT ASIN,NomeProdotto,C.Qt,C.CostoTOT,Taglie,Materiale,CostoReso,CostoProdotto,TempoSpedizione,Societa,Categoria,DescProdotto,Venditore FROM Carrello as C,Prodotto,Utente WHERE ID = '{filtroRicerca.Utente.ID}' AND C.Prodotto = ASIN AND C.Utente = ID";
            try
            {
                using (SqlCommand command = new SqlCommand(queryLogin, connessione.Connection))
                {
                    using (SqlDataReader exe = command.ExecuteReader())
                    { //eseguo la query      
                        while (exe.Read())
                        {
                            carrelloAppoggio = new Carrello();
                            carrelloAppoggio.Prodotto = new Prodotto()
                            {
                                ASIN = exe.GetString(0),
                                NomeProdotto = exe.GetString(1),
                                Taglie = exe.GetString(4),
                                Materiale = exe.GetString(5),
                                CostoReso = exe.GetDouble(6),
                                CostoProdotto = exe.GetDouble(7),
                                TempoSpedizione = exe.GetString(8),
                                Categoria = exe.GetString(10),
                                DescrizioneProdotto = exe.GetString(11),
                                Venditore = new Utente
                                {
                                    ID = exe.GetString(12)
                                }
                            };
                            carrelloAppoggio.Utente = new Utente()
                            {
                                Societa = exe.GetString(9)
                            };
                            carrelloAppoggio.Quantita = exe.GetInt32(2);
                            //GetFloat con un type Float da errore di cast utilizzare type Double e GetDouble
                            carrelloAppoggio.CostoTot = exe.GetDouble(3); 
                            //carrelloAppoggio.Prodotto.Immagine = exe.GetString(12); //da aggiungere
                            lista.Add(carrelloAppoggio);

                            //Console.WriteLine(carrelloAppoggio.Prodotto.CostoProdotto);
                        }
                        exe.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace();
                var sf = st.GetFrame(1);
                Console.WriteLine("{1} => Commit Exception Type : {0}", ex.GetType(), sf.GetMethod());
                Console.WriteLine("  Message: {0}", ex.Message);
            }
            return lista;
        }
        
        //METODO che aggiunge un elemento al carrello del utente
        public bool AddCartProduct(Carrello carrello)
        {
            using (SqlCommand command = connessione.Connection.CreateCommand())
            {
                try
                {
                    //per passargli un numero con separatore '.' anzichè ',' => crea problemi altrimenti
                    string specifier;
                    System.Globalization.CultureInfo culture;
                    // Use standard numeric format specifiers.
                    specifier = "G";
                    culture = System.Globalization.CultureInfo.CreateSpecificCulture("eu-ES");
                    string costoTOT = carrello.CostoTot.ToString(specifier, System.Globalization.CultureInfo.InvariantCulture);
                    // Displays:    16325.62901 => anizhè 16323,62901
                    command.CommandText =
                        $"INSERT INTO [dbo].[Carrello] ([Utente],[Prodotto],[Qt],[CostoTOT]) VALUES ('{carrello.Utente.ID}','{carrello.Prodotto.ASIN}' ,'{carrello.Quantita}' , {costoTOT})";
                    command.ExecuteNonQuery();

                    Console.WriteLine("Nuovo prodotto nel carrello di {0} => ASIN: {1} Qt. {2}", carrello.Utente.ID, carrello.Prodotto.ASIN, carrello.Quantita);
                    return true;
                }
                catch (Exception ex)
                {
                    var st = new StackTrace();
                    var sf = st.GetFrame(1);
                    Console.WriteLine("{1} => Commit Exception Type : {0}", ex.GetType(), sf.GetMethod());
                    Console.WriteLine("  Message: {0}", ex.Message);
                }
                return false;
            }
        }

        //METODO per eliminare un prodotto dal carrello
        public bool DeleteProductFromCart(Carrello carrello)
        {
            using (SqlCommand command = connessione.Connection.CreateCommand())
            {
                try
                {
                    command.CommandText =
                        $"DELETE FROM [dbo].[Carrello] WHERE [Prodotto] = '{carrello.Prodotto.ASIN}' AND [Utente] = '{carrello.Utente.ID}'";
                    command.ExecuteNonQuery();
                    //Console.WriteLine("DELETE FROM [dbo].[Carrello] WHERE [Prodotto] = '{0}' AND [Utente] = '{1}'", carrello.Prodotto.ASIN, carrello.Utente.ID); //test mode
                    //Console.WriteLine("Prodotto eliminato => {0}: {1} dal carrello di {2}", carrello.Prodotto.ASIN, carrello.Prodotto.NomeProdotto,carrello.Utente.ID);
                    return true;
                }
                catch (Exception ex)
                {
                    var st = new StackTrace();
                    var sf = st.GetFrame(1);
                    Console.WriteLine("{1} => Commit Exception Type : {0}", ex.GetType(), sf.GetMethod());
                    Console.WriteLine("  Message: {0}", ex.Message);
                }
                return false;
            }
        }

        //METODO che nel caso in cui nel DB sia già presente aggiorna l'elemetno con la giusta Quantità e il prezzo corretto
        public bool UpdateCartProduct(Carrello carrello)
        {
            using (SqlCommand command = connessione.Connection.CreateCommand())
            {
                try
                {
                    //per passargli un numero con separatore '.' anzichè ',' => crea problemi altrimenti
                    string specifier;
                    System.Globalization.CultureInfo culture;
                    // Use standard numeric format specifiers.
                    specifier = "G";
                    culture = System.Globalization.CultureInfo.CreateSpecificCulture("eu-ES");
                    string costoTOT = carrello.CostoTot.ToString(specifier, System.Globalization.CultureInfo.InvariantCulture);
                    // Displays:    16325.62901 => anizhè 16323,62901
                    command.CommandText =
                        $"UPDATE [dbo].[Carrello] SET [Prodotto] = '{carrello.Prodotto.ASIN}',[Qt] = '{carrello.Quantita}',[CostoTOT] = '{costoTOT}' WHERE [Utente] = '{carrello.Utente.ID}' AND [Prodotto] = '{carrello.Prodotto.ASIN}'";
                    //Console.WriteLine(command.CommandText);
                    command.ExecuteNonQuery();

                    Console.WriteLine("Prodotto nel carrello aggiornato {0} => ASIN: {1} Qt. {2}", carrello.Utente.ID, carrello.Prodotto.ASIN, carrello.Quantita);
                    return true;
                }
                catch (Exception ex)
                {
                    var st = new StackTrace();
                    var sf = st.GetFrame(1);
                    Console.WriteLine("{1} => Commit Exception Type : {0}", ex.GetType(), sf.GetMethod());
                    Console.WriteLine("  Message: {0}", ex.Message);
                }
                return false;
            }
        }

        //METODO per svuotare il carrello dopo aver acquistato
        private void DeleteAllCart(Credenziali utente)
        {
            using (SqlCommand command = connessione.Connection.CreateCommand())
            {
                try
                {
                    command.CommandText =
                        $"DELETE FROM [dbo].[Carrello] WHERE [Utente] = '{utente.Utente.ID}'";
                    command.ExecuteNonQuery();
                    Console.WriteLine("Carrello di {0} ({1}) svuotato dopo acquisto", utente.Utente.ID, utente.Username);
                }
                catch (Exception ex)
                {
                    var st = new StackTrace();
                    var sf = st.GetFrame(1);
                    Console.WriteLine("{1} => Commit Exception Type : {0}", ex.GetType(), sf.GetMethod());
                    Console.WriteLine("  Message: {0}", ex.Message);
                }
            }
        }

        //METODO per l'acquisto dei prodotti
        public bool ShopProducts(Credenziali utente)
        {
            List<Carrello> prodotti = GetAllElementOfCarrello(utente);
            int numberOfId = prodotti.Count() * 2;
           // Console.WriteLine("Dimensione array: {0}", numberOfId); //test debug
            string[] idList = Program.IdGen.IDCompraVendi(numberOfId);

            using SqlCommand command = connessione.Connection.CreateCommand();
                SqlTransaction transaction;

            // Start a local transaction.
            transaction = connessione.Connection.BeginTransaction("Transation For Shop");

            // Must assign both transaction object and connection
            // to Command object for a pending local transaction
            command.Connection = connessione.Connection;
            command.Transaction = transaction;

            try
            {
                int numberOfCycleId = 0;
                DateTime data = DateTime.Now; //ottengo la data corrente
                string dataString = data.Year + "-" + data.Month + "-" + data.Day; //imposto la data in formato stringa
                int newQt = 0;
                foreach (var prodotto in prodotti)
                {
                    command.CommandText =
                    $"INSERT INTO CompraVendita (ID,Data,Prodotto,Utente,Tipo,Qt) VALUES ('{idList[numberOfCycleId]}','{dataString}','{prodotto.Prodotto.ASIN}','{utente.Utente.ID}','A','{prodotto.Quantita}')";
                    //Console.WriteLine(command.CommandText); //test debug
                    command.ExecuteNonQuery();
                    numberOfCycleId++; //passo all'ID successivo che ho generato precedentemente
                    command.CommandText =
                    $"INSERT INTO CompraVendita (ID,Data,Prodotto,Utente,Tipo,Qt) VALUES ('{idList[numberOfCycleId]}','{dataString}','{prodotto.Prodotto.ASIN}','{prodotto.Prodotto.Venditore.ID}','V','{prodotto.Quantita}')";
                    //Console.WriteLine(command.CommandText); //test debug
                    command.ExecuteNonQuery();
                    numberOfCycleId++; //passo all'ID successivo che ho generato precedentemente
                    //ottengo la quantità disponibile prima della vendita del prodotto
                    command.CommandText =
                    $"SELECT Qt FROM Prodotto WHERE ASIN = '{prodotto.Prodotto.ASIN}'";
                    using (SqlDataReader exe = command.ExecuteReader())
                    {
                        //eseguo la query      
                        if (!exe.Read())
                        {
                            throw new Exception("Nessuna qt del prodotto ottenuta!");
                        }
                        newQt = exe.GetInt32(0) - prodotto.Quantita; //ottengo e scalo la qt
                    }
                    //aggiorno la quantità disponibile dei prodotti
                    command.CommandText =
                    $"UPDATE Prodotto SET Qt = '{newQt}'WHERE ASIN = '{prodotto.Prodotto.ASIN}'";
                    command.ExecuteNonQuery();
                }
                // Attempt to commit the transaction.
                transaction.Commit();
                DeleteAllCart(utente);
                Console.WriteLine("Acquisto eseguito correttamente per {0} ({1})", utente.Utente.ID, utente.Username);
                return true;
            }
            catch (Exception ex)
            {
                var st = new StackTrace();
                var sf = st.GetFrame(1);
                Console.WriteLine("{1} => Commit Exception Type : {0}", ex.GetType(), sf.GetMethod());
                Console.WriteLine("  Message: {0}", ex.Message);
                // Attempt to roll back the transaction.
                try
                {
                    transaction.Rollback();
                }
                catch (Exception ex2)
                {
                    // This catch block will handle any errors that may have occurred
                    // on the server that would cause the rollback to fail, such as
                    // a closed connection.
                    var st2 = new StackTrace();
                    var sf2 = st.GetFrame(1);
                    Console.WriteLine("{1} => Commit Exception Type : {0}", ex2.GetType(), sf2.GetMethod());
                    Console.WriteLine("  Message: {0}", ex2.Message);
                }
            }
            return false;
        }

        //METODO che verifica se esite già un elemento con stesso id nel carrello => restituisce la quantità già presente nel DB
        public int CheckIfAlreadyExistInCartUser(Carrello carrello)
        {
            try
            {
                string query =
                    $"SELECT [Qt] FROM [dbo].[Carrello] WHERE [Utente] = '{carrello.Utente.ID}' AND [Prodotto] = '{carrello.Prodotto.ASIN}'";
                using SqlCommand command = new SqlCommand(query, connessione.Connection);
                using (SqlDataReader exe = command.ExecuteReader())
                {
                    //eseguo la query      
                    if (exe.Read())
                    {
                        return exe.GetInt32(0); //restituisco la quantità già presente nel DB
                    }
                }
                return 0;
            }
            catch (Exception ex)
            {
                var st = new StackTrace();
                var sf = st.GetFrame(1);
                Console.WriteLine("{1} => Commit Exception Type : {0}", ex.GetType(), sf.GetMethod());
                Console.WriteLine("  Message: {0}", ex.Message);
            }
            return -1; //se c'è un problema restituisco un valore negativo
        }

        //METODO che aggiunge un prodotto nel db
        public bool AddProdotto(Prodotto prodotto)
        {
            using (SqlCommand command = connessione.Connection.CreateCommand())
            {
                try
                { 
                    //per passargli un numero con separatore '.' anzichè ',' => crea problemi altrimenti
                    string specifier;
                    System.Globalization.CultureInfo culture;
                    // Use standard numeric format specifiers.
                    specifier = "G";
                    culture = System.Globalization.CultureInfo.CreateSpecificCulture("eu-ES");
                    string costoReso = prodotto.CostoReso.ToString(specifier, System.Globalization.CultureInfo.InvariantCulture);
                    string costoProdotto = prodotto.CostoProdotto.ToString(specifier, System.Globalization.CultureInfo.InvariantCulture);
                    string ASIN = Program.IdGen.ASINProdotto(); //genero automaticamente ASIN univoco
                    prodotto.ImmagineProdotto = ASIN + ".png"; //creo immagine con ASIN.png per tutte
                    command.CommandText =
                        $"INSERT INTO Prodotto (ASIN,NomeProdotto,Qt,Taglie,Materiale,CostoReso,CostoProdotto,TempoSpedizione,Venditore,Categoria,DescProdotto,ImmagineProdotto) VALUES ('{ASIN}','{prodotto.NomeProdotto}','{prodotto.Quantita}','{prodotto.Taglie}','{prodotto.Materiale}','{costoReso}','{costoProdotto}','{prodotto.TempoSpedizione}','{prodotto.Venditore.ID}','{prodotto.Categoria}','{prodotto.DescrizioneProdotto}','{prodotto.ImmagineProdotto}')";
                    command.ExecuteNonQuery();
                    Console.WriteLine("Nuovo prodotto in vendita => {0}: {1}", ASIN, prodotto.NomeProdotto);
                    return true;
                }
                catch (Exception ex)
                {
                    var st = new StackTrace();
                    var sf = st.GetFrame(1);
                    Console.WriteLine("{1} => Commit Exception Type : {0}", ex.GetType(), sf.GetMethod());
                    Console.WriteLine("  Message: {0}", ex.Message);
                }
                return false;
            }
        }

        //METODO per eliminare un prodotto in vendita
        public bool DeleteProdotto(Prodotto prodotto)
        {
            using (SqlCommand command = connessione.Connection.CreateCommand())
            {
                try
                {
                    /* ELIMINAZIONE DIRETTA DEL PRODOTTO => problema del'eliminazione per la chiave referenziale associata
                    command.CommandText =
                        $"DELETE FROM [dbo].[Prodotto] WHERE [ASIN] = '{prodotto.ASIN}'";
                    command.ExecuteNonQuery();

                    Console.WriteLine("Prodotto eliminato => {0}: {1}", prodotto.ASIN, prodotto.NomeProdotto);
                    */

                    //Anzichè eliminare il prodotto imposto la quantità a zero
                    command.CommandText =
                        $"UPDATE [dbo].[Prodotto] SET [Qt] = 0 WHERE [ASIN] = '{prodotto.ASIN}'";
                    command.ExecuteNonQuery();

                    Console.WriteLine("Prodotto eliminato => {0}: {1} (qt. = 0)", prodotto.ASIN, prodotto.NomeProdotto);
                    return true;
                }
                catch (Exception ex)
                {
                    var st = new StackTrace();
                    var sf = st.GetFrame(1);
                    Console.WriteLine("{1} => Commit Exception Type : {0}", ex.GetType(), sf.GetMethod());
                    Console.WriteLine("  Message: {0}", ex.Message);
                }
                return false;
            }
        }

        //METODO per aggiornare un prodotto in vendita
        public bool UpdateProdotto(Prodotto prodotto)
        {
            using (SqlCommand command = connessione.Connection.CreateCommand())
            {
                try
                {
                    //per passargli un numero con separatore '.' anzichè ',' => crea problemi altrimenti
                    string specifier;
                    System.Globalization.CultureInfo culture;
                    // Use standard numeric format specifiers.
                    specifier = "G";
                    culture = System.Globalization.CultureInfo.CreateSpecificCulture("eu-ES");
                    string costoProdotto = prodotto.CostoProdotto.ToString(specifier, System.Globalization.CultureInfo.InvariantCulture);
                    string costoReso = prodotto.CostoReso.ToString(specifier, System.Globalization.CultureInfo.InvariantCulture);
                    // Displays:    16325.62901 => anizhè 16323,62901
                    command.CommandText =
                        $"UPDATE [dbo].[Prodotto] SET  [NomeProdotto] = '{prodotto.NomeProdotto}', [Qt] = '{prodotto.Quantita}',[Taglie] = '{prodotto.Taglie}',[Materiale] = '{prodotto.Materiale}',[CostoReso] = {costoReso},[CostoProdotto] = {costoProdotto},[TempoSpedizione] = '{prodotto.TempoSpedizione}',[Venditore] = '{prodotto.Venditore.ID}',[Categoria] = '{prodotto.Categoria}',[DescProdotto] = '{prodotto.DescrizioneProdotto}' WHERE [ASIN] = '{prodotto.ASIN}'";
                      command.ExecuteNonQuery();
                    //Console.WriteLine(command.CommandText); //test debug
                    Console.WriteLine("Prodotto aggiornato => {0}: {1}", prodotto.ASIN, prodotto.NomeProdotto);
                    return true;
                }
                catch (Exception ex)
                {
                    //Test------------------------
                    //Console.WriteLine("UPDATE comando => {0}", command.CommandText);
                    //------------------------------------
                    //serve per stampare il nome del metodo in cui ci troviamo
                    var st = new StackTrace();
                    var sf = st.GetFrame(1);
                    Console.WriteLine("{1} => Commit Exception Type : {0}", ex.GetType(), sf.GetMethod());
                    Console.WriteLine("  Message: {0}", ex.Message);
                }
                return false;
            }
        }

        //METODO per verificare se posso aumentare la quantità di un prodotto nel carrello
        public bool CheckIfQtIsOk(string ASIN, int qt)
        {
            try
            {
                string query =
                    $"SELECT [Qt] FROM [dbo].[Prodotto] WHERE [ASIN] = '{ASIN}' AND [Qt] >= '{qt}'";
                using SqlCommand command = new SqlCommand(query, connessione.Connection);
                using (SqlDataReader exe = command.ExecuteReader())
                {
                    //eseguo la query
                    if (exe.Read())
                    {
                        return true; //quantità non aumentabile
                    }
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace();
                var sf = st.GetFrame(1);
                Console.WriteLine("{1} => Commit Exception Type : {0}", ex.GetType(), sf.GetMethod());
                Console.WriteLine("  Message: {0}", ex.Message);
            }
            return false;
        }

        //METODO che restituisce la qt del prodotto presente del DB
        public int GetProductQtAviable(string ASIN)
        {
            try
            {
                string query =
                    $"SELECT [Qt] FROM [dbo].[Prodotto] WHERE [ASIN] = '{ASIN}'";
                using SqlCommand command = new SqlCommand(query, connessione.Connection);
                using (SqlDataReader exe = command.ExecuteReader())
                {
                    //eseguo la query
                    if (exe.Read())
                    {
                        return exe.GetInt32(0); //restituisco la qt disponibile in DB
                    }
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace();
                var sf = st.GetFrame(1);
                Console.WriteLine("{1} => Commit Exception Type : {0}", ex.GetType(), sf.GetMethod());
                Console.WriteLine("  Message: {0}", ex.Message);
            }
            return -1;
        }

        //METODO per ottenere tutti i prodotti di un venditore
        public List<Prodotto> GetAllProdottiOfSeller(Credenziali filtroRicerca)
        {
            List<Prodotto> lista = new List<Prodotto>();
            Prodotto prodottoAppoggio;
            String query = $"SELECT ASIN,NomeProdotto,Qt,Taglie,Materiale,CostoReso,CostoProdotto,TempoSpedizione,Venditore,Categoria,DescProdotto,ImmagineProdotto FROM Prodotto WHERE Venditore = '{filtroRicerca.Utente.ID}'";
            try
            {
                using (SqlCommand command = new SqlCommand(query, connessione.Connection))
                {
                    using (SqlDataReader exe = command.ExecuteReader())
                    { //eseguo la query      
                        while (exe.Read())
                        {
                            prodottoAppoggio = new Prodotto();
                            prodottoAppoggio.ASIN = exe.GetString(0);
                            prodottoAppoggio.NomeProdotto = exe.GetString(1);
                            prodottoAppoggio.Quantita = exe.GetInt32(2);
                            prodottoAppoggio.Taglie = exe.GetString(3);
                            prodottoAppoggio.Materiale = exe.GetString(4);
                            prodottoAppoggio.CostoReso = exe.GetDouble(5);
                            prodottoAppoggio.CostoProdotto = exe.GetDouble(6);
                            prodottoAppoggio.TempoSpedizione = exe.GetString(7);
                            prodottoAppoggio.Venditore.ID = exe.GetString(8);
                            prodottoAppoggio.Categoria = exe.GetString(9);
                            prodottoAppoggio.DescrizioneProdotto = exe.GetString(10);
                            prodottoAppoggio.ImmagineProdotto = exe.GetString(11);
                            lista.Add(prodottoAppoggio);
                        }
                        exe.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace();
                var sf = st.GetFrame(1);
                Console.WriteLine("{1} => Commit Exception Type : {0}", ex.GetType(), sf.GetMethod());
                Console.WriteLine("  Message: {0}", ex.Message);
            }
            return lista;
        }


        //METODO per ottenere tutti i prodotti in vendita tranne quelli venduti dall'utente che li sta visualizzando
        public List<Prodotto> GetAllProdotti(string id)
        {
            List<Prodotto> lista = new List<Prodotto>();
            Prodotto prodottoAppoggio;
            String query = $"SELECT * FROM Prodotto WHERE Qt <> 0 AND Venditore <> '{id}'";
            try
            {
                using (SqlCommand command = new SqlCommand(query, connessione.Connection))
                {
                    using (SqlDataReader exe = command.ExecuteReader())
                    { //eseguo la query      
                        while (exe.Read())
                        {
                            prodottoAppoggio = new Prodotto();
                            prodottoAppoggio.ASIN = exe.GetString(0);
                            prodottoAppoggio.NomeProdotto = exe.GetString(1);
                            prodottoAppoggio.Quantita = exe.GetInt32(2);
                            prodottoAppoggio.Taglie = exe.GetString(3);
                            prodottoAppoggio.Materiale = exe.GetString(4);
                            prodottoAppoggio.CostoReso = exe.GetDouble(5);
                            prodottoAppoggio.CostoProdotto = exe.GetDouble(6);
                            prodottoAppoggio.TempoSpedizione = exe.GetString(7);
                            prodottoAppoggio.Venditore.ID = exe.GetString(8);
                            prodottoAppoggio.Categoria = exe.GetString(9);
                            prodottoAppoggio.DescrizioneProdotto = exe.GetString(10);
                            prodottoAppoggio.ImmagineProdotto = exe.GetString(11);
                            //carrelloAppoggio.Prodotto.Immagine = exe.GetString(11); //da aggiungere
                            lista.Add(prodottoAppoggio);
                            //Console.WriteLine("");
                        }
                        exe.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace();
                var sf = st.GetFrame(1);
                Console.WriteLine("{1} => Commit Exception Type : {0}", ex.GetType(), sf.GetMethod());
                Console.WriteLine("  Message: {0}", ex.Message);
            }
            return lista;
        }


        //METODO per ottenere un prodotto selezionato attraverso ASIN del db
        public Prodotto GetSingleProduct(string ASIN)
        {
            Prodotto prodotto = new Prodotto();
            String query = $"SELECT * FROM Prodotto WHERE ASIN = '{ASIN}'";
            try
            {
                using (SqlCommand command = new SqlCommand(query, connessione.Connection))
                {
                    using (SqlDataReader exe = command.ExecuteReader())
                    { //eseguo la query      
                        while (exe.Read())
                        {
                            prodotto = new Prodotto();
                            prodotto.ASIN = exe.GetString(0);
                            prodotto.NomeProdotto = exe.GetString(1);
                            prodotto.Quantita = exe.GetInt32(2);
                            prodotto.Taglie = exe.GetString(3);
                            prodotto.Materiale = exe.GetString(4);
                            prodotto.CostoReso = exe.GetDouble(5);
                            prodotto.CostoProdotto = exe.GetDouble(6);
                            prodotto.TempoSpedizione = exe.GetString(7);
                            prodotto.Venditore.ID = exe.GetString(8);
                            prodotto.Categoria = exe.GetString(9);
                            prodotto.DescrizioneProdotto = exe.GetString(10);
                            prodotto.ImmagineProdotto = exe.GetString(11);
                        }
                        exe.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace();
                var sf = st.GetFrame(1);
                Console.WriteLine("{1} => Commit Exception Type : {0}", ex.GetType(), sf.GetMethod());
                Console.WriteLine("  Message: {0}", ex.Message);
            }
            return prodotto;
        }


        //METODO per ottenere i prodotti selezionati attraverso categoria del db
        public List<Prodotto> GetProductByCategory(string Categoria)
        {
            List<Prodotto> lista = new List<Prodotto>();
            Prodotto prodottoAppoggio;
            String query = $"SELECT * FROM Prodotto WHERE Categoria = '{Categoria}'";
            try
            {
                using (SqlCommand command = new SqlCommand(query, connessione.Connection))
                {
                    using (SqlDataReader exe = command.ExecuteReader())
                    { //eseguo la query      
                        while (exe.Read())
                        {
                            prodottoAppoggio = new Prodotto();
                            prodottoAppoggio.ASIN = exe.GetString(0);
                            prodottoAppoggio.NomeProdotto = exe.GetString(1);
                            prodottoAppoggio.Quantita = exe.GetInt32(2);
                            prodottoAppoggio.Taglie = exe.GetString(3);
                            prodottoAppoggio.Materiale = exe.GetString(4);
                            prodottoAppoggio.CostoReso = exe.GetDouble(5);
                            prodottoAppoggio.CostoProdotto = exe.GetDouble(6);
                            prodottoAppoggio.TempoSpedizione = exe.GetString(7);
                            prodottoAppoggio.Venditore.ID = exe.GetString(8);
                            prodottoAppoggio.Categoria = exe.GetString(9);
                            prodottoAppoggio.DescrizioneProdotto = exe.GetString(10);
                            prodottoAppoggio.ImmagineProdotto = exe.GetString(11);
                            //carrelloAppoggio.Prodotto.Immagine = exe.GetString(11); //da aggiungere
                            lista.Add(prodottoAppoggio);
                            //Console.WriteLine("");
                        }
                        exe.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace();
                var sf = st.GetFrame(1);
                Console.WriteLine("{1} => Commit Exception Type : {0}", ex.GetType(), sf.GetMethod());
                Console.WriteLine("  Message: {0}", ex.Message);
            }
            return lista;
        }


        //Controlla se l'email inserita nel DB è già presente o meno
        private bool CheckIfEmailUserExist(Credenziali userinfo)
        {
            String queryCheck = $"SELECT Username FROM Credenziali WHERE Username = '{userinfo.Username}'";
            try
            {
                using (SqlCommand command = new SqlCommand(queryCheck, connessione.Connection))
                {
                    using (SqlDataReader exe = command.ExecuteReader())
                    { //eseguo la query      
                        if (exe.Read())
                        {
                            Console.WriteLine("Username {0} già presente!", userinfo.Username);
                            exe.Close();
                            return true;
                        }
                    }
                }
                queryCheck = $"SELECT Email FROM Credenziali WHERE Email = '{userinfo.Email}'";
                using (SqlCommand command = new SqlCommand(queryCheck, connessione.Connection))
                {
                    using (SqlDataReader exe = command.ExecuteReader())
                    { //eseguo la query      
                        if (exe.Read())
                        {
                            Console.WriteLine("Email {0} già presente!", userinfo.Email);
                            exe.Close();
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace();
                var sf = st.GetFrame(1);
                Console.WriteLine("{1} => Commit Exception Type : {0}", ex.GetType(), sf.GetMethod());
                Console.WriteLine("  Message: {0}", ex.Message);
                return true;
            }
            //Non già presente
            return false;
        }

        //METODO per controllare se id già presente nel DB per Utente
        public bool CheckIfExistIdUtente(string id)
        {
            String queryCheck = $"SELECT ID FROM Utente WHERE ID = '{id}'";
            try
            {
                using SqlCommand command = new(queryCheck, connessione.Connection);
                using (SqlDataReader exe = command.ExecuteReader())
                {
                    //eseguo la query      
                    if (exe.Read())
                    {
                        Console.WriteLine("ID {0} già presente! Ne genero uno nuovo..", id);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace();
                var sf = st.GetFrame(1);
                Console.WriteLine("{1} => Commit Exception Type : {0}", ex.GetType(), sf.GetMethod());
                Console.WriteLine("  Message: {0}", ex.Message);
                return false;//in caso di eccezione non genero un loop
            }
            //Non già presente
            return false;
        }

        //METODO per controllare se asin già presente nel DB per Prodotto
        public bool CheckIfExistASINProdotto(string asin)
        {
            String queryCheck = $"SELECT ASIN FROM Prodotto WHERE ASIN = '{asin}'";
            try
            {
                using (SqlCommand command = new SqlCommand(queryCheck, connessione.Connection))
                {
                    using (SqlDataReader exe = command.ExecuteReader())
                    { //eseguo la query      
                        if (exe.Read())
                        {
                            Console.WriteLine("ID {0} già presente! Ne genero uno nuovo..", asin);
                            exe.Close();
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace();
                var sf = st.GetFrame(1);
                Console.WriteLine("{1} => Commit Exception Type : {0}", ex.GetType(), sf.GetMethod());
                Console.WriteLine("  Message: {0}", ex.Message);
                return false;//in caso di eccezione non genero un loop
            }
            //Non già presente
            return false;
        }

        //METODO per controllare se ID compravendi già presente nel DB
        public bool CheckIfExistIDCompraVendi(string id)
        {
            String queryCheck = $"SELECT ID FROM CompraVendita WHERE ID = '{id}'";
            try
            {
                using (SqlCommand command = new SqlCommand(queryCheck, connessione.Connection))
                {
                    using (SqlDataReader exe = command.ExecuteReader())
                    { //eseguo la query      
                        if (exe.Read())
                        {
                            Console.WriteLine("ID {0} già presente! Ne genero uno nuovo..", id);
                            exe.Close();
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace();
                var sf = st.GetFrame(1);
                Console.WriteLine("{1} => Commit Exception Type : {0}", ex.GetType(), sf.GetMethod());
                Console.WriteLine("  Message: {0}", ex.Message);
                return false;//in caso di eccezione non genero un loop
            }
            //Non già presente
            return false;
        }

        //METODO che mi restituisce il credito di un utente
        public double GetCredito(string ID)
        {
            double credito = 0;
            String queryCheck = $"SELECT Portafoglio FROM Utente WHERE ID = '{ID}'";
            try
            {
                using (SqlCommand command = new SqlCommand(queryCheck, connessione.Connection))
                {
                    using (SqlDataReader exe = command.ExecuteReader())
                    { //eseguo la query      
                        if (exe.Read())
                        {
                            credito = exe.GetDouble(0);
                        }
                        exe.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace();
                var sf = st.GetFrame(1);
                Console.WriteLine("{1} => Commit Exception Type : {0}", ex.GetType(), sf.GetMethod());
                Console.WriteLine("  Message: {0}", ex.Message);
            }
            
            return credito;
        }

        //METODO per aggiornare il portafoglio
        public bool UpdatePortafoglio(Utente utente)
        {
            double nuovo_credito = GetCredito(utente.ID) + utente.Portafoglio; //seconda query per ottenere credito utente

            //per passargli un numero con separatore '.' anzichè ',' => crea problemi altrimenti
            string specifier;
            System.Globalization.CultureInfo culture;
            // Use standard numeric format specifiers.
            specifier = "G";
            culture = System.Globalization.CultureInfo.CreateSpecificCulture("eu-ES");
            string portafoglio = nuovo_credito.ToString(specifier, System.Globalization.CultureInfo.InvariantCulture);
            // Displays:    16325.62901 => anizhè 16323,62901

            using (SqlCommand command = connessione.Connection.CreateCommand())
            {
                try
                {
                    command.CommandText =
                        $"UPDATE [dbo].[Utente] SET [Portafoglio] = ('{portafoglio}') WHERE [ID] = '{utente.ID}'";
                    command.ExecuteNonQuery();

                    Console.WriteLine("Portafoglio aggiornato => {0}: {1}", utente.ID, utente.Portafoglio);
                    return true;
                }
                catch (Exception ex)
                {
                    var st = new StackTrace();
                    var sf = st.GetFrame(1);
                    Console.WriteLine("{1} => Commit Exception Type : {0}", ex.GetType(), sf.GetMethod());
                    Console.WriteLine("  Message: {0}", ex.Message);
                }
                return false;
            }
        }

        //METODO che mi restituisce le vendite di un utente
        public List<CompraVendita> GetVendite(string ID)
        {
            List<CompraVendita> lista = new List<CompraVendita>();
            CompraVendita prodottoAppoggio;



            String queryCheck1 = $"SELECT cv.Data, ASIN, NomeProdotto, Taglie, CostoProdotto, cv.Qt FROM Prodotto, CompraVendita as cv WHERE cv.Utente = '{ID}' AND cv.Tipo = 'V' AND ASIN = cv.Prodotto";

            try
            {
                using (SqlCommand command = new SqlCommand(queryCheck1, connessione.Connection))
                {
                    using (SqlDataReader exe = command.ExecuteReader())
                    { //eseguo la query      
                        while (exe.Read())
                        {
                            prodottoAppoggio = new CompraVendita();

                            //prodottoAppoggio.ID = exe.GetString(0);
                            prodottoAppoggio.Data = exe.GetDateTime(0);
                            prodottoAppoggio.Prodotto = new Prodotto()
                            {
                                ASIN = exe.GetString(1),
                                NomeProdotto = exe.GetString(2),
                                Taglie = exe.GetString(3),
                                CostoProdotto = exe.GetDouble(4),

                            };

                            prodottoAppoggio.Qt = exe.GetInt32(5);
                            lista.Add(prodottoAppoggio);
                        }
                        exe.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace();
                var sf = st.GetFrame(1);
                Console.WriteLine("{1} => Commit Exception Type : {0}", ex.GetType(), sf.GetMethod());
                Console.WriteLine("  Message: {0}", ex.Message);
            }

            return lista;
        }


        //METODO che mi restituisce gli acquisti di un utente
        public List<CompraVendita> GetAcquisti(string ID)
        {
            List<CompraVendita> lista = new List<CompraVendita>();
            CompraVendita prodottoAppoggio;

            

            String queryCheck1 = $"SELECT cv.Data, ASIN, NomeProdotto, Taglie, CostoProdotto, cv.Qt FROM Prodotto, CompraVendita as cv WHERE cv.Utente = '{ID}' AND cv.Tipo = 'A' AND ASIN = cv.Prodotto";
            try
            {
                using (SqlCommand command = new SqlCommand(queryCheck1, connessione.Connection))
                {
                    using (SqlDataReader exe = command.ExecuteReader())
                    { //eseguo la query      
                        while (exe.Read())
                        {
                            prodottoAppoggio = new CompraVendita();

                            //prodottoAppoggio.ID = exe.GetString(0);
                            prodottoAppoggio.Data = exe.GetDateTime(0);
                            prodottoAppoggio.Prodotto = new Prodotto()
                            {
                                ASIN = exe.GetString(1),
                                NomeProdotto = exe.GetString(2),
                                Taglie = exe.GetString(3),
                                CostoProdotto = exe.GetDouble(4),

                            };
                           
                            prodottoAppoggio.Qt = exe.GetInt32(5);
                            lista.Add(prodottoAppoggio);
                        }
                        exe.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace();
                var sf = st.GetFrame(1);
                Console.WriteLine("{1} => Commit Exception Type : {0}", ex.GetType(), sf.GetMethod());
                Console.WriteLine("  Message: {0}", ex.Message);
            }

            return lista;
        }

        //METODO che aggiorna saldo per le vendite di un utente
        public bool AccreditoVendite(Credenziali credenziali)
        {
            
            List<Carrello> prodotti = GetAllElementOfCarrello(credenziali);

            try
            {

                //per passargli un numero con separatore '.' anzichè ',' => crea problemi altrimenti
                string specifier;
                System.Globalization.CultureInfo culture;
                // Use standard numeric format specifiers.
                specifier = "G";
                culture = System.Globalization.CultureInfo.CreateSpecificCulture("eu-ES");
                string portafoglio;
                // Displays:    16325.62901 => anizhè 16323,62901
                double credito_precedente, accredito;
                foreach (Carrello item in prodotti)
                {
                    credito_precedente = GetCredito(item.Prodotto.Venditore.ID);
                    accredito = credito_precedente + item.Prodotto.CostoProdotto * item.Quantita;
                    portafoglio = accredito.ToString(specifier, System.Globalization.CultureInfo.InvariantCulture);

                    using (SqlCommand command = connessione.Connection.CreateCommand())
                    {
                        command.CommandText =
                        $"UPDATE Utente SET Portafoglio = '{portafoglio}' WHERE ID = '{item.Prodotto.Venditore.ID}'";
                        command.ExecuteNonQuery();
                       // Console.WriteLine(item);
                       // Console.WriteLine(item.Prodotto.CostoProdotto);    TEST
                       // Console.WriteLine(item.Prodotto.Venditore.ID);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                var st = new StackTrace();
                var sf = st.GetFrame(1);
                Console.WriteLine("{1} => Commit Exception Type : {0}", ex.GetType(), sf.GetMethod());
                Console.WriteLine("  Message: {0}", ex.Message);
            }

            return false;
        }

        //METODO che aggiunge una valutazione ad un prodotto nel db
        public bool AddValutazione(Valutazione valutazione)
        {
            using (SqlCommand command = connessione.Connection.CreateCommand())
            {
                try
                {
                    DateTime data = DateTime.Now; //ottengo la data corrente
                    string dataString = data.Year + "-" + data.Month + "-" + data.Day; //imposto la data in formato stringa
                    string ASIN = Program.IdGen.IDValutazione(); //genero ID univoco per valutazione
                    command.CommandText =
                        $"INSERT INTO Valutazione (ID,Utente,Stelle,Recensione,Data,Prodotto) VALUES ('{ASIN}','{valutazione.Utente.ID}','{valutazione.Stelle}','{valutazione.Recensione}','{dataString}','{valutazione.Prodotto.ASIN}')";
                    command.ExecuteNonQuery();
                    //Console.WriteLine(command.CommandText); //test debug
                    Console.WriteLine("Nuovo valutazione inserita per prodotto {0} da user {1}", valutazione.Prodotto.ASIN, valutazione.Utente.ID);
                    return true;
                }
                catch (Exception ex)
                {
                    var st = new StackTrace();
                    var sf = st.GetFrame(1);
                    Console.WriteLine("{1} => Commit Exception Type : {0}", ex.GetType(), sf.GetMethod());
                    Console.WriteLine("  Message: {0}", ex.Message);
                }
                return false;
            }
        }

        //METODO che verifica se un ID per la valutazione è stato già generato
        public bool CheckIfExistIdValutazione(string id)
        {
            String queryCheck = $"SELECT ID FROM Valutazione WHERE ID = '{id}'";
            try
            {
                using (SqlCommand command = new SqlCommand(queryCheck, connessione.Connection))
                {
                    using (SqlDataReader exe = command.ExecuteReader())
                    { //eseguo la query      
                        if (exe.Read())
                        {
                            Console.WriteLine("ID {0} già presente! Ne genero uno nuovo..", id);
                            exe.Close();
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace();
                var sf = st.GetFrame(1);
                Console.WriteLine("{1} => Commit Exception Type : {0}", ex.GetType(), sf.GetMethod());
                Console.WriteLine("  Message: {0}", ex.Message);
                return false;//in caso di eccezione non genero un loop
            }
            //Non già presente
            return false;
        }

        /*
         * METODO che verifica se un utente ha acquistato un prodotto cosichè possa valutarlo una ed una sola volta
         * Inoltre una volta effettuata la recensione non sarà più possibile modificarla o cambiarla
        */
        public bool CheckIfUserBoughtProduct(string userId, string ASIN)
        {
            //verifico oltre all'utente e al prodotto che l'user sia un acquirente e non un venditore
            String queryCheck = $"SELECT * FROM CompraVendita WHERE Utente = '{userId}' AND Prodotto = '{ASIN}' AND Tipo = 'A'";
            //Console.WriteLine(queryCheck); //test debug
            try
            {
                using (SqlCommand command = new SqlCommand(queryCheck, connessione.Connection))
                {
                    using (SqlDataReader exe = command.ExecuteReader())
                    {
                        //eseguo la query      
                        if (exe.Read())
                        {
                            exe.Close();//chiuso il datareader per poter eseguire un'altra query da altro metodo
                            return Program.Db.CheckIfUserAlreadyEvaluated(userId, ASIN);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace();
                var sf = st.GetFrame(1);
                Console.WriteLine("{1} => Commit Exception Type : {0}", ex.GetType(), sf.GetMethod());
                Console.WriteLine("  Message: {0}", ex.Message);
                            }
            return false;
        }

        public bool CheckIfUserAlreadyEvaluated(string userId, string ASIN)
        {
            //controllo che sia stato valutato una ed una sola volta dall'user
            String queryCheck = $"SELECT COUNT(*) FROM Valutazione WHERE Utente = '{userId}' AND Prodotto = '{ASIN}'";
            //Console.WriteLine(queryCheck); //test debug
            try
            {
                using (SqlCommand command = new SqlCommand(queryCheck, connessione.Connection))
                {
                    //true solo se non ha ancora valutato il prodotto
                    return (int)command.ExecuteScalar() < 1;
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace();
                var sf = st.GetFrame(1);
                Console.WriteLine("{1} => Commit Exception Type : {0}", ex.GetType(), sf.GetMethod());
                Console.WriteLine("  Message: {0}", ex.Message);
                return false;//in caso di eccezione non genero un loop
            }
        }

        public List<Valutazione> GetValutazioni(string ASIN)
        {
            List<Valutazione> lista = new List<Valutazione>();
            Valutazione valutazione;

            String queryCheck = $"SELECT * FROM Valutazione WHERE Prodotto = '{ASIN}' ";
            try
            {
                

                using (SqlCommand command = new SqlCommand(queryCheck, connessione.Connection))
                {
                    using (SqlDataReader exe = command.ExecuteReader())
                    { //eseguo la query      
                        while (exe.Read())
                        {
                            valutazione = new Valutazione();

                            valutazione.ID = exe.GetString(0);
                            valutazione.Utente = new Utente()
                            {
                                ID = exe.GetString(1)

                            };
                            
                            valutazione.Stelle = exe.GetInt32(2);
                            valutazione.Recensione = exe.GetString(3);
                            valutazione.Data = exe.GetDateTime(4);

                            lista.Add(valutazione);
                        }
                        exe.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace();
                var sf = st.GetFrame(1);
                Console.WriteLine("{1} => Commit Exception Type : {0}", ex.GetType(), sf.GetMethod());
                Console.WriteLine("  Message: {0}", ex.Message);
            }

            return lista;
        }


        //METODO per ottenere ultimi 20 prodotti venduti tranne quelli venduti dall'utente che li sta visualizzando
        public List<Prodotto> GetLastSales()
        {
            List<Prodotto> lista = new List<Prodotto>();
            Prodotto prodottoAppoggio;

            String query = $"SELECT DISTINCT(p.ASIN), p.NomeProdotto, p.CostoProdotto, p.ImmagineProdotto FROM Prodotto as p, Compravendita WHERE Tipo = 'V' AND p.ASIN = Prodotto AND p.Qt <> 0 AND p.ASIN IN (SELECT TOP 20 p1.ASIN FROM Prodotto as p1, Compravendita as c WHERE c.Tipo = 'V' AND p.ASIN = c.Prodotto AND p.Qt <> 0 ORDER BY Data,p.ASIN)";
            //Console.WriteLine(query); //test debug
            try
            {
                using (SqlCommand command = new SqlCommand(query, connessione.Connection))
                {
                    using (SqlDataReader exe = command.ExecuteReader())
                    { //eseguo la query      
                        while (exe.Read())
                        {
                            prodottoAppoggio = new Prodotto();

                            prodottoAppoggio.ASIN = exe.GetString(0);
                            prodottoAppoggio.NomeProdotto = exe.GetString(1);
                            prodottoAppoggio.CostoProdotto = exe.GetDouble(2);
                            prodottoAppoggio.ImmagineProdotto = exe.GetString(3);
                            
                            lista.Add(prodottoAppoggio);
                        }
                        exe.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                var st = new StackTrace();
                var sf = st.GetFrame(1);
                Console.WriteLine("{1} => Commit Exception Type : {0}", ex.GetType(), sf.GetMethod());
                Console.WriteLine("  Message: {0}", ex.Message);
            }
            return lista;
        }
    }
}