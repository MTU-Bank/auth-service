using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTUAuthService
{
    internal class Verification
    {


        public class TwoFactorAuth
        {
            public static void Enable2FA(string username)
            {
                // Сгенерируйте общий секрет для пользователя
                string sharedSecret = KeyGeneration.GenerateRandomKey();
                // Сохранение общего секрета в записи базы данных пользователя
                StoreSharedSecretInDatabase(username, sharedSecret);
            }
            public static bool VerifyOTP(string username, string userEnteredOTP)
            {
                // Повторный запрос извлекаем общий секрет из строки записи базы данных пользователя
                string sharedSecret = RetrieveSharedSecretFromDatabase(username);
                //Проверка введенного пользователем OTP
                TwoStepsAuthenticator.ValidateResult result = TOTP.ValidateTwoFactorPIN(sharedSecret, userEnteredOTP);
                return result.IsValid;
            }
            // Вам потребуется реализовать эти операции с базой данных
            private static void StoreSharedSecretInDatabase(string username, string sharedSecret)
            {
                // Сохранение общего секрета в записи базы данных пользователя
                using (ApplicationContext db = new ApplicationContext())
                {


                    db.EnteresTabel.Add(username, sharedSecret);
                    db.SaveChanges();

                }
            }
            private static string RetrieveSharedSecretFromDatabase(string username)
            {
                // Извлечение общего секрета из записи базы данных пользователя
                using (ApplicationContext db = new ApplicationContext())
                {
                    var enterField = db.UsersTabel.FirstOrDefault(enterField => enterField.username == username);
                    return enterField.Secret;
                }

            }
        }

    }
}
