using System;
using System.Web;
using System.Linq;
using System.Web.UI;
using System.Collections;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using CoreLib;
using Telerik.Web.UI;

namespace UI
{
    [CoreLib.AllowedTasks(CoreLib.TaskType.AddEmployee, CoreLib.TaskType.DeleteEmployee,
                            CoreLib.TaskType.EditEmployeeDetails, CoreLib.TaskType.ViewEmployeeDetails)]
    public partial class EmployeeSetup : BasePage
    {
        protected void m_btnDelete_Click(object sender, EventArgs e)
        {
            LinkButton btnDelete = sender as LinkButton;
            m_businessEntity = Entity(m_cboEntities.SelectedValue);
            m_employee = m_businessEntity.FindEmployee(Int(m_hidden.Value));

            var contactDetail = ContactDetail(btnDelete.CommandArgument, m_employee);

            if (m_employee.RemoveContactDetail(contactDetail))
            {
                m_dgvGrid.Rebind();
            }
            else
            {
                DisplayError("Failed to delete the requested contact field.", m_lblError);
            }
        }

        protected void m_dgvGrid_ItemCreated(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridEditableItem && e.Item.ItemIndex >= 0)
            {
                (e.Item as GridEditableItem)["EditColumn"].Controls[0].Visible = IsSupported(TaskType.EditEmployeeDetails);
                (e.Item as GridEditableItem)["DeleteColumn"].Controls[0].Visible = IsSupported(TaskType.DeleteEmployee);
            }
        }
        
        protected void m_dgvGrid_ItemDataBound(object source, GridItemEventArgs e)
        {
            var item = e.Item as GridEditableItem;

            // Already in edit mode but not a new insert
            if (item != null && item.IsInEditMode) // Edit or Insert
            {
                if (!item.OwnerTableView.IsItemInserted)
                {
                    m_employee = Employee((e.Item as GridEditableItem).GetDataKeyValue("ID"), Entity(m_cboEntities.SelectedValue));
                    RadioButtonList rList = e.Item.FindControl("m_rblTypes") as RadioButtonList;
                    rList.SelectedIndex = -1;

                    foreach (ListItem rItem in rList.Items)
                    {
                        rItem.Selected = false;

                        if (rItem.Value.Equals(m_employee.Gender.ToString(), StringComparison.OrdinalIgnoreCase))
                        {
                            rItem.Selected = true;
                            rList.DataBind();
                            break;
                        }
                    }

                    var assignedType = m_employee.Type;
                    var assignedDepartment = m_employee.Department;
                    var assignedDesignation = m_employee.Designation;

                    var cboDepartments = e.Item.FindControl("m_cboDepartments") as RadComboBox;
                    m_cboDepartments_ItemsRequested(cboDepartments, null);
                    cboDepartments.SelectedValue = assignedDepartment == null ? "" : assignedDepartment.ID.ToString();

                    var cboDesignations = e.Item.FindControl("m_cboDesignations") as RadComboBox;
                    m_cboDesignations_ItemsRequested(cboDesignations, null);
                    cboDesignations.SelectedValue = assignedDesignation == null ? "" : assignedDesignation.ID.ToString();

                    var cboEmployeeTypes = e.Item.FindControl("m_cboEmployeeTypes") as RadComboBox;
                    m_cboEmployeeTypes_ItemsRequested(cboEmployeeTypes, null);
                    cboEmployeeTypes.SelectedValue = assignedType == null ? "" : assignedType.ID.ToString();

                    var cboRoles = e.Item.FindControl("m_cboRoles") as RadComboBox;
                    m_cboRoles_ItemsRequested(cboRoles, null);
                    cboRoles.SelectedValue = (m_employee.Login == null || m_employee.Login.Role == null) ? "" : m_employee.Login.Role.ID.ToString();

                    m_hidden.Value = m_employee.ID.ToString();
                    Repeater repeater = e.Item.FindControl("m_rpCompanyContactInfo") as Repeater;
                    repeater.DataSource = m_employee.ContactDetails;
                    repeater.DataBind();
                }
                else
                {
                    var cboContactDetails = e.Item.FindControl("m_cboContactDetails") as RadComboBox;
                    cboContactDetails.Enabled = false;
                }
            }
        }

        protected void m_dgvGrid_CancelCommand(object source, GridCommandEventArgs e)
        {
            DisplayError("", m_lblError);
        }

