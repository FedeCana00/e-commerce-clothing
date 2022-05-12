using MVCWebSite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCWebSite.Controllers
{
    public class AdministratorController : Controller
    {
        // GET: Administrator
        public ActionResult Users()
        {
            try { 
                //var wcf = new MVCWebSite.ServiceReferenceServer.ServiceServerClient();
                return View(PassAllUsers());
            } catch(Exception ex)
            {
                ViewBag.alert = true;
                ViewBag.alertMessage = ex.Message;
            }
            return View();
        }

        [HttpPost]
        public ActionResult Users(string ID,string stato)
        {
            try
            {
                bool statoPassato;
                if (!Boolean.TryParse(stato, out statoPassato)) //converto e passo alla mia variabile
                {
                    throw new Exception("Errore di conversione!");
                }
                bool newState = !statoPassato; //gli passo lo stato negato per poter svolgere l'operazione inversa
                var wcf = new MVCWebSite.ServiceReferenceServer.ServiceServerClient();
                ServiceReferenceServer.Utente user = new ServiceReferenceServer.Utente
                {
                    ID = ID,
                    Attivo = newState
                };

                if (!wcf.EnableDisableUser(user))
                {
                    throw new Exception("Aggiornamento stato di " + ID + " non andato a buon fine!");
                }
            }
            catch (Exception ex)
            {
                ViewBag.alert = true;
                ViewBag.alertMessage = ex.Message;
            }
            //messaggio di feedback dell'esecuzione corretta per il cambio di stato di un utente
            ViewBag.AlertSucces = true;
            ViewBag.MessageSuccess = "Stato di " + ID + "cambiato correttamente!";
            return View("Users", PassAllUsers());
        }
        public ActionResult EnableDisable(string ID, bool stato)
        {
            try
            {
                bool newState = !stato; //gli passo lo stato negato per poter svolgere l'operazione inversa
                var wcf = new MVCWebSite.ServiceReferenceServer.ServiceServerClient();
                ServiceReferenceServer.Utente user = new ServiceReferenceServer.Utente
                {
                    ID = ID,
                    Attivo = newState
                };

                if (!wcf.EnableDisableUser(user))
                {
                    throw new Exception("Aggiornamento stato di " + ID + " non andato a buon fine!");
                }
            }
            catch (Exception ex)
            {
                ViewBag.alert = true;
                ViewBag.alertMessage = ex.Message;
            }
            //messaggio di feedback dell'esecuzione corretta per il cambio di stato di un utente
            ViewBag.AlertSucces = true;
            ViewBag.MessageSuccess = "Stato di " + ID + "cambiato correttamente!";
            return View("Users",PassAllUsers());
        }

        //METODO per passare tutti gli utenti alla view
        public CredenzialiModel PassAllUsers()
        {
            var wcf = new MVCWebSite.ServiceReferenceServer.ServiceServerClient();
            CredenzialiModel users = new CredenzialiModel();
            foreach (var el in wcf.GetAllUsers())
            {
                users.ListOfUsers.Add(el);
            }
            return users;
        }
    }
}