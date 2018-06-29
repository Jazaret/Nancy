namespace NancyApplication
{
    using Microsoft.Extensions.Caching.Distributed;
    using Nancy;
    using System;
    using System.Collections.Generic;

    public class AccountModule : NancyModule
    {
        private IAccountService _accountService;

        public AccountModule(IAccountService accountService)
        {
            _accountService = accountService;

            Post("Account/Add/Name={name}&Desc={description}", args =>
            {
                Account result = _accountService.AddAccount(args.name, args.description);
                return Response.AsJson(result);
            });

            Put("Account/{accountId}/Update/Name={name}&Desc={description}", args =>
            {
                _accountService.UpdateAccount(args.accountId, args.name, args.description);
                return HttpStatusCode.OK;
            });

            //Get("/test/{name}", args => new Task() { Name = args.name });
        }
    }
}
