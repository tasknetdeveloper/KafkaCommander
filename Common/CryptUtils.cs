using System;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace Common
{
    public class CryptUtils
    {
        private Utils _utils = null;
        private const string _default_passPhrase = "dksfjhO*IUYasd1dfg87sdf";
        //private const string _plusWord = "dksfjhO*IUYasd1dfg87sdf";
        private const string _plusWord = "dksfjhO*IUYasd1dfg87sdf+xzcvlkjh";
        private const int Keysize = 256;
        private int _a = 0;
        private int _b = 0;
        private int _c = 0;
        private int _l = 0;
        private const int BLOCKSIZE = 128;//256

        private const int DerivationIterations = 1000;

        public CryptUtils(int a, int b, int c, int l, Utils utils) {
            _utils = utils;
            _a = a;
            _b = b;
            _c = c;
            _l = l;
        }

        public string Encrypt(string plainText, string passPhrase)
        {
            var saltStringBytes = Generate128BitsOfRandomEntropy();//Generate256BitsOfRandomEntropy();
            var ivStringBytes = Generate128BitsOfRandomEntropy();//Generate256BitsOfRandomEntropy();
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
            {
                var keyBytes = password.GetBytes(Keysize / 8);
                using (var symmetricKey = new RijndaelManaged())
                {
                    symmetricKey.BlockSize = BLOCKSIZE;//256;
                    symmetricKey.Mode = CipherMode.CBC;
                    symmetricKey.Padding = PaddingMode.PKCS7;
                    using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                            {
                                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
                                cryptoStream.FlushFinalBlock();

                                var cipherTextBytes = saltStringBytes;
                                cipherTextBytes = cipherTextBytes.Concat(ivStringBytes).ToArray();
                                cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();
                                memoryStream.Close();
                                cryptoStream.Close();
                                return Convert.ToBase64String(cipherTextBytes);
                            }
                        }
                    }
                }
            }
        }

        public string Decrypt(string cipherText, string passPhrase)
        {
            //if(!_utils.isBase64(cipherText))
            //    cipherText = _utils.EncodeStringToBase64(cipherText);// + "==";//== для правильной кодировки
            var divisor = 8;//8;

            var cipherTextBytesWithSaltAndIv = Convert.FromBase64String(cipherText);//(_utils.EncodeStringToBase64_ASCII(cipherText));
            var saltStringBytes = cipherTextBytesWithSaltAndIv.Take(Keysize / divisor).ToArray();
            var ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(Keysize / divisor).Take(Keysize / divisor).ToArray();
            var cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip((Keysize / divisor) * 2).Take(cipherTextBytesWithSaltAndIv.Length - ((Keysize / divisor) * 2)).ToArray();

            using (var password = new Rfc2898DeriveBytes(passPhrase, saltStringBytes, DerivationIterations))
            {
                var keyBytes = password.GetBytes(Keysize / divisor);
                using (var symmetricKey = new RijndaelManaged())
                {
                    try
                    {
                        symmetricKey.BlockSize = BLOCKSIZE;//256;
                        symmetricKey.Mode = CipherMode.CBC;
                        symmetricKey.Padding = PaddingMode.PKCS7;
                        using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
                        {
                            using (var memoryStream = new MemoryStream(cipherTextBytes))
                            {
                                using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                                {
                                    var plainTextBytes = new byte[cipherTextBytes.Length];
                                    var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
                                    memoryStream.Close();
                                    cryptoStream.Close();
                                    return Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
                                }
                            }
                        }
                    }
                    catch (Exception exp)
                    {
                        var err = exp.Message;
                    }
                    return "";
                }
            }
        }

        #region 2
        public string Encrypt2(string plainInput, string key)
        {
            key += _plusWord;
            key = key.Substring(0, 32);

            byte[] iv = new byte[16];
            byte[] array;
            using (Aes aes = Aes.Create())
            {
                aes.Key = Encoding.UTF8.GetBytes(key);
                aes.IV = iv;
                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter streamWriter = new StreamWriter((Stream)cryptoStream))
                        {
                            streamWriter.Write(plainInput);
                        }

                        array = memoryStream.ToArray();
                    }
                }
            }

            return Convert.ToBase64String(array);
        }

        public string Decrypt2(string cipherText, string key)
        {
            key += _plusWord;
            key = key.Substring(0,32);

            byte[] iv = new byte[16];
            byte[] buffer = Convert.FromBase64String(cipherText);
            try
            {
                using (Aes aes = Aes.Create())
                {
                    aes.Key = Encoding.UTF8.GetBytes(key);
                    aes.IV = iv;
                    ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
                    using (MemoryStream memoryStream = new MemoryStream(buffer))
                    {
                        using (CryptoStream cryptoStream = new CryptoStream((Stream)memoryStream, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader streamReader = new StreamReader((Stream)cryptoStream))
                            {
                                //return streamReader.ReadToEnd();
                                var s64 = streamReader.ReadToEnd();
                                s64 = _utils.GetUtf8String(s64);
                                return _utils.DecodeStringFromBase64(s64);
                                //return _utils.GetUtf8String(s64);
                            }
                        }
                    }
                }
            }
            catch (Exception exp)
            {
            }
            return "";
        }
        #endregion

        private byte[] Generate128BitsOfRandomEntropy()
        {
            var randomBytes = new byte[16];
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetBytes(randomBytes);
            }
            return randomBytes;
        }

        private byte[] Generate256BitsOfRandomEntropy()
        {
            var randomBytes = new byte[32];
            using (var rngCsp = new RNGCryptoServiceProvider())
            {
                rngCsp.GetBytes(randomBytes);
            }
            return randomBytes;
        }

        public string Get_passPhrase(string[] txt)
        {
            var result = "";
            if (txt.Length == 3)
            {
                result = (_a * _a * txt[0].Length).ToString() +
                            GetSubString(0, txt[0]) +
                            (2 * _b * txt[1].Length).ToString() +
                            GetSubString(_l, txt[1]) +
                            (_c * txt[2].Length).ToString() +
                            GetSubString(0, txt[2]);
            }
            else if (txt.Length == 2)
            {
                result = (_a * _a * txt[0].Length).ToString() +
                            GetSubString(_l, txt[0]) +
                            (2 * _b * txt[1].Length).ToString() +
                            GetSubString(0, txt[1]);
            }
            else if (txt.Length == 1)
            {
                result = (_a * _a * txt[0].Length).ToString() +
                            GetSubString(_l, txt[0]);
            }
            else
                result = _default_passPhrase;

            //if (txt == null || (txt.Length - 1) < i)
            //result = _default_passPhrase;

            return result;
        }

        private string GetSubString(int i, string s)
        {
            var result = "";
            if (!string.IsNullOrEmpty(s))
            {
                result = s.Substring(0,i);
            }
            return result;
        }
    }
}
