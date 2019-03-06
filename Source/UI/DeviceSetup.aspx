<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="DeviceSetup.aspx.cs" Inherits="UI.DeviceSetup" %>
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
                                         MarkFirstMatch="true"
                                         ShowWhileLoading="true"
                                         MaxHeight="100"
                                         EnableLoadOnDemand="true"
                                         OnSelectedIndexChanged="m_cboEntities_SelectedIndexChanged"
                                         OnItemsRequested="m_cboEntities_ItemsRequested"
                                         EmptyMessage="Click here to select" />
                </div>
                <div>
                    <telerik:RadGrid ID="m_dgvGrid" 
                                        runat="server"
                                        Width="100%"
                                        OnItemDataBound="m_dgvGrid_ItemDataBound"
                                        AllowPaging="True" 
                                        PageSize="50"
                                        OnItemCreated="m_dgvGrid_ItemCreated"
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
                            <CommandItemSettings AddNewRecordText="Add Device" />
                            <EditFormSettings EditColumn-ButtonType="ImageButton" />
                            <Columns>
                                <telerik:GridEditCommandColumn ButtonType="ImageButton" 
                                                               UniqueName="EditColumn" HeaderStyle-Width="25px" />
                                <telerik:GridButtonColumn ButtonType="ImageButton"
                                                          CommandName="Delete" 
                                                          Text="Delete"
                                                          UniqueName="DeleteColumn" HeaderStyle-Width="25px"  />
                                <telerik:GridBoundColumn DataField="Name" HeaderText="Name"/>
                                <telerik:GridBoundColumn DataField="Address" HeaderText="IP Address" />
                                <telerik:GridBoundColumn DataField="SubnetMask" HeaderText="Subnet Mask" />
                                <telerik:GridBoundColumn DataField="GatewayIP" HeaderText="Gateway IP Address" />
                                <telerik:GridBoundColumn DataField="MACAddress" HeaderText="MAC Address (Optional)" />
                                <telerik:GridTemplateColumn DataField="Type" HeaderText="Direction">
                                    <ItemTemplate>
                                        <asp:Label ID="Label1"  runat="server" Text='<%# Eval("Type") %>' />
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:RadioButtonList runat="server"
                                                          RepeatDirection="Horizontal"
                                                          ID="m_rblTypes">
                                            <asp:ListItem Text="Entry" Value="Entry"/>
                                            <asp:ListItem Text="Exit" Value="Exit" />
                                            <asp:ListItem Text="Entry & Exit" Value="Both" Selected="True" />
                                        </asp:RadioButtonList>
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