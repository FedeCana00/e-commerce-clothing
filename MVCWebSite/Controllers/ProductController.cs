using MVCWebSite.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCWebSite.Controllers
{
    [OutputCache(NoStore = true, Duration = 0)]
    public class ProductController : Controller
    {
        public ActionResult ShowMyProduct()
        {
            try
            {
                if(Session["activeID"] == null)
                {
                    throw new Exception("Non sei loggato!");
                }
                return View("ShowMyProduct", PassAllMyProduct());
            }catch(Exception ex)
            {
                ViewBag.alert = true;
                ViewBag.Message = ex.Message;
            }
            return View();
        }

        public ActionResult Delete(string ASINDelete, string nomeProdottoDel)
        {
            try
            {
                var wcf = new MVCWebSite.ServiceReferenceServer.ServiceServerClient();
                if (!wcf.GetUserState(Session["activeID"].ToString()))
                {
                    throw new Exception("L'amministratore ti ha disabilitato!");
                }
                if(!wcf.DeleteProdotto(new ServiceReferenceServer.Prodotto() { ASIN = ASINDelete, NomeProdotto = nomeProdottoDel }))
                {
                    throw new Exception("Errore durante l'eliminazione del prodotto!");
                }
                ViewBag.Success = true;
                ViewBag.SuccessMessage = "Eliminazione prodotto dal mercato avvenuta!";
            } catch(Exception ex)
            {
                ViewBag.alert = true;
                ViewBag.Message = ex.Message;
            }
            return View("ShowMyProduct", PassAllMyProduct());
        }


        public ActionResult UpdateProduct(string ASIN, string nome, string materiale, string venditoreID, int quantita, string taglia, float costoReso, float costoProdotto, string tempoSpedizione, string categoria, string descrizione)
        {
            try
            {
                Prodotto prodotto = new Prodotto()
                {
                    ASIN = ASIN,
                    NomeProdotto = nome,
                    Materiale = materiale,
                    Quantita = quantita,
                    Taglie = taglia,
                    CostoReso = costoReso,
                    CostoProdotto = costoProdotto,
                    TempoSpedizione = tempoSpedizione,
                    Categoria = categoria,
                    DescrizioneProdotto = descrizione,
                    Venditore = new Utente()
                    {
                        ID = venditoreID
                    }
                };
                return View(prodotto);
            }
            catch (Exception ex)
            {
                ViewBag.alert = true;
                ViewBag.Message = ex.Message;
            }
            return View("ShowMyProduct");
        }

        [HttpPost]
        public ActionResult UpdateProduct(Prodotto prodottoForm)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(prodottoForm);
                }
                var wcf = new ServiceReferenceServer.ServiceServerClient();
                ServiceReferenceServer.Prodotto modelServer = new ServiceReferenceServer.Prodotto();
                modelServer.ASIN = prodottoForm.ASIN;
                modelServer.NomeProdotto = prodottoForm.NomeProdotto;
                modelServer.Materiale = prodottoForm.Materiale;
                modelServer.ImmagineProdotto = prodottoForm.ImmagineProdotto;
                modelServer.Categoria = prodottoForm.Categoria;
                modelServer.CostoProdotto = prodottoForm.CostoProdotto;
                modelServer.CostoReso = prodottoForm.CostoReso;
                modelServer.DescrizioneProdotto = prodottoForm.DescrizioneProdotto;
                modelServer.Taglie = prodottoForm.Taglie;
                modelServer.Quantita = prodottoForm.Quantita;
                modelServer.TempoSpedizione = prodottoForm.TempoSpedizione;
                modelServer.Venditore = new ServiceReferenceServer.Utente()
                {
                    ID = prodottoForm.Venditore.ID
                };

                //se non va a buon fine l'aggiornamento ricarico la pagina
                if (!wcf.UpdateProdotto(modelServer))
                {
                    throw new Exception("Aggiornamento prodotto non riuscito! Riprova!");
                }
            }
            catch (Exception ex)
            {
                ViewBag.alert = true;
                ViewBag.Message = ex.Message;
                return View(prodottoForm);
            }
            return View("ShowMyProduct", PassAllMyProduct());
        }

        public ActionResult AddProduct()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddProduct(Prodotto prodottoForm)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception("Dati non compilati in modo corretto!");
                }
                    var wcf = new ServiceReferenceServer.ServiceServerClient();
                if (!wcf.GetUserState(Session["activeID"].ToString()))
                {
                    throw new Exception("L'amministratore ti ha disabilitato!");
                }
                ServiceReferenceServer.Prodotto modelServer = new ServiceReferenceServer.Prodotto()
                {
                    //ASIN = wcf.GenerateASINProdotto(), //genero un ASIN univoco 
                    NomeProdotto = prodottoForm.NomeProdotto,
                    Materiale = prodottoForm.Materiale,
                    ImmagineProdotto = prodottoForm.ImmagineProdotto,
                    Categoria = prodottoForm.Categoria,
                    Quantita = prodottoForm.Quantita,
                    Taglie = prodottoForm.Taglie,
                    DescrizioneProdotto = prodottoForm.DescrizioneProdotto,
                    CostoProdotto = prodottoForm.CostoProdotto,
                    CostoReso = prodottoForm.CostoReso,
                    TempoSpedizione = prodottoForm.TempoSpedizione,
                    Venditore = new ServiceReferenceServer.Utente
                    {
                        ID = Session["activeID"].ToString()
                    }
                };

                if (!wcf.AddProdotto(modelServer))
                {
                    throw new Exception("Errore nell'aggiunta del prodotto!");
                }
                ViewBag.Success = true;
                ViewBag.SuccessMessage = "Aggiunta prodotto avvenuta correttamente!";
            } catch(Exception ex)
            {
                ViewBag.alert = true;
                ViewBag.Message = ex.Message;
            }
            return View("showMyProduct", PassAllMyProduct());
        }

        //Metodo non legato da view bensì utile per raggruppare funzioni comuni del controller
        private ProdottoModel PassAllMyProduct()
        {
            try
            {
                var wcf = new MVCWebSite.ServiceReferenceServer.ServiceServerClient();
                ProdottoModel prodotti = new ProdottoModel();
                ServiceReferenceServer.Credenziali us = new ServiceReferenceServer.Credenziali();
                us.Utente = new ServiceReferenceServer.Utente
                {
                    ID = Session["activeID"].ToString()
                };

                foreach (var el in wcf.GetAllProdottiSeller(us))
                {
                    prodotti.ListaProdotti.Add(el);
                }
                return prodotti;
            }
            catch (Exception ex)
            {
                ViewBag.alert = true;
                ViewBag.Message = ex.Message;
            }
            return null;
        }

        public ActionResult UploadImageProduct(string ASIN)
        {
            try
            {
                Session["imagePath"] = ASIN;
                return View();
            }catch(Exception ex)
            {
                ViewBag.Alert = true;
                ViewBag.Message = ex.Message;
            }
            return View();
        }

        [HttpPost]
        public ActionResult UploadImageProduct(HttpPostedFileBase file)
        {
            try
            {
            if (file == null || file.ContentLength == 0)
            {
                throw new Exception("Non hai immesso un file!");
            }
                string path = Path.Combine(Server.MapPath("~/ImmaginiProdotti"),
                                            Path.GetFileName(Session["imagePath"].ToString() + ".png"));
                file.SaveAs(path); //aggiunge il file nella cartella
                ViewBag.Success = true;
                ViewBag.SuccessMessage = "Immagine del prodotto caricata correttamente!";
            }
            catch (Exception ex)
            {
                ViewBag.Alert = true;
                ViewBag.Message = ex.Message;
            }
            return View();
        }
    }
}