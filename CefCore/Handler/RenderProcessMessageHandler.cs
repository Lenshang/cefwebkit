using CefSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CefWebKit.CefCore.Handler
{
    public class RenderProcessMessageHandler: IRenderProcessMessageHandler
    {
        public bool CanScript = false;
        public void OnContextCreated(IWebBrowser browserControl, IBrowser browser, IFrame frame)
        {
            this.CanScript = true;
        }

        public void OnContextReleased(IWebBrowser browserControl, IBrowser browser, IFrame frame)
        {
            this.CanScript = false;
        }

        public void OnFocusedNodeChanged(IWebBrowser browserControl, IBrowser browser, IFrame frame, IDomNode node)
        {
        }

        public void OnUncaughtException(IWebBrowser browserControl, IBrowser browser, IFrame frame, JavascriptException exception)
        {
            
        }
    }
}
