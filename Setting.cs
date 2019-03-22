using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CefWebKit
{
    public class Setting
    {
        public static Setting setting = null;
        public string Version { get; set; } = "0.1";
        public List<string> AppKeys { get; set; } = new List<string>();
        public int Port { get; set; } = 7321;
        public int BrowserAlive { get; set; } = 300;
        public int MaxBrowserCount { get; set; } = 10;
    }
}
