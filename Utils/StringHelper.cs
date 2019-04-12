using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CefWebKit.Utils
{
    public class StringHelper
    {
        public static string localPathEncode(string url)
        {
            string r = url.Replace("\\", "/")
                .Replace(" ", "%20")
                .Replace("#", "%23")
                .Replace("%", "%25")
                .Replace("+", "%2B")
                .Replace("%", "%25")
                .Replace("=", "%3D");
            return r;
        }
    }
}
