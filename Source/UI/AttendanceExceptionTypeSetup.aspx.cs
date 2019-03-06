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
    [CoreLib.AllowedTasks(CoreLib.TaskType.AddAttendanceExceptionType, CoreLib.TaskType.DeleteAttendanceExceptionType,
                            CoreLib.TaskType.EditAttendanceExceptionTypeDetails, CoreLib.TaskType.ViewAttendanceExceptionTypeDetails)]
    public partial class AttendanceExceptionTypeSetup : BasePage
    {
        protected void m_dgvGrid_ItemCreated(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridEditableItem && e.Item.ItemIndex >= 0)
            {
                (e.Item as GridEditableItem)["EditColumn"].Controls[0].Visible = IsSupported(TaskType.EditAttendanceExceptionTypeDetails);
                (e.Item as GridEditableItem)["DeleteColumn"].Controls[0].Visible = IsSupported(TaskType.DeleteAttendanceExceptionType);
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

            if (!CoreLib.AttendanceExceptionType.Add(String(values["Name"])))
            {
                e.Canceled = true;
                DisplayError("Failed to add entry type.", m_lblError);
            }
        }

        protected void m_dgvGrid_DeleteCommand(object source, GridCommandEventArgs e)
        {
            DisplayError("", m_lblError);
            GridEditableItem item = e.Item as GridEditableItem;

            if (!CoreLib.AttendanceExceptionType.Remove(AttendanceExceptionType(item.GetDataKeyValue("ID"))))
            {
                e.Canceled = true;
                DisplayError("Failed to delete entry type.", m_lblError);
            }
        }

        protected void m_dgvGrid_UpdateCommand(object source, GridCommandEventArgs e)
        {
            DisplayError("", m_lblError);
            Hashtable values = new Hashtable();
            GridEditableItem item = e.Item as GridEditableItem;
            item.ExtractValues(values);
            m_attendanceExceptionType = AttendanceExceptionType(item.GetDataKeyValue("ID"));

            if (!m_attendanceExceptionType.UpdateName(String(values["Name"])))
            {
                e.Canceled = true;
                DisplayError("Failed to update the entry type details.", m_lblError);
            }
        }

        protected void m_dgvGrid_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            m_dgvGrid.DataSource = CoreLib.AttendanceExceptionType.All;

            if (!IsSupported(TaskType.AddAttendanceExceptionType))
            {
                m_dgvGrid.MasterTableView.CommandItemDisplay = GridCommandItemDisplay.None;
            }
        }
    }
}