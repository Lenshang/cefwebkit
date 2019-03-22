using CefOperator.CefCore.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CefOperator.Model
{
    public enum RULEMODE
    {
        REGEX=1,
        CONTAIN=2,
        NOTCONTAIN=3,
        EQUAL=4
    }
    public class HtmlContent
    {
        public string url { get; set; }
        public string text { get; set; }
        public string contentB64 { get; set; }
    }
    public class UrlMonitorModel
    {
        public string id { get; set; }
        public string urlRule { get; set; }
        public RULEMODE mode { get; set; }
        public MemoryStreamResponseFilter filter { get; set; }
        public List<HtmlContent> htmls { get; set; }
        
        private object locker { get; set; }
        public UrlMonitorModel()
        {
            this.locker = new object();
        }
        public bool isMatch(string url)
        {
            if (mode == RULEMODE.CONTAIN)
            {
                return url.Contains(urlRule);
            }
            else if(mode == RULEMODE.EQUAL)
            {
                return url == urlRule;
            }
            else if(mode == RULEMODE.NOTCONTAIN)
            {
                return !url.Contains(urlRule);
            }
            else
            {
                return Regex.IsMatch(url, urlRule);
            }
        }
        public bool SetResult(string url,string htmlText)
        {
            lock (locker)
            {
                this.htmls.Add(new HtmlContent() {
                    url = url,
                    text = htmlText,
                    contentB64=Convert.ToBase64String(Encoding.UTF8.GetBytes(htmlText))
                });
            }
            return true;
        }
        public bool SetResult(string url, byte[] htmlBytes)
        {
            lock (locker)
            {
                string htmlText = Encoding.UTF8.GetString(htmlBytes);
                this.htmls.Add(new HtmlContent() {
                    url = url,
                    text = htmlText,
                    contentB64 = Convert.ToBase64String(htmlBytes)
                });
            }
            return true;
        }
    }
}
