using System;
using System.Web;
using System.Linq;
using System.Web.UI;
using System.Collections;
using System.Web.Security;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using CoreLib;
using Telerik.Web.UI;

namespace UI
{
    public partial class Site : System.Web.UI.MasterPage
    {
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            m_pnlItemCredentials.Visible = Global.CurrentUser != null;

            if (m_pnlItemCredentials.Visible)
            {
                m_pnlItemLogonUser.Text = string.Format("Sign Out ({0})", Global.CurrentUser.Name);
            }
        }

        protected void m_pnlBar_ItemClick(object sender, RadPanelBarEventArgs e)
        {
            if (e.Item == m_pnlItemLogonUser)
            {
                Response.Redirect("Login.aspx");
            }
            else
            {
                if (!string.IsNullOrEmpty(e.Item.Value))
                {
                    Response.Redirect(e.Item.Value); ;
                }
            }
        }
    }
}