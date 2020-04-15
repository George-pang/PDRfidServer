using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Security;

using System.Diagnostics;
using System.Security;
using System.Security.Cryptography;
using System.IO;

namespace Anke.SHManage.Utility
{
    public class SecurityHelper
    {

        //wxb add2019-12-11 DES加密 start
        private bool isReturnNum;
        private bool isCaseSensitive;
        public SecurityHelper() { }
        /// <summary>
        /// IsReturnNum:是否返回为加密后字符的Byte代码
        /// </summary>
        /// <param name="IsReturnNum">是否返回为加密后字符的Byte代码</param>
        public SecurityHelper(bool IsReturnNum)
        {
            this.isReturnNum = IsReturnNum;
        }
        //wxb add2019-12-11 DES加密 end

        /// <summary>
        /// 使用 票据对象 将 用户数据 加密成字符串
        /// </summary>
        /// <param name="userInfo"></param>
        /// <returns></returns>
        public static string EncryptUserReleted(string userData)
        {
            //将用户数据 存入 票据对象
            FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, "AnkeManagement", DateTime.Now, DateTime.Now, true, userData);
            //将票据对象 加密成字符串（可逆）
            string strData = FormsAuthentication.Encrypt(ticket);
            return strData;
        }

        /// <summary>
        /// 加密字符串 解密
        /// </summary>
        /// <param name="cryptograph">加密字符串</param>
        /// <returns></returns>
        public static string DecryptUserInfo(string cryptograph)
        {
            //将 加密字符串 解密成 票据对象
            FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(cryptograph);
            //1.2 将票据里的 用户数据 返回
            return ticket.UserData;
        }


        /// <summary>
        /// 返回 MD5 加密字符串
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string GetMD5(string s)
        {
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(s);
            bytes = md5.ComputeHash(bytes);
            md5.Clear();

            string ret = "";
            for (int i = 0; i < bytes.Length; i++)
            {
                ret += Convert.ToString(bytes[i], 16).PadLeft(2, '0');
            }

            return ret.PadLeft(32, '0');
        }


        /// <summary>
        /// 使用DES加密//wxb add2019-12-11 DES加密 start
        /// </summary>
        /// <param name="originalValue">待加密的字符串</param>
        /// <returns>加密后的字符串</returns>
        private static string DESEncrypt(string originalValue, string key, string IV)
        {
            //将key和IV处理成8个字符
            key += "12345678";
            IV += "12345678";
            key = key.Substring(0, 8);
            IV = IV.Substring(0, 8);

            SymmetricAlgorithm sa;
            ICryptoTransform ct;
            MemoryStream ms;
            CryptoStream cs;
            byte[] byt;

            sa = new DESCryptoServiceProvider();
            sa.Key = Encoding.UTF8.GetBytes(key);
            sa.IV = Encoding.UTF8.GetBytes(IV);
            ct = sa.CreateEncryptor();

            byt = Encoding.UTF8.GetBytes(originalValue);

            ms = new MemoryStream();
            cs = new CryptoStream(ms, ct,
            CryptoStreamMode.Write);
            cs.Write(byt, 0, byt.Length);
            cs.FlushFinalBlock();

            cs.Close();

            return Convert.ToBase64String(ms.ToArray());

        }
        /// <summary>
        /// 使用DES加密
        /// </summary>
        /// <param name="originalValue">待加密的字符串</param>
        /// <param name="key">密钥(最大长度8)</param>
        /// <returns></returns>
        public static string DESEncrypt(string originalValue)
        {
            return DESEncrypt(originalValue, "", "");
        }

        /// <summary>
        /// 使用DES解密
        /// </summary>
        /// <param name="encryptedValue">待解密的字符串</param>
        /// <returns>解密后的字符串</returns>
        private static string DESDecrypt(string encryptedValue, string key, string IV)
        {
            //将key和IV处理成8个字符
            key += "12345678";
            IV += "12345678";
            key = key.Substring(0, 8);
            IV = IV.Substring(0, 8);

            SymmetricAlgorithm sa;
            ICryptoTransform ct;
            MemoryStream ms;
            CryptoStream cs;
            byte[] byt;

            sa = new DESCryptoServiceProvider();
            sa.Key = Encoding.UTF8.GetBytes(key);
            sa.IV = Encoding.UTF8.GetBytes(IV);
            ct = sa.CreateDecryptor();

            byt = Convert.FromBase64String(encryptedValue);

            ms = new MemoryStream();
            cs = new CryptoStream(ms, ct,
            CryptoStreamMode.Write);
            cs.Write(byt, 0, byt.Length);
            cs.FlushFinalBlock();

            cs.Close();

            return Encoding.UTF8.GetString(ms.ToArray());

        }
        /// <summary>
        /// 使用DES解密（Add in xueshulin）
        /// </summary>
        /// <param name="encryptedValue">待解密的字符串</param>
        /// <param name="key">密钥(最大长度8)</param>
        /// <returns></returns>
        public static string DESDecrypt(string encryptedValue, string key)
        {
            return DESDecrypt(encryptedValue, "", "");
        }
        //wxb add2019-12-11 DES加密 end

    }
}
