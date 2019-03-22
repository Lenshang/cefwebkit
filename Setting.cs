using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CefOperator
{
    public class Setting
    {
        public static Setting setting = null;
        public string Version { get; set; }
        public List<string> AppKeys { get; set; }
        public int Port { get; set; }
        public int BrowserAlive { get; set; }
        public int MaxBrowserCount { get; set; }
    }
}
