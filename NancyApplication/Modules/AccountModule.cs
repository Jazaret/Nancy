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

            Post("Account/Add", args =>
            {
                var request = Nancy.Extensions.RequestStreamExtensions.AsString(Nancy.IO.RequestStream.FromStream(this.Request.Body));
                NameValueCollection coll = HttpUtility.ParseQueryString(request);
                var name = coll["name"];
                var pwd = coll["pwd"];
                Account result = _accountService.AddAccount(name,pwd);
                return Response.AsJson(result);
            });

            Put("Account/{accountId}/Update", args =>
            {
                var request = Nancy.Extensions.RequestStreamExtensions.AsString(Nancy.IO.RequestStream.FromStream(this.Request.Body));
                NameValueCollection coll = HttpUtility.ParseQueryString(request);
                var name = coll["name"];
                var pwd = coll["pwd"];
                _accountService.UpdateAccount(args.accountId, name, pwd);
                return HttpStatusCode.OK;
            });
        }
    }
}
