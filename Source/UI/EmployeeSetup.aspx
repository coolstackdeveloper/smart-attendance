<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="EmployeeSetup.aspx.cs" Inherits="UI.EmployeeSetup" %>
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
                                            ItemStyle-Wrap="true"
                                            HierarchyDefaultExpanded="true"
                                            AutoGenerateColumns="false">
                            <HeaderStyle Font-Bold="true" />
                            <CommandItemSettings AddNewRecordText="Add Employee" />
                            <EditFormSettings EditColumn-ButtonType="ImageButton" />
                            <Columns>
                                <telerik:GridEditCommandColumn ButtonType="ImageButton"
                                                               UniqueName="EditColumn" HeaderStyle-Width="25px" />
                                <telerik:GridButtonColumn ButtonType="ImageButton"
                                                          CommandName="Delete" 
                                                          Text="Delete"
                                                          UniqueName="DeleteColumn" HeaderStyle-Width="25px"   />
                                <telerik:GridBoundColumn DataField="Code" HeaderText="Staff Code"/>
                                <telerik:GridBoundColumn DataField="FirstName" HeaderText="First Name"/>
                                <telerik:GridBoundColumn DataField="MiddleName" HeaderText="Middle Name" />
                                <telerik:GridBoundColumn DataField="LastName" HeaderText="Last Name"/>
                                <telerik:GridDateTimeColumn DataFormatString="{0:dd-MMM-yyyy}" DataField="JoiningDate" HeaderText="Joining Date"/>
                                <telerik:GridTemplateColumn DataField="Gender" HeaderText="Gender" >
                                    <ItemTemplate>
                                        <asp:Label  runat="server" Text='<%# Eval("Gender") %>' />
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <asp:RadioButtonList runat="server"
                                                             RepeatDirection="Horizontal"
                                                             ID="m_rblTypes">
                                            <asp:ListItem Text="Male" Value="Male" Selected="True" />
                                            <asp:ListItem Text="Female" Value="Female" />
                                        </asp:RadioButtonList>
                                    </EditItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn HeaderText="Type">
                                    <ItemTemplate>
                                        <asp:Label runat="server" Text='<%# Eval("Type") %>' />
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <telerik:RadComboBox runat="server"
                                                             ID="m_cboEmployeeTypes"
                                                             Width="230px"
                                                             ShowWhileLoading="true"
                                                             ViewStateMode="Enabled"
                                                             MaxHeight="100"
                                                             MarkFirstMatch="true"
                                                             EnableLoadOnDemand="true"
                                                             OnItemsRequested="m_cboEmployeeTypes_ItemsRequested"
                                                             EmptyMessage="Assign employee type" />
                                    </EditItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn HeaderText="Department">
                                    <ItemTemplate>
                                        <asp:Label runat="server" Text='<%# Eval("Department") %>' />
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <telerik:RadComboBox runat="server"
                                                             ID="m_cboDepartments"
                                                             Width="230px"
                                                             ViewStateMode="Enabled"
                                                             ShowWhileLoading="true"
                                                             MaxHeight="100"
                                                             MarkFirstMatch="true"
                                                             EnableLoadOnDemand="true"
                                                             OnItemsRequested="m_cboDepartments_ItemsRequested"
                                                             EmptyMessage="Assign department" />
                                    </EditItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn HeaderText="Designation">
                                    <ItemTemplate>
                                        <asp:Label  runat="server" Text='<%# Eval("Designation") %>' />
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <telerik:RadComboBox runat="server"
                                                             ID="m_cboDesignations"
                                                             Width="230px"
                                                             ShowWhileLoading="true"
                                                             ViewStateMode="Enabled"
                                                             MaxHeight="100"
                                                             MarkFirstMatch="true"
                                                             EnableLoadOnDemand="true"
                                                             OnItemsRequested="m_cboDesignations_ItemsRequested"
                                                             EmptyMessage="Assign designation" />
                                    </EditItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridBoundColumn DataField="Login" HeaderText="Login"/>
                                <telerik:GridTemplateColumn HeaderText="Role">
                                    <ItemTemplate>
                                        <asp:Label ID="m_lblRole" runat="server" Text='<%# Eval("Login.Role") %>' />
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                        <telerik:RadComboBox runat="server"
                                                             ID="m_cboRoles"
                                                             Width="230px"
                                                             ShowWhileLoading="true"
                                                             ViewStateMode="Enabled"
                                                             MarkFirstMatch="true"
                                                             MaxHeight="100"
                                                             EnableLoadOnDemand="true"
                                                             OnItemsRequested="m_cboRoles_ItemsRequested"
                                                             EmptyMessage="Select role" />
                                    </EditItemTemplate>
                                </telerik:GridTemplateColumn>
                                <telerik:GridTemplateColumn HeaderText=" Contact Info" DataField="Address">
                                    <ItemTemplate>
                                        <asp:Label ID="Label1" runat="server" Text='<%# Eval("Address") %>'/>
                                    </ItemTemplate>
                                    <EditItemTemplate>
                                           <div style="padding-bottom:10px;padding-top:5px">
                                                <telerik:RadComboBox runat="server"
                                                                     ID="m_cboContactDetails"
                                                                     Width="300px"
                                                                     AutoPostBack="true"
                                                                     ShowWhileLoading="false"
                                                                     MaxHeight="100"
                                                                     MarkFirstMatch="true"
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
                <div>
                    <asp:Label runat="server" ID="m_lblError" Font-Bold="true" ForeColor="Red" />
                </div>
            </div>
        </ContentTemplate>          
        </asp:UpdatePanel>
</asp:Content>