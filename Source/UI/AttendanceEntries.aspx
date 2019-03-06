<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AttendanceEntries.aspx.cs" Inherits="UI.AttendanceEntries" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="m_head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="m_cpChild" runat="server">
    <asp:UpdatePanel ID="UpdatePanel2" runat="server"
                        UpdateMode="Conditional"
                        ChildrenAsTriggers="true">
        <ContentTemplate>
            <div>
                <div style="padding-bottom:2px" runat="server" id="m_divTop">
                    <asp:Label runat="server" 
                               ID="m_lblSelected" 
                               Font-Bold="true"
                               Text="Select Company/Branch"/>
                    <telerik:RadComboBox runat="server"
                                         ID="m_cboEntities"
                                         Width="230px"
                                         AutoPostBack="true"
                                         ShowWhileLoading="true"
                                         MaxHeight="100"
                                         MarkFirstMatch="true"
                                         EnableLoadOnDemand="true"
                                         OnSelectedIndexChanged="m_cboEntities_SelectedIndexChanged"
                                         OnItemsRequested="m_cboEntities_ItemsRequested"
                                         EmptyMessage="Click here to select" />

                    <asp:Label runat="server" 
                               Font-Bold="true"
                               Text="Select employee"/>
                    <telerik:RadComboBox runat="server"
                                         ID="m_cboEmployees"
                                         Width="230px"
                                         AutoPostBack="true"
                                         MarkFirstMatch="true"
                                         ShowWhileLoading="true"
                                         MaxHeight="100"
                                         EnableLoadOnDemand="true"
                                         OnSelectedIndexChanged="m_cboEmployees_SelectedIndexChanged"
                                         OnItemsRequested="m_cboEmployees_ItemsRequested"
                                         EmptyMessage="Click here to select" />
                </div>
                <div>
                    <telerik:RadGrid ID="m_dgvGrid" 
                                        runat="server"
                                        Width="100%"
                                        AllowPaging="True" 
                                        PageSize="50"
                                        OnItemCreated="m_dgvGrid_ItemCreated"
                                        OnItemDataBound="m_dgvGrid_ItemDataBound"
                                        OnCancelCommand="m_dgvGrid_CancelCommand"
                                        OnNeedDataSource="m_dgvGrid_NeedDataSource"
                                        OnUpdateCommand="m_dgvGrid_UpdateCommand"
                                        OnInsertCommand="m_dgvGrid_InsertCommand"
                                        OnDeleteCommand="m_dgvGrid_DeleteCommand">
                        <PagerStyle Mode="NumericPages" Position="Top" />
                        <MasterTableView TableLayout="Fixed"  runat="server" 
                                            DataKeyNames="ID" 
                                            CommandItemDisplay="Top"
                                            NoMasterRecordsText=""
                                            HierarchyDefaultExpanded="true"
                                            AutoGenerateColumns="false">
                            <HeaderStyle Font-Bold="true" />
                            <CommandItemSettings AddNewRecordText="Add Entry" />
                            <EditFormSettings EditColumn-ButtonType="ImageButton" />
                            <Columns>
                                <telerik:GridEditCommandColumn ButtonType="ImageButton" 
                                                               UniqueName="EditColumn" HeaderStyle-Width="25px" />
                                <telerik:GridButtonColumn ButtonType="ImageButton"
                                                          CommandName="Delete" 
                                                          Text="Delete"
                                                          UniqueName="DeleteColumn" HeaderStyle-Width="25px"  />
                                <telerik:GridTemplateColumn HeaderText="Type">
                                    <ItemTemplate>
                                        <asp:Label ID="m_lblExceptionType" runat="server" Text='<%# Eval("Type") %>' />
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <telerik:RadComboBox runat="server"
                                                             ID="m_cboExceptionTypes"
                                                             Width="230px"
                                                             ViewStateMode="Enabled"
                                                             MarkFirstMatch="true"
                                                             ShowWhileLoading="true"
                                                             MaxHeight="100"
                                                             EnableLoadOnDemand="true"
                                                             OnItemsRequested="m_cboExceptionTypes_ItemsRequested"
                                                             EmptyMessage="Select entry type" />
                                    </EditItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridDateTimeColumn DataField="Start" 
                                                            PickerType="DateTimePicker"
                                                            HeaderText="Start" />
                                <telerik:GridDateTimeColumn DataField="End" 
                                                            PickerType="DateTimePicker"
                                                            HeaderText="End" />
                                <telerik:GridBoundColumn DataField="Remarks" HeaderText="Remarks" ItemStyle-Width="300px" />
                                <telerik:GridTemplateColumn UniqueName="ApprovedColumn" HeaderText="Approved">
                                    <ItemTemplate>
                                        <asp:CheckBox runat="server" Enabled="false" Checked='<%# Eval("Approved") %>' />
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:CheckBox runat="server" ID="m_chkApproved" Text="" />
                                    </EditItemTemplate>
                                </telerik:GridTemplateColumn>
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