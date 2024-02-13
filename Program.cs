using MTUAuthService.ServiceUtils;
using MTUBankBase.Config;
using MTUBankBase.Helpers;

namespace MTUAuthService
{
    internal class Program
    {
        public static ServiceConfig serviceConfig;

        static async Task Main(string[] args)
        {
            // load service configuration
            serviceConfig = ServiceConfig.Load("auth_config.json");

            // init the service
            ServiceInitializer.InitService();

            // connect the service
            await ServiceInitializer.BindServiceAsync();

            await Task.Delay(-1); // we are done here. Go to Controller for Web API methods.
        }
    }
}
