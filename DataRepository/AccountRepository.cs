namespace NancyApplication
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Net;
    using System.Collections.Generic;
    using Microsoft.Azure.Documents;
    using Microsoft.Azure.Documents.Client;
    using Newtonsoft.Json;

    /// <summary>
    /// Repository that handles the operations for the Account documents in DynamoDB
    /// </summary>
    public class AccountRepository : BaseRepository, IAccountRepository {
        protected const string AccountsCollection = "AccountsCollection";
        public AccountRepository() : base() {
            Initialize().Wait();
        }

        /// <summary>
        /// Creates a client object and creates the Account document Collection if it does not exist.
        /// </summary>
        /// <returns></returns>
        private async Task Initialize()
        {
            DocumentCollection accuonts = new DocumentCollection();
            accuonts.Id = AccountsCollection;
            await this.client.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri(TopicsDB), new DocumentCollection { Id = AccountsCollection });
        }

        /// <summary>
        /// Adds an account object to the repository. Note - password should be stored with salt and hash
        /// </summary>
        public Account AddAccount(string accountName, string password) {
            var account = new Account{
                ID = Guid.NewGuid().ToString(),
                AccountName = accountName,
                AccountPassword = password
            };

            var task = CreateDocument(account);
            task.Wait();

            return account;
        }

        /// <summary>
        /// Update Account document in the repository. Note - password should be stored with salt and hash
        /// </summary>
        public Account UpdateAccount(string Id, string accountName, string password) {
            var account = new Account{
                ID = Guid.NewGuid().ToString(),
                AccountName = accountName,
                AccountPassword = password
            };

            var task = ReplaceDocument(account);
            task.Wait();

            return account;
        }

        private async Task ReplaceDocument(Account account)
        {
            await this.client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(TopicsDB, AccountsCollection, account.ID), account);
        }  

        private async Task CreateDocument(Account account)
        {
            await this.client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(TopicsDB, AccountsCollection), account);
        }               
    }
}