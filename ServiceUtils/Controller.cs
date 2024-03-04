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

            // check if password is correct
            if (!UserManager.IsPasswordCorrect(user, EntUserPhone.Password)) return new AuthResult() { Success = false, Error = "Password isn't correct" };
            
            // check if 2FA is enabled
            if (user.TwoFASecret is not null)
            {
                // create a 2FA token and ask user to enter 2FA info
                var twoFAtoken = await UserManager.IssueTokenForUser(user, TokenType.TwoFA);

                return new AuthResult() { Success = true, TwoFARequired = true, TwoFAToken = twoFAtoken.TokenValue };
            }

            // if no, issue a proper token and let user in
            var token = await UserManager.IssueTokenForUser(user, TokenType.Active);
            var ar = (AuthResult)user;
            ar.Success = true; ar.Token = token.TokenValue;

            return ar;
        }

        [Route(HttpVerbs.Post, "/api/2FA")]
        public async Task<AuthResult> TwoFAVerification([JsonData] TwoFARequest TwoFACode)
        {
            var userToken = await CurrentToken;
            // check if token is 2FA one and it hadn't expired
            if (userToken.TokenType != TokenType.TwoFA ||
                (DateTime.Now - userToken.CreationDate) >= TimeSpan.FromMinutes(10))
                return new AuthResult() { Success = false, Error = "Token is incorrect or expired" };

            // check the 2FA value provided by user
            var checkValue = UserManager.VerifyOTP(userToken.Owner, TwoFACode.TwoFAValue);
            if (!checkValue) return new AuthResult() { Success = false, Error = "OTP is incorrect" };

            // generate a proper token for this user
            var newToken = await UserManager.IssueTokenForUser(userToken.Owner, TokenType.Active);

            // generate authresult
            var ar = (AuthResult)userToken.Owner;
            ar.Success = true;
            ar.Token = newToken.TokenValue;

            return ar;
        }

        [Route(HttpVerbs.Post, "/api/set2FA")]
        public async Task<AuthResult> SetTwoFAStatus([JsonData] Set2FAStatus TwoFACode)
        {
            // ensure user has correct data
            var userToken = await CurrentToken;
            if (userToken is null || !userToken.IsAuthed) return new AuthResult() { Success = false, Error = "Not authorized" };

            // set a 2FA token and show it to the user
            var token = await UserManager.Change2FAStatus(userToken.Owner, TwoFACode.NewStatus);
            return token;
        }

        [Route(HttpVerbs.Post, "/api/internal/checkToken")]
        public async Task<Token?> CheckToken([JsonData] CheckTokenRequest tokenReq)
        {
            // ensure the request came from a local service
            if (!LocalServiceAdmin) return null;

            // check the token requested against a db
            using (ApplicationContext db = new ApplicationContext())
            {
                return await db.Tokens.FindAsync(tokenReq.Token);
            }
        }
    }
}
