using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace WCFServer
{
    // NOTA: è possibile utilizzare il comando "Rinomina" del menu "Refactoring" per modificare il nome di interfaccia "IService1" nel codice e nel file di configurazione contemporaneamente.
    [ServiceContract]
    public interface IServiceServer
    {
        [OperationContract]
        bool VerifyUserIdentity(Credenziali user);
        [OperationContract]
        string GetIDActiveUser(Credenziali user);
        [OperationContract]
        bool AddUserWithTransaction(Credenziali userInfo);
        [OperationContract]
        List<Carrello> GetAllElementCarrello(Credenziali user);
        [OperationContract]
        List<Prodotto> GetAllProdottiSeller(Credenziali user);
        [OperationContract]
        List<Prodotto> GetAllProdotti(string id);
        [OperationContract]
        Prodotto GetSingleProduct(string ASIN);
        [OperationContract]
        public List<Prodotto> GetProductByCategory(string Categoria);
        [OperationContract]
        bool AddProdotto(Prodotto prodotto);
        [OperationContract]
        bool DeleteProdotto(Prodotto prodotto);
        [OperationContract]
        bool UpdateProdotto(Prodotto prodotto);
        [OperationContract]
        bool AddCartProduct(Carrello carrello);
        [OperationContract]
        int CheckIfAlreadyExistInCartUser(Carrello carrello);
        [OperationContract]
        bool UpdateCartProduct(Carrello carrello);
        [OperationContract]
        double GetCredito(string ID);
        [OperationContract]
        bool CheckIfQtIsOk(string ASIN, int qt);
        [OperationContract]
        bool DeleteProductFromCart(Carrello carrello);
        [OperationContract]
        bool UpdatePortafoglio(Utente utente);
        [OperationContract]
        int GetProductQtAviable(string ASIN);
        [OperationContract]
        bool ShopProducts(Credenziali utente);
        [OperationContract]
        List<CompraVendita> GetVendite(string ID);
        [OperationContract]
        List<CompraVendita> GetAcquisti(string ID);
        [OperationContract]
        List<Credenziali> GetAllUsers();
        [OperationContract]
        bool EnableDisableUser(Utente user);
        [OperationContract]
        string GetRoleOfUser(string ID);
        [OperationContract]
        bool AccreditoVendite(Credenziali credenziali);
        [OperationContract]
        bool GetUserState(string ID);
        [OperationContract]
        Credenziali GetInformationUser(string ID);
        [OperationContract]
        bool UpdateInformationUser(Credenziali userinfo);
        [OperationContract]
        bool CheckOldPassword(string ID, string passw);
        [OperationContract]
        bool AddValutazione(Valutazione valutazione);
        [OperationContract]
        bool CheckIfUserBoughtProduct(string userId, string ASIN);
        [OperationContract]
        public List<Valutazione> GetValutazioni(string ASIN);
        [OperationContract]
        public List<Prodotto> GetLastSales();
        [OperationContract]
        public void WriteLogout(string user);
        [OperationContract]
        bool CheckIfUserAlreadyEvaluated(string userId, string ASIN);
    }
}