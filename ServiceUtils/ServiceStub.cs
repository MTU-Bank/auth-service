using EmbedIO.Routing;
using EmbedIO;
using MTUBankBase.Helpers;
using MTUBankBase.ServiceManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmbedIO.WebApi;
using MTUBankBase.Database.Models;
using MTUBankBase.Auth;

namespace MTUAuthService.ServiceUtils
{
    public class ServiceStub : WebApiController
    {
        public string Name { get; } = "MTU Bank Auth Service";
        public string Description { get; } = "This service concludes authentication, authorization and identification of users.";
        public string BaseUrl { get; } = WebControllerMethods.BindString(Program.serviceConfig.Hostname, Program.serviceConfig.Port);
        public ServiceType ServiceType { get; } = ServiceType.Auth;

        [Route(HttpVerbs.Get, "/getStatus")]
        public async Task<string> GetStatus() => "OK";

        [Route(HttpVerbs.Get, "/getServiceInfo")]
        public async Task<Service> GetServiceInfo() => new Service(ServiceInitializer.LocalService);

        [Route(HttpVerbs.Get, "/disconnectService")]
        public async Task<string> DisconnectService() => "OK";

        public Task<Token?> CurrentToken
        {
            get => AuthValidator.GetCurrentToken(HttpContext);
        }
    }
}
