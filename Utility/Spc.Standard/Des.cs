using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Utility.Standard
{
    public class Des
    {
        static Des instance = null;

        /// <summary>  
        /// myiv is  iv  
        /// </summary>  
        string myiv = "Nuctech2";//"Hyey20100430";
        /// <summary>  
        /// mykey is key  
        /// </summary>  
        string mykey = "c1q9C4E$";//"HyeyWl30";

        /// <summary>  
        /// DES加密偏移量  
        /// 必须是>=8位长的字符串  
        /// </summary>  
        public string IV
        {
            get { return myiv; }
            set { myiv = value; }
        }

        /// <summary>  
        /// DES加密的私钥  
        /// 必须是8位长的字符串  
        /// </summary>  
        public string Key
        {
            get { return mykey; }
            set { mykey = value; }
        }

        // 实例化
        public static Des Instance()
        {
            if (instance == null)
            {
                instance = new Des();
                if (instance.m_bInitError)
                    instance = null;
            }
            return instance;
        }

        private Des()
        {
            try
            {
                m_bInitError = false;
            }
            catch (Exception e)
            {
                m_bInitError = true;
                return;
            }

            m_bInitError = false;
        }

        /// <summary>  
        /// 对字符串进行DES加密  
        /// Encrypts the specified sourcestring.  
        /// </summary>  
        /// <param name="sourcestring">The sourcestring.待加密的字符串</param>  
        /// <returns>加密后的BASE64编码的字符串</returns>  
        public string Encrypt(string sourceString)
        {
            string sRe = "";
            try
            {
                byte[] btKey = Encoding.Default.GetBytes(mykey);
                byte[] btIV = Encoding.Default.GetBytes(myiv);
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                using (MemoryStream ms = new MemoryStream())
                {
                    byte[] inData = Encoding.Default.GetBytes(sourceString);
                    try
                    {
                        using (CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(btKey, btIV), CryptoStreamMode.Write))
                        {
                            cs.Write(inData, 0, inData.Length);
                            cs.FlushFinalBlock();
                            cs.Close();
                        }
                        sRe = Convert.ToBase64String(ms.ToArray());
                    }
                    catch (System.Exception ex)
                    {
                        sRe = "";
                    }
                    ms.Close();
                }
                btKey = null;
                btIV = null;
                des = null;
            }
            catch (System.Exception ex)
            {
                sRe = "";
            }
            finally
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            return sRe;
        }

        /// <summary>  
        /// Decrypts the specified encrypted string.  
        /// 对DES加密后的字符串进行解密  
        /// </summary>  
        /// <param name="encryptedString">The encrypted string.待解密的字符串</param>  
        /// <returns>解密后的字符串</returns>  
        public string Decrypt(string encryptedString)
        {
            string sRe = "";
            try
            {
                byte[] btKey = Encoding.Default.GetBytes(mykey);
                byte[] btIV = Encoding.Default.GetBytes(myiv);
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();

                using (MemoryStream ms = new MemoryStream())
                {
                    byte[] inData = Convert.FromBase64String(encryptedString);
                    try
                    {
                        using (CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(btKey, btIV), CryptoStreamMode.Write))
                        {
                            cs.Write(inData, 0, inData.Length);
                            cs.FlushFinalBlock();
                            cs.Close();
                        }
#if WINCEVERSION
                        //return Encoding.Default.GetString(ms.ToArray(), 0, (int)ms.Length);
                        //return Convert.ToBase64String(ms.ToArray());
                        sRe = Convert.ToBase64String(ms.ToArray());
#else
                        //return Encoding.Default.GetString(ms.ToArray());
                        sRe = Encoding.Default.GetString(ms.ToArray());
#endif

                    }
                    catch (Exception expError)
                    {
                        //MessageBox.Show("expError:" + expError.ToString());
                        return "";
                    }
                    inData = null;
                    ms.Close();
                }

                btKey = null;
                btIV = null;
                des = null;

            }
            catch (System.Exception ex)
            {
                sRe = "";
            }
            finally
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            return sRe;
        }

        /// <summary>  
        /// Encrypts the file.  
        /// 对文件内容进行DES加密  
        /// </summary>  
        /// <param name="sourceFile">The source file.待加密的文件绝对路径</param>  
        /// <param name="destFile">The dest file.加密后的文件保存的绝对路径</param>  
        public bool EncryptFile(string sourceFile, string destFile)
        {
            bool bRe = true;
            try
            {
                byte[] btKey = Encoding.Default.GetBytes(mykey);
                byte[] btIV = Encoding.Default.GetBytes(myiv);
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
#if WINCEVERSION
                byte[] btFile = ReadFileBytes(sourceFile);
#else
                byte[] btFile = File.ReadAllBytes(sourceFile);
#endif

                using (FileStream fs = new FileStream(destFile, FileMode.Create, FileAccess.Write))
                {
                    try
                    {
                        using (CryptoStream cs = new CryptoStream(fs, des.CreateEncryptor(btKey, btIV), CryptoStreamMode.Write))
                        {
                            cs.Write(btFile, 0, btFile.Length);
                            cs.FlushFinalBlock();
                            cs.Close();
                        }
                    }
                    catch (Exception expError)
                    {
                        bRe = false;
                    }
                    finally
                    {
                        fs.Close();
                    }
                }

                btKey = null;
                btIV = null;
                des = null;
                btFile = null;

            }
            catch (Exception expError)
            {
                bRe = false;
            }
            finally
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            return bRe;
        }

        /// <summary>  
        /// Encrypts the file.  
        /// 对文件内容进行DES加密，加密后覆盖掉原来的文件  
        /// </summary>  
        /// <param name="sourceFile">The source file.待加密的文件的绝对路径</param>  
        public bool EncryptFile(string sourceFile)
        {
            return EncryptFile(sourceFile, sourceFile);
        }

        /// <summary>  
        /// Decrypts the file.  
        /// 对文件内容进行DES解密  
        /// </summary>  
        /// <param name="sourceFile">The source file.待解密的文件绝对路径</param>  
        /// <param name="destFile">The dest file.解密后的文件保存的绝对路径</param>  
        public bool DecryptFile(string sourceFile, string destFile)
        {
            bool bRe = true;
            try
            {
                byte[] btKey = Encoding.Default.GetBytes(mykey);
                byte[] btIV = Encoding.Default.GetBytes(myiv);
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
#if WINCEVERSION
                byte[] btFile = ReadFileBytes(sourceFile);
#else
                byte[] btFile = File.ReadAllBytes(sourceFile);
#endif

                using (FileStream fs = new FileStream(destFile, FileMode.Create, FileAccess.Write))
                {
                    try
                    {
                        using (CryptoStream cs = new CryptoStream(fs, des.CreateDecryptor(btKey, btIV), CryptoStreamMode.Write))
                        {
                            cs.Write(btFile, 0, btFile.Length);
                            cs.FlushFinalBlock();
                            cs.Close();
                        }
                    }
                    catch (Exception expError)
                    {

                        bRe = false;
                        //MessageBox.Show("expError: " + expError.ToString());
                    }
                    finally
                    {
                        fs.Close();
                    }
                }
                btKey = null;
                btIV = null;
                des = null;
                btFile = null;
            }
            catch (Exception expError)
            {
                bRe = false;
            }
            finally
            {
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            return bRe;
        }

        /// <summary>  
        /// Decrypts the file.  
        /// 对文件内容进行DES解密，加密后覆盖掉原来的文件.  
        /// </summary>  
        /// <param name="sourceFile">The source file.待解密的文件的绝对路径.</param>  
        public bool DecryptFile(string sourceFile)
        {
            return DecryptFile(sourceFile, sourceFile);
        }

        private bool m_bInitError = false;
        public bool InitError
        {
            get { return m_bInitError; }
        }

        public byte[] ReadFileBytes(String fileName)
        {
            byte[] bytes;
            using (FileStream fs = new FileStream(fileName, FileMode.Open))
            {
                bytes = new byte[fs.Length];
                int length = fs.Read(bytes, 0, (int)fs.Length);  // 将文件中的数据读到arrFile数组中； 
                fs.Close();
            }
            return bytes;
        }

        //简易加密函数
        public string StringEncoding(string pwd)
        {
            char[] arrChar = pwd.ToCharArray();
            string strChar = "";
            for (int i = 0; i < arrChar.Length; i++)
            {
                arrChar[i] = Convert.ToChar(arrChar[i] + 3);
                strChar = strChar + arrChar[i].ToString();
            }
            return strChar;
        }

        //简易解密函数
        public string StringDecoding(string pwd)
        {
            char[] arrChar = pwd.ToCharArray();
            string strChar = "";
            for (int i = 0; i < arrChar.Length; i++)
            {
                arrChar[i] = Convert.ToChar(arrChar[i] - 3);
                strChar = strChar + arrChar[i].ToString();
            }
            return strChar;
        }


    }
}
