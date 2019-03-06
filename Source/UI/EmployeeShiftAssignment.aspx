<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="EmployeeShiftAssignment.aspx.cs" Inherits="UI.EmployeeShiftAssignment" %>
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

                    <asp:Label runat="server" 
                               Font-Bold="true"
                               Text="Select employee"/>
                    <telerik:RadComboBox runat="server"
                                         ID="m_cboEmployees"
                                         Width="230px"
                                         AutoPostBack="true"
                                         ShowWhileLoading="true"
                                         MaxHeight="100"
                                         MarkFirstMatch="true"
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
                                        OnDeleteCommand="m_dgvGrid_DeleteCommand"
                                        OnInsertCommand="m_dgvGrid_InsertCommand">
                        <PagerStyle Mode="NumericPages" Position="Top" />
                        <MasterTableView TableLayout="Fixed"  runat="server"
                                            NoMasterRecordsText=""
                                            CommandItemDisplay="Top"
                                            DataKeyNames="ID"  
                                            ItemStyle-Wrap="true"
                                            HierarchyDefaultExpanded="true"
                                            AutoGenerateColumns="false">
                            <HeaderStyle Font-Bold="true" />
                            <CommandItemSettings AddNewRecordText="Assign New Shift" />
                            <EditFormSettings EditColumn-ButtonType="ImageButton" />
                            <Columns>
                                <telerik:GridEditCommandColumn ButtonType="ImageButton"
                                                               UniqueName="EditColumn" HeaderStyle-Width="25px" />
                                <telerik:GridButtonColumn ButtonType="ImageButton"
                                                          CommandName="Delete" 
                                                          Text="Delete"
                                                          UniqueName="DeleteColumn" HeaderStyle-Width="25px"   />
                                <telerik:GridTemplateColumn HeaderText="Shift">
                                    <ItemTemplate>
                                        <asp:Label runat="server" Text='<%# Eval("Shift.Description") %>' />
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <telerik:RadComboBox runat="server"
                                                             ID="m_cboShifts"
                                                             Width="300px"
                                                             ShowWhileLoading="true"
                                                             ViewStateMode="Enabled"
                                                             MarkFirstMatch="true"
                                                             MaxHeight="100"
                                                             EnableLoadOnDemand="true"
                                                             OnItemsRequested="m_cboShifts_ItemsRequested"
                                                             EmptyMessage="Assign shift" />
                                    </EditItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridDateTimeColumn DataFormatString="{0:dd-MMM-yyyy}" DataField="Start" HeaderText="Start" />
                                <telerik:GridDateTimeColumn DataFormatString="{0:dd-MMM-yyyy}" DataField="End" HeaderText="End" />
                            </Columns>
                        </MasterTableView>
                    </telerik:RadGrid>
                </div>
                <div>
                    <asp:HiddenField runat="server" ID="m_hidden" />
                </div>
                <div>
                    <asp:Label runat="server" ID="m_lblError" Font-Bold="true" ForeColor="Red" />
                </div>
            </div>
        </ContentTemplate>          
        </asp:UpdatePanel>
</asp:Content>