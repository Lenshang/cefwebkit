using CefWebKit.CefCore;
using CefWebKit.CefScripts;
using CefSharp;
using Chen.CommonLibrary;
using Nancy.Hosting.Self;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace CefWebKit
{
    class Program
    {
        static void Main(string[] args)
        {
            FileHelper fh = new FileHelper();
            bool useApi = false;
            Console.WriteLine("+++++++CEF RENDERER START++++++++");
            #region 初始化CEF
            if (!Cef.IsInitialized)
            {
                var setting = new CefSetting();
                setting.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/45.0.2454.101 Safari/537.36";
                setting.LogSeverity = LogSeverity.Disable;
                
                CefSharpSettings.LegacyJavascriptBindingEnabled = true;
                CefSharpSettings.Proxy = new ProxyOptions("127.0.0.1", "8888");
                Cef.Initialize(setting);
            }
            #endregion

            #region 加载Setting
            string settingPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "setting.json");
            if (File.Exists(settingPath))
            {
                Setting.setting = JsonConvert.DeserializeObject<Setting>(fh.readFile(settingPath));
            }
            else
            {
                Setting.setting = new Setting();
            }
            #endregion

            #region 其他初始化
            Nancy.Json.JsonSettings.MaxJsonLength = int.MaxValue;
            CefTaskPool.DefaultPool = new CefTaskPool();
            #endregion

            #region 判断启动参数
            args = new string[1]{"./TestScripts/game.js"};
            string arg = args?.FirstOrDefault();
            if (!string.IsNullOrEmpty(arg))
            {
                var cefScript = CefScript.Create(arg);
                cefScript.Run();
                cefScript.ScriptForm.FormClosed += (obj, eventArg) => {
                    Environment.Exit(0);
                };
            }
            else
            {
                useApi = true;
            }
            #endregion

            #region 启动restfulApi接口
            if (useApi)
            {
                Thread.Sleep(500);//延迟0.5秒启动接口。
                StartApi();
            }
            #endregion

            while (true)
            {
                string cmd = Console.ReadLine();
                //string url= Console.ReadLine();
                //url = "http://www.189.cn/dqmh/ssoLink.do?method=linkTo&platNo=10028&toStUrl=http%3A%2F%2Fgs.189.cn%2Fservice%2Fv7%2Ffycx%2Fxd%2Findex.shtml%3Ffastcode%3D10000600&cityCode=gs";
                //CefForm cef1 = null;
                //var r = CefTaskPool.DefaultPool.getOrCreate("abc", out cef1);
                //Thread.Sleep(1000);
                //string id = cef1.SetUrlMonitor("aggsite/SubCategories", Model.RULEMODE.CONTAIN);
                //cef1.LoadUrl(url);


                //Console.WriteLine("按回车获得结果");
                //Console.ReadLine();
                //Console.WriteLine(cef1.GetUrlMonitorLatestContent(id)?.text);
            }
            
        }

        static void StartApi()
        {
            HostConfiguration hostConf = new HostConfiguration();
            hostConf.RewriteLocalhost = true;
            var url = "http://localhost:" + Setting.setting.Port;
            var host = new NancyHost(new Uri(url));
            host.Start();
            LogHelper.Message($"WebApi Running on {url}", "Nancy", ConsoleColor.Red, ConsoleColor.White);
        }
    }
}
