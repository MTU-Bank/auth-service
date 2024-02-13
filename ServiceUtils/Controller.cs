using EmbedIO.Routing;
using EmbedIO;
using EmbedIO.WebApi;
using MTUBankBase.ServiceManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTUAuthService.ServiceUtils
{
    internal class Controller : ServiceStub
    {
        // :D
        [Route(HttpVerbs.Get, "/api/exampleAPI")]
        public async Task<string> ExampleAPI() {
            return await Task.FromResult("Hello world!");
        }
    }
}
