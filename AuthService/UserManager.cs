using EmbedIO;
using Microsoft.EntityFrameworkCore;
using MTUBankBase.Auth.Models;
using MTUBankBase.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TwoStepsAuthenticator;

namespace MTUAuthService.AuthService
{
    public class UserManager
    {
        /// <summary>
        /// Данный метод регистрирует пользователя в системе
        /// </summary>
        /// <param name="user"></param>
        public static async Task<User> RegisterUser(RegisterRequest req)
        {
            // making sure the user doesn't already exist
            var userExists = await DoesUserExist(req) is null;
            if (userExists) throw new HttpException(409, "Such user account already exists");

            using (ApplicationContext db = new ApplicationContext())
            {
                var passwordReq = req.Password;
                var userReq = (User)req;
                userReq.PasswordHash = GeneratePwdHash(userReq, passwordReq);

                db.Users.Add(userReq);
                await db.SaveChangesAsync();
            }

            return req;
        }

        /// <summary>
        /// Данный метод получает пользователя по уникальному ID
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public static async Task<User?> GetUser(string uid)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                var userApplicable = from z in db.Users where z.Id == uid select z;
                return await userApplicable.FirstOrDefaultAsync();
            }
        }

        /// <summary>
        /// Данный метод создаёт токен для пользователя
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        public static async Task<Token> IssueTokenForUser(User u, TokenType type)
        {
            if (u is null) throw new ArgumentNullException(nameof(u));

            // create an appropriate token
            using (ApplicationContext db = new ApplicationContext())
            {
                var randomString = RandomProvider.GenerateRandomString();
                var newToken = new Token() { CreationDate = DateTime.Now, Owner = u, TokenType = type, TokenValue = randomString };
                db.Tokens.Add(newToken);
                await db.SaveChangesAsync();
                return newToken;
            }
        }

        /// <summary>
        /// Проверяет существование пользователя в БД
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        public static async Task<User?> DoesUserExist(User u)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                var possibleUser = from z in db.Users where z.Id == u.Id || z.PhoneNum == u.PhoneNum || z.Email == u.Email select z;
                return await possibleUser.FirstOrDefaultAsync();
            }
        }

        public static async Task<AuthResult> Change2FAStatus(User user, bool enable)
        {
            // Generate a shared secret for the user
            string sharedSecret = RandomProvider.GenerateRandomString(32);
            // Store the shared secret in the user's database record
            // StoreSharedSecretInDatabase(username, sharedSecret);

            using (ApplicationContext db = new ApplicationContext())
            {
                user.TwoFASecret = sharedSecret;
                db.Entry(user).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return new AuthResult() { Success = true, Token = sharedSecret };
            }
        }

        public static bool VerifyOTP(User user, string OTP)
        {
            // Retrieve the shared secret from the user's database record
            string sharedSecret = user.TwoFASecret;

            // Verify the user's entered OTP
            var authenticator = new TimeAuthenticator();
            return authenticator.CheckCode(sharedSecret, OTP);
        }

        /// <summary>
        /// Данный метод выполняет хеширование пароля
        /// </summary>
        /// <param name="user"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public static string GeneratePwdHash(User user, string pwd)
        {
            string rawPwd = $"{user.Id}{pwd}{user.Email}";
            using (var sha = SHA256.Create())
            {
                return BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(rawPwd))).ToLower().Replace("-", "");
            }
        }

        public static bool IsPasswordCorrect(User user, string pwd) => user.PasswordHash == GeneratePwdHash(user, pwd);
    }
}
