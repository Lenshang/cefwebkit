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

                //CefSharpSettings.Proxy = new ProxyOptions("127.0.0.1", "8888");
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
            args = new string[2] { "./TestScripts/pixivDownload.js","abc" };
            //args = new string[2] { "./TestScripts/main.js","abc" };
            string arg = args?.FirstOrDefault();
            if (!string.IsNullOrEmpty(arg))
            {
                var cefScript = CefScript.Create(arg);
                var otherArgs = args.Skip(1).ToArray();
                cefScript.arguments = otherArgs;
                cefScript.Run();
                cefScript.ScriptForm.FormClosed += (obj, eventArg) =>
                {
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
                //CefForm cef1 = null;
                //var r = CefTaskPool.DefaultPool.getOrCreate("abc", out cef1);
                //cef1.WaitInitialized();
                
                //cef1.LoadUrl("https://www.baidu.com");
                //Console.ReadLine();
                //cef1.SetProxy("http://127.0.0.1:8388");
                //cef1.LoadUrl("https://www.baidu.com");
                //Console.ReadLine();
                //string cmd = Console.ReadLine();
                ////url = "http://www.189.cn/dqmh/ssoLink.do?method=linkTo&platNo=10028&toStUrl=http%3A%2F%2Fgs.189.cn%2Fservice%2Fv7%2Ffycx%2Fxd%2Findex.shtml%3Ffastcode%3D10000600&cityCode=gs";
                //CefForm cef1 = null;
                //var r = CefTaskPool.DefaultPool.getOrCreate("abc", out cef1);
                //cef1.LoadUrl("https://kd.meituan.com/open_store/pclogin?post=post&bg_source=3&part_type=0&service=kaidian&extChannel=12&ext_sign_up_channel=12&platform=2&source=12&continue=https%3A%2F%2Fkd.meituan.com%2Fsetbtoken%3Fsource%3D12%26redirect%3Dhttps%253A%252F%252Fkd.meituan.com%252Flogin%253Fsource%253D12");
                //cef1.ShowDevTools();
                //cef1.browser.GetBrowserHost().SetFocus(true);

                //while (true)
                //{
                //    Console.WriteLine("输入鼠标移动位置 x,y");
                //    var pos = Console.ReadLine();

                //    int x = Convert.ToInt32(pos.Split(',')[0]);
                //    int y = Convert.ToInt32(pos.Split(',')[1]);
                //    var mouseMove = new MouseEvent(x, y, CefEventFlags.None);
                //    //cef1.browser.GetBrowserHost().SetFocus(true);
                //    var mouseClick = new MouseEvent(x, y, CefEventFlags.None);
                //    cef1.browser.GetBrowserHost().SendMouseMoveEvent(mouseMove, false);
                //    Console.WriteLine("按回车点击");
                //    //Console.ReadLine();
                //    //cef1.browser.GetBrowserHost().SetFocus(true);
                //    cef1.browser.GetBrowserHost().SendMouseClickEvent(mouseClick, MouseButtonType.Left, false, 1);
                //    Thread.Sleep(100);
                //    cef1.browser.GetBrowserHost().SendMouseClickEvent(mouseClick, MouseButtonType.Left, true, 1);
                //    Console.WriteLine("按回车键入A");
                //    //Console.ReadLine();
                //    //cef1.browser.GetBrowserHost().SetFocus(true);
                //    var ke = new KeyEvent();
                //    ke.Type = KeyEventType.Char;
                //    ke.WindowsKeyCode = 65;
                //    cef1.browser.GetBrowserHost().SendKeyEvent(ke);
                //}

                //cef1.browser.GetBrowserHost().SendKeyEvent(ke);
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
