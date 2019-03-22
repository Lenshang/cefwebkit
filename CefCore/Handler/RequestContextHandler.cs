using CefSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CefOperator.CefCore.Handler
{
    class RequestContextHandler: IRequestContextHandler
    {
        CookieManager cookies;
        public RequestContextHandler(string cookiePath = "cookie")
        {
            //cookies = new CookieHandler(cookiePath);
            cookies = new CookieManager(cookiePath, true, null);
        }
        public ICookieManager GetCookieManager()
        {
            return cookies;
        }

        public bool OnBeforePluginLoad(string mimeType, string url, bool isMainFrame, string topOriginUrl, WebPluginInfo pluginInfo, ref PluginPolicy pluginPolicy)
        {
            return false;
        }

        public void OnRequestContextInitialized(IRequestContext requestContext)
        {
            
        }
    }
}
