using MVCWebSite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCWebSite.Controllers
{
    public class ValutazioniController : Controller
    {
        // GET: Valutazioni
        public ActionResult Valutazioni(string ASIN, string nomeProdotto)
        {
            //passo le informazioni importanti tramite ViewBag
            ViewBag.ASIN = ASIN;
            ViewBag.nomeProdotto = nomeProdotto;
            return View();
        }


        [HttpPost]
        public ActionResult Valutazioni(Valutazioni valutazioneForm)
        {
            try
            {
                //Validazione parziale dei campi
                if (!ModelState.IsValidField("Stelle") || !ModelState.IsValidField("Recensione"))
                {
                    throw new Exception("Dati non compilati in modo corretto!");
                }
                var wcf = new ServiceReferenceServer.ServiceServerClient();
                if (!wcf.GetUserState(Session["activeID"].ToString()))
                {
                    throw new Exception("L'amministratore ti ha disabilitato!");
                }

                ServiceReferenceServer.Valutazione valutazione = new ServiceReferenceServer.Valutazione
                {
                    Utente = new ServiceReferenceServer.Utente
                    {
                        ID = Session["activeID"].ToString()
                    },
                    Recensione = valutazioneForm.Recensione,
                    Stelle = valutazioneForm.Stelle,
                    Prodotto = new ServiceReferenceServer.Prodotto
                    {
                        ASIN = valutazioneForm.Prodotto.ASIN,
                        NomeProdotto = valutazioneForm.Prodotto.NomeProdotto
                    }
                };

                if (!wcf.AddValutazione(valutazione))
                {
                    throw new Exception("Errore durante l'aggiunta della tua valutazione!");
                }
                //feedback di esecuzione corretta della richiesta
                ViewBag.Success = true;
                ViewBag.SuccessMessage = "Prodotto valutato!";
                return View("~/Views/Home/Index.cshtml", IndexProduct()); //decidere dove mandare l'utente successivamente
            }
            catch (Exception ex)
            {
                ViewBag.alert = true;
                ViewBag.Message = ex.Message;
            }
            //ripasso i ViewBag passati precendetemente 
            ViewBag.ASIN = valutazioneForm.Prodotto.ASIN;
            ViewBag.nomeProdotto = valutazioneForm.Prodotto.NomeProdotto;
            return View("Valutazioni", valutazioneForm);
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