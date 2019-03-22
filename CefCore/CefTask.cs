using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CefOperator.CefCore
{
    public class CefTask:IDisposable
    {
        public DateTime refreshTime { get; set; }
        public string token { get; set; }
        public CefForm cefForm { get; set; }
        public CefTask(string _token)
        {
            this.token = _token;
            this.refreshTime = DateTime.Now;
            cefForm = CefForm.CreateAndRun(_token);
            CheckAlive();
        }
        /// <summary>
        /// 刷新任务时间
        /// </summary>
        public void RefreshTask()
        {
            this.refreshTime = DateTime.Now;
        }

        /// <summary>
        /// 检测任务是否到期
        /// </summary>
        private void CheckAlive()
        {
            if ((DateTime.Now - this.refreshTime) > TimeSpan.FromSeconds(Setting.setting.BrowserAlive))
            {
                CefTaskPool.DefaultPool.remove(this.token);
                this.Dispose();
            }
            else
            {
                Task.Delay(5000).ContinueWith(t => {
                    CheckAlive();
                });
            }
        }
        public void Dispose()
        {
            try
            {
                cefForm.Invoke(new Action(() => {
                    cefForm.Dispose();
                }));
                cefForm = null;
            }
            catch
            {
                cefForm = null;
            }
        }
    }
}
