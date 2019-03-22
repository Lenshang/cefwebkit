using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CefWebKit.Controller.Model
{
    public class RegAction: IAppAction
    {
        public string action { get; set; }
        public string param { get; set; }
        public string mode { get; set; }
    }
}
