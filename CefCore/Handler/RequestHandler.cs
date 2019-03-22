using CefWebKit.CefCore.Filters;
using CefWebKit.Model;
using CefSharp;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace CefWebKit.CefCore.Handler
{
    public class RequestHandler : IRequestHandler
    {
        public ConcurrentDictionary<string,UrlMonitorModel> UrlMonitor { get; set; }
        public Dictionary<UInt64, MemoryStreamResponseFilter> responseDictionary { get; set; }
        public RequestHandler(ConcurrentDictionary<string, UrlMonitorModel> urlMonitor)
        {
            this.responseDictionary = new Dictionary<UInt64, MemoryStreamResponseFilter>();
            this.UrlMonitor = urlMonitor;
        }
        public bool CanGetCookies(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request)
        {
            
            return true;
        }

        public bool CanSetCookie(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, Cookie cookie)
        {
            return true;
        }

        public bool GetAuthCredentials(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, bool isProxy, string host, int port, string realm, string scheme, IAuthCallback callback)
        {
            //Console.WriteLine("GetAuthCredentials");
            return true;
        }

        public IResponseFilter GetResourceResponseFilter(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IResponse response)
        {
            //Console.WriteLine("GetResourceResponseFilter");
            string url = request.Url;
            foreach(var item in this.UrlMonitor.Values)
            {
                if (item.isMatch(url))
                {
                    var dataFilter = new MemoryStreamResponseFilter();
                    dataFilter.UrlMonitorId = item.id;
                    responseDictionary.Add(request.Identifier, dataFilter);
                    return dataFilter;
                }
            }
            return null;
        }

        public bool OnBeforeBrowse(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, bool userGesture, bool isRedirect)
        {
            //Console.WriteLine("OnBeforeBrowse");
            return false;
        }

        public CefReturnValue OnBeforeResourceLoad(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IRequestCallback callback)
        {
            //Console.WriteLine("OnBeforeResourceLoad");
            return CefSharp.CefReturnValue.Continue;
        }

        public bool OnCertificateError(IWebBrowser chromiumWebBrowser, IBrowser browser, CefErrorCode errorCode, string requestUrl, ISslInfo sslInfo, IRequestCallback callback)
        {
            //Console.WriteLine("OnCertificateError");
            return false;
        }

        public bool OnOpenUrlFromTab(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, string targetUrl, WindowOpenDisposition targetDisposition, bool userGesture)
        {
            //Console.WriteLine("OnOpenUrlFromTab");
            return false;
        }

        public void OnPluginCrashed(IWebBrowser chromiumWebBrowser, IBrowser browser, string pluginPath)
        {
            //Console.WriteLine("OnPluginCrashed");
        }

        public bool OnProtocolExecution(IWebBrowser chromiumWebBrowser, IBrowser browser, string url)
        {
            //Console.WriteLine("OnProtocolExecution");
            return true;
        }

        public bool OnQuotaRequest(IWebBrowser chromiumWebBrowser, IBrowser browser, string originUrl, long newSize, IRequestCallback callback)
        {
            //Console.WriteLine("OnQuotaRequest");
            return true;
        }

        public void OnRenderProcessTerminated(IWebBrowser chromiumWebBrowser, IBrowser browser, CefTerminationStatus status)
        {
            //Console.WriteLine("OnRenderProcessTerminated");
        }

        public void OnRenderViewReady(IWebBrowser chromiumWebBrowser, IBrowser browser)
        {
            //Console.WriteLine("OnRenderViewReady");
        }

        public void OnResourceLoadComplete(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IResponse response, UrlRequestStatus status, long receivedContentLength)
        {
            //Console.WriteLine("OnResourceLoadComplete");
            if (responseDictionary.ContainsKey(request.Identifier))
            {
                MemoryStreamResponseFilter filter = responseDictionary[request.Identifier];
                if (!UrlMonitor.ContainsKey(filter.UrlMonitorId))
                {
                    return;
                }
                UrlMonitor[filter.UrlMonitorId].SetResult(request.Url, filter.Data);
            }
        }

        public void OnResourceRedirect(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IResponse response, ref string newUrl)
        {
            //Console.WriteLine("OnResourceRedirect");
        }

        public bool OnResourceResponse(IWebBrowser chromiumWebBrowser, IBrowser browser, IFrame frame, IRequest request, IResponse response)
        {
            //Console.WriteLine("OnResourceResponse");
            return false;
        }

        public bool OnSelectClientCertificate(IWebBrowser chromiumWebBrowser, IBrowser browser, bool isProxy, string host, int port, X509Certificate2Collection certificates, ISelectClientCertificateCallback callback)
        {
            //Console.WriteLine("OnSelectClientCertificate");
            return true;
        }
    }
}
