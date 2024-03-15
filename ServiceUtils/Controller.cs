using EmbedIO.Routing;
using EmbedIO;
using EmbedIO.WebApi;
using MTUBankBase.ServiceManager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MTUModelContainer.Auth.Models;
using MTUAuthService.AuthService;
using MTUModelContainer.Database.Models;

namespace MTUAuthService.ServiceUtils
{
    internal class Controller : ServiceStub
    {
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
                var ar = new AuthResult(newUser);
                ar.Success = true;
                ar.Token = token;

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
            var user = await UserManager.GetUserByPhone(EntUserPhone.Phone);

            if (user is null) return new AuthResult() { Success = false, Error = "User doesn't exist" };

            // check if password is correct
            if (!UserManager.IsPasswordCorrect(user, EntUserPhone.Password)) return new AuthResult() { Success = false, Error = "Password isn't correct" };
            
            // check if 2FA is enabled
            if (user.TwoFASecret is not null)
            {
                // create a 2FA token and ask user to enter 2FA info
                var twoFAtoken = await UserManager.IssueTokenForUser(user, TokenType.TwoFA);

                return new AuthResult() { Success = true, TwoFARequired = true, TwoFAToken = twoFAtoken };
            }

            // if no, issue a proper token and let user in
            var token = await UserManager.IssueTokenForUser(user, TokenType.Active);
            var ar = new AuthResult(user);
            ar.Success = true; ar.Token = token;

            return ar;
        }

        [Route(HttpVerbs.Post, "/api/2FA")]
        public async Task<AuthResult> TwoFAVerification([JsonData] TwoFARequest TwoFACode)
        {
            // check if token is 2FA one and it hadn't expired


            // check the 2FA value provided by user
            /*var checkValue = UserManager.VerifyOTP(userToken.Owner, TwoFACode.TwoFAValue);
            if (!checkValue) return new AuthResult() { Success = false, Error = "OTP is incorrect" };

            // generate a proper token for this user
            var newToken = await UserManager.IssueTokenForUser(userToken.Owner, TokenType.Active);

            // generate authresult
            var ar = new AuthResult(userToken.Owner);
            ar.Success = true;
            ar.Token = newToken.TokenValue;

            return ar;*/
            return null;
        }

        [Route(HttpVerbs.Post, "/api/set2FA")]
        public async Task<AuthResult> SetTwoFAStatus([JsonData] Set2FAStatus TwoFACode)
        {
            // ensure user has correct data
            /*var userToken = await CurrentToken;
            if (userToken is null || !userToken.IsAuthed) return new AuthResult() { Success = false, Error = "Not authorized" };

            // set a 2FA token and show it to the user
            var token = await UserManager.Change2FAStatus(userToken.Owner, TwoFACode.NewStatus);
            return token;*/
            return null;
        }
    }
}
