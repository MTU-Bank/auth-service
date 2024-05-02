using EmbedIO;
using Microsoft.EntityFrameworkCore;
using MTUModelContainer.Auth.Models;
using MTUModelContainer.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
            var userExists = await GetRealUser(req) is not null;
            if (userExists) throw new HttpException(409, "Such user account already exists");

            using (ApplicationContext db = new ApplicationContext())
            {
                var passwordReq = req.Password;
                var userReq = (User)req;
                userReq.Id = Guid.NewGuid().ToString();
                userReq.CreationDate = DateTime.Now;
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
        /// Данный метод получает пользователя по номеру телефона
        /// </summary>
        /// <param name="phone"></param>
        /// <returns></returns>
        public static async Task<User?> GetUserByPhone(string phone)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                var userApplicable = from z in db.Users where z.PhoneNum == phone select z;
                return await userApplicable.FirstOrDefaultAsync();
            }
        }

        /// <summary>
        /// Данный метод создаёт токен для пользователя
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        public static async Task<string> IssueTokenForUser(User u, TokenType type)
        {
            if (u is null) throw new ArgumentNullException(nameof(u));

            // create an appropriate token
            return Program.jwtService.GenerateToken(u, type.ToString());
        }

        /// <summary>
        /// Проверяет существование пользователя в БД
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        public static async Task<User?> GetRealUser(User u)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                var possibleUser = from z in db.Users.Include(z => z.Accounts) where z.Id == u.Id || z.PhoneNum == u.PhoneNum || z.Email == u.Email select z;
                return await possibleUser.FirstOrDefaultAsync();
            }
        }

        public static async Task<AuthResult> Change2FAStatus(User user, bool enable)
        {
            // Generate a shared secret for the user
            string sharedSecret = RandomProvider.GenerateRandom2FA();

            // Store the shared secret in the user's database record
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

    public enum TokenType
    {
        Active, TwoFA
    }
}
