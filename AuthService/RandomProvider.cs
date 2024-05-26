using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MTUAuthService.AuthService
{
    public class RandomProvider
    {
        public static RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

        public static int Next(int minValue, int maxExclusiveValue)
        {
            if (minValue >= maxExclusiveValue)
                throw new ArgumentOutOfRangeException("minValue must be lower than maxExclusiveValue");

            long diff = (long)maxExclusiveValue - minValue;
            long upperBound = uint.MaxValue / diff * diff;

            uint ui;
            do
            {
                ui = GetRandomUInt();
            } while (ui >= upperBound);
            return (int)(minValue + (ui % diff));
        }

        private static uint GetRandomUInt()
        {
            var randomBytes = GenerateRandomBytes(sizeof(uint));
            return BitConverter.ToUInt32(randomBytes, 0);
        }

        private static byte[] GenerateRandomBytes(int bytesNumber)
        {
            byte[] buffer = new byte[bytesNumber];
            rng.GetBytes(buffer);
            return buffer;
        }

        public static string GenerateRandomString(int length = 32)
        {
            var newBA = GenerateRandomBytes(length);
            return Convert.ToBase64String(newBA);
        }

        public static string GenerateRandom2FA(int length = 18)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[Next(0, s.Length)]).ToArray());
        }
    }
}
