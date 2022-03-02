using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace WemaBankAssignment.Services
{
    public class CryptoServices
    {
        private TripleDESCryptoServiceProvider TDESAlgo;
        private string keystring;

        private string _password;
        private int _salt;



        public CryptoServices()
        {
            //keystring = "B738C0DB478907CAE98CF476";
            keystring = "6DB18246D230DF0B36C01DB9";
            init();
        }

        public CryptoServices(string strPassword, int nSalt)
        {
            _password = strPassword;
            _salt = nSalt;
        }

        public CryptoServices(bool usePadding)
        {
            //keystring = "B738C0DB478907CAE98CF476";
            keystring = "6DB18246D230DF0B36C01DB9";
            init();
        }

        public CryptoServices(string key)
        {
            if (!string.IsNullOrEmpty(key))
            {
                if (key.Length == 24)
                {
                    keystring = key;
                }
                else
                {
                    throw new ArgumentException("key is invalid. Must be hexadecimal and 24 characters long");
                }
            }
            else
            {
                throw new ArgumentException("key is invalid. Must be hexadecimal and 24 characters long");
            }
        }

        private void init()
        {
            TDESAlgo = new TripleDESCryptoServiceProvider();
            TDESAlgo.Padding = PaddingMode.PKCS7;
            TDESAlgo.Mode = CipherMode.ECB;
            byte[] key = Encoding.UTF8.GetBytes(keystring);
            TDESAlgo.Key = key;
        }

        public string EncryptData(string plaintext)
        {
            string output = null;
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, TDESAlgo.CreateEncryptor(), CryptoStreamMode.Write);
            StreamWriter writer = new StreamWriter(cs);
            writer.Write(plaintext);
            writer.Flush();
            writer.Close();
            cs.Close();
            byte[] encMsg = ms.ToArray();
            ms.Close();

            output = Convert.ToBase64String(encMsg);
            return output;
        }
        public string DecryptData(string ciphertext)
        {
            string output = null;
            byte[] enc = Convert.FromBase64String(ciphertext);
            ICryptoTransform decryptor = TDESAlgo.CreateDecryptor();
            byte[] input = Convert.FromBase64String(ciphertext);
            byte[] plainBytes = decryptor.TransformFinalBlock(input, 0, input.Length);
            output = Encoding.UTF8.GetString(plainBytes);
            return output;
        }
        public string DecryptTripleDES(string cipherText)
        {
            byte[] result;
            byte[] dataToDecrypt = Convert.FromBase64String(cipherText);

            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            byte[] keyB = hashmd5.ComputeHash(Encoding.UTF8.GetBytes("6DB18246D230DF0B36C01DB9"));
            hashmd5.Clear();

            var tdes = new TripleDESCryptoServiceProvider { Key = keyB, Mode = CipherMode.CBC, IV = new byte[8], Padding = PaddingMode.PKCS7 };

            using (ICryptoTransform cTransform = tdes.CreateDecryptor())
            {
                result = cTransform.TransformFinalBlock(dataToDecrypt, 0, dataToDecrypt.Length);
                tdes.Clear();
            }

            return Encoding.UTF8.GetString(result);
        }
        public string EncryptTripleDES(string cipherText)
        {
            byte[] byt = Encoding.UTF8.GetBytes(cipherText);
            string mdo = Convert.ToBase64String(byt);
            byte[] result;
            byte[] dataToEncrypt = Encoding.UTF8.GetBytes(cipherText);

            MD5CryptoServiceProvider hashmd5 = new MD5CryptoServiceProvider();
            byte[] keyB = hashmd5.ComputeHash(Encoding.UTF8.GetBytes("6DB18246D230DF0B36C01DB9"));
            hashmd5.Clear();

            var tdes = new TripleDESCryptoServiceProvider { Key = keyB, Mode = CipherMode.CBC, IV = new byte[8], Padding = PaddingMode.PKCS7 };

            using (ICryptoTransform cTransform = tdes.CreateEncryptor())
            {
                result = cTransform.TransformFinalBlock(dataToEncrypt, 0, dataToEncrypt.Length);
                tdes.Clear();
            }

            return Convert.ToBase64String(result, 0, result.Length);
        }
        public string Encode(string clearText)
        {
            byte[] encoded = Encoding.UTF8.GetBytes(clearText);
            return Convert.ToBase64String(encoded);
        }
        public string Decode(string encodedText)
        {
            byte[] encoded = Convert.FromBase64String(encodedText);
            return Encoding.UTF8.GetString(encoded);
        }
        public static string CreateRandomPassword(int PasswordLength)
        {
            string _allowedChars = "abcdefghijkmnopqrstuvwxyzABCDEFGHJKLMNOPQRSTUVWXYZ23456789";
            byte[] randomBytes = new byte[PasswordLength];
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetBytes(randomBytes);
            char[] chars = new char[PasswordLength];
            int allowedCharCount = _allowedChars.Length;

            for (int i = 0; i < PasswordLength; i++)
            {
                chars[i] = _allowedChars[randomBytes[i] % allowedCharCount];
            }

            return new string(chars);
        }
        public static int CreateRandomSalt()
        {
            byte[] _saltBytes = new byte[4];
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            rng.GetBytes(_saltBytes);

            return (_saltBytes[0] << 24) + (_saltBytes[1] << 16) +
              (_saltBytes[2] << 8) + _saltBytes[3];
        }
        public string ComputeSaltedHash()
        {
            // Create Byte array of password string
            ASCIIEncoding encoder = new ASCIIEncoding();
            byte[] _secretBytes = encoder.GetBytes(_password);

            // Create a new salt
            byte[] _saltBytes = new byte[4];
            _saltBytes[0] = (byte)(_salt >> 24);
            _saltBytes[1] = (byte)(_salt >> 16);
            _saltBytes[2] = (byte)(_salt >> 8);
            _saltBytes[3] = (byte)_salt;

            // append the two arrays
            byte[] toHash = new byte[_secretBytes.Length + _saltBytes.Length];
            Array.Copy(_secretBytes, 0, toHash, 0, _secretBytes.Length);
            Array.Copy(_saltBytes, 0, toHash, _secretBytes.Length, _saltBytes.Length);

            SHA1 sha1 = SHA1.Create();
            byte[] computedHash = sha1.ComputeHash(toHash);

            return encoder.GetString(computedHash);
        }
    }
}
