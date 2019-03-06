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
    [CoreLib.AllowedTasks(CoreLib.TaskType.AddEmployeeShift, CoreLib.TaskType.DeleteEmployeeShift,
                            CoreLib.TaskType.EditEmployeeShiftDetails, CoreLib.TaskType.ViewEmployeeShiftDetails)]
    public partial class EmployeeShiftAssignment : BasePage
    {
        protected void m_dgvGrid_ItemCreated(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridEditableItem && e.Item.ItemIndex >= 0)
            {
                (e.Item as GridEditableItem)["DeleteColumn"].Controls[0].Visible = IsSupported(TaskType.DeleteEmployeeShift);
                (e.Item as GridEditableItem)["EditColumn"].Controls[0].Visible = IsSupported(TaskType.EditEmployeeShiftDetails);
            }
        }

        protected void m_dgvGrid_DeleteCommand(object source, GridCommandEventArgs e)
        {
            DisplayError("", m_lblError);
            GridEditableItem item = e.Item as GridEditableItem;
            m_businessEntity = Entity(m_cboEntities.SelectedValue);
            m_employee = Employee(m_cboEmployees.SelectedValue, m_businessEntity);
            var shiftAssignment = m_employee.ShiftAssignments.Find(a => a.ID == Int(item.GetDataKeyValue("ID")));

            if (!m_employee.RemoveShiftAssignment(shiftAssignment))
            {
                e.Canceled = true;
                DisplayError("Failed to delete employee shift.", m_lblError);
            }
        }

        protected void m_dgvGrid_ItemDataBound(object source, GridItemEventArgs e)
        {
            var item = e.Item as GridEditableItem;

            // Already in edit mode but not a new insert
            if (item != null &&
                item.IsInEditMode && !item.OwnerTableView.IsItemInserted) // Edit or Insert
            {
                m_employee = Employee(m_cboEmployees.SelectedValue, Entity(m_cboEntities.SelectedValue));
                var shiftAssignment = m_employee.ShiftAssignments.Find(a => a.ID == Int(item.GetDataKeyValue("ID")));
                var cboShifts = e.Item.FindControl("m_cboShifts") as RadComboBox;
                m_cboShifts_ItemsRequested(cboShifts, null);
                cboShifts.SelectedValue = shiftAssignment.Shift.ID.ToString();
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

            var shiftID = Int((e.Item.FindControl("m_cboShifts") as RadComboBox).SelectedValue);
            m_shift = Shift(shiftID, m_businessEntity);

            if (!m_employee.AssignShift(m_shift, DateTime(values["Start"]), DateTime(values["End"])))
            {
                e.Canceled = true;
                DisplayError("Failed to assign shift to employee.", m_lblError);
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

            var shiftID = Int((e.Item.FindControl("m_cboShifts") as RadComboBox).SelectedValue);
            var shiftAssignment = m_employee.ShiftAssignments.Find(a => a.ID == Int(item.GetDataKeyValue("ID")));
            m_shift = Shift(shiftID, m_businessEntity);

            if (!shiftAssignment.UpdateDetails(m_shift, DateTime(values["Start"]), DateTime(values["End"])))
            {
                e.Canceled = true;
                DisplayError("Failed to update shift to employee.", m_lblError);
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

        protected void m_dgvGrid_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            m_businessEntity = Entity(m_cboEntities.SelectedValue);
            m_employee = Employee(m_cboEmployees.SelectedValue, m_businessEntity);

            if (m_businessEntity != null && m_employee != null)
            {
                m_dgvGrid.DataSource = m_employee.ShiftAssignments;
            }

            if (!IsSupported(TaskType.AddEmployeeShift))
            {
                m_dgvGrid.MasterTableView.CommandItemDisplay = GridCommandItemDisplay.None;
            }
        }

        protected void m_cboShifts_ItemsRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
        {
            RadComboBox cboShifts = sender as RadComboBox;

            if ((m_businessEntity = Entity(m_cboEntities.SelectedValue)) != null)
            {
                foreach (var entity in m_businessEntity.Shifts)
                {
                    var timeDifference = (entity.EndTime - entity.StartTime).TotalHours;
                    string description = string.Format("{0} [{1}-{2}] [{3} Hours]", entity.Name, entity.StartTime.ToShortTimeString(), entity.EndTime.ToShortTimeString(), timeDifference);
                    cboShifts.Items.Add(new RadComboBoxItem(description, entity.ID.ToString()));
                }
            }
        }
    }
}