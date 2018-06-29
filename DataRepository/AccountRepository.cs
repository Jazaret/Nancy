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

    public class AccountRepository : BaseRepository, IAccountRepository {
        protected const string AccountsCollection = "AccountsCollection";
        public AccountRepository() : base() {
            Initialize().Wait();
        }

        private async Task Initialize()
        {
            DocumentCollection myCollection = new DocumentCollection();
            myCollection.Id = AccountsCollection;
            await this.client.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri(TopicsDB), new DocumentCollection { Id = AccountsCollection });
        }

        public Account AddAccount(string accountName, string displayName) {
            var account = new Account{
                ID = Guid.NewGuid().ToString(),
                AccountName = accountName,
                AccountPassword = displayName
            };

            var task = CreateAccount(account);
            task.Wait();

            return account;
        }

        private async Task CreateAccount(Account newAccount)
        {
            await this.client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(TopicsDB, AccountsCollection), newAccount);
        }          

        public Account UpdateAccount(string Id, string accountName, string displayName) {
            var account = new Account{
                ID = Guid.NewGuid().ToString(),
                AccountName = accountName,
                AccountPassword = displayName
            };

            var task = ReplaceDocument(account);
            task.Wait();

            return account;
        }

        private async Task ReplaceDocument(Account account)
        {
            await this.client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(TopicsDB, AccountsCollection, account.ID), account);
        }        
    }
}