using EmbedIO.Routing;
using EmbedIO;
using EmbedIO.WebApi;
using MTUBankBase.ServiceManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTUBankBase.Auth.Models;
using MTUAuthService.AuthService;

namespace MTUAuthService.ServiceUtils
{
    internal class Controller : ServiceStub
    {
        // :D
        [Route(HttpVerbs.Get, "/api/exampleAPI")]
        public async Task<string> ExampleAPI() {
            return await Task.FromResult("Hello world!");
        }

        [Route(HttpVerbs.Post, "/api/registerUser")]
        public async Task<AuthResult> RegisterUser([JsonData] RegisterRequest user)
        {
            // attempting to register the user
            try
            {
                var newUser = UserManager.RegisterUser(user);
            } catch (Exception ex)
            {
                return new AuthResult() { Success = false, Error = ex.Message };
            }

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
