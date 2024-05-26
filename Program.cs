using EmbedIO.Routing;
using EmbedIO.WebApi;
using EmbedIO;
using MTUAuthService.ServiceUtils;
using MTUBankBase.Config;
using MTUBankBase.Helpers;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;
using MTUModelContainer;

namespace MTUAuthService
{
    internal class Program
    {
        public static JwtService jwtService;
        public static ServiceConfig serviceConfig = new ServiceConfig();

        static async Task Main(string[] args)
        {
            // load service configuration
            serviceConfig = ServiceConfig.Load("auth_config.json");

            // build JWT Service
            jwtService = new JwtService("MTUBank", JwtKeyGenerator.GetSecurityKey(serviceConfig.BindToken));

            // init the service
            ServiceInitializer.InitService();

            // connect the service
            await ServiceInitializer.BindServiceAsync();

            await Task.Delay(-1); // we are done here. Go to Controller for Web API methods.
        }
    }
}
