namespace NancyApplication
{
    using Nancy;
    using Nancy.IO;
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
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
                Account result = _accountService.AddAccount(name,pwd);

                var links = new List<HyperMedia>{
                    new HyperMedia { 
                        Href = this.Request.Url, 
                        Rel = "self" 
                    },
                    new HyperMedia {
                        Href = $"{this.Request.Url.SiteBase}/Account/{result.Id}/Update", 
                        Rel = "edit" 
                    }
                };
                result.Links = links;

                return Response.AsJson(result);
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
                _accountService.UpdateAccount(args.accountId, name, pwd);
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
                return HttpStatusCode.OK;
            });
        }
    }
}
