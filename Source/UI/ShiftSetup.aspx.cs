using System;
using System.Web;
using System.Linq;
using System.Web.UI;
using System.Collections;
using System.Globalization;
using System.Web.UI.WebControls;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using CoreLib;
using Telerik.Web.UI;

namespace UI
{
    [CoreLib.AllowedTasks(CoreLib.TaskType.AddShift, CoreLib.TaskType.DeleteShift,
                            CoreLib.TaskType.EditShiftDetails, CoreLib.TaskType.ViewShiftDetails)]
    public partial class ShiftSetup : BasePage
    {
        protected void m_dgvGrid_ItemCreated(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridEditableItem && e.Item.ItemIndex >= 0)
            {
                (e.Item as GridEditableItem)["EditColumn"].Controls[0].Visible = IsSupported(TaskType.EditShiftDetails);
                (e.Item as GridEditableItem)["DeleteColumn"].Controls[0].Visible = IsSupported(TaskType.DeleteShift);
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

            m_shiftPunchInStart = DateTime(values["PunchInStartTime"]);
            m_shiftStart = DateTime(values["StartTime"]);
            m_shiftPunchInEnd = DateTime(values["PunchInEndTime"]);
            m_shiftPunchOutStart = DateTime(values["PunchOutStartTime"]);
            m_shiftEnd = DateTime(values["EndTime"]);
            m_shiftPunchOutEnd = DateTime(values["PunchOutEndTime"]);

            m_shiftStart = new DateTime(m_shiftPunchInStart.Year, m_shiftPunchInStart.Month, m_shiftPunchInStart.Day, m_shiftStart.Hour, m_shiftStart.Minute, m_shiftStart.Second, m_shiftStart.Millisecond);

            if (m_shiftStart < m_shiftPunchInStart)
            {
                m_shiftStart = m_shiftStart.AddDays(1);
            }

            m_shiftPunchInEnd = new DateTime(m_shiftStart.Year, m_shiftStart.Month, m_shiftStart.Day, m_shiftPunchInEnd.Hour, m_shiftPunchInEnd.Minute, m_shiftPunchInEnd.Second, m_shiftPunchInEnd.Millisecond);

            if (m_shiftPunchInEnd < m_shiftStart)
            {
                m_shiftPunchInEnd = m_shiftPunchInEnd.AddDays(1);
            }

            m_shiftPunchOutStart = new DateTime(m_shiftPunchInEnd.Year, m_shiftPunchInEnd.Month, m_shiftPunchInEnd.Day, m_shiftPunchOutStart.Hour, m_shiftPunchOutStart.Minute, m_shiftPunchOutStart.Second, m_shiftPunchOutStart.Millisecond); 
            
            if (m_shiftPunchOutStart < m_shiftPunchInEnd)
            {
                m_shiftPunchOutStart = m_shiftPunchOutStart.AddDays(1);
            }

            m_shiftEnd = new DateTime(m_shiftPunchOutStart.Year, m_shiftPunchOutStart.Month, m_shiftPunchOutStart.Day, m_shiftEnd.Hour, m_shiftEnd.Minute, m_shiftEnd.Second, m_shiftEnd.Millisecond); 

            if (m_shiftEnd < m_shiftPunchOutStart)
            {
                m_shiftEnd = m_shiftEnd.AddDays(1);
            }

            m_shiftPunchOutEnd = new DateTime(m_shiftEnd.Year, m_shiftEnd.Month, m_shiftEnd.Day, m_shiftPunchOutEnd.Hour, m_shiftPunchOutEnd.Minute, m_shiftPunchOutEnd.Second, m_shiftPunchOutEnd.Millisecond);
            
            if (m_shiftPunchOutEnd < m_shiftEnd)
            {
                m_shiftPunchOutEnd = m_shiftPunchOutEnd.AddDays(1);
            }

            if (!m_businessEntity.AddShift(String(values["Name"]),
                                         m_shiftStart, m_shiftEnd,
                                         m_shiftPunchInStart, m_shiftPunchInEnd,
                                         m_shiftPunchOutStart, m_shiftPunchOutEnd))
            {
                e.Canceled = true;
                DisplayError("Failed to add shift.", m_lblError);
            }
        }

        protected void m_dgvGrid_DeleteCommand(object source, GridCommandEventArgs e)
        {
            DisplayError("", m_lblError);
            GridEditableItem item = e.Item as GridEditableItem;
            m_businessEntity = Entity(m_cboEntities.SelectedValue);

            if (!m_businessEntity.RemoveShift(Shift(item.GetDataKeyValue("ID"), m_businessEntity)))
            {
                e.Canceled = true;
                DisplayError("Failed to delete shift.", m_lblError);
            }
        }

        protected void m_dgvGrid_UpdateCommand(object source, GridCommandEventArgs e)
        {
            DisplayError("", m_lblError);
            Hashtable values = new Hashtable();
            GridEditableItem item = e.Item as GridEditableItem;
            item.ExtractValues(values);
            m_businessEntity = Entity(m_cboEntities.SelectedValue);
            m_shift = Shift(item.GetDataKeyValue("ID"), m_businessEntity);

            m_shiftStart = DateTime(values["StartTime"]);
            m_shiftEnd = DateTime(values["EndTime"]);
            m_shiftPunchInStart = DateTime(values["PunchInStartTime"]);
            m_shiftPunchInEnd = DateTime(values["PunchInEndTime"]);
            m_shiftPunchOutStart = DateTime(values["PunchOutStartTime"]);
            m_shiftPunchOutEnd = DateTime(values["PunchOutEndTime"]);

            m_shiftStart = new DateTime(m_shiftPunchInStart.Year, m_shiftPunchInStart.Month, m_shiftPunchInStart.Day, m_shiftStart.Hour, m_shiftStart.Minute, m_shiftStart.Second, m_shiftStart.Millisecond);

            if (m_shiftStart < m_shiftPunchInStart)
            {
                m_shiftStart = m_shiftStart.AddDays(1);
            }

            m_shiftPunchInEnd = new DateTime(m_shiftStart.Year, m_shiftStart.Month, m_shiftStart.Day, m_shiftPunchInEnd.Hour, m_shiftPunchInEnd.Minute, m_shiftPunchInEnd.Second, m_shiftPunchInEnd.Millisecond);

            if (m_shiftPunchInEnd < m_shiftStart)
            {
                m_shiftPunchInEnd = m_shiftPunchInEnd.AddDays(1);
            }

            m_shiftPunchOutStart = new DateTime(m_shiftPunchInEnd.Year, m_shiftPunchInEnd.Month, m_shiftPunchInEnd.Day, m_shiftPunchOutStart.Hour, m_shiftPunchOutStart.Minute, m_shiftPunchOutStart.Second, m_shiftPunchOutStart.Millisecond);

            if (m_shiftPunchOutStart < m_shiftPunchInEnd)
            {
                m_shiftPunchOutStart = m_shiftPunchOutStart.AddDays(1);
            }

            m_shiftEnd = new DateTime(m_shiftPunchOutStart.Year, m_shiftPunchOutStart.Month, m_shiftPunchOutStart.Day, m_shiftEnd.Hour, m_shiftEnd.Minute, m_shiftEnd.Second, m_shiftEnd.Millisecond);

            if (m_shiftEnd < m_shiftPunchOutStart)
            {
                m_shiftEnd = m_shiftEnd.AddDays(1);
            }

            m_shiftPunchOutEnd = new DateTime(m_shiftEnd.Year, m_shiftEnd.Month, m_shiftEnd.Day, m_shiftPunchOutEnd.Hour, m_shiftPunchOutEnd.Minute, m_shiftPunchOutEnd.Second, m_shiftPunchOutEnd.Millisecond);

            if (m_shiftPunchOutEnd < m_shiftEnd)
            {
                m_shiftPunchOutEnd = m_shiftPunchOutEnd.AddDays(1);
            }

            if (!m_shift.UpdateDetails(String(values["Name"]),
                                         m_shiftStart, m_shiftEnd,
                                         m_shiftPunchInStart, m_shiftPunchInEnd,
                                         m_shiftPunchOutStart, m_shiftPunchOutEnd))
            {
                e.Canceled = true;
                DisplayError("Failed to update the shift details.", m_lblError);
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
                m_dgvGrid.DataSource = m_businessEntity.Shifts;
            }

            if (!IsSupported(TaskType.AddShift))
            {
                m_dgvGrid.MasterTableView.CommandItemDisplay = GridCommandItemDisplay.None;
            }
        }
    }
}