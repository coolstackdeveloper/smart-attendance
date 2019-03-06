<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="EmployeeTypeSetup.aspx.cs" Inherits="UI.EmployeeTypeSetup" %>
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
                                        OnNeedDataSource="m_dgvGrid_NeedDataSource"
                                        AllowPaging="True" 
                                        PageSize="50"
                                        OnItemCreated="m_dgvGrid_ItemCreated"
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
                            <CommandItemSettings AddNewRecordText="Add Employee Type" />
                            <EditFormSettings EditColumn-ButtonType="ImageButton" />
                            <Columns>
                                <telerik:GridEditCommandColumn ButtonType="ImageButton" 
                                                               UniqueName="EditColumn" HeaderStyle-Width="25px" />
                                <telerik:GridButtonColumn ButtonType="ImageButton"
                                                          CommandName="Delete" 
                                                          Text="Delete"
                                                          UniqueName="DeleteColumn" HeaderStyle-Width="25px"  />
                                <telerik:GridBoundColumn DataField="Name" HeaderText="Name"/>
                            </Columns>
                        </MasterTableView>
                    </telerik:RadGrid>
                </div>
                <div>
                    <asp:Label runat="server" ID="m_lblError" Font-Bold="true" ForeColor="Red" />
                </div>
            </div>
        </ContentTemplate>          
        </asp:UpdatePanel>
</asp:Content>