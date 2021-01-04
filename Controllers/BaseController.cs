using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebServer.Controllers
{
    public class BaseController
    {
        public string GetExecutingRouteName {
            get {
                var currentMethodName = new StackTrace().GetFrame(1).GetMethod();

                return currentMethodName.Name.ToLower();
            }
        }
    }
}
