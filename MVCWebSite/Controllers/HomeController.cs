using MVCWebSite.Classi;
using MVCWebSite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebSiteMVC.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            try
            {
               return View("Index", IndexProduct());
            }
            catch (Exception ex)
            {
                ViewBag.alert = true;
                ViewBag.alertMessage = ex.Message;
            }
     
            //serve per stampare l'alert 
            //ViewBag.Alert = true;
            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Login()
        {
            //ViewBag.Alert = false;
            //ViewBag.Message = "Inserisci le tue credenziali per l'accesso.";
            return View("Login"); //ritorna la view con lo stesso nome del metodo
        }

        [HttpPost]
        public ActionResult Login(Credenziali model)
        {
            try
            {
                var wcf = new MVCWebSite.ServiceReferenceServer.ServiceServerClient();
                MVCWebSite.ServiceReferenceServer.Credenziali modelServer = new MVCWebSite.ServiceReferenceServer.Credenziali
                {
                    Username = model.Username,
                    Password = model.Password
                };

                if (!wcf.VerifyUserIdentity(modelServer))
                {
                    throw new Exception("Credenziali errate!");
                }

                Session["activeUsername"] = model.Username;
                string id = wcf.GetIDActiveUser(modelServer);
                Session["activeID"] = id;
                Session["activeRole"] = wcf.GetRoleOfUser(id);

                //feedback tramite alert di corretto login
                ViewBag.AlertSucces = true;
                ViewBag.MessageSuccess = "Benvenuto " + model.Username;
            } catch(Exception ex)
            {
                ViewBag.Alert = true;
                ViewBag.Message = ex.Message;
                return View("Login", model);
            }
            return View("Index", IndexProduct());
        }

        public ActionResult SignUp()
        {
            try
            {
                ViewBag.Message = "Create your account";
                return View("SignUp"); //ritorna la view con lo stesso nome del metodo
            } catch(Exception ex)
            {
                ViewBag.Alert = true;
                ViewBag.Message = ex.Message;
            }
            return View("Index", IndexProduct());
        }

        [HttpPost]
        public ActionResult SignUp(Credenziali model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var wcf = new MVCWebSite.ServiceReferenceServer.ServiceServerClient();
                    MVCWebSite.ServiceReferenceServer.Credenziali modelServer = new MVCWebSite.ServiceReferenceServer.Credenziali();
                    modelServer.Utente = new MVCWebSite.ServiceReferenceServer.Utente()
                    {
                        Nome = model.Utente.Nome,
                        Cognome = model.Utente.Cognome,
                        DataNascita = model.Utente.DataNascita,
                        IndirizzoSpedizione = new MVCWebSite.ServiceReferenceServer.Luogo()
                        {
                            Via = model.Utente.IndirizzoSpedizione.Via,
                            Civico = model.Utente.IndirizzoSpedizione.Civico,
                            Citta = model.Utente.IndirizzoSpedizione.Citta,
                            Nazione = model.Utente.IndirizzoSpedizione.Nazione
                        },
                        Societa = model.Utente.Societa,
                        Attivo = true,
                        Portafoglio = 0
                    };

                    //cliente o venditore in base se ha inserito la società nella registrazione
                    modelServer.Ruolo = model.Utente.Societa != null ? "V" : "C";
                    modelServer.Email = model.Email;
                    modelServer.Username = model.Username;
                    modelServer.Password = model.Password;

                    if (!wcf.AddUserWithTransaction(modelServer))
                    {
                        throw new Exception("Errore durante l'aggiunta dell'user in Db! Cambiare username e/o email!");
                    }
                    //feedback per utente in fase di registrazione
                    ViewBag.Success = true;
                    ViewBag.SuccessMessage = "Registrazione avvenuta per " + model.Username;
                    return View("Index", IndexProduct());
                }
            } catch(Exception ex)
            {
                ViewBag.Alert = true;
                ViewBag.Message = ex.Message;
            }
            return View("SignUp", model);
        }

        [HttpPost]
        public ActionResult Logout()
        {
            try
            {
                if(Session["activeUsername"] == null)
                {
                    throw new Exception("Mi spiace qualcosa è andato storto. Riprova!");
                }
                //salvo nome user per essere sicuro che venga eseguito prima il clear della session che il feedback a console server
                string user = Session["activeUsername"].ToString(); 
                Session.Clear(); //elimino tutti i dati della sessione
                var wcf = new MVCWebSite.ServiceReferenceServer.ServiceServerClient();
                wcf.WriteLogout(user); //scrittura a console del logout dell'utente
            } catch(Exception ex)
            {
                ViewBag.Alert = true;
                ViewBag.Message = ex.Message;
            }
            return View("Index", IndexProduct());
        }

        public ActionResult UpdateProfile()
        {
            try
            {
                if(Session["activeID"] == null)
                {
                    throw new Exception("Non sei loggato!");
                }
                var wcf = new MVCWebSite.ServiceReferenceServer.ServiceServerClient();
                MVCWebSite.ServiceReferenceServer.Credenziali userInfo = wcf.GetInformationUser(Session["activeID"].ToString());
                Credenziali user = new Credenziali
                {
                    Username = userInfo.Username,
                    Password = userInfo.Password,
                    Email = userInfo.Email,
                    Utente = new Utente
                    {
                        ID = userInfo.Utente.ID,
                        Nome = userInfo.Utente.Nome,
                        Cognome = userInfo.Utente.Cognome,
                        DataNascita = userInfo.Utente.DataNascita,
                        IndirizzoSpedizione = new Luogo
                        {
                            Via = userInfo.Utente.IndirizzoSpedizione.Via,
                            Civico = userInfo.Utente.IndirizzoSpedizione.Civico,
                            Citta = userInfo.Utente.IndirizzoSpedizione.Citta,
                            Nazione = userInfo.Utente.IndirizzoSpedizione.Nazione,
                        }
                    }
                };
                return View(user);
            } catch(Exception ex)
            {
                ViewBag.Alert = true;
                ViewBag.Message = ex.Message;
            }
            return View("Index", IndexProduct());
        }

        [HttpPost]
        public ActionResult UpdateProfile(Credenziali userInfo)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception("Attenzione curarsi di immettere correttamente tutti i campi prima di procedere!");
                }
                    var wcf = new MVCWebSite.ServiceReferenceServer.ServiceServerClient();
                if(!wcf.CheckOldPassword(userInfo.Utente.ID, userInfo.OldPassword)) //verifico la correttazza della password vecchia prima di impostarne una nuova
                {
                    throw new Exception("Password corrente inserita errata! Riprova!");
                }

                MVCWebSite.ServiceReferenceServer.Credenziali userToUpdate = new MVCWebSite.ServiceReferenceServer.Credenziali
                {
                    Username = userInfo.Username,
                    Password = userInfo.Password,
                    Email = userInfo.Email,
                    Utente = new MVCWebSite.ServiceReferenceServer.Utente
                    {
                        ID = userInfo.Utente.ID,
                        Nome = userInfo.Utente.Nome,
                        Cognome = userInfo.Utente.Cognome,
                        DataNascita = userInfo.Utente.DataNascita,
                        IndirizzoSpedizione = new MVCWebSite.ServiceReferenceServer.Luogo
                        {
                            Via = userInfo.Utente.IndirizzoSpedizione.Via,
                            Civico = userInfo.Utente.IndirizzoSpedizione.Civico,
                            Citta = userInfo.Utente.IndirizzoSpedizione.Citta,
                            Nazione = userInfo.Utente.IndirizzoSpedizione.Nazione,
                        }
                    }
                };
                if (!wcf.UpdateInformationUser(userToUpdate))
                {
                    throw new Exception("Errore durante l'aggiornamento dati utente!");
                }
                //feedback di aggiornamento dati riuscito correttamente
                ViewBag.Success = true;
                ViewBag.MessageSuccess = "Aggiornamento dati profilo eseguito correttamente!";
            } catch(Exception ex)
            {
                ViewBag.Alert = true;
                ViewBag.Message = ex.Message;
                userInfo.Password = ""; //imposto la nuova password da immmettere nuovamente
                userInfo.OldPassword = ""; //imposto la vecchia password da immmettere nuovamente
                return View("UpdateProfile",userInfo);
            }
            return View("Index", IndexProduct());
        }

        //METODO per passare i prodotti nel INDEX page
        public ProdottoModel IndexProduct()
        {
            var wcf = new MVCWebSite.ServiceReferenceServer.ServiceServerClient();
            ProdottoModel prodotti = new ProdottoModel();
            var listOfProduct = wcf.GetLastSales();

            foreach (var el in listOfProduct)
            {
                prodotti.ListaProdotti.Add(el);
            }
            return prodotti;
        }
    }
}