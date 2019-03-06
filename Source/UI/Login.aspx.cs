using System;
using System.Web;
using System.Linq;
using System.Web.UI;
using System.Collections;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using CoreLib;
using Utilities;

namespace UI
{
    public partial class Login : Page
    {
        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);

            if (!BusinessEntity.Initialized)
            {
                Response.Redirect("CompanySetup.aspx");
            }
        }

        protected void m_btnLogin_Click(object sender, EventArgs e)
        {
            m_lblError.Text = "";

            if (string.IsNullOrEmpty(m_txtLogin.Text))
            {
                m_lblError.Text = string.Format(BasePage.ERROR_FORMAT, "Invalid user name provided.");
            }
            else
            {
                var sessionID = HttpContext.Current.Session.SessionID;
                var login = CoreLib.Login.Authenticate(m_txtLogin.Text, m_txtPassword.Text);

                if (login != null)
                {
                    if (!Global.m_sSessionUserMap.ContainsKey(sessionID))
                    {
                        Global.m_sSessionUserMap.Add(sessionID, login);
                    }

                    Global.m_sSessionUserMap[sessionID] = login;
                    Response.Redirect("CompanySetup.aspx");
                }
                else
                {
                    m_lblError.Text = string.Format(BasePage.ERROR_FORMAT, "Invalid user name/password provided.");
                }
            }
        }
    }
}