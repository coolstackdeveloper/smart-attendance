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
    [CoreLib.AllowedTasks(CoreLib.TaskType.AddShiftExceptionType, CoreLib.TaskType.DeleteShiftExceptionType,
                            CoreLib.TaskType.EditShiftExceptionTypeDetails, CoreLib.TaskType.ViewShiftExceptionTypeDetails)]
    public partial class ShiftExceptionTypeSetup : BasePage
    {
        protected void m_dgvGrid_ItemCreated(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridEditableItem && e.Item.ItemIndex >= 0)
            {
                (e.Item as GridEditableItem)["EditColumn"].Controls[0].Visible = IsSupported(TaskType.EditShiftExceptionTypeDetails);
                (e.Item as GridEditableItem)["DeleteColumn"].Controls[0].Visible = IsSupported(TaskType.DeleteShiftExceptionType);
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

            if (!CoreLib.ShiftExceptionType.Add(String(values["Name"])))
            {
                e.Canceled = true;
                DisplayError("Failed to add shift exception type.", m_lblError);
            }
        }

        protected void m_dgvGrid_DeleteCommand(object source, GridCommandEventArgs e)
        {
            DisplayError("", m_lblError);
            GridEditableItem item = e.Item as GridEditableItem;

            if (!CoreLib.ShiftExceptionType.Remove(ShiftExceptionType(item.GetDataKeyValue("ID"))))
            {
                e.Canceled = true;
                DisplayError("Failed to delete shift exception type.", m_lblError);
            }
        }

        protected void m_dgvGrid_UpdateCommand(object source, GridCommandEventArgs e)
        {
            DisplayError("", m_lblError);
            Hashtable values = new Hashtable();
            GridEditableItem item = e.Item as GridEditableItem;
            item.ExtractValues(values);
            m_shiftExceptionType = ShiftExceptionType(item.GetDataKeyValue("ID"));

            if (!m_shiftExceptionType.UpdateName(String(values["Name"])))
            {
                e.Canceled = true;
                DisplayError("Failed to update the shift exception type details.", m_lblError);
            }
        }

        protected void m_dgvGrid_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            m_dgvGrid.DataSource = CoreLib.ShiftExceptionType.All;

            if (!IsSupported(TaskType.AddShiftExceptionType))
            {
                m_dgvGrid.MasterTableView.CommandItemDisplay = GridCommandItemDisplay.None;
            }
        }
    }
}