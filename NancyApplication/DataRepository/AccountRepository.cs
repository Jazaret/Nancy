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
            await this.Client.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri(TopicsDB), new DocumentCollection { Id = AccountsCollection });
        }

        /// <summary>
        /// Adds an account object to the repository. Note - password should be stored with salt and hash
        /// </summary>
        public async Task<HttpStatusCode> AddAccount(Account account) {
            return await CreateDocument(account);            
        }

        /// <summary>
        /// Update Account document in the repository. Note - password should be stored with salt and hash
        /// </summary>
        public async Task<HttpStatusCode> UpdateAccount(Account account) {
            return await ReplaceDocument(account);
        }

        /// <summary>
        /// Replace account document in repository. Uses ETag match for optimistic concurrency
        /// </summary>
        private async Task<HttpStatusCode> ReplaceDocument(Account account)
        {
            var ac = new AccessCondition {Condition = account.ETag, Type = AccessConditionType.IfMatch};
            var result = await this.Client.ReplaceDocumentAsync(UriFactory.CreateDocumentUri(TopicsDB, AccountsCollection, account.Id), account, new RequestOptions {AccessCondition = ac});
            return result.StatusCode;
        }  

        /// <summary>
        /// Creates the account document in the repository
        /// </summary>
        private async Task<HttpStatusCode> CreateDocument(Account account)
        {
            var result = await this.Client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(TopicsDB, AccountsCollection), account);
            return result.StatusCode;
        }               
    }
}