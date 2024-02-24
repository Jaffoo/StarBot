using System.Security.Cryptography;
using System.Text;

namespace IdolBot.Helper
{
    public class MD5Helper
    {
        public static string MD5Encrypt(string str)
        {
            string result = "";
            var buffer = Encoding.UTF8.GetBytes(str);
            var md5 = MD5.HashData(buffer);
            foreach (var item in md5)
            {
                result += item.ToString("x2");
            }
            return result;
        }
    }
}