        protected void m_dgvGrid_InsertCommand(object source, GridCommandEventArgs e)
        {
            DisplayError("", m_lblError);
            Hashtable values = new Hashtable();
            GridEditableItem item = e.Item as GridEditableItem;
            item.ExtractValues(values);
            m_businessEntity = Entity(m_cboEntities.SelectedValue);
            m_joiningDate = DateTime(values["JoiningDate"]);
            var departmentID = Int((e.Item.FindControl("m_cboDepartments") as RadComboBox).SelectedValue);
            var designationID = Int((e.Item.FindControl("m_cboDesignations") as RadComboBox).SelectedValue);
            var employeeTypeID = Int((e.Item.FindControl("m_cboEmployeeTypes") as RadComboBox).SelectedValue);
            var roleID = Int((e.Item.FindControl("m_cboRoles") as RadComboBox).SelectedValue);
            
            m_role = Role(roleID);
            m_department = Department(departmentID);
            m_designation = Designation(designationID);
            m_employeeType = EmployeeType(employeeTypeID);
            var buttonList = e.Item.FindControl("m_rblTypes") as RadioButtonList;
            Gender gender = (Gender)Enum.Parse(typeof(Gender), buttonList.SelectedValue);

            var success = m_businessEntity.AddEmployee(String(values["Code"]), gender,
                                              String(values["FirstName"]), String(values["MiddleName"]), String(values["LastName"]),
                                              m_joiningDate,
                                              m_department, m_designation, m_employeeType);

            if (!success)
            {
                e.Canceled = true;
                DisplayError("Failed to add employee.", m_lblError);
            }

            if (success)
            {
                m_employee = m_businessEntity.FindEmployee(String(values["Code"]));

                if (!string.IsNullOrEmpty(String(values["Login"])) && m_role != null)
                {
                    if (!m_employee.AssignLogin(String(values["Login"]), String(values["Login"]), m_role))
                    {
                        e.Canceled = true;
                        DisplayError("Failed to assign role to employee.", m_lblError);
                    }
                }
            }
        }

        protected void m_dgvGrid_DeleteCommand(object source, GridCommandEventArgs e)
        {
            DisplayError("", m_lblError);
            GridEditableItem item = e.Item as GridEditableItem;
            m_businessEntity = Entity(m_cboEntities.SelectedValue);

            if (!m_businessEntity.RemoveEmployee(Employee(item.GetDataKeyValue("ID"), m_businessEntity)))
            {
                e.Canceled = true;
                DisplayError("Failed to delete employee.", m_lblError);
            }
        }

        protected void m_dgvGrid_UpdateCommand(object source, GridCommandEventArgs e)
        {
            DisplayError("", m_lblError);
            Hashtable values = new Hashtable();
            GridEditableItem item = e.Item as GridEditableItem;
            item.ExtractValues(values);
            m_businessEntity = Entity(m_cboEntities.SelectedValue);
            m_employee = Employee(item.GetDataKeyValue("ID"), m_businessEntity);

            m_joiningDate = DateTime(values["JoiningDate"]);
            var departmentID = Int((e.Item.FindControl("m_cboDepartments") as RadComboBox).SelectedValue);
            var designationID = Int((e.Item.FindControl("m_cboDesignations") as RadComboBox).SelectedValue);
            var employeeTypeID = Int((e.Item.FindControl("m_cboEmployeeTypes") as RadComboBox).SelectedValue);
            var roleID = Int((e.Item.FindControl("m_cboRoles") as RadComboBox).SelectedValue);

            m_department = Department(departmentID);
            m_designation = Designation(designationID);
            m_employeeType = EmployeeType(employeeTypeID);
            m_role = Role(roleID);
            var buttonList = e.Item.FindControl("m_rblTypes") as RadioButtonList;
            Gender gender = (Gender)Enum.Parse(typeof(Gender), buttonList.SelectedValue);

            var success = m_employee.UpdateDetails(String(values["Code"]), gender,
                                                      String(values["FirstName"]), String(values["MiddleName"]), String(values["LastName"]),
                                                      m_joiningDate,
                                                      m_department, m_designation, m_employeeType);

            if (!success)
            {
                e.Canceled = true;
                DisplayError("Failed to update employee details.", m_lblError);
            }

            if (success)
            {
                if (!string.IsNullOrEmpty(String(values["Login"])) && m_role != null)
                {
                    if (m_employee.Login == null)
                    {
                        if (!m_employee.AssignLogin(String(values["Login"]), String(values["Login"]), m_role))
                        {
                            e.Canceled = true;
                            DisplayError("Failed to assign role to employee.", m_lblError);
                        }
                    }
                    else
                    {
                        bool shouldReset = (Global.CurrentUser == m_employee.Login) &&
                                           (m_employee.Login.Role == CoreLib.Role.Administrator && m_role != CoreLib.Role.Administrator);

                        if (!m_employee.Login.UpdateDetails(String(values["Login"]), m_role, m_employee.Login.Password))
                        {
                            e.Canceled = true;
                            DisplayError("Failed to update login details of employee.", m_lblError);
                        }
                        else
                        {
                            if (shouldReset)
                            {
                                Response.Redirect("Login.aspx");
                            }
                        }
                    }
                }
            }

            if (!e.Canceled)
            {
                Repeater repeater = e.Item.FindControl("m_rpCompanyContactInfo") as Repeater;

                foreach (RepeaterItem rItem in repeater.Items)
                {
                    var txtValue = rItem.FindControl("m_txtData") as TextBox;
                    var btnDelete = rItem.FindControl("m_btnDelete") as LinkButton;
                    var contactDetail = ContactDetail(btnDelete.CommandArgument, m_employee);

                    if (!contactDetail.UpdateValue(txtValue.Text))
                    {
                        e.Canceled = true;
                        DisplayError("Failed to update the one or more contact information.", m_lblError);
                    }
                }
            }
        }

