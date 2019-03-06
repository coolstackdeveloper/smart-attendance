<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ShiftSetup.aspx.cs" Inherits="UI.ShiftSetup" %>
<%@ Register Assembly="Telerik.Web.UI" Namespace="Telerik.Web.UI" TagPrefix="telerik" %>

<asp:Content ID="Content1" ContentPlaceHolderID="m_head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="m_cpChild" runat="server">
    <asp:UpdatePanel ID="UpdatePanel2" runat="server"
                        UpdateMode="Conditional"
                        ChildrenAsTriggers="true">
        <ContentTemplate>
            <div>
                <div style="padding-bottom:2px">
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
                </div>
                <div>
                    <telerik:RadGrid ID="m_dgvGrid" 
                                        runat="server"
                                        Width="100%"
                                        AllowPaging="True" 
                                        PageSize="50"
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
                            <HeaderStyle Font-Bold="true" />
                            <ItemStyle Wrap="true" />
                            <CommandItemSettings AddNewRecordText="Add Shift" />
                            <EditFormSettings EditColumn-ButtonType="ImageButton" />
                            <Columns>
                                <telerik:GridEditCommandColumn ButtonType="ImageButton" 
                                                               UniqueName="EditColumn" HeaderStyle-Width="25px" />
                                <telerik:GridButtonColumn ButtonType="ImageButton"
                                                          CommandName="Delete" 
                                                          Text="Delete"
                                                          UniqueName="DeleteColumn" HeaderStyle-Width="25px"  />
                                <telerik:GridBoundColumn DataField="Name" HeaderText="Name"/>
                                <telerik:GridDateTimeColumn DataField="PunchInStartTime" 
                                                            DataFormatString="{0:hh:mm tt}"
                                                            PickerType="TimePicker"
                                                            HeaderText="Punch-In Start" />
                                <telerik:GridDateTimeColumn DataField="StartTime" 
                                                            DataFormatString="{0:hh:mm tt}"
                                                            PickerType="TimePicker"
                                                            HeaderText="Start" />
                                <telerik:GridDateTimeColumn DataField="PunchInEndTime" 
                                                            DataFormatString="{0:hh:mm tt}"
                                                        PickerType="TimePicker"
                                                        HeaderText="Punch-In End" />
                                <telerik:GridDateTimeColumn DataField="PunchOutStartTime" 
                                                            DataFormatString="{0:hh:mm tt}"
                                                            PickerType="TimePicker"
                                                            HeaderText="Punch-Out Start" />
                                <telerik:GridDateTimeColumn DataField="EndTime" 
                                                            DataFormatString="{0:hh:mm tt}"
                                                        PickerType="TimePicker"
                                                        HeaderText="End" />
                                <telerik:GridDateTimeColumn DataField="PunchOutEndTime" 
                                                        PickerType="TimePicker"
                                                            DataFormatString="{0:hh:mm tt}"
                                                        HeaderText="Punch-Out End" />
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