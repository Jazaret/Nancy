namespace NancyApplication
{
    using Nancy;
    using Nancy.IO;
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Threading.Tasks;
    using System.Web;


    /// <summary>
    /// Nancy module that handles the endpoints for Account actions.
    /// </summary>
    public class AccountModule : NancyModule
    {
        private IAccountService _accountService;

        public AccountModule(IAccountService accountService)
        {
            _accountService = accountService;

            /// <summary>
            /// Adds account to repository.  Body must contain Query string of name="AccountNameHere"&pwd="PasswordHere"
            /// </summary>
            Post("Account/Add", args =>
            {
                var request = Nancy.Extensions.RequestStreamExtensions.AsString(Nancy.IO.RequestStream.FromStream(this.Request.Body));
                NameValueCollection coll = HttpUtility.ParseQueryString(request);
                var name = coll["name"];
                var pwd = coll["pwd"];
                Task<ActionResult<Account>> addTask = _accountService.UpdateAccount(args.accountId, name, pwd);
                ActionResult<Account> addResult = addTask.Result;
                if (addResult.statusCode != (System.Net.HttpStatusCode)HttpStatusCode.Created) {
                    return addResult.statusCode;
                }
                var account = addResult.resposeObject;

                var links = new List<HyperMedia>{
                    new HyperMedia { 
                        Href = this.Request.Url, 
                        Rel = "self" 
                    },
                    new HyperMedia {
                        Href = $"{this.Request.Url.SiteBase}/Account/{account.Id}/Update", 
                        Rel = "edit" 
                    }
                };

                return Response.AsJson(new {account = account, links = links});
            });

            /// <summary>
            /// Updates account on registry.  Body must contain Query string of name="AccountNameHere"&pwd="PasswordHere"
            /// </summary>
            Put("Account/{accountId}/Update", args =>
            {
                var request = Nancy.Extensions.RequestStreamExtensions.AsString(Nancy.IO.RequestStream.FromStream(this.Request.Body));
                NameValueCollection coll = HttpUtility.ParseQueryString(request);
                var name = coll["name"];
                var pwd = coll["pwd"];
                Task<ActionResult<Account>> updateTask = _accountService.UpdateAccount(args.accountId, name, pwd);
                ActionResult<Account> updatResult = updateTask.Result;
                if (updatResult.statusCode != (System.Net.HttpStatusCode)HttpStatusCode.OK) {
                    return updatResult.statusCode;
                }
                var account = updatResult.resposeObject;
                var links = new List<HyperMedia>{
                    new HyperMedia { 
                        Href = this.Request.Url, 
                        Rel = "self" 
                    },
                    new HyperMedia {
                        Href = $"{this.Request.Url.SiteBase}/Account/Add", 
                        Rel = "add"
                    }
                };
                return Response.AsJson(new {account = account, links = links});
            });
        }
    }
}
