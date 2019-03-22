using CefSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CefWebKit.Controller.Model
{
    public class ApiResult
    {
        public bool IsSuccess { get; set; } = true;
        public string Message { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public object Data { get; set; }
        public List<Cookie> Cookies { get; set; }
    }
    public class ApiResult<T>
    {
        public bool IsSuccess { get; set; } = true;
        public string Message { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public T Data { get; set; }
    }
}
