using CefWebKit.CefCore.Handler;
using CefWebKit.Model;
using CefSharp;
using CefSharp.WinForms;
using Chen.CommonLibrary;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CefWebKit.CefCore
{
    public class CefForm : Form, IDisposable
    {
        public ChromiumWebBrowser browser { get; private set; }
        public ICookieManager cookieManager { get; set; }
        public RenderProcessMessageHandler renderProcess { get; private set; }
        public RequestHandler requestHandler { get; private set; }
        public RequestContext requestContext { get; set; }
        public bool IsInitialized { get; set; } = false;
        public bool ScriptMode { get; set; } = false;

        public System.Drawing.Size FormSize { get; set; } = new System.Drawing.Size(800, 600);
        public bool FullScreen { get; set; } = false;
        public CefForm(string cookiePath="",int formWidth=800,int formHeight=600,bool fullScreen=false)
        {
            this.FormSize = new System.Drawing.Size(formWidth, formHeight);
            //初始化CEF
            var requestContextHandler = new RequestContextHandler(cookiePath);
            this.requestContext = new RequestContext(requestContextHandler);
            renderProcess = new RenderProcessMessageHandler();
            requestHandler= new RequestHandler(new ConcurrentDictionary<string, UrlMonitorModel>());
            browser = new ChromiumWebBrowser("about:blank")
            {
                Dock = DockStyle.Fill,
                RequestContext = requestContext,
                RenderProcessMessageHandler= renderProcess,
                RequestHandler= requestHandler
            };
            browser.IsBrowserInitializedChanged += Browser_IsBrowserInitializedChanged;
            browser.LifeSpanHandler = new LifeSpanHandler();
            this.Controls.Add(browser);
            cookieManager = requestContextHandler.GetCookieManager();
            //cookieManager = Cef.GetGlobalCookieManager();
        }

        public void ChangeScreenSize()
        {
            this.Invoke(new Action(() => {
                //是否全屏
                if (this.FullScreen)
                {
                    this.MaximumSize = new System.Drawing.Size();
                    this.FormBorderStyle = FormBorderStyle.None;     //设置窗体为无边框样式
                    this.WindowState = FormWindowState.Maximized;    //最大化窗体 
                }
                else
                {
                    //设置窗口固定大小
                    this.MaximumSize = this.FormSize;
                    this.MinimumSize = this.FormSize;
                }
            }));
        }

        private void Browser_IsBrowserInitializedChanged(object sender, IsBrowserInitializedChangedEventArgs e)
        {
            this.IsInitialized = e.IsBrowserInitialized;
        }

        public void WaitInitialized()
        {
            while (!this.IsInitialized)
            {
                Thread.Sleep(100);
            }
            
        }

        public void LoadUrl(string url)
        {
            this.WaitInitialized();
            this.ChangeScreenSize();
            //browser.ShowDevTools();//debug
            this.browser.Load(url);
        }

        public void ShowDevTools()
        {
            browser.ShowDevTools();//debug
        }
        /// <summary>
        /// 直到可运行脚本，运行脚本
        /// </summary>
        /// <param name="script"></param>
        /// <param name="waitTimeout"></param>
        /// <param name="timeout"></param>
        public string WaitToScript(string script, int waitTimeout = 2000,int timeout = 2000)
        {
            DateTime dtStart = DateTime.Now;
            while (!renderProcess.CanScript&&((DateTime.Now-dtStart).TotalMilliseconds<waitTimeout))
            {
                Thread.Sleep(100);
            }
            return RunScript(script, timeout);
        }
        /// <summary>
        /// 运行JS代码
        /// </summary>
        /// <param name="script">JS代码</param>
        /// <param name="timeout">超时时间</param>
        /// <returns></returns>
        public string RunScript(string script, int timeout = 2000)
        {
            if (renderProcess.CanScript)
            {
                Task<JavascriptResponse> Tjs = browser.EvaluateScriptAsync(script);
                if (Tjs.Wait(timeout))
                {
                    if (Tjs.Result.Result != null)
                    {
                        string a = Tjs.Result.Result.ToString();
                        return a;
                    }
                }
            }
            return null;
        }
        /// <summary>
        /// 获得当前标签页的URL
        /// </summary>
        /// <returns></returns>
        public string GetUrl()
        {
            return browser.Address;
        }
        /// <summary>
        /// 设置Cookie
        /// </summary>
        /// <param name="name">名称</param>
        /// <param name="value">对应值</param>
        /// <param name="domain">所在域名</param>
        public bool SetCookie(Cookie cookie, string url = "/")
        {
            return cookieManager.SetCookie(url, cookie);
        }
        /// <summary>
        /// 获得指定站点的Cookie，为空则获得当前页面站点Cookie
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public List<Cookie> GetUrlCookie(string url = "")
        {
            if (string.IsNullOrEmpty(url))
            {
                string _url = GetUrl();
            }
            var _cookies = cookieManager.VisitUrlCookiesAsync(url, false);
            _cookies.Wait(5000);
            var cookies = _cookies.Result;
            return cookies;
        }
        /// <summary>
        /// 獲得所有站点的Cookie，为空则获得当前页面站点Cookie
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public List<Cookie> GetAllCookie(string url = "")
        {
            if (string.IsNullOrEmpty(url))
            {
                string _url = GetUrl();
            }
            var _cookies = cookieManager.VisitAllCookiesAsync();
            _cookies.Wait(5000);
            var cookies = _cookies.Result;
            return cookies;
        }
        /// <summary>
        /// 设置多个Cookie
        /// </summary>
        /// <param name="cookies"></param>
        public void SetCookies(Cookie[] cookies, string url = "/")
        {
            foreach (Cookie i in cookies)
            {
                SetCookie(i, url);
            }
        }
        /// <summary>
        /// 设置Cookie
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="path"></param>
        /// <param name="domain"></param>
        /// <returns></returns>
        public bool SetCookie(string name, string value, string path, string domain,string url="/")
        {
            if (string.IsNullOrEmpty(url))
            {
                Cookie cookie = new Cookie()
                {
                    Creation = DateTime.Now,
                    Name = name,
                    Value = value,
                    Path = path,
                    Domain = domain
                };
                return SetCookie(cookie, "/");
            }
            else if (url.Contains("@current"))
            { 
                url = url.Replace("@current", "");
                Cookie cookie = new Cookie()
                {
                    Creation = DateTime.Now,
                    Name = name,
                    Value = value,
                    Path = path,
                    Domain = domain
                };
                if (!string.IsNullOrEmpty(domain))
                {
                    url = domain;
                    if (url.StartsWith("."))
                    {
                        url = url.Substring(1);
                    }
                    url = "http://" + url;
                }
                return SetCookie(cookie, url);
            }
            else
            {
                Cookie cookie = new Cookie()
                {
                    Creation = DateTime.Now,
                    Name = name,
                    Value = value
                    //Path = path,
                    //Domain = domain
                };
                return SetCookie(cookie, url);
            }
        }
        /// <summary>
        /// 删除Cookie如果Url和Name都为空，则删除所有Cookie
        /// </summary>
        /// <param name="url"></param>
        /// <param name="name"></param>
        /// <param name="waitTime"></param>
        /// <returns></returns>
        public int DeleteCookie(string url = null, string name = null, int waitTime = 3000)
        {
            Task<int> _t = cookieManager.DeleteCookiesAsync(url, name);
            if (_t.Wait(waitTime))
            {
                return _t.Result;
            }
            else
            {
                return -1;
            }
        }
        /// <summary>
        /// 通过Query语句获得Html代码
        /// </summary>
        /// <param name="Selector"></param>
        /// <returns></returns>
        public string GetHtmlByQuery(string Selector)
        {
            string Script = JavaScriptLibrary.QuerySelector(Selector);
            string result = RunScript(Script);
            return result;
        }
        /// <summary>
        /// 获得当前页面的HTML代码
        /// </summary>
        /// <returns></returns>
        public String GetHtml()
        {
            string Script = JavaScriptLibrary.QuerySelector("html");
            string result = RunScript(Script);
            return result;
            //Task<string> Thtml = wb.GetBrowser().MainFrame.GetSourceAsync();
            //Thtml.Wait();
            //HtmlCode = Thtml.Result;
            //return HtmlCode;
        }
        /// <summary>
        /// 判断指定标签对象 是否存在页面上
        /// </summary>
        /// <param name="querySelector">对象选择器</param>
        /// <returns></returns>
        public bool HasNode(string querySelector)
        {
            bool result = false;
            string _html = GetHtmlByQuery(querySelector);
            if (_html != null)
            {
                result = true;
            }
            return result;
        }
        /// <summary>
        /// 等待渲染完成
        /// </summary>
        /// <param name="querySelector">判断渲染标签</param>
        /// <param name="timeout">超时时间 单位：秒</param>
        /// <returns></returns>
        public bool WaitRenderOver(string querySelector,int timeout=30)
        {
            bool result = false;
            DateTime stime = DateTime.Now;
            while ((DateTime.Now - stime) < TimeSpan.FromSeconds(timeout))
            {
                if (this.HasNode(querySelector))
                {
                    result = true;
                    break;
                }
                Thread.Sleep(800);
            }
            return result;
        }
        /// <summary>
        /// 添加一个Url请求监视器
        /// </summary>
        /// <param name="urlRule"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public string SetUrlMonitor(string urlRule,RULEMODE mode)
        {
            string id = MD5Helper.getMd5Hash(mode.ToString() + urlRule);
            if (this.requestHandler.UrlMonitor.ContainsKey(id))
            {
                return id;
            }

            UrlMonitorModel urlMonitor = new UrlMonitorModel()
            {
                htmls=new List<HtmlContent>(),
                id=id,
                mode=mode,
                urlRule=urlRule
            };
            if(this.requestHandler.UrlMonitor.TryAdd(id, urlMonitor))
            {
                return id;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 获得一个Url监听器
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public UrlMonitorModel GetUrlMonitor(string id)
        {
            UrlMonitorModel result = null;
            if(this.requestHandler.UrlMonitor.TryGetValue(id,out result))
            {
                return result;
            }
            else
            {
                return null;
            }
        }
        /// <summary>
        /// 根据Url监听器ID获得最后一个监听到的内容
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public HtmlContent GetUrlMonitorLatestContent(string id)
        {
            var urlMonitor = GetUrlMonitor(id);
            if (urlMonitor == null) return null;
            return urlMonitor.htmls.LastOrDefault();
        }

        public string SetProxy(string server)
        {
            string error = "";
            Cef.UIThreadTaskFactory.StartNew(()=> {
                DateTime stime = DateTime.Now;
                while (!this.requestContext.CanSetPreference("proxy")&&(DateTime.Now-stime)<TimeSpan.FromSeconds(10))
                {
                    Thread.Sleep(100);
                }
                Dictionary<string, object> proxyValue = new Dictionary<string, object>();
                proxyValue.Add("mode", "fixed_servers");
                proxyValue.Add("server", server);
                bool success = this.requestContext.SetPreference("proxy", proxyValue, out error);
                
            }).Wait();
            return error;
        }

        /// <summary>
        /// 创建一个实例对象，并在另一个线程启动它，返回该实例对象
        /// </summary>
        /// <returns></returns>
        public static CefForm CreateAndRun(string token)
        {
            CefForm cef = new CefForm(token);
            cef.Size = new System.Drawing.Size(1280, 720);
            Thread t = new Thread(new ThreadStart(() => {
                cef.ShowDialog();
            }));
            t.Start();
            return cef;
        }

        public new void Dispose()
        {
            this.browser.Dispose();
            base.Dispose();
        }
    }
}
