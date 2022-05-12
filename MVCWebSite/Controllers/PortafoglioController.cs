using MVCWebSite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCWebSite.Controllers
{
    public class PortafoglioController : Controller
    {
        // GET: Portafoglio
        public ActionResult Portafoglio()
        {
            try
            {
                var wcf = new MVCWebSite.ServiceReferenceServer.ServiceServerClient();
                ServiceReferenceServer.Utente utente = new ServiceReferenceServer.Utente();
                utente.Portafoglio = wcf.GetCredito(Session["activeID"].ToString());

                Utente ut = new Utente()
                {
                    Portafoglio = utente.Portafoglio
                };
                ViewBag.Portafoglio = ut.Portafoglio;
                return View("Portafoglio");
            }
            catch (Exception ex)
            {
                ViewBag.alert = true;
                ViewBag.Message = ex.Message;
            }
            return View("Error");
        }

        [HttpPost]
        public ActionResult Portafoglio(Utente utenteForm)
        {
            try
            {
                var wcf = new ServiceReferenceServer.ServiceServerClient();
                //in caso in cui vada a male la richiesta ripristino il vecchio credito
                double oldCredit = wcf.GetCredito(Session["activeID"].ToString());
                ViewBag.Portafoglio = oldCredit;
                if (utenteForm.Portafoglio <= 0)
                {
                    throw new Exception("Importo inserito errato!");
                }
               
                ServiceReferenceServer.Utente modelServer = new ServiceReferenceServer.Utente()
                {
                    ID = (string)Session["activeID"],
                    Portafoglio = utenteForm.Portafoglio
                };

                if (!wcf.UpdatePortafoglio(modelServer))
                {

                    return View(utenteForm);
                }

                double new_credito = wcf.GetCredito(Session["activeID"].ToString());
                Utente ut = new Utente()
                {
                    Portafoglio = new_credito
                };
                return View("Portafoglio2", ut);
            }
            catch (Exception ex)
            {
                ViewBag.alert = true;
                ViewBag.Message = ex.Message;
            }
            return View("Portafoglio");
        }

        public ActionResult Portafoglio2()
        {
            return View();
        }
    }
}