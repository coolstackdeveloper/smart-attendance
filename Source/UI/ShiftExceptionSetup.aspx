<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ShiftExceptionSetup.aspx.cs" Inherits="UI.ShiftExceptionSetup" %>
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
                                         MarkFirstMatch="true"
                                         MaxHeight="100"
                                         EnableLoadOnDemand="true"
                                         OnSelectedIndexChanged="m_cboEntities_SelectedIndexChanged"
                                         OnItemsRequested="m_cboEntities_ItemsRequested"
                                         EmptyMessage="Click here to select" />

                    <asp:Label runat="server" 
                               ID="m_lblSelectedShift" 
                               Font-Bold="true"
                               Text="Select shift"/>
                    <telerik:RadComboBox runat="server"
                                         ID="m_cboShifts"
                                         Width="300px"
                                         AutoPostBack="true"
                                         ShowWhileLoading="true"
                                         MarkFirstMatch="true"
                                         MaxHeight="100"
                                         EnableLoadOnDemand="true"
                                         OnSelectedIndexChanged="m_cboShifts_SelectedIndexChanged"
                                         OnItemsRequested="m_cboShifts_ItemsRequested"
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
                                            DataKeyNames="ID" NoMasterRecordsText=""
                                            CommandItemDisplay="Top"
                                            HierarchyDefaultExpanded="true"
                                            AutoGenerateColumns="false">
                            <HeaderStyle Font-Bold="true" />
                            <CommandItemSettings AddNewRecordText="Add Shift Exception" />
                            <EditFormSettings EditColumn-ButtonType="ImageButton" />
                            <Columns>
                                <telerik:GridEditCommandColumn ButtonType="ImageButton" 
                                                               UniqueName="EditColumn" HeaderStyle-Width="25px" />
                                <telerik:GridButtonColumn ButtonType="ImageButton"
                                                          CommandName="Delete" 
                                                          Text="Delete"
                                                          UniqueName="DeleteColumn" HeaderStyle-Width="25px"  />
                                <telerik:GridDateTimeColumn DataField="StartTime" 
                                                            DataFormatString="{0:hh:mm tt}"
                                                            PickerType="TimePicker"
                                                            HeaderText="Start" />
                                <telerik:GridDateTimeColumn DataField="EndTime" 
                                                            DataFormatString="{0:hh:mm tt}"
                                                            PickerType="TimePicker"
                                                            HeaderText="End" />
                                <telerik:GridTemplateColumn HeaderText="Week Day">
                                    <ItemTemplate>
                                        <asp:Label runat="server" Text='<%# Eval("WeekDay") %>' />
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <telerik:RadComboBox runat="server"
                                                             ID="m_cboWeekDays"
                                                             Width="230px"
                                                             ShowWhileLoading="true"
                                                             ViewStateMode="Enabled"
                                                             MaxHeight="100"
                                                             MarkFirstMatch="true"
                                                             EnableLoadOnDemand="true"
                                                             OnItemsRequested="m_cboWeekDays_ItemsRequested"
                                                             EmptyMessage="Select week day" />
                                    </EditItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn HeaderText="Type">
                                    <ItemTemplate>
                                        <asp:Label runat="server" Text='<%# Eval("Type") %>' />
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
                                                             EmptyMessage="Select exception type" />
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