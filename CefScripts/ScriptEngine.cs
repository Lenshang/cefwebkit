using CefOperator.CefCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CefOperator.CefScripts
{
    public class ScriptEngine
    {
        public void writeln(string msg)
        {
            Console.WriteLine(msg);
        }

        public void sleep(int seconds)
        {
            Thread.Sleep(seconds);
        }

        public void loadUrl(string url)
        {
            CefForm jsCef = null;
            var r = CefTaskPool.DefaultPool.getOrCreate("MainScript", out jsCef);
            jsCef.WaitToScript($"window.location.href=\"{url}\"");
            jsCef.WaitInitialized();
        }
    }
}
