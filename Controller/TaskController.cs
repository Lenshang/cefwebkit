using CefWebKit.CefCore;
using CefWebKit.Controller.Model;
using CefWebKit.Model;
using Nancy;
using Nancy.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CefWebKit.Controller
{
    public class TaskController : NancyModule
    {
        public TaskController() : base("/TaskApi")
        {
            Post["/action"] = x => {
                ApiResult result = new ApiResult();
                TaskAction action = this.Bind<TaskAction>();
                if (!CheckAppKey(action.appKey))
                {
                    result.IsSuccess = false;
                    result.Message = "未授权的ApiKey";
                    return result;
                }

                return DoAction(action,result);
            };

            Post["/reg"] = x => {
                ApiResult result = new ApiResult();
                RegAction action = this.Bind<RegAction>();
                if (!CheckAppKey(action.appKey))
                {
                    result.IsSuccess = false;
                    result.Message = "未授权的ApiKey";
                    return result;
                }
                return DoRegAction(action, result);
            };

            Get["/sleep"] = x =>
            {
                Thread.Sleep(80*1000);
                return "success";
            };
        }

        public ApiResult DoAction(TaskAction action,ApiResult result)
        {
            CefForm cef = null;
            var r=CefTaskPool.DefaultPool.getOrCreate(action.token, out cef);
            if (r < 0)
            {
                result.IsSuccess = false;
                result.Message = "当前Server的最大任务数量已达到上线"+ Setting.setting.MaxBrowserCount;
            }
            Thread.Sleep(800);
            SetCookies(cef, action);
            //Console.WriteLine("cookie已写入");
            Thread.Sleep(800);
            if (action.action == "loadurl")
            {
                cef.LoadUrl(action.param);
            }
            else if (action.action == "html")
            {
                result.Data=cef.GetHtml();
            }
            else if (action.action == "query")
            {
                result.Data = cef.GetHtmlByQuery(action.param);
            }
            else if (action.action == "script")
            {
                result.Data = cef.RunScript(action.param);
            }
            else if (action.action == "script_waitrender")
            {
                var pos = action.param.LastIndexOf("@@");
                var scr = action.param.Substring(0, pos);
                var query = action.param.Substring(pos + 2);
                result.Data = cef.RunScript(scr);
                result.IsSuccess = cef.WaitRenderOver(query);
            }
            else if(action.action== "loadurl_waitrender")
            {
                var pos = action.param.LastIndexOf("@@");
                var url = action.param.Substring(0, pos);
                var query = action.param.Substring(pos + 2);
                cef.LoadUrl(url);
                result.IsSuccess=cef.WaitRenderOver(query);
            }
            else if (action.action == "loadurl_getbyquery")
            {
                var pos = action.param.LastIndexOf("@@");
                if (pos < 0)
                {
                    cef.LoadUrl(action.param);
                }
                else
                {
                    var url = action.param.Substring(0, pos);
                    var query = action.param.Substring(pos + 2);
                    cef.LoadUrl(url);
                    result.IsSuccess = cef.WaitRenderOver(query);
                    result.Data = cef.GetHtmlByQuery(query);
                }
            }
            else
            {
                result.IsSuccess = false;
                result.Message = "没有找到有效指令";
            }
            Thread.Sleep(500);
            result.Cookies = cef.GetAllCookie("http://qq.com");
            return result;
        }

        public ApiResult DoRegAction(RegAction action,ApiResult result)
        {
            CefForm cef = null;
            var r=CefTaskPool.DefaultPool.getOrCreate(action.token, out cef);
            if (r < 0)
            {
                result.IsSuccess = false;
                result.Message = "当前Server的最大任务数量已达到上线" + Setting.setting.MaxBrowserCount;
            }

            if (action.action == "seturlmon")
            {
                var urlRule = action.param;
                var mode = GetMode(action.mode);
                string id = cef.SetUrlMonitor(urlRule, mode);
                if (string.IsNullOrEmpty(id))
                {
                    result.IsSuccess = false;
                    result.Message = "添加规则失败，请重试";
                    return result;
                }
                result.Data = id;
            }
            else if (action.action == "geturlmon")
            {
                var id = action.param;
                if (action.mode == "all")
                {
                    var mon=cef.GetUrlMonitor(id);
                    result.Data = mon.htmls;
                }
                else if (action.mode == "first")
                {
                    var mon = cef.GetUrlMonitor(id);
                    result.Data = mon.htmls.FirstOrDefault();
                }
                else
                {
                    result.Data = cef.GetUrlMonitorLatestContent(id);
                }
            }
            else
            {
                result.IsSuccess = false;
                result.Message = "没有找到有效指令";
            }
            return result;
        }
        private RULEMODE GetMode(string mode)
        {
            switch (mode)
            {
                case "contain":return RULEMODE.CONTAIN;
                case "notcontain":return RULEMODE.NOTCONTAIN;
                case "equal":return RULEMODE.EQUAL;
                case "regex":return RULEMODE.REGEX;
                default:return RULEMODE.CONTAIN;
            }
        }
        private void SetCookies(CefForm cef, TaskAction action)
        {
            if (action.setCookie == null)
            {
                return;
            }
            foreach(var cookie in action.setCookie)
            {
                cef.SetCookie(cookie.key, cookie.value, cookie.path, cookie.domain,action.setCookieUrl);
            }
        }

        public bool CheckAppKey(string key)
        {
            if (Setting.setting.AppKeys.Contains(key))
            {
                return true;
            }
            return false;
        }
    }
}
