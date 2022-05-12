using MVCWebSite.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCWebSite.Controllers
{
    public class SingleProductController : Controller
    {
        // GET: SingleProduct
        private static readonly int NUMBERPRODUCTSINPAGE = 9;
        public ActionResult SingleProduct(string ASIN)
        {
            try
            {
                var wcf = new MVCWebSite.ServiceReferenceServer.ServiceServerClient();
                ServiceReferenceServer.Prodotto prodotto = new ServiceReferenceServer.Prodotto();
                prodotto = wcf.GetSingleProduct(ASIN);

                Prodotto viewprodotto = new Prodotto()
                {
                    ASIN = prodotto.ASIN,
                    NomeProdotto = prodotto.NomeProdotto,
                    Quantita = prodotto.Quantita,
                    Taglie = prodotto.Taglie,
                    Materiale = prodotto.Materiale,
                    CostoReso = prodotto.CostoReso,
                    CostoProdotto = prodotto.CostoProdotto,
                    TempoSpedizione = prodotto.TempoSpedizione,
                    Venditore = new Utente()
                    {
                        ID = prodotto.Venditore.ID
                    },
                    Categoria = prodotto.Categoria,
                    DescrizioneProdotto = prodotto.DescrizioneProdotto,
                    ImmagineProdotto = prodotto.ImmagineProdotto
                };
                //verifico se devo o no stampre il pulsante per la valutazione utente
                if (Session["activeID"] != null)
                {
                    ViewBag.Valutazione = wcf.CheckIfUserBoughtProduct(Session["activeID"].ToString(), prodotto.ASIN);
                }

                ViewBag.Visualizza = ASIN != null;

                return View("SingleProduct", viewprodotto); //valore di ritorno errato bisogna passare 
            }
            catch (Exception ex)
            {
                ViewBag.alert = true;
                ViewBag.Message = ex.Message;
            }
            return View("CardProduct");
        }

        public ActionResult CardProduct(int page = 0) //valore di default di pagina è zero
        {
            try
            {
                return View("CardProduct", PassAllProduct(page));
            } catch(Exception ex)
            {
                ViewBag.alert = true;
                ViewBag.alertMessage = ex.Message;
            }
            return View("~/Views/Home/Index.cshtml"); //in caso di errore ritorno alla view index

        }

        //Gestione del carrello dell'utente => aggiunge un elemento al carrello
        public ActionResult AddCartProduct(string ASIN, int quantita, double costoProdotto)
        {
            try
            {
                if (Session["activeUSername"] == null)
                {
                    throw new Exception("Non sei loggato!");
                }

                if (Session["activeRole"].ToString() == "V")
                {
                    throw new Exception("Le società non possono acquistare!");
                }

                var wcf = new ServiceReferenceServer.ServiceServerClient();
                string id = Session["activeID"].ToString();
                if (!wcf.GetUserState(id))
                {
                    throw new Exception("L'amministratore ti ha disabilitato!");
                }

                ServiceReferenceServer.Carrello carrello = new ServiceReferenceServer.Carrello()
                {
                    Prodotto = new ServiceReferenceServer.Prodotto()
                    {
                        ASIN = ASIN
                    },
                    Quantita = quantita,
                    CostoTot = costoProdotto,
                    Utente = new ServiceReferenceServer.Utente()
                    {
                        ID = id
                    }
                };
                //ottengo la quantità di prodottio già presente del DB
                int qtInDB = wcf.CheckIfAlreadyExistInCartUser(carrello);
                if (qtInDB == 0)
                {
                    if (!wcf.AddCartProduct(carrello))
                    {
                        throw new Exception("Aggiunta al carrello non riuscita!");
                    }
                }
                else if (qtInDB < 0)
                {
                    throw new Exception("Errore nella quantità trovata nel DB!");
                }
                else
                {
                    //aggiorno le quantità e il costo dei prodotti
                    carrello.Quantita = qtInDB + 1;
                    carrello.CostoTot = costoProdotto * carrello.Quantita;
                    if (!wcf.UpdateCartProduct(carrello))
                    {
                        throw new Exception("Errore nell'aggiornamento della quantita!");
                    }
                }
            }
            catch (Exception ex)
            {
                //Vedere come gestire le eccezioni a video
                ViewBag.Alert = true;
                ViewBag.Message = ex.Message;
            }
            //ViewBag.TotalPrize = TotalPrizeCart(); //setto il valore di prezzo totale
            return View("Carrello", PassAllMyCart());
        }

        public ActionResult Carrello()
        {
            try
            {
                return View(PassAllMyCart());
            }
            catch (Exception ex)
            {
                ViewBag.alert = true;
                ViewBag.Message = ex.Message;
            }
            return View("Error");
        }

        //Da pulsante - nel carrello diminuisco la quantità nel carrello del prodotto
        public ActionResult SubElementCard(string ASIN, int quantita, double prezzoProdotto)
        {
            try
            {
                var wcf = new ServiceReferenceServer.ServiceServerClient();
                if (!wcf.GetUserState(Session["activeID"].ToString()))
                {
                    throw new Exception("L'amministratore ti ha disabilitato!");
                }

                ServiceReferenceServer.Carrello carrello = new ServiceReferenceServer.Carrello()
                {
                    Prodotto = new ServiceReferenceServer.Prodotto()
                    {
                        ASIN = ASIN
                    },
                    Quantita = quantita - 1,
                    CostoTot = prezzoProdotto * (quantita - 1),
                    Utente = new ServiceReferenceServer.Utente()
                    {
                        ID = Session["activeID"].ToString()
                    }
                };
                
                if (!wcf.UpdateCartProduct(carrello))
                {
                    throw new Exception("Errore nello della quantità del prodotto!");
                }
            } catch(Exception ex)
            {
                ViewBag.Alert = true;
                ViewBag.Message = ex.Message;
            }
            return View("Carrello", PassAllMyCart());
        }

        //Da pulsante + nel carrello aumento la quantità nel carrello del prodotto
        public ActionResult AddElementCard(string ASIN, int quantita, double prezzoProdotto)
        {
            try
            {
                var wcf = new ServiceReferenceServer.ServiceServerClient();
                if (!wcf.GetUserState(Session["activeID"].ToString()))
                {
                    throw new Exception("L'amministratore ti ha disabilitato!");
                }
                ServiceReferenceServer.Carrello carrello = new ServiceReferenceServer.Carrello()
                {
                    Prodotto = new ServiceReferenceServer.Prodotto()
                    {
                        ASIN = ASIN
                    },
                    Quantita = quantita + 1,
                    CostoTot = prezzoProdotto * (quantita + 1),
                    Utente = new ServiceReferenceServer.Utente()
                    {
                        ID = Session["activeID"].ToString()
                    }
                };

                if (!wcf.CheckIfQtIsOk(ASIN, carrello.Quantita)) //creare un check per la quantità disponibile
                {
                    throw new Exception("Quantita' del prodotto non disponibile!");
                }
                if (!wcf.UpdateCartProduct(carrello))
                {
                    throw new Exception("Errore nell'aggiunta della quantità del prodotto!");
                }
            } catch(Exception ex)
            {
                ViewBag.Alert = true;
                ViewBag.Message = ex.Message;
            }
            return View("Carrello", PassAllMyCart());
        }

        //Da pulsante elimina nel carrello elimino prodotto dal carrello del utente
        public ActionResult DeleteFromCart(string ASIN, string nomeProdotto)
        {
            try
            {
                var wcf = new ServiceReferenceServer.ServiceServerClient();
                if (!wcf.GetUserState(Session["activeID"].ToString()))
                {
                    throw new Exception("L'amministratore ti ha disabilitato!");
                }
                ServiceReferenceServer.Carrello carrello = new ServiceReferenceServer.Carrello()
                {
                    Prodotto = new ServiceReferenceServer.Prodotto()
                    {
                        ASIN = ASIN,
                        NomeProdotto = nomeProdotto
                    },
                    Utente = new ServiceReferenceServer.Utente()
                    {
                        ID = Session["activeID"].ToString()
                    }
                };
                
                if (!wcf.DeleteProductFromCart(carrello))
                {
                    throw new Exception("Errore eliminazione prodotto dal carrello!");
                }
            }
            catch (Exception ex)
            {
                ViewBag.Alert = true;
                ViewBag.Message = ex.Message;
            }
            return View("Carrello", PassAllMyCart());
        }

        //Gestisco l'acquisto da parte di un utente
        public ActionResult ShopCartProduct(double totale)
        {
            try
            {
                if (Session["activeRole"].ToString() == "V")
                {
                    throw new Exception("Le società non possono acquistare!");
                }

                if(totale == 0)
                {
                    throw new Exception("Nessun prodotto nel carrello!");
                }
                var wcf = new MVCWebSite.ServiceReferenceServer.ServiceServerClient();
                if (!wcf.GetUserState(Session["activeID"].ToString()))
                {
                    throw new Exception("L'amministratore ti ha disabilitato!");
                }
                ServiceReferenceServer.Credenziali utente = new ServiceReferenceServer.Credenziali
                {
                    Username = Session["activeUsername"].ToString(),
                    Utente = new ServiceReferenceServer.Utente
                    {
                        ID = Session["activeID"].ToString()
                    }
                };
                if (wcf.GetCredito(utente.Utente.ID) >= totale)
                {
                    utente.Utente.Portafoglio = -totale;
                    if (!wcf.UpdatePortafoglio(utente.Utente))
                    {
                        throw new Exception("Errore nell'operazione di acquisto!");
                    }

                    if (!wcf.AccreditoVendite(utente))
                    {
                        throw new Exception("Errore nell'operazione di acquisto!");
                    }

                    if (!wcf.ShopProducts(utente))
                    {
                        throw new Exception("Errore nell'operazione di acquisto!");
                    }
                    ViewBag.AlertSucces = true;
                    ViewBag.MessageSuccess = "Acquisto andato a buon fine!";

                }
                else
                {
                    ViewBag.Alert = true;
                    ViewBag.Message = "Credito Insufficiente!";
                }
               
            }
            catch (Exception ex)
            {
                ViewBag.Alert = true;
                ViewBag.Message = ex.Message;
            }
            return View("Carrello",PassAllMyCart());
        }

        //Metodo non utilizzato come ActionResult bensì per raggruppare funzionalità comuni
        public CarrelloModel PassAllMyCart()
        {
            try
            {
                if(Session["activeID"] == null)
                {
                    throw new Exception("Non sei loggato!");
                }
                var wcf = new MVCWebSite.ServiceReferenceServer.ServiceServerClient();
                CarrelloModel carrello = new CarrelloModel();
                ServiceReferenceServer.Credenziali credenziali = new ServiceReferenceServer.Credenziali()
                {
                    Utente = new ServiceReferenceServer.Utente
                    {
                        ID = Session["activeID"].ToString()
                    }
                };
                bool printEl;
                foreach (var el in wcf.GetAllElementCarrello(credenziali))
                {

                    printEl = true;
                    if (!wcf.CheckIfQtIsOk(el.Prodotto.ASIN, el.Quantita))
                    {
                        //stampo il messaggio di alert per avvisare l'utente del controllo di disponibilità dei prodotti nel carrello
                        ViewBag.AlertWarning = true;
                        ViewBag.MessageWarning = "Attenzione potrebbero esser cambiate qt e/o prodotti poichè non più disponibili!";

                        //ottengo la qt disponibile nel DB
                        int qtAviable = wcf.GetProductQtAviable(el.Prodotto.ASIN);
                        if(qtAviable == 0)
                        {
                            //imposto l'ID utente dell'elemento prima di effettuare operazioni nel DB
                            el.Utente = new ServiceReferenceServer.Utente
                            {
                                ID = Session["activeID"].ToString()
                            };
                            if (!wcf.DeleteProductFromCart(el))
                            {
                                throw new Exception("Errore nell'eliminazione di un prodotto dal carrello!");
                            } else
                            {
                                printEl = false; //non stampo il prodotto poichè non più in DB
                            }
                        } else if(qtAviable < 0)
                        {
                            throw new Exception("Errore nell'ottenimento della qt disponibile dal DB!");
                        } else
                        {
                            //aggiorno la quantità del prodotto nel carrello
                            el.Quantita = qtAviable;
                            el.CostoTot = el.Quantita * el.Prodotto.CostoProdotto;
                            el.Utente = new ServiceReferenceServer.Utente()
                            {
                                ID = Session["activeID"].ToString()
                            };
                            if (!wcf.UpdateCartProduct(el))
                            {
                                throw new Exception("Errore nell'aggiornamento di un prodotto nel carrello!");
                            }
                        }
                    }
                    if (printEl)
                    {
                        carrello.ListaElementiCarrello.Add(el);
                    }
                }
                ViewBag.TotalPrize = TotalPrizeCart(); //setto il valore di prezzo totale
                return carrello;
            } catch(Exception ex)
            {
                ViewBag.Alert = true;
                ViewBag.Message = ex.Message;
            }
            return null;
        }

        //METODO per ottenere il prezzo totale del carrello
        public double TotalPrizeCart()
        {
            try
            {
                if (Session["activeID"] == null)
                {
                    throw new Exception("Non sei loggato!");
                }
                var wcf = new MVCWebSite.ServiceReferenceServer.ServiceServerClient();
                ServiceReferenceServer.Credenziali credenziali = new ServiceReferenceServer.Credenziali()
                {
                    Utente = new ServiceReferenceServer.Utente
                    {
                        ID = Session["activeID"].ToString()
                    }
                };
                double prize = 0;
                foreach (var el in wcf.GetAllElementCarrello(credenziali))
                {
                    prize += el.CostoTot;
                }
                return prize;
            } catch(Exception ex)
            {
                ViewBag.Alert = true;
                ViewBag.Message = ex.Message;
            }
            return 0;
        }


        public ActionResult ProductByCategory(string categoria)
        {
            try
            {
                var wcf = new MVCWebSite.ServiceReferenceServer.ServiceServerClient();
                ProdottoModel prodotti = new ProdottoModel();

                foreach (var el in wcf.GetProductByCategory(categoria))
                {
                    prodotti.ListaProdotti.Add(el);
                }
                ViewBag.Title = categoria;
                ViewBag.Visualizza = categoria != null;
                return View("ProductByCategory", prodotti); //valore di ritorno errato bisogna passare 
            }
            catch (Exception ex)
            {
                ViewBag.alert = true;
                ViewBag.Message = ex.Message;
            }
            return View("CardProduct", PassAllProduct(0));
        }


        [ChildActionOnly]
        public ActionResult ValutazioniProdotto(string ASIN)
        {
            try
            {
                var wcf = new MVCWebSite.ServiceReferenceServer.ServiceServerClient();
                ValutazioniModel valutazioni = new ValutazioniModel();


                foreach (var el in wcf.GetValutazioni(ASIN))
                {
                    valutazioni.ListaValutazioni.Add(el);
                }

                return PartialView("ValutazioniProdotto", valutazioni);
            }
            catch (Exception ex)
            {
                ViewBag.alert = true;
                ViewBag.Message = ex.Message;
            }
            return View("SingleProduct");
        }

        public ProdottoModel PassAllProduct(int page)
        {
            string id = (string)Session["activeID"];
            var wcf = new MVCWebSite.ServiceReferenceServer.ServiceServerClient();
            ProdottoModel prodotti = new ProdottoModel();
            var listOfProduct = wcf.GetAllProdotti(id);
            int numerOfProduct = listOfProduct.Length;
            //prendo solo una parta di elementi pagina per pagina
            var prodottiInPageNumber = listOfProduct.Skip(page * NUMBERPRODUCTSINPAGE).Take(NUMBERPRODUCTSINPAGE);
            prodottiInPageNumber.ToList(); //tramuto in lista gli elementi otttenuti

            foreach (var el in prodottiInPageNumber)
            {
                prodotti.ListaProdotti.Add(el);
            }
            //controllo se ho altri elementi da stampare
            if (numerOfProduct > (page + 1) * NUMBERPRODUCTSINPAGE)
            {
                ViewBag.NextPage = page + 1;
            }
            //controllo se ci sono pagine precedenti alla corrente
            if (page >= 1)
            {
                ViewBag.PreviousPage = page - 1;
            }
            return prodotti;
        }
    }
}