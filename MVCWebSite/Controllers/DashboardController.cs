using MVCWebSite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCWebSite.Controllers
{
    public class DashboardController : Controller
    {
        // GET: Dashboard
        public ActionResult DashboardVendite()
        {
            try
            {
                string ID = (string)Session["activeID"];
                var wcf = new MVCWebSite.ServiceReferenceServer.ServiceServerClient();
                CompraVenditaModel prodotti = new CompraVenditaModel();

                foreach (var el in wcf.GetVendite(ID))
                {
                    prodotti.ListaProdotti.Add(el);
                }
                return View("DashboardVendite", prodotti);
            }
            catch (Exception ex)
            {
                ViewBag.alert = true;
                ViewBag.Message = ex.Message;
            }
            return View("Error");
        }

        public ActionResult DashboardAcquisti()
        {
            try
            {
                string ID = (string)Session["activeID"];
                var wcf = new MVCWebSite.ServiceReferenceServer.ServiceServerClient();
                CompraVenditaModel prodotti = new CompraVenditaModel();
                List<bool> evaluated = new List<bool>();

                foreach (var el in wcf.GetAcquisti(ID))
                {
                    prodotti.ListaProdotti.Add(el);
                    evaluated.Add(wcf.CheckIfUserAlreadyEvaluated(Session["activeID"].ToString(), el.Prodotto.ASIN));
                }
                ViewBag.EvaluatedList = evaluated;
                return View("DashboardAcquisti", prodotti);
            }
            catch (Exception ex)
            {
                ViewBag.alert = true;
                ViewBag.Message = ex.Message;
            }
            return View("Error");
        }
    }
}