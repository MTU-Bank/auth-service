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
            if (HttpContext.CurrentClaims is null)
                return new AuthResult() { Success = false, Error = "Incorrect 2FA token" };

            // check if token is 2FA one
            var claims = HttpContext.CurrentClaims.Claims;
            var claimType = claims.FirstOrDefault((z) => z.Type == "type");
            if (Enum.Parse<TokenType>(claimType.Value) != TokenType.TwoFA)
                return new AuthResult() { Success = false, Error = "Incorrect 2FA token" };

            // check the 2FA value provided by user
            var checkValue = UserManager.VerifyOTP(HttpContext.CurrentUser, TwoFACode.TwoFAValue);
            if (!checkValue) return new AuthResult() { Success = false, Error = "OTP is incorrect" };

            // get the proper user from the database, we won't need the abstract one
            var realUser = await UserManager.GetRealUser(HttpContext.CurrentUser);

            // generate a proper token for this user
            var newToken = await UserManager.IssueTokenForUser(realUser, TokenType.Active);

            // generate authresult
            var ar = new AuthResult(HttpContext.CurrentUser);
            ar.Success = true; ar.Token = newToken;

            return ar;
        }

        [Route(HttpVerbs.Post, "/api/set2FA")]
        [RequiresAuth]
        public async Task<AuthResult> SetTwoFAStatus([JsonData] Set2FAStatus TwoFACode)
        {
            var realUser = await UserManager.GetRealUser(HttpContext.CurrentUser);

            // set a 2FA token and show it to the user
            var token = await UserManager.Change2FAStatus(realUser, TwoFACode.NewStatus);
            return token;
        }
    }
}
