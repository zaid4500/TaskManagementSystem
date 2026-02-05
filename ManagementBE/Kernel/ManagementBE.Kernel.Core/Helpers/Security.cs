using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace ManagementBE.Kernel.Core.Helpers
{
    public class Security
    {
        private static byte[] GetIV()
        {
            return new byte[]   {
                55, 34, 87, 64, 87, 195, 54, 21 , 44, 75,
                          35, 86, 142, 95, 57, 21 };
        }
        private static byte[] IV = GetIV();

        public static string Encrypt(string plainText)
        {
            string key = "YOUR_KEY";
            byte[] EncryptKey = { };
            EncryptKey = System.Text.Encoding.UTF8.GetBytes(key);
            AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
            byte[] inputByte = Encoding.UTF32.GetBytes(plainText);
            MemoryStream mStream = new MemoryStream();
            CryptoStream cStream = new CryptoStream(mStream, aes.CreateEncryptor(EncryptKey, IV), CryptoStreamMode.Write);
            cStream.Write(inputByte, 0, inputByte.Length);
            cStream.FlushFinalBlock();
            return Convert.ToBase64String(mStream.ToArray());
        }
        /// <summary>
        /// To Decrypt values in the system takes an encrypted Text
        /// </summary>
        /// <param name="encryptedText"></param>
        /// <returns></returns>
        public static string Decrypt(string encryptedText)
        {
            try
            {
                string key = "YOUR_KEY";
                byte[] DecryptKey = { };
                byte[] inputByte = new byte[encryptedText.Length];
                DecryptKey = System.Text.Encoding.UTF8.GetBytes(key);
                AesCryptoServiceProvider aes = new AesCryptoServiceProvider();
                inputByte = Convert.FromBase64String(encryptedText);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(DecryptKey, IV), CryptoStreamMode.Write);
                cs.Write(inputByte, 0, inputByte.Length);
                cs.FlushFinalBlock();
                System.Text.Encoding encoding = System.Text.Encoding.UTF32;
                return encoding.GetString(ms.ToArray());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static string EncryptPh2(string toEncrypt)
        {
            byte[] keyArray;
            byte[] toEncryptArray = UTF8Encoding.UTF8.GetBytes(toEncrypt);

            string key = "YOUR_KEY";
            keyArray = UTF8Encoding.UTF8.GetBytes(key);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.CBC;
            tdes.Padding = PaddingMode.PKCS7;
            tdes.IV = new byte[] { 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000 };

            ICryptoTransform cTransform = tdes.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            tdes.Clear();

            StringBuilder HexStr = new StringBuilder(resultArray.Length * 2);
            foreach (byte b in resultArray)
                HexStr.AppendFormat("{0:x2}", b);

            return HexStr.ToString();
        }
        public static string DecryptPh2(string cipherText)
        {
            byte[] keyArray;
            byte[] toEncryptArray = StringToByteArray(cipherText);

            string key = "YOUR_KEY";
            keyArray = UTF8Encoding.UTF8.GetBytes(key);

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            tdes.Key = keyArray;
            tdes.Mode = CipherMode.CBC;
            tdes.Padding = PaddingMode.PKCS7;
            tdes.IV = new byte[] { 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000, 0x0000 };

            ICryptoTransform cTransform = tdes.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            tdes.Clear();

            return UTF8Encoding.UTF8.GetString(resultArray);
        }
        public static byte[] StringToByteArray(String hex)
        {
            int NumberChars = hex.Length;
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }


        public static string GenerateRandomPassword()
        {
            const string lettersCapital = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lettersSmall = "abcdefghijklmnopqrstuvwxyz";
            const string numbers = "0123456789";
            const string specialCharacters = "!@#$%^&*()_-+=<>?";
           
            string allCharacters = lettersCapital + lettersSmall + numbers + specialCharacters;

            Random random = new Random();
            StringBuilder password = new StringBuilder();

            // Ensure at least one character from each category
            password.Append(GetRandomCharacter(lettersCapital, random));
            password.Append(GetRandomCharacter(lettersSmall, random));
            password.Append(GetRandomCharacter(numbers, random));
            password.Append(GetRandomCharacter(specialCharacters, random));

            // Fill the remaining length with random characters
            for (int i = 4; i < 4; i++)
            {
                password.Append(GetRandomCharacter(allCharacters, random));
            }

            // Shuffle the password characters
            char[] passwordArray = password.ToString().ToCharArray();
            for (int i = passwordArray.Length - 1; i > 0; i--)
            {
                int j = random.Next(0, i + 1);
                char temp = passwordArray[i];
                passwordArray[i] = passwordArray[j];
                passwordArray[j] = temp;
            }

            return new string(passwordArray);

        }

        static char GetRandomCharacter(string characterSet, Random random)
        {
            int index = random.Next(characterSet.Length);
            return characterSet[index];
        }
    }
}
