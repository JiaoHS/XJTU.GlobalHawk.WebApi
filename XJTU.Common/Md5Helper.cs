using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace XJTU.Common
{
    public static class Md5Helper
    {
        public static string Md5(string str)
        {
            byte[] b = Encoding.Default.GetBytes(str);
            if (b.Length > 0)
            {
                try
                {
                    var m = new MD5CryptoServiceProvider();
                    byte[] b2 = m.ComputeHash(b);
                    if (b2.Length > 0)
                    {
                        string ret = "";
                        for (int i = 0; i < b2.Length; i++)
                        {
                            ret += b2[i].ToString("x").PadLeft(2, '0');
                        }
                        return ret;
                    }
                }
                catch
                {
                    //nothing
                }
            }
            return string.Empty;
        }
    }
}
