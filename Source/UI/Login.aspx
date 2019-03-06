<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="UI.Login" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
    <head runat="server">
        <title>Time & Attendance Management System</title>
    </head>
    <body style="min-height:600px;min-width:800px;">
        <form id="form1" runat="server">
            <div style="overflow:auto;
                        border-bottom-style: solid; 
                        border-bottom-width: thin; 
                        border-bottom-color: #C0C0C0">
                <img alt="" src="Images/company_logo.png" height="60px" style="vertical-align:text-bottom" />
                <span style="font-size:x-large;visibility:visible">Time & Attendance Management System</span>
            </div>
            <div style="border: thin outset #C0C0C0; 
                        width:300px;
                        margin-top:5px;
                        padding:5px;
                        clear:both">
                <table cols="2" width="100%">
                    <tr>
                        <td colspan="2">
                            <asp:Label runat="server" Font-Bold="true" Text="Login" />
                            <hr />
                        </td>
                    </tr>
                    <tr>
                        <td width="90px">User Name</td>
                        <td>
                             <asp:TextBox runat="server" ID="m_txtLogin" Width="99%" /> 
                        </td>
                    </tr>
                    <tr>
                        <td width="90px">Password</td>
                        <td>
                            <asp:TextBox runat="server" 
                                         TextMode="Password"
                                         ID="m_txtPassword" Width="99%" /> 
                        </td>
                    </tr>
                    <tr>
                        <td />
                        <td style="text-align:right;">
                            <asp:Button runat="server" 
                                        ID="m_btnLogin" 
                                        OnClick="m_btnLogin_Click"
                                        Text="Login" />
                        </td>
                    </tr>
                </table>
            </div>
            <asp:Label runat="server" ID="m_lblError"  Font-Bold="true" ForeColor="Red" />
        </form>
    </body>
</html>
