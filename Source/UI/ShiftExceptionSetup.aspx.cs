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
    [CoreLib.AllowedTasks(CoreLib.TaskType.AddShiftException, CoreLib.TaskType.DeleteShiftException,
                            CoreLib.TaskType.EditShiftExceptionDetails, CoreLib.TaskType.ViewShiftExceptionDetails)]
    public partial class ShiftExceptionSetup : BasePage
    {
        protected void m_dgvGrid_ItemCreated(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridEditableItem && e.Item.ItemIndex >= 0)
            {
                (e.Item as GridEditableItem)["EditColumn"].Controls[0].Visible = IsSupported(TaskType.EditShiftExceptionDetails);
                (e.Item as GridEditableItem)["DeleteColumn"].Controls[0].Visible = IsSupported(TaskType.DeleteShiftException);
            }
        }

        protected void m_dgvGrid_ItemDataBound(object source, GridItemEventArgs e)
        {
            var item = e.Item as GridEditableItem;

            // Already in edit mode but not a new insert
            if (item != null &&
                item.IsInEditMode && !item.OwnerTableView.IsItemInserted) // Edit or Insert
            {
                m_shift = Shift(m_cboShifts.SelectedValue, Entity(m_cboEntities.SelectedValue));
                m_shiftException = ShiftException((e.Item as GridEditableItem).GetDataKeyValue("ID"), m_shift);

                var cboExceptionTypes = e.Item.FindControl("m_cboExceptionTypes") as RadComboBox;
                m_cboExceptionTypes_ItemsRequested(cboExceptionTypes, null);
                cboExceptionTypes.SelectedValue = m_shiftException.Type.ID.ToString();

                var cboWeekDays = e.Item.FindControl("m_cboWeekDays") as RadComboBox;
                m_cboWeekDays_ItemsRequested(cboWeekDays, null);
                cboWeekDays.SelectedValue = m_shiftException.WeekDay.HasValue ? m_shiftException.WeekDay.Value.ToString() : "";
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
            m_shift = Shift(m_cboShifts.SelectedValue, m_businessEntity);

            m_shiftStart = DateTime(values["StartTime"]);
            m_shiftEnd = DateTime(values["EndTime"]);
            m_shiftEnd = new DateTime(m_shiftStart.Year, m_shiftStart.Month, m_shiftStart.Day, m_shiftEnd.Hour, m_shiftEnd.Minute, m_shiftEnd.Second, m_shiftEnd.Millisecond);

            if (m_shiftEnd < m_shiftStart)
            {
                m_shiftEnd = m_shiftEnd.AddDays(1);
            }

            var exceptionTypeID = Int((e.Item.FindControl("m_cboExceptionTypes") as RadComboBox).SelectedValue);
            m_shiftExceptionType = ShiftExceptionType(exceptionTypeID);

            var weekDayName = String((e.Item.FindControl("m_cboWeekDays") as RadComboBox).SelectedValue);
            DayOfWeek? weekDay = string.IsNullOrEmpty(weekDayName) ? (DayOfWeek?)null : (DayOfWeek)Enum.Parse(typeof(DayOfWeek), weekDayName);

            if (!m_shift.AddException(m_shiftStart, m_shiftEnd, weekDay, m_shiftExceptionType))
            {
                e.Canceled = true;
                DisplayError("Failed to add shift exception.", m_lblError);
            }
        }

        protected void m_dgvGrid_DeleteCommand(object source, GridCommandEventArgs e)
        {
            DisplayError("", m_lblError);
            GridEditableItem item = e.Item as GridEditableItem;
            m_businessEntity = Entity(m_cboEntities.SelectedValue);
            m_shift = Shift(m_cboShifts.SelectedValue, m_businessEntity);

            if (!m_shift.RemoveException(ShiftException(item.GetDataKeyValue("ID"), m_shift)))
            {
                e.Canceled = true;
                DisplayError("Failed to delete shift exception.", m_lblError);
            }
        }

        protected void m_dgvGrid_UpdateCommand(object source, GridCommandEventArgs e)
        {
            DisplayError("", m_lblError);
            Hashtable values = new Hashtable();
            GridEditableItem item = e.Item as GridEditableItem;
            item.ExtractValues(values);
            m_businessEntity = Entity(m_cboEntities.SelectedValue);
            m_shift = Shift(m_cboShifts.SelectedValue, m_businessEntity);
            m_shiftException = ShiftException(item.GetDataKeyValue("ID"), m_shift);

            m_shiftStart = DateTime(values["StartTime"]);
            m_shiftEnd = DateTime(values["EndTime"]);
            m_shiftEnd = new DateTime(m_shiftStart.Year, m_shiftStart.Month, m_shiftStart.Day, m_shiftEnd.Hour, m_shiftEnd.Minute, m_shiftEnd.Second, m_shiftEnd.Millisecond);

            if (m_shiftEnd < m_shiftStart)
            {
                m_shiftEnd = m_shiftEnd.AddDays(1);
            }

            var exceptionTypeID = Int((e.Item.FindControl("m_cboExceptionTypes") as RadComboBox).SelectedValue);
            m_shiftExceptionType = ShiftExceptionType(exceptionTypeID);

            var weekDayName = String((e.Item.FindControl("m_cboWeekDays") as RadComboBox).SelectedValue);
            DayOfWeek? weekDay = string.IsNullOrEmpty(weekDayName) ? (DayOfWeek?)null : (DayOfWeek)Enum.Parse(typeof(DayOfWeek), weekDayName);

            if (!m_shiftException.UpdateDetails(m_shiftStart, m_shiftEnd, weekDay, m_shiftExceptionType))
            {
                e.Canceled = true;
                DisplayError("Failed to update shift exception details.", m_lblError);
            }
        }

        protected void m_dgvGrid_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            m_businessEntity = Entity(m_cboEntities.SelectedValue);
            m_shift = Shift(m_cboShifts.SelectedValue, m_businessEntity);

            if (m_shift != null)
            {
                m_dgvGrid.DataSource = m_shift.Exceptions;
            }

            if (!IsSupported(TaskType.AddShiftException))
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
            m_cboShifts.Items.Clear();
            m_dgvGrid.Rebind();
        }

        protected void m_cboWeekDays_ItemsRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
        {
            RadComboBox cboWeekDays = sender as RadComboBox;

            foreach (DayOfWeek entity in Enum.GetValues(typeof(DayOfWeek)))
            {
                cboWeekDays.Items.Add(new RadComboBoxItem(entity.ToString(), entity.ToString()));
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

        protected void m_cboShifts_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            m_dgvGrid.Rebind();
        }

        protected void m_cboExceptionTypes_ItemsRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
        {
            RadComboBox cboExceptionTypes = sender as RadComboBox;

            foreach (var entity in CoreLib.ShiftExceptionType.All)
            {
                cboExceptionTypes.Items.Add(new RadComboBoxItem(entity.Name, entity.ID.ToString()));
            }
        }
    }
}