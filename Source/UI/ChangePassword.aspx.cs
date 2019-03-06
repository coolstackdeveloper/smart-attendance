using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace UI
{
    public partial class ChangePassword : System.Web.UI.Page
    {
        protected void m_btnSavePassword_Click(object sender, EventArgs e)
        {
            if (CoreLib.Login.ChangeCredential(Global.CurrentUser.Name, m_txtExistingPassword.Text, m_txtNewPassword.Text))
            {
                Response.Redirect("Login.aspx");
            }
            else
            {
                m_lblError.Text = string.Format(BasePage.ERROR_FORMAT, "Failed to change the credentials. Please check the password (existing/new) provided.");
            }
        }
    }
}