<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="RoleSetup.aspx.cs" Inherits="UI.RoleSetup" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="m_head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="m_cpChild" runat="server">
    <asp:UpdatePanel ID="UpdatePanel2" runat="server"
                        UpdateMode="Conditional"
                        ChildrenAsTriggers="true">
        <ContentTemplate>
            <div>
                <div>
                    <telerik:RadGrid ID="m_dgvGrid" 
                                        runat="server"
                                        Width="100%"
                                        AllowPaging="True" 
                                        PageSize="50"
                                        OnItemDataBound="m_dgvGrid_ItemDataBound"
                                        OnItemCreated="m_dgvGrid_ItemCreated"
                                        OnNeedDataSource="m_dgvGrid_NeedDataSource"
                                        OnCancelCommand="m_dgvGrid_CancelCommand"
                                        OnUpdateCommand="m_dgvGrid_UpdateCommand"
                                        OnInsertCommand="m_dgvGrid_InsertCommand"
                                        OnDeleteCommand="m_dgvGrid_DeleteCommand">
                        <PagerStyle Mode="NumericPages" Position="Top" />
                        <MasterTableView TableLayout="Fixed"  runat="server" 
                                        DataKeyNames="ID" NoMasterRecordsText=""
                                        CommandItemDisplay="Top"
                                        HierarchyDefaultExpanded="true"
                                        AutoGenerateColumns="false">
                            <CommandItemSettings AddNewRecordText="Add Role" />
                            <EditFormSettings EditColumn-ButtonType="ImageButton" />
                            <Columns>
                                <telerik:GridEditCommandColumn ButtonType="ImageButton"
                                                               UniqueName="EditColumn" HeaderStyle-Width="25px"  />
                                <telerik:GridButtonColumn ButtonType="ImageButton"
                                                          UniqueName="DeleteColumn" HeaderStyle-Width="25px" 
                                                          CommandName="Delete" 
                                                          Text="Delete" />
                                <telerik:GridBoundColumn  HeaderText="Name" DataField="Name" />
                                <telerik:GridTemplateColumn Visible="false" HeaderText="Task List">
                                    <EditItemTemplate>
                                        <hr />
                                        <div style="height:300px;width:300px;overflow:auto">
                                            <asp:Repeater runat="server" 
                                                            ID="m_rpTaskList">
                                                <ItemTemplate>
                                                    <div style="clear:both;width:100%;">
                                                        <asp:CheckBox ID="m_chkTask" runat="server" OnCheckedChanged="m_chkTask_CheckedChanged" />
                                                        <asp:Label ID="m_lblLabel" runat="server"   Text='<%# Eval("Name") %>' Font-Bold="true"/>
                                                        <asp:LinkButton ID="m_btnDelete"
                                                                        Visible="false"
                                                                        OnClick="m_btnDelete_Click"
                                                                        CommandName='<%# Eval("ID") %>'
                                                                        CommandArgument='<%# Eval("ID") %>'
                                                                        runat="server" Text="Delete">
                                                        </asp:LinkButton>
                                                    </div>
                                                </ItemTemplate>
                                            </asp:Repeater>
                                        </div>
                                    </EditItemTemplate>
                                </telerik:GridTemplateColumn>
                            </Columns>
                        </MasterTableView>
                    </telerik:RadGrid>
                </div>
                <div>
                    <asp:HiddenField runat="server" ID="m_hidden" />
                    <asp:Label runat="server" ID="m_lblError" Font-Bold="true" ForeColor="Red" />
                </div>
            </div>
        </ContentTemplate>          
        </asp:UpdatePanel>
</asp:Content>