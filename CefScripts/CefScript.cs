using CefWebKit.CefCore;
using Chen.CommonLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CefSharp;
using System.IO;
using System.Windows.Forms;

namespace CefWebKit.CefScripts
{
    public class CefScript
    {
        public FileInfo filePath { get; set; }
        public FileHelper fileHelper { get; set; }
        public CefForm ScriptForm { get; set; }
        public CefScript(string scriptFile)
        {
            this.filePath = new FileInfo(scriptFile);
            this.fileHelper = new FileHelper();
            this.Init();
        }

        public void Init()
        {
            CefForm jsCef = null;
            var r = CefTaskPool.DefaultPool.getOrCreate("MainScript", out jsCef);
            this.ScriptForm = jsCef;
            this.ScriptForm.browser.RegisterAsyncJsObject("scriptEngine", new ScriptEngine(this));
            this.ScriptForm.WaitInitialized();
            string url = AppDomain.CurrentDomain.BaseDirectory + @"CefScripts\MainPage.html";
            url = url.Replace("\\", "/").Replace(" ", "%20");
            this.ScriptForm.LoadUrl(url);
            jsCef.ShowDevTools();
        }

        public Task<JavascriptResponse> Run()
        {
            DateTime dtStart = DateTime.Now;
            while (!this.ScriptForm.renderProcess.CanScript && ((DateTime.Now - dtStart).TotalMilliseconds < 1000))
            {
                Thread.Sleep(100);
            }
            return this.ScriptForm.browser.EvaluateScriptAsync(fileHelper.readFile(this.filePath.FullName));
        }

        public static CefScript Create(string scriptFile)
        {
            if (!File.Exists(scriptFile))
            {
                scriptFile= Path.Combine(Environment.CurrentDirectory, scriptFile);
            }
            return new CefScript(scriptFile);
        }
    }
}