        protected void m_cboContactDetails_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            RadComboBox cboContactFields = sender as RadComboBox;
            cboContactFields.Text = e.Text;

            m_businessEntity = Entity(m_cboEntities.SelectedValue);
            m_employee = m_businessEntity.FindEmployee(Int(m_hidden.Value));

            if (m_employee.AddContactDetail(CoreLib.Profile.Contact.FindField(Int(e.Value)), string.Empty))
            {
                m_dgvGrid.Rebind();
            }
            else
            {
                DisplayError("Failed to add requested contact field.", m_lblError);
            }
        }

        protected void m_cboContactDetails_ItemsRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
        {
            RadComboBox cboContactFields = sender as RadComboBox;

            foreach (var profileField in CoreLib.Profile.Contact.Fields)
            {
                cboContactFields.Items.Add(new RadComboBoxItem(profileField.Name, profileField.ID.ToString()));
            }
        }

        protected void m_cboEntities_ItemsRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
        {
            foreach (var entity in BusinessEntity.All)
            {
                var cboItem = new RadComboBoxItem(entity.Name, entity.ID.ToString());
                cboItem.ImageUrl = entity.Type == BusinessEntityType.Company ? "Images/headquater.png" : "Images/branch.png";
                m_cboEntities.Items.Add(cboItem);
            }
        }

        protected void m_cboEntities_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            m_dgvGrid.Rebind();
        }

        protected void m_dgvGrid_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            m_businessEntity = Entity(m_cboEntities.SelectedValue);

            if (m_businessEntity != null)
            {
                m_dgvGrid.DataSource = m_businessEntity.Employees;
            }

            if (!IsSupported(TaskType.AddEmployee))
            {
                m_dgvGrid.MasterTableView.CommandItemDisplay = GridCommandItemDisplay.None;
            }
        }

        protected void m_cboRoles_ItemsRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
        {
            RadComboBox cboRoles = sender as RadComboBox;

            foreach (var entity in CoreLib.Role.All)
            {
                cboRoles.Items.Add(new RadComboBoxItem(entity.Name, entity.ID.ToString()));
            }
        }

        protected void m_cboDepartments_ItemsRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
        {
            RadComboBox cboDepartments = sender as RadComboBox;

            foreach (var entity in CoreLib.Department.All)
            {
                cboDepartments.Items.Add(new RadComboBoxItem(entity.Name, entity.ID.ToString()));
            }
        }

        protected void m_cboEmployeeTypes_ItemsRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
        {
            RadComboBox cboEmployeeTypes = sender as RadComboBox;

            foreach (var entity in CoreLib.EmployeeType.All)
            {
                cboEmployeeTypes.Items.Add(new RadComboBoxItem(entity.Name, entity.ID.ToString()));
            }
        }

        protected void m_cboDesignations_ItemsRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
        {
            RadComboBox cboDesignations = sender as RadComboBox;

            foreach (var entity in CoreLib.Designation.All)
            {
                cboDesignations.Items.Add(new RadComboBoxItem(entity.Name, entity.ID.ToString()));
            }
        }
    }
}