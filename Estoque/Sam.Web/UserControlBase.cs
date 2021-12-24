using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sam.Web
{
    public class UserControlBase : System.Web.UI.UserControl
    {
        public static void SetSession<T>(T obj, string sessionName)
        {
            HttpContext.Current.Session.Add(sessionName, obj);
        }
        public static T GetSession<T>(string sessionName)
        {
            if (HttpContext.Current.Session[sessionName] != null)
                return (T)HttpContext.Current.Session[sessionName];
            else
                return default(T);
        }
        public static void RemoveSession(string sessionName)
        {
            HttpContext.Current.Session[sessionName] = null;
            HttpContext.Current.Session.Remove(sessionName);
        }
    }
}
