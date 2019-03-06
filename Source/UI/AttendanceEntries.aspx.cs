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
    [CoreLib.AllowedTasks(CoreLib.TaskType.AddAttendanceException, CoreLib.TaskType.DeleteAttendanceException,
                            CoreLib.TaskType.EditAttendanceExceptionDetails, CoreLib.TaskType.ViewAttendanceExceptionDetails)]
    public partial class AttendanceEntries : BasePage
    {
        protected void m_dgvGrid_ItemCreated(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridEditableItem && e.Item.ItemIndex >= 0)
            {
                (e.Item as GridEditableItem)["EditColumn"].Controls[0].Visible = IsSupported(TaskType.EditAttendanceExceptionDetails);
                (e.Item as GridEditableItem)["DeleteColumn"].Controls[0].Visible = IsSupported(TaskType.DeleteAttendanceException);
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
                    m_employee = Employee(m_cboEmployees.SelectedValue, Entity(m_cboEntities.SelectedValue));
                    m_attendanceException = AttendanceException((e.Item as GridEditableItem).GetDataKeyValue("ID"), m_employee);

                    var cboExceptionTypes = e.Item.FindControl("m_cboExceptionTypes") as RadComboBox;
                    m_cboExceptionTypes_ItemsRequested(cboExceptionTypes, null);
                    cboExceptionTypes.SelectedValue = m_attendanceException.Type.ID.ToString();

                    var chkApproved = e.Item.FindControl("m_chkApproved") as CheckBox;
                    chkApproved.Checked = m_attendanceException.Approved;
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
            m_employee = Employee(m_cboEmployees.SelectedValue, m_businessEntity);

            m_start = DateTime(values["Start"]);
            m_end = DateTime(values["End"]);

            var exceptionTypeID = Int((e.Item.FindControl("m_cboExceptionTypes") as RadComboBox).SelectedValue);
            m_attendanceExceptionType = AttendanceExceptionType(exceptionTypeID);

            if (!m_employee.AddAttendanceException(m_start, m_end, m_attendanceExceptionType, String(values["Remarks"])))
            {
                e.Canceled = true;
                DisplayError("Failed to add entry.", m_lblError);
            }
        }

        protected void m_dgvGrid_DeleteCommand(object source, GridCommandEventArgs e)
        {
            DisplayError("", m_lblError);
            GridEditableItem item = e.Item as GridEditableItem;
            m_businessEntity = Entity(m_cboEntities.SelectedValue);
            m_employee = Employee(m_cboEmployees.SelectedValue, m_businessEntity);

            if (!m_employee.RemoveAttendanceException(AttendanceException(item.GetDataKeyValue("ID"), m_employee)))
            {
                e.Canceled = true;
                DisplayError("Failed to delete entry.", m_lblError);
            }
        }

        protected void m_dgvGrid_UpdateCommand(object source, GridCommandEventArgs e)
        {
            DisplayError("", m_lblError);
            Hashtable values = new Hashtable();
            GridEditableItem item = e.Item as GridEditableItem;
            item.ExtractValues(values);
            m_businessEntity = Entity(m_cboEntities.SelectedValue);
            m_employee = Employee(m_cboEmployees.SelectedValue, m_businessEntity);
            m_attendanceException = AttendanceException(item.GetDataKeyValue("ID"), m_employee);

            m_start = DateTime(values["Start"]);
            m_end = DateTime(values["End"]);

            var exceptionTypeID = Int((e.Item.FindControl("m_cboExceptionTypes") as RadComboBox).SelectedValue);
            m_attendanceExceptionType = AttendanceExceptionType(exceptionTypeID);

            var approved = Bool((e.Item.FindControl("m_chkApproved") as CheckBox).Checked);

            if (!m_attendanceException.UpdateDetails(String(values["Remarks"]), m_start, m_end, approved))
            {
                e.Canceled = true;
                DisplayError("Failed to update entry details.", m_lblError);
            }
        }

        protected void m_dgvGrid_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            // If other user has logged in besides an administrator
            if (Global.CurrentUser != null && Global.CurrentUser.Role != CoreLib.Role.Administrator)
            {
                m_divTop.Visible = false;
                m_employee = Global.CurrentUser.Employee;
            }
            else
            {
                m_businessEntity = Entity(m_cboEntities.SelectedValue);
                m_employee = Employee(m_cboEmployees.SelectedValue, m_businessEntity);
            }

            if (m_employee != null)
            {
                m_dgvGrid.DataSource = m_employee.AttendanceExceptions;
            }

            if (!IsSupported(TaskType.EditAttendanceExceptionDetails))
            {
                var column = m_dgvGrid.MasterTableView.Columns.FindByUniqueName("ApprovedColumn") as GridEditableColumn;
                column.ReadOnly = true;
            }

            if (!IsSupported(TaskType.AddAttendanceException))
            {
                m_dgvGrid.MasterTableView.CommandItemDisplay = GridCommandItemDisplay.None;
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
            m_cboEmployees.Items.Clear();
            m_dgvGrid.Rebind();
        }

        protected void m_cboEmployees_ItemsRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
        {
            RadComboBox cboEmployees = sender as RadComboBox;

            if ((m_businessEntity = Entity(m_cboEntities.SelectedValue)) != null)
            {
                foreach (var entity in m_businessEntity.Employees)
                {
                    cboEmployees.Items.Add(new RadComboBoxItem(entity.Name, entity.ID.ToString()));
                }
            }
        }

        protected void m_cboEmployees_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            m_dgvGrid.Rebind();
        }

        protected void m_cboExceptionTypes_ItemsRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
        {
            RadComboBox cboExceptionTypes = sender as RadComboBox;

            foreach (var entity in CoreLib.AttendanceExceptionType.All)
            {
                cboExceptionTypes.Items.Add(new RadComboBoxItem(entity.Name, entity.ID.ToString()));
            }
        }
    }
}