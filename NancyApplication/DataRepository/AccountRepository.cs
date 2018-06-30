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
            this.Collection = await this.Client.CreateDocumentCollectionIfNotExistsAsync(UriFactory.CreateDatabaseUri(TopicsDB), new DocumentCollection { Id = AccountsCollection });
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
            return await ReplaceAccountOptimistic(account);
        }

        /// <summary>
        /// Creates the account document in the repository
        /// </summary>
        private async Task<HttpStatusCode> CreateDocument(Account account)
        {
            var result = await this.Client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(TopicsDB, AccountsCollection), account);
            return result.StatusCode;
        }

        /// <summary>
        /// Replace account document in repository. Uses ETag match for optimistic concurrency
        /// </summary>
        public async Task<HttpStatusCode> ReplaceAccountOptimistic(Account item) {

            var document = (from f in this.Client.CreateDocumentQuery(this.Collection.SelfLink)
                            where f.Id == item.Id
                            select f).AsEnumerable().FirstOrDefault();

            if (document == null) {return HttpStatusCode.NotFound;}

            var editAccount = (Account)(dynamic) document;
            editAccount.ReplaceWith(item);

            var ac = new AccessCondition {Condition = document.ETag, Type = AccessConditionType.IfMatch};
            try {
                var result = await this.Client.ReplaceDocumentAsync(document.SelfLink, editAccount, new RequestOptions {AccessCondition = ac});

                return result.StatusCode;
            } catch (DocumentClientException ex) {
                Console.WriteLine(ex.Message);
                return HttpStatusCode.Conflict;
            }
        }
    }
}