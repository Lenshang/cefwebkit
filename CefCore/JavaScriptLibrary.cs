using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CefOperator.CefCore
{
    public class JavaScriptLibrary
    {
        /// <summary>
        /// 模拟用户点击按钮JS
        /// </summary>
        /// <param name="QuerySelector">QEURY选择器</param>
        /// <returns></returns>
        public static string ButtonClick(string QuerySelector)
        {
            string result =
                "var _tempClick=document.querySelector(\"" + QuerySelector + "\");" +
                "_tempClick.click();";
            return result;
        }
        /// <summary>
        /// 模拟表单提交
        /// </summary>
        /// <param name="QuerySelector">QEURY选择器</param>
        /// <returns></returns>
        public static string SubmitForm(string QuerySelector)
        {
            string result =
                "var _tempForm=document.querySelector(\"" + QuerySelector + "\");" +
                "_tempForm.submit();";
            return result;
        }
        /// <summary>
        /// 模拟用户填写文本框JS
        /// </summary>
        /// <param name="QuerySelector">绑定的值</param>
        /// <param name="value">QEURY选择器</param>
        /// <returns></returns>
        public static string BindValue(string QuerySelector, string value)
        {
            string result =
                "var _tempClick=document.querySelector(\"" + QuerySelector + "\");" +
                "_tempClick.value=\"" + value + "\"";
            return result;
        }
        /// <summary>
        /// Jquery选择器
        /// </summary>
        /// <param name="Selector">QEURY选择器</param>
        /// <returns></returns>
        public static string QuerySelector(string Selector)
        {
            string result = "var _tempa=document.querySelector(\"" + Selector + "\");" +
                " _tempa.outerHTML";
            return result;
        }
        /// <summary>
        /// 滚动至最下
        /// </summary>
        /// <returns></returns>
        public static string ScrollBottom()
        {
            return "window.scrollTo(0,9999);";
        }
        /// <summary>
        /// 按下按键事件KeyPress
        /// </summary>
        /// <param name="el"></param>
        /// <param name="keyCode"></param>
        /// <returns></returns>
        public static string KeyPress(string el, int keyCode)
        {
            string result = "var _tempa=document.querySelector(\"" + el + "\");" +
                "var evtObj = document.createEvent('UIEvents');" +
                "evtObj.initUIEvent(\"keypress\", true, true, window, 1 );" +
                "delete evtObj.keyCode;" +
                "Object.defineProperty(evtObj,\"keyCode\",{value:" + keyCode + "});" +
                "_tempa.dispatchEvent(evtObj);";
            return result;
        }
        /// <summary>
        /// 按下按键事件KeyDown
        /// </summary>
        /// <param name="el"></param>
        /// <param name="keyCode"></param>
        /// <returns></returns>
        public static string KeyDown(string el, int keyCode)
        {
            string result = "var _tempa=document.querySelector(\"" + el + "\");" +
                "var evtObj = document.createEvent('UIEvents');" +
                "evtObj.initUIEvent(\"keydown\", true, true, window, 1 );" +
                "delete evtObj.keyCode;" +
                "Object.defineProperty(evtObj,\"keyCode\",{value:" + keyCode + "});" +
                "_tempa.dispatchEvent(evtObj);";
            return result;
        }
        /// <summary>
        /// 触发失去焦点事件
        /// </summary>
        /// <param name="el"></param>
        /// <returns></returns>
        public static string Blur(string el)
        {
            string result = "var _tempa=document.querySelector(\"" + el + "\");" +
                "var evtObj = document.createEvent('HTMLEvents');" +
                "evtObj.initEvent(\"blur\", true, true);" +
                "_tempa.dispatchEvent(evtObj);";
            return result;
        }
        /// <summary>
        /// 触发焦点事件
        /// </summary>
        /// <param name="el"></param>
        /// <returns></returns>
        public static string Focus(string el)
        {
            string result = "var _tempa=document.querySelector(\"" + el + "\");" +
                "var evtObj = document.createEvent('HTMLEvents');" +
                "evtObj.initEvent(\"focus\", true, true);" +
                "_tempa.dispatchEvent(evtObj);";
            return result;
        }
        public static string FrameStopLoad()
        {
            string result = "window.stop ? window.stop() : document.execCommand('Stop');";
            return result;
        }
    }
    public class JqueryScriptLibrary
    {
        /// <summary>
        /// 使页面加载Jquery
        /// </summary>
        /// <returns></returns>
        public static string LoadJquery()
        {
            return null;
        }
        /// <summary>
        /// Jquery选择器
        /// </summary>
        /// <param name="Selector"></param>
        /// <returns></returns>
        public static string QuerySelector(string Selector)
        {
            string result = $"var _tempa=$('{Selector}');" +
                " _tempa.outerHTML";
            return result;
        }
    }

    public class JavaScript
    {
        CefForm cefBrowser;
        public JavaScript(CefForm _cefBrowser)
        {
            cefBrowser = _cefBrowser;
        }
        public List<JavaScriptItem> QuerySelecorAll(string query)
        {
            List<JavaScriptItem> result = new List<JavaScriptItem>();
            cefBrowser.RunScript("var _tempa=document.querySelectorAll(\"" + query + "\");");
            int Count = Convert.ToInt32(cefBrowser.RunScript("_tempa.length;"));
            for (int i = 0; i < Count; i++)
            {
                //string Name = Tookit.GetRandomString(10, false, true, false, false, "_");
                string Name = Guid.NewGuid().ToString();
                cefBrowser.RunScript($"var {Name}=_tempa[{i}]");
                result.Add(new JavaScriptItem(Name, cefBrowser, $"document.querySelectorAll(\"{query}\")[{i}];"));
            }
            return result;
        }
    }
    /// <summary>
    /// JavaScript对象
    /// </summary>
    public class JavaScriptItem
    {
        private string ValueName = "";
        private CefForm cefBrowser;
        private string FromQuery = "";
        /// <summary>
        /// 对象内HTML代码
        /// </summary>
        public string Html { get; set; }
        public JavaScriptItem(string Name, CefForm _cefBrowser, string _fromQuery = "")
        {
            ValueName = Name;
            cefBrowser = _cefBrowser;
            FromQuery = _fromQuery;
            Html = OutHtml();
        }
        /// <summary>
        /// 获得变量名称
        /// </summary>
        /// <returns></returns>
        public string GetName()
        {
            return ValueName;
        }
        /// <summary>
        /// 重新加载标签内信息
        /// </summary>
        public void Reload()
        {
            cefBrowser.RunScript($"{ValueName}={FromQuery}");
            Html = OutHtml();
        }
        /// <summary>
        /// 选择Jquery对象全部匹配
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public List<JavaScriptItem> QuerySelectorAll(string query)
        {
            List<JavaScriptItem> result = new List<JavaScriptItem>();
            cefBrowser.RunScript($"var _tempa={this.ValueName}.querySelectorAll(\"" + query + "\");");
            int Count = Convert.ToInt32(cefBrowser.RunScript("_tempa.length;"));
            for (int i = 0; i < Count; i++)
            {
                //string Name = Tookit.GetRandomString(10, false, true, false, false, "_");
                string Name = Guid.NewGuid().ToString();
                cefBrowser.RunScript($"var {Name}=_tempa[{i}]");
                result.Add(new JavaScriptItem(Name, cefBrowser, $"document.querySelectorAll(\"{query}\")[{i}];"));
            }
            return result;
        }
        /// <summary>
        /// 选择Jquery对象匹配一个
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public JavaScriptItem QuerySelector(string query)
        {
            var lists = QuerySelectorAll(query);
            if (lists.Count > 0)
            {
                return lists[0];
            }
            return null;
        }
        /// <summary>
        /// 点击对象
        /// </summary>
        public void Click()
        {
            cefBrowser.RunScript($"{ValueName}.click();");
        }
        /// <summary>
        /// 点击一个对象并等待页面渲染
        /// </summary>
        /// <param name="Selector"></param>
        public bool ClickAndWaitRender(string Selector, int TimeOut = 5000)
        {
            cefBrowser.RunScript($"{ValueName}.click();");
            return WaitRenderOver(Selector, new System.Threading.CancellationTokenSource(TimeOut));
        }
        /// <summary>
        /// 点击一个对象并等待页面渲染
        /// </summary>
        /// <param name="waitTime">等待时间</param>
        public void ClickAndWaitRender(int waitTime = 500)
        {
            cefBrowser.RunScript($"{ValueName}.click();");
            Thread.Sleep(waitTime);
        }
        /// <summary>
        /// 判断对象中是否包含某个标签
        /// </summary>
        /// <returns></returns>
        public bool HasNode(string Selector)
        {
            bool result = false;
            string jsCode = "var _tempa = " + this.ValueName + ".querySelector(\"" + Selector + "\");" +
                " _tempa.outerHTML";
            string _html = cefBrowser.RunScript(jsCode);
            if (_html != null)
            {
                result = true;
            }
            return result;
        }
        /// <summary>
        /// 通过判断指定标签对象，等待页面渲染完成
        /// </summary>
        /// <param name="querySelector">JQ选择器</param>
        /// <returns></returns>
        public bool WaitRenderOver(string querySelector, CancellationTokenSource CancelToken)
        {
            bool result = false;
            while (!result && !CancelToken.IsCancellationRequested)
            {
                result = HasNode(querySelector);
                Thread.Sleep(500);
            }
            return result;
        }
        /// <summary>
        /// 通过判断指定标签对象，等待页面渲染完成
        /// </summary>
        /// <param name="querySelector">JQ选择器</param>
        /// <param name="TimeOut">超时时间</param>
        /// <returns></returns>
        public bool WaitRenderOver(string querySelector, int TimeOut)
        {
            return WaitRenderOver(querySelector, new System.Threading.CancellationTokenSource(TimeOut));
        }
        /// <summary>
        /// 获得对象的HTML代码
        /// </summary>
        /// <returns></returns>
        public string OutHtml()
        {
            string result = cefBrowser.RunScript($"{ValueName}.outerHTML");

            return result;
        }
    }
}
