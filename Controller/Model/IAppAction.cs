using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CefWebKit.Controller.Model
{
    public abstract class IAppAction
    {
        public string token { get; set; }

        public string appKey { get; set; }
    }
}
