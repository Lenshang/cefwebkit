using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CefOperator.Controller.Model
{
    public class TaskAction: IAppAction
    {
        /// <summary>
        /// 动作
        /// </summary>
        public string action { get; set; }
        /// <summary>
        /// 参数
        /// </summary>
        public string param { get; set; }
        /// <summary>
        /// cookie
        /// </summary>
        public SetCookie[] setCookie { get; set; }
        /// <summary>
        /// 设置cookie的URL
        /// </summary>
        public string setCookieUrl { get; set; }
    }
    public class SetCookie
    {
        public string key { get; set; }
        public string value { get; set; }
        public string path { get; set; }
        public string domain { get; set; }
    }
}
