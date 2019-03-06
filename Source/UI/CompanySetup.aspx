<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CompanySetup.aspx.cs" Inherits="UI.CompanySetup" %>
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
                                        OnInsertCommand="m_dgvGrid_InsertCommand"
                                        OnItemCreated="m_dgvGrid_ItemCreated"
                                        OnDeleteCommand="m_dgvGrid_DeleteCommand"
                                        OnItemDataBound="m_dgvGrid_ItemDataBound"
                                        OnUpdateCommand="m_dgvGrid_UpdateCommand"
                                        OnNeedDataSource="m_dgvGrid_NeedDataSource">
                        <PagerStyle Mode="NumericPages" Position="Top" />
                        <MasterTableView TableLayout="Fixed"  runat="server" 
                                         Width="100%"
                                        DataKeyNames="ID"  
                                        NoMasterRecordsText=""
                                        CommandItemDisplay="Top"
                                        HierarchyDefaultExpanded="true"
                                        AutoGenerateColumns="false">
                            <CommandItemSettings AddNewRecordText="Add Company" />
                            <EditFormSettings EditColumn-ButtonType="ImageButton"  />
                            <Columns>
                                <telerik:GridEditCommandColumn ButtonType="ImageButton" 
                                                               UniqueName="EditColumn" HeaderStyle-Width="25px"   />
                                <telerik:GridButtonColumn ButtonType="ImageButton"
                                                          CommandName="Delete" 
                                                          UniqueName="DeleteColumn" HeaderStyle-Width="25px" 
                                                          Text="Delete"  />
                                <telerik:GridTemplateColumn HeaderText="Company Name">
                                    <ItemTemplate>
                                        <asp:Label runat="server" Text='<%# Eval("Name") %>' />
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:TextBox runat="server" ID="m_txtCompName" />
                                    </EditItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn HeaderText=" Contact Info" DataField="Address">
                                    <ItemTemplate>
                                        <asp:Label runat="server" Text='<%# Eval("Address") %>'/>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                           <div style="padding-bottom:10px;padding-top:5px">
                                                <telerik:RadComboBox runat="server"
                                                                     ID="m_cboContactDetails"
                                                                     Width="300px"
                                                                     AutoPostBack="true"
                                                                     MarkFirstMatch="true"
                                                                     ShowWhileLoading="false"
                                                                     MaxHeight="100"
                                                                     EnableLoadOnDemand="true"
                                                                     OnItemsRequested="m_cboContactDetails_ItemsRequested"
                                                                     OnSelectedIndexChanged="m_cboContactDetails_SelectedIndexChanged"
                                                                     EmptyMessage="Select contact fields" />
                                            </div>
                                            <div>
                                                <asp:Repeater runat="server"
                                                              ID="m_rpCompanyContactInfo">
                                                    <ItemTemplate>
                                                        <div>
                                                            <div>
                                                                <asp:LinkButton ID="m_btnDelete"
                                                                                OnClick="m_btnDelete_Click"
                                                                                CommandArgument='<%# Eval("ID") %>'
                                                                                runat="server" Text="Delete" />
                                                                <asp:Label ID="m_lblLabel" runat="server" Text='<%# Eval("Name") %>'/>
                                                            </div>
                                                            <div>
                                                                <asp:TextBox ID="m_txtData" runat="server"
                                                                             Width="400px"
                                                                             Text='<%# Eval("Value") %>' />
                                                            </div>
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
                </div>
            </div>
            <div>
                <asp:Label runat="server" ID="m_lblError" Font-Bold="true" ForeColor="Red" />
            </div>
        </ContentTemplate>          
        </asp:UpdatePanel>
</asp:Content>
