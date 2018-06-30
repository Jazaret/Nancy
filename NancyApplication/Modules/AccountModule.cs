namespace NancyApplication
{
    using Microsoft.Extensions.Caching.Distributed;
    using Nancy;
    using System;
    using System.Collections.Generic;


    /// <summary>
    /// Nancy module that handles the endpoints for Account actions.
    /// </summary>
    public class AccountModule : NancyModule
    {
        private IAccountService _accountService;

        public AccountModule(IAccountService accountService)
        {
            _accountService = accountService;

            Post("Account/Add/Name={name}", args =>
            {
                var password = Request.Body.ToString();
                Account result = _accountService.AddAccount(args.name, password);
                return Response.AsJson(result);
            });

            Put("Account/{accountId}/Update/Name={name}", args =>
            {
                var password = Request.Body.ToString();
                _accountService.UpdateAccount(args.accountId, args.name, password);
                return HttpStatusCode.OK;
            });
        }
    }
}
