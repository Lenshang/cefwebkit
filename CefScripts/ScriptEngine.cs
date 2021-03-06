﻿using CefSharp;
using CefWebKit.CefCore;
using CefWebKit.Utils;
using Chen.CommonLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CefWebKit.CefScripts
{
    public class ScriptEngine
    {
        public CefScript cefScript { get; set; }
        public ScriptEngine(CefScript script)
        {
            this.cefScript = script;
        }
        public void writeln(string msg)
        {
            Console.WriteLine(msg);
        }

        public void sleep(int seconds)
        {
            Thread.Sleep(seconds);
        }

        public void import(string file)
        {
            CefForm jsCef = null;
            var r = CefTaskPool.DefaultPool.getOrCreate("MainScript", out jsCef);
            var filePath = Path.Combine(this.cefScript.runTempPath.FullName, file);
            string scriptStr = $@"
                var script = document.createElement('script');
                script.src = '{StringHelper.localPathEncode(filePath)}';
                document.getElementsByTagName('head')[0].appendChild(script);
            ";
            jsCef.WaitToScript(scriptStr);
        }

        public void changeSize(int width,int height)
        {
            CefForm jsCef = null;
            var r = CefTaskPool.DefaultPool.getOrCreate("MainScript", out jsCef);
            jsCef.FormSize = new System.Drawing.Size(width, height);
            jsCef.ChangeScreenSize();
        }

        public string[] getArgs()
        {
            return this.cefScript.arguments;
        }

        public void debug()
        {
            CefForm jsCef = null;
            var r = CefTaskPool.DefaultPool.getOrCreate("MainScript", out jsCef);
            jsCef.ShowDevTools();
            //string scriptStr = $@"
            //    await waitClick('点击这里继续');
            //";
            //jsCef.WaitToScript(scriptStr);
        }
    }
}
