<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ChangePassword.aspx.cs" Inherits="UI.ChangePassword" %>
<asp:Content ID="Content1" ContentPlaceHolderID="m_head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="m_cpChild" runat="server">
    <asp:UpdatePanel runat="server" UpdateMode="Conditional" ChildrenAsTriggers="true">
        <ContentTemplate>
            <div style="width:300px;
                        border-style:outset;
                        padding:2px;
                        clear:both">
                <table cols="2" width="100%">
                    <tr>
                        <td colspan="2">
                            <asp:Label ID="Label1" runat="server" 
                                       Font-Bold="true" Text="Change Password" Width="100%" />
                            <hr />
                        </td>
                    </tr>
                    <tr>
                        <td width="150px">Existing Password</td>
                        <td>
                            <asp:TextBox runat="server" 
                                            TextMode="Password"
                                            ID="m_txtExistingPassword" Width="99%" /> 
                        </td>
                    </tr>
                    <tr>
                        <td width="150px">New Password</td>
                        <td>
                            <asp:TextBox runat="server" 
                                            TextMode="Password"
                                            ID="m_txtNewPassword" Width="99%" /> 
                        </td>
                    </tr>
                    <tr>
                        <td width="150px" />
                        <td style="text-align:right;">
                            <asp:Button runat="server" 
                                        ID="m_btnSavePassword" 
                                        OnClick="m_btnSavePassword_Click"
                                        Text="Save" />
                        </td>
                    </tr>
                </table>
            </div>
            <asp:Label runat="server" ID="m_lblError"  Font-Bold="true" ForeColor="Red" Width="100%" />
        </ContentTemplate>
    </asp:UpdatePanel>
</asp:Content>
