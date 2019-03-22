using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Chen.CommonLibrary
{
    /// <summary>
    /// 启动参数管理器
    /// </summary>
    public class ArgModule
    {
        private static ArgModule argManager { get; set; }
        Dictionary<string, string> argsDic { get; set; } = new Dictionary<string, string>();
        internal ArgModule(string[] args)
        {
            if (args != null)
            {
                for (int i = 0; i < args.Count(); i++)
                {
                    if (args[i].StartsWith("--"))
                    {
                        string key = args[i].Substring(2);
                        string value = "";
                        if ((i + 1) < args.Count() && !args[i + 1].StartsWith("--"))
                        {
                            value = args[i + 1];
                        }
                        argsDic.Add(key, value);
                    }
                }
            }
        }
        /// <summary>
        /// 创建一个Manager
        /// </summary>
        /// <param name="args"></param>
        public static void Create(string[] args)
        {
            argManager = new ArgModule(args);
        }
        /// <summary>
        /// 获得参数
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Get(string key, string defaultString = "")
        {
            if (argManager == null)
            {
                argManager = new ArgModule(null);
            }
            try
            {
                string result = null;
                if (argManager.argsDic.TryGetValue(key, out result))
                {
                    return result;
                }
                else
                {
                    return defaultString;
                }
            }
            catch
            {
                return defaultString;
            }
        }
        /// <summary>
        /// 设置参数（已存在的会被覆盖）
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static void Set(string key, string value)
        {
            if (Get(key) == null)
            {
                argManager.argsDic.Add(key, value);
            }
            else
            {
                argManager.argsDic[key] = value;
            }
        }
        /// <summary>
        /// 创建启动文件
        /// </summary>
        public static void CreateShell(string path="./start.sh",int type=1)
        {
            var fh = new FileHelper();
            string runfile = new System.Diagnostics.StackTrace(1, true).GetFrame(0).GetFileName();
            FileInfo runf = new FileInfo(runfile);
            string workFolder = runf.Directory.FullName;
            var files = fh.GetAllFile(workFolder, ".cs");
            Dictionary<string, string> _argsDic = new Dictionary<string, string>();
            foreach(var file in files)
            {
                string source = fh.readFile(file.FullName);
                Regex rg = new Regex("ArgModule.Get\\(.*?\\\"(.*?)\".*?\\,.*?\\\"(.*?)\".*?\\);");
                string m1=rg.Match(source).Groups[0].Value;
                string key = rg.Match(source).Groups[1].Value;
                string value = rg.Match(source).Groups[2].Value;
                if (string.IsNullOrEmpty(m1))
                {
                    rg = new Regex("ArgModule.Get\\(.*?\\\"(.*?)\".*?\\);");
                    m1 = rg.Match(source).Groups[0].Value;
                    key= rg.Match(source).Groups[1].Value;
                }
                if (!string.IsNullOrEmpty(key))
                {
                    if (!_argsDic.ContainsKey(key))
                    {
                        _argsDic.Add(key, value);
                    }
                    
                }
            }
            string runScript = "";
            if (type == 1)
            {
                string dllFile = Assembly.GetEntryAssembly().Location;
                runScript = $"dotnet {dllFile} {string.Join(" ", _argsDic.Select(i => "--" + i.Key + " '" + i.Value+"'"))}";
            }
            if (type == 2)
            {
                string execFile = Process.GetCurrentProcess().MainModule.FileName;
                runScript = $"{execFile} {string.Join(" ", _argsDic.Select(i => "--" + i.Key + " '" + i.Value+"'"))}";
            }
            fh.SaveFile(path, runScript);
        }
        public override string ToString()
        {
            if (argsDic != null)
            {
                return string.Join(" ", argsDic.Select(i => "--" + i.Key + " " + i.Value));
            }
            return null;
        }
    }
}
