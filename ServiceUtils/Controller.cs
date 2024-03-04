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
using MTUBankBase.Database.Models;

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
                var newUser = await UserManager.RegisterUser(user);

                // now we generate a token for the user
                var token = await UserManager.IssueTokenForUser(newUser, TokenType.Active);

                // generate authresult
                var ar = (AuthResult)newUser;
                ar.Success = true;
                ar.Token = token.TokenValue;

                return ar;
            } catch (Exception ex)
            {
                return new AuthResult() { Success = false, Error = ex.Message };
            }
        }

        [Route(HttpVerbs.Post, "/api/loginUser")]
        public async Task<AuthResult> LoginPerson([JsonData] AuthRequest EntUserPhone)
        {
            // first we shall get the user requested
            var user = await UserManager.GetUser(EntUserPhone.Phone);

            if (user is null) return new AuthResult() { Success = false, Error = "User doesn't exist" };
            
            // check if 2FA is enabled
            if (user.TwoFASecret is not null)
            {
                // create a 2FA token and ask user to enter 2FA info
                var twoFAtoken = await UserManager.IssueTokenForUser(user, TokenType.TwoFA);

            }
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
