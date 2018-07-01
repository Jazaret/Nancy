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
        public async Task<ActionResult<Account>> AddAccount(Account account) {
            return await CreateDocument(account);            
        }

        /// <summary>
        /// Update Account document in the repository. Note - password should be stored with salt and hash
        /// </summary>
        public async Task<ActionResult<Account>> UpdateAccount(Account account) {
            return await ReplaceAccountOptimistic(account);
        }

        /// <summary>
        /// Creates the account document in the repository
        /// </summary>
        private async Task<ActionResult<Account>> CreateDocument(Account account)
        {
            var result = new ActionResult<Account>();
            try {
                var createResult = await this.Client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(TopicsDB, AccountsCollection), account);
                result.statusCode = createResult.StatusCode;
                var doc = createResult.Resource;
                result.resposeObject = (Account)(dynamic)doc;
                return result;
            } catch (DocumentClientException ex) {
                Console.WriteLine(ex.Message + " " + ex.StatusCode);
                result.statusCode = ex.StatusCode.Value;
                return result;
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
                result.statusCode = HttpStatusCode.InternalServerError;
                return result;
            }
        }

        /// <summary>
        /// Replace account document in repository. Uses ETag match for optimistic concurrency
        /// </summary>
        public async Task<ActionResult<Account>> ReplaceAccountOptimistic(Account item) {
            var result = new ActionResult<Account>();

            var document = (from f in this.Client.CreateDocumentQuery(this.Collection.SelfLink)
                            where f.Id == item.Id
                            select f).AsEnumerable().FirstOrDefault();

            if (document == null) {
                result.statusCode = HttpStatusCode.NotFound;
                return result;
            }

            var editAccount = (Account)(dynamic) document;
            editAccount.ReplaceWith(item);

            var ac = new AccessCondition {Condition = document.ETag, Type = AccessConditionType.IfMatch};
            try {
                var replaceResult = await this.Client.ReplaceDocumentAsync(document.SelfLink, editAccount, new RequestOptions {AccessCondition = ac});
                result.statusCode = replaceResult.StatusCode;
                result.resposeObject = (Account)(dynamic)replaceResult.Resource;
                return result;
            } catch (DocumentClientException ex) {
                Console.WriteLine(ex.Message);
                result.statusCode = ex.StatusCode.Value; 
                return result;
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
                result.statusCode = HttpStatusCode.InternalServerError;
                return result;
            }
        }
    }
}