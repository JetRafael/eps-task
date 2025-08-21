using System;
using System.Text;

namespace EPS.Server.Utilities
{
    public static class RandomCodeGenerator
    {
        private static Random random = new Random();

        public static string[] GetRandomCodes(ushort count, byte length) {
            string[] codes = new string[count];
            for (var i = 0; i < count; i++) {
                codes[i] = GenerateRandomCode(length);
            }

            return codes;
        }
        private static string GenerateRandomCode(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            StringBuilder codeBuilder = new StringBuilder(length);

            for (int i = 0; i < length; i++)
            {
                int index = random.Next(chars.Length);
                codeBuilder.Append(chars[index]);
            }

            return codeBuilder.ToString();
        }
    }
}
