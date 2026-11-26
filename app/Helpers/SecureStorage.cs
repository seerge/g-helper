using System.Security.Cryptography;
using System.Text;

namespace GHelper.Helpers
{
    public static class SecureStorage
    {
        private static readonly byte[] Entropy = Encoding.UTF8.GetBytes("GHelper.Keyboard.SecureText.Entropy.v1");

        public static string EncryptString(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
                return "";

            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
            try
            {
                byte[] cipherBytes = ProtectedData.Protect(plainBytes, Entropy, DataProtectionScope.CurrentUser);
                return Convert.ToBase64String(cipherBytes);
            }
            finally
            {
                Array.Clear(plainBytes, 0, plainBytes.Length);
            }
        }

        public static string DecryptString(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
                return "";

            try
            {
                byte[] cipherBytes = Convert.FromBase64String(cipherText);
                byte[] plainBytes = ProtectedData.Unprotect(cipherBytes, Entropy, DataProtectionScope.CurrentUser);
                try
                {
                    return Encoding.UTF8.GetString(plainBytes);
                }
                finally
                {
                    Array.Clear(plainBytes, 0, plainBytes.Length);
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine("SecureStorage decryption failed: " + ex.Message);
                return "";
            }
        }
    }
}
