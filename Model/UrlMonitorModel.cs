using CefWebKit.CefCore.Filters;
using Chen.CommonLibrary;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CefWebKit.Model
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
        private string dirName = "html_content_temp";
        private FileHelper fh = new FileHelper();
        public HtmlContent()
        {
            if (!Directory.Exists(dirName))
            {
                Directory.CreateDirectory(dirName);
            }
        }
        public string _id { get; set; }
        public string url { get; set; }
        public string text {
            get
            {
                return Encoding.UTF8.GetString(this.getBytes());
            }
        }
        public string contentB64 {
            get
            {
                return Convert.ToBase64String(this.getBytes());
            }
        }
        private string cacheFile { get; set; }
        public void setBytes(byte[] bts,string name)
        {
            var filename=MD5Helper.getMd5Hash(name)+".cache";
            this._id = filename;
            this.cacheFile = Path.Combine(dirName, filename);
            fh.SaveFile(this.cacheFile, bts);
        }

        public byte[] getBytes()
        {
            if (File.Exists(this.cacheFile))
            {
                return fh.readFileByte(this.cacheFile);
            }
            return null;
        }
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

        public bool SetResult(string url, byte[] htmlBytes)
        {
            lock (locker)
            {
                //string htmlText = Encoding.UTF8.GetString(htmlBytes);

                var content = new HtmlContent()
                {
                    url = url
                };
                content.setBytes(htmlBytes,url);
                this.htmls.Add(content);
                //this.htmls.Add(new HtmlContent() {
                //    url = url,
                //    text = htmlText,
                //    contentB64 = Convert.ToBase64String(htmlBytes)
                //});
            }
            return true;
        }
    }
}
