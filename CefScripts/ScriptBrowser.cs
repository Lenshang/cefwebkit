using CefWebKit.CefCore;
using CefWebKit.Model;
using Chen.CommonLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CefWebKit.CefScripts
{
    class ScriptBrowser
    {
        public CefScript cefScript { get; set; }
        public ScriptBrowser(CefScript script)
        {
            this.cefScript = script;
        }
        public void loadUrl(string url, string id = "web")
        {
            CefForm jsCef = null;
            var r = CefTaskPool.DefaultPool.getOrCreate(id, out jsCef);
            jsCef.WaitInitialized();
            jsCef.LoadUrl(url);
        }

        public void waitRender(string query,int timeout=30,string id = "web")
        {
            CefForm jsCef = null;
            var r = CefTaskPool.DefaultPool.getOrCreate(id, out jsCef);
            jsCef.WaitInitialized();
            jsCef.WaitRenderOver(query, timeout);
        }

        public bool hasElement(string query,string id = "web")
        {
            CefForm jsCef = null;
            var r = CefTaskPool.DefaultPool.getOrCreate(id, out jsCef);
            jsCef.WaitInitialized();
            return jsCef.HasNode(query);
        }

        public string setUrlMonitor(string urlRule, string mode, string id = "web")
        {
            CefForm jsCef = null;
            var r = CefTaskPool.DefaultPool.getOrCreate(id, out jsCef);
            jsCef.WaitInitialized();
            RULEMODE _mode = RULEMODE.CONTAIN;
            if (mode == "EQUAL")
            {
                _mode = RULEMODE.EQUAL;
            }
            else if (mode== "NOTCONTAIN")
            {
                _mode = RULEMODE.NOTCONTAIN;

            }
            else if (mode == "REGEX")
            {
                _mode = RULEMODE.REGEX;
            }
            return jsCef.SetUrlMonitor(urlRule, _mode);
        }

        public string[] getUrlMonitorAllHtmls(string monitorId, string id = "web")
        {
            CefForm jsCef = null;
            var r = CefTaskPool.DefaultPool.getOrCreate(id, out jsCef);
            jsCef.WaitInitialized();
            var mon=jsCef.GetUrlMonitor(monitorId);
            List<string> result = new List<string>();
            foreach(var item in mon.htmls)
            {
                result.Add(item.text);
            }
            return result.ToArray();
        }

        public string[] saveUrlMonitorAllContents(string monitorId,string directory, string id = "web")
        {
            CefForm jsCef = null;
            var r = CefTaskPool.DefaultPool.getOrCreate(id, out jsCef);
            jsCef.WaitInitialized();
            var mon = jsCef.GetUrlMonitor(monitorId);
            List<string> result = new List<string>();
            FileHelper fh = new FileHelper();
            foreach (var item in mon.htmls)
            {
                string filename = item.url.Substring(item.url.LastIndexOf("/")+1);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                string fullpath = Path.Combine(directory, filename);
                fh.SaveFile(fullpath, Convert.FromBase64String(item.contentB64));
                result.Add(fullpath);
            }
            return result.ToArray();
        }
    }
}
