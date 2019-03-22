using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CefWebKit.CefCore
{

    class CefTaskPool
    {
        object locker = new object();
        Dictionary<string, CefTask> tasks { get; set; }
        int maxTasks { get; set; }
        public CefTaskPool()
        {
            tasks = new Dictionary<string, CefTask>();
            maxTasks = Setting.setting.MaxBrowserCount;
        }

        public int getOrCreate(string token,out CefForm cefForm)
        {
            lock (locker)
            {
                if (this.tasks.ContainsKey(token))
                {
                    this.tasks[token].RefreshTask();
                    cefForm = this.tasks[token].cefForm;
                    return 1;
                }
                else if(tasks.Count()<maxTasks)
                {
                    var task = new CefTask(token);
                    tasks.Add(token, task);
                    cefForm = task.cefForm;
                    return 1;
                }
                else
                {
                    cefForm = null;
                    return -1;
                }
            }
        }

        public void remove(string token)
        {
            lock (locker)
            {
                if (this.tasks.ContainsKey(token))
                {
                    this.tasks.Remove(token);
                }
            }
        }

        public static CefTaskPool DefaultPool = null;
    }
}
