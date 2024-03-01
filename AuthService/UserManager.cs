using EmbedIO;
using MTUBankBase.Auth.Models;
using MTUBankBase.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MTUAuthService.AuthService
{
    public class UserManager
    {
        /// <summary>
        /// Данный метод регистрирует пользователя в системе
        /// </summary>
        /// <param name="user"></param>
        public static User RegisterUser(RegisterRequest req)
        {
            // making sure the user doesn't already exist
            var userExists = DoesUserExist(req) is null;
            if (userExists) throw new HttpException(409, "Such user account already exists");

            using (ApplicationContext db = new ApplicationContext())
            {
                var passwordReq = req.Password;
                var userReq = (User)req;
                userReq.PasswordHash = GeneratePwdHash(userReq, passwordReq);

                db.Users.Add(userReq);
                db.SaveChanges();
            }

            return req;
        }

        /// <summary>
        /// Данный метод получает пользователя по уникальному ID
        /// </summary>
        /// <param name="uid"></param>
        /// <returns></returns>
        public static User GetUser(string uid)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                var userApplicable = from z in db.Users where z.Id == uid select z;
                return userApplicable.FirstOrDefault();
            }
        }

        /// <summary>
        /// Проверяет существование пользователя в БД
        /// </summary>
        /// <param name="u"></param>
        /// <returns></returns>
        public static User? DoesUserExist(User u)
        {
            using (ApplicationContext db = new ApplicationContext())
            {
                var possibleUser = from z in db.Users where z.Id == u.Id || z.PhoneNum == u.PhoneNum || z.Email == u.Email select z;
                return possibleUser.FirstOrDefault();
            }
        }

        public static string GeneratePwdHash(User user, string pwd)
        {
            string rawPwd = $"{user.Id}{pwd}{user.Email}";
            using (var sha = SHA256.Create())
            {
                return BitConverter.ToString(sha.ComputeHash(Encoding.UTF8.GetBytes(rawPwd))).ToLower().Replace("-", "");
            }
        }
    }
}
