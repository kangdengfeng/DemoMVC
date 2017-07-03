using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ZSZ.Common
{
    public static class CommonHelper
    {
        //
        public static string CalcMD5(this string str)
        {
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(str);
            return CalcMD5(bytes);
        }
        public static string CalcMD5(byte[] bytes)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] computeBytes = md5.ComputeHash(bytes);
                string result = "";
                for (int i = 0; i < computeBytes.Length; i++)
                {
                    result += computeBytes[i].ToString("X").Length == 1 ? "0" +
                    computeBytes[i].ToString("X") : computeBytes[i].ToString("X");
                }
                return result;
            }
        }
        public static string CalcMD5(Stream stream)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] computeBytes = md5.ComputeHash(stream);
                string result = "";
                for (int i = 0; i < computeBytes.Length; i++)
                {
                    result += computeBytes[i].ToString("X").Length == 1 ? "0" +
                    computeBytes[i].ToString("X") : computeBytes[i].ToString("X");
                }
                return result;
            }
        }

        //验证码
        //参数：验证码的长度
        public static string CreateVerifyCode(int len)
        {
            char[] data = { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'j', 'k', 'm', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };
            StringBuilder sb = new StringBuilder();
            Random r = new Random();
            for (int i = 0; i < len; i++)
            {
                //随机生成一个位置 r.Net[0,4)
                int index = r.Next(data.Length);
                //取位置的字符
                char c = data[index];
                sb.Append(c);
            }
            return sb.ToString();
        }
    }
}
