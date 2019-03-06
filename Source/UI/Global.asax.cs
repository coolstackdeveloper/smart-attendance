using System;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Configuration;
using System.Web.SessionState;
using System.Collections.Generic;

using DataAccess;

namespace UI
{
    public class Global : System.Web.HttpApplication
    {
        internal static Dictionary<string, CoreLib.Login> m_sSessionUserMap = new Dictionary<string, CoreLib.Login>();

        internal static CoreLib.Login CurrentUser
        {
            get
            {
                lock (Global.m_sSessionUserMap)
                {
                    CoreLib.Login currentUser = null;

                    if (Global.m_sSessionUserMap.ContainsKey(HttpContext.Current.Session.SessionID))
                    {
                        currentUser = Global.m_sSessionUserMap[HttpContext.Current.Session.SessionID];
                    }

                    return currentUser;
                }
            }
            set
            {
                lock (Global.m_sSessionUserMap)
                {
                    if (value == null)
                    {
                        if (Global.m_sSessionUserMap.ContainsKey(HttpContext.Current.Session.SessionID))
                        {
                            m_sSessionUserMap.Remove(HttpContext.Current.Session.SessionID);
                        }
                    }
                }
            }
        }

        void Application_Start(object sender, EventArgs e)
        {
            DataAccess.Properties.Settings.Default["ITradsConnectionString"] = ConfigurationManager.ConnectionStrings["LocalSqlServer"].ConnectionString;
        }

        void Application_End(object sender, EventArgs e)
        {
        }

        void Application_Error(object sender, EventArgs e)
        {
        }

        void Session_Start(object sender, EventArgs e)
        {
        }

        void Session_End(object sender, EventArgs e)
        {
            if (HttpContext.Current != null && HttpContext.Current.Session != null)
            {
                m_sSessionUserMap.Remove(HttpContext.Current.Session.SessionID);
            }
        }
    }
}
