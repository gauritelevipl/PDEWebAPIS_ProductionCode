using System.Text;
using Microsoft.VisualBasic;
using System.Security.Cryptography;
using Newtonsoft.Json;

namespace PDEWebAPIS
{
    public class Security
    {
        private System.Security.Cryptography.RijndaelManaged? m_aes = null!;
        public static string? nickey = "vCHFzG+JS+Sw+ER/KjPFZJ7AsTdaVzwiUNg9Unrgutw=";
        public static string? niciv = "1P1ehtwlHdyknht9h5bwfA==";
        public static byte[] Key_valuenic = Convert.FromBase64String(nickey!);
        public static byte[] Init_Vectornic = Convert.FromBase64String(niciv!);
        public static string? aeskey = "6XhX8NxtWrlC/NbK3GXoh3TtH9UUt8KmgcuUG0RFEJM=";
        public static string aesiv = "t0tOwviXTieE5SZoh9/hzw==";
        public static byte[] Key_value = Convert.FromBase64String(aeskey!);
        public static byte[] Init_Vector = Convert.FromBase64String(aesiv!);
        private byte[] m_key = null!;
        private byte[] m_iv = null!;
        private byte[] m_AesKey = null!;
        private byte[] m_AesIv = null!;

        private bool IskeyVectorCustom = false;
        public static int PROVIDER_RSA_FULL = 1;
        public static string CONTAINER_NAME = "KeyContainer";
        public static string EnCryptData(string input)
        {
            byte[] encrypted;
            using (RijndaelManaged rijAlg = new RijndaelManaged())
            {
                byte[] input1 = Encoding.UTF8.GetBytes(input);
                rijAlg.Mode = CipherMode.CBC;
                // rijAlg.Padding = PaddingMode.PKCS7;
                rijAlg.Key = Key_value;
                rijAlg.IV = Init_Vector;
                ICryptoTransform aestran = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);
                encrypted = aestran.TransformFinalBlock(input1, 0, input1.Length);
                return Convert.ToBase64String(encrypted);
            }
        }

        public static string DeCryptData(string input)
        {
            if (string.IsNullOrEmpty(input) || input.Length % 4 != 0
                || input.Contains(" ") || input.Contains("\t") || input.Contains("\r") || input.Contains("\n"))
                return input;
            else
            {
#pragma warning disable SYSLIB0022 // Type or member is obsolete
                using (RijndaelManaged rijAlg = new RijndaelManaged())
                {
                    rijAlg.Mode = CipherMode.CBC;
                    rijAlg.Padding = PaddingMode.PKCS7;
                    rijAlg.Key = Key_value;
                    rijAlg.IV = Init_Vector;
                    ICryptoTransform decrypt = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);
                    byte[] input1 = Convert.FromBase64String(input);
                    byte[] decrypted = decrypt.TransformFinalBlock(input1, 0, input1.Length);
                    var decryptedtext = Encoding.UTF8.GetString(decrypted);
                    return decryptedtext;
                }
#pragma warning restore SYSLIB0022 // Type or member is obsolete
            }
        }

        public static string EnCryptDataNIC(string input)
        {
            byte[] encrypted;
            using (RijndaelManaged rijAlg = new RijndaelManaged())
            {
                byte[] input1 = Encoding.UTF8.GetBytes(input);
                rijAlg.Mode = CipherMode.CBC;
                // rijAlg.Padding = PaddingMode.PKCS7;
                rijAlg.Key = Key_valuenic;
                rijAlg.IV = Init_Vectornic;
                ICryptoTransform aestran = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);
                encrypted = aestran.TransformFinalBlock(input1, 0, input1.Length);
                return Convert.ToBase64String(encrypted);
            }
        }
        public static string DeCryptDataNIC(string input)
        {
            if (string.IsNullOrEmpty(input) || input.Length % 4 != 0
                || input.Contains(" ") || input.Contains("\t") || input.Contains("\r") || input.Contains("\n"))
                return input;
            else
            {
#pragma warning disable SYSLIB0022 // Type or member is obsolete
                using (RijndaelManaged rijAlg = new RijndaelManaged())
                {
                    rijAlg.Mode = CipherMode.CBC;
                    rijAlg.Padding = PaddingMode.PKCS7;
                    rijAlg.Key = Key_valuenic;
                    rijAlg.IV = Init_Vectornic;
                    ICryptoTransform decrypt = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);
                    byte[] input1 = Convert.FromBase64String(input);
                    byte[] decrypted = decrypt.TransformFinalBlock(input1, 0, input1.Length);
                    var decryptedtext = Encoding.UTF8.GetString(decrypted);
                    return decryptedtext;
                }
#pragma warning restore SYSLIB0022 // Type or member is obsolete
            }
        }

        public static bool IsBase64String(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            Span<byte> buffer = new Span<byte>(new byte[input.Length]);
            return Convert.TryFromBase64String(input, buffer, out _);
        }
    }
    public class Decryptor
    {
        public string DecryptData(string encryptedData)
        {
            //string key = "VaTPMihixMlbvKnhCJPAtQ3qYPnCKbIB";
            //string iv = "CJPAtQ3qYPnCKbIB";

            //Local
            /* string base64Key = Convert.ToBase64String(Encoding.UTF8.GetBytes("VaTPMihixMlbvKnhCJPAtQ3qYPnCKbIB"));
             string base64IV = Convert.ToBase64String(Encoding.UTF8.GetBytes("CJPAtQ3qYPnCKbIB"));*/

            //Production
            string base64Key = Convert.ToBase64String(Encoding.UTF8.GetBytes("71r9BBcnRHWvfMG1JQinb8E4rzCR5Mpa"));
            string base64IV = Convert.ToBase64String(Encoding.UTF8.GetBytes("JQinb8E4rzCR5Mpa"));

            // Convert the base64 ciphertext to a byte array
            // Decode the Base64-encoded string into a byte array
            byte[] decodedBytes = Convert.FromBase64String(encryptedData);

            // Convert the byte array to a string
            string decodedString = Encoding.UTF8.GetString(decodedBytes);
            byte[] cipherText = Convert.FromBase64String(decodedString);

            byte[] key = Convert.FromBase64String(base64Key);
            byte[] iv = Convert.FromBase64String(base64IV);

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.KeySize = 256; // Set AES to 256-bit key length
                aesAlg.Key = key;
                aesAlg.IV = iv;
                aesAlg.Mode = CipherMode.CBC; // CBC mode
                aesAlg.Padding = PaddingMode.PKCS7; // Use PKCS7 padding

                using (var decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV))
                {
                    byte[] plainTextBytes = decryptor.TransformFinalBlock(cipherText, 0, cipherText.Length);
                    return Encoding.UTF8.GetString(plainTextBytes);
                }
            }
        }
    }
}




