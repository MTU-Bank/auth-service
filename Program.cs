using EmbedIO.Routing;
using EmbedIO.WebApi;
using EmbedIO;
using MTUAuthService.ServiceUtils;
using MTUBankBase.Auth.Models;
using MTUBankBase.Config;
using MTUBankBase.Helpers;

namespace MTUAuthService
{
    public class UserManager
    {
        public static void Add(RegisterRequest user)
        {
            using (ApplicationContext db = new ApplicationContext())
            {


                db.UsersTable.Add(user);
                db.SaveChanges();

            }
        }

        public static AuthResult GetUser(string EntUserPhone)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                var res = db.UsersTabel.FirstOrDefault(res => res.phone_number == EntUserPhone);
                Verification.TwoFactorAuth.Enable2FA(res.username);
                return res;
            }

        }

    }

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

        [Route(HttpVerbs.Post, "/api/registerUser")]
        public async Task<AuthResult> DescribePerson([JsonData] RegisterRequest user)
        {
            UserManager.Add(user);

            return user;
        }

        [Route(HttpVerbs.Post, "/api/loginUser")]
        public async Task<AuthResult> LoginPerson([JsonData] AuthRequest EntUserPhone)
        {

            return UserManager.GetUser(EntUserPhone);
        }

        [Route(HttpVerbs.Post, "/api/2FA")]
        public async Task<bool> TwoFAVerification([JsonData] TwoFARequest TwoFACode)
        {
            string x = TwoFACode.TwoFAToken;
            string y = TwoFACode.TwoFAValue;
            return Verification.TwoFactorAuth.VerifyOTP(username: x, userEnteredOTP: y);
        }
    }
}
