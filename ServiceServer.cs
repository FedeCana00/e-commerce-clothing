using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace WCFServer
{
    // NOTA: è possibile utilizzare il comando "Rinomina" del menu "Refactoring" per modificare il nome di classe "Service1" nel codice e nel file di configurazione contemporaneamente.
    public class ServiceServer : IServiceServer
    {
        public bool AddUserWithTransaction(Credenziali userInfo)
        {
            return Program.Db.AddUserWithTransaction(userInfo);      
        }

        public bool VerifyUserIdentity(Credenziali user)
        {
            return Program.Db.VerifyUserIdentity(user);
        }

        public string GetIDActiveUser(Credenziali user)
        {
            return Program.Db.GetIDActiveUser(user);
        }

        public List<Carrello> GetAllElementCarrello(Credenziali user)
        {
            return Program.Db.GetAllElementOfCarrello(user);
        }

        public List<Prodotto> GetAllProdottiSeller(Credenziali user)
        {
            return Program.Db.GetAllProdottiOfSeller(user);
        }

        public List<Prodotto> GetAllProdotti(string id)
        {
            return Program.Db.GetAllProdotti(id);
        }
        public Prodotto GetSingleProduct(string ASIN)
        {
            return Program.Db.GetSingleProduct(ASIN);
        }
        public List<Prodotto> GetProductByCategory(string Categoria)
        {
            return Program.Db.GetProductByCategory(Categoria);
        }
        public bool AddProdotto(Prodotto prodotto)
        {
            return Program.Db.AddProdotto(prodotto);
        }

        public bool DeleteProdotto(Prodotto prodotto)
        {
            return Program.Db.DeleteProdotto(prodotto);
        }

        public bool UpdateProdotto(Prodotto prodotto)
        {
            return Program.Db.UpdateProdotto(prodotto);
        }

        public bool AddCartProduct(Carrello carrello)
        {
            return Program.Db.AddCartProduct(carrello);
        }

        public int CheckIfAlreadyExistInCartUser(Carrello carrello)
        {
            return Program.Db.CheckIfAlreadyExistInCartUser(carrello);
        }
        
        public bool UpdateCartProduct(Carrello carrello)
        {
            return Program.Db.UpdateCartProduct(carrello);
        }

        public double GetCredito(string ID)
        {
            return Program.Db.GetCredito(ID);
        }

        public bool UpdatePortafoglio(Utente utente)
        {
            return Program.Db.UpdatePortafoglio(utente);
        }

        public bool CheckIfQtIsOk(string ASIN, int qt)
        {
            return Program.Db.CheckIfQtIsOk(ASIN, qt);
        }

        public bool DeleteProductFromCart(Carrello carrello)
        {
            return Program.Db.DeleteProductFromCart(carrello);
        }

        public int GetProductQtAviable(string ASIN)
        {
            return Program.Db.GetProductQtAviable(ASIN);
        }

        public bool ShopProducts(Credenziali utente)
        {
            return Program.Db.ShopProducts(utente);
        }

        public List<CompraVendita> GetVendite(string ID)
        {
            return Program.Db.GetVendite(ID);
        }
        public List<CompraVendita> GetAcquisti(string ID)
        {
            return Program.Db.GetAcquisti(ID);
        }

        public List<Credenziali> GetAllUsers()
        {
            return Program.Db.GetAllUsers();
        }

        public bool EnableDisableUser(Utente user)
        {
            return Program.Db.EnableDisableUser(user);
        }

        public string GetRoleOfUser(string ID)
        {
            return Program.Db.GetRoleOfUser(ID);
        }

        public bool AccreditoVendite(Credenziali credenziali)
        {
            return Program.Db.AccreditoVendite(credenziali);
        }

        public bool GetUserState(string ID)
        {
            return Program.Db.GetUserState(ID);
        }

        public Credenziali GetInformationUser(string ID)
        {
            return Program.Db.GetInformationUser(ID);
        }

        public bool UpdateInformationUser(Credenziali userinfo)
        {
            return Program.Db.UpdateInformationUser(userinfo);
        }

        public bool CheckOldPassword(string ID, string passw)
        {
            return Program.Db.CheckOldPassword(ID, passw);
        }

        public bool AddValutazione(Valutazione valutazione)
        {
            return Program.Db.AddValutazione(valutazione);
        }

        public bool CheckIfUserBoughtProduct(string userId, string ASIN)
        {
            return Program.Db.CheckIfUserBoughtProduct(userId, ASIN);
        }

        public List<Valutazione> GetValutazioni(string ASIN)
        {
            return Program.Db.GetValutazioni(ASIN);
        }

        public List<Prodotto> GetLastSales()
        {
            return Program.Db.GetLastSales();
        }

        public void WriteLogout(string user)
        {
            Program.ConsoleManager.WriteLogout(user);
        }

        public bool CheckIfUserAlreadyEvaluated(string userId, string ASIN)
        {
            return Program.Db.CheckIfUserAlreadyEvaluated(userId, ASIN);
        }
    }
}