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
        public DirectoryInfo runTempPath { get; set; }
        public string[] arguments { get; set; }
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
            this.ScriptForm.browser.RegisterAsyncJsObject("scriptBrowser", new ScriptBrowser(this));
            this.ScriptForm.WaitInitialized();
            //创建执行临时文件夹
            var tempPath = Path.Combine(Environment.CurrentDirectory, "_cefrun");
            this.runTempPath = new DirectoryInfo(tempPath);
            if (Directory.Exists(tempPath))
            {
                Directory.Delete(tempPath,true);
            }
            Directory.CreateDirectory(tempPath);
            //复制文件执行必要文件到临时文件夹
            fileHelper.CopyAll(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CefContent"), tempPath);

            //复制执行代码下的文件到临时文件夹
            fileHelper.CopyAll(filePath.DirectoryName, tempPath,x=> {
                return x != "_cefrun";
            });

            string url= tempPath + @"\MainPage.html";
            url = url.Replace("\\", "/").Replace(" ", "%20");
            this.ScriptForm.LoadUrl(url);

            var _filePath = Path.Combine(this.runTempPath.FullName, this.filePath.Name);
            string scriptStr = $@"
                var script = document.createElement('script');
                script.src = '{_filePath.Replace("\\", "/").Replace(" ", "%20")}';
                document.getElementsByTagName('head')[0].appendChild(script);
            ";
            jsCef.WaitToScript(scriptStr);
            //jsCef.ShowDevTools();
        }

        public Task<JavascriptResponse> Run()
        {
            DateTime dtStart = DateTime.Now;
            while (!this.ScriptForm.renderProcess.CanScript && ((DateTime.Now - dtStart).TotalMilliseconds < 1000))
            {
                Thread.Sleep(100);
            }
            return this.ScriptForm.browser.EvaluateScriptAsync("__main();");
            //DateTime dtStart = DateTime.Now;
            //while (!this.ScriptForm.renderProcess.CanScript && ((DateTime.Now - dtStart).TotalMilliseconds < 1000))
            //{
            //    Thread.Sleep(100);
            //}
            //return this.ScriptForm.browser.EvaluateScriptAsync(fileHelper.readFile(Path.Combine(this.runTempPath.FullName,this.filePath.Name)));
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
