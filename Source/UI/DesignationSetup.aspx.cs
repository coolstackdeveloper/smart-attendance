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
    [CoreLib.AllowedTasks(CoreLib.TaskType.AddDesignation, CoreLib.TaskType.DeleteDesignation,
                            CoreLib.TaskType.EditDesignationDetails, CoreLib.TaskType.ViewDesignationDetails)]
    public partial class DesignationSetup : BasePage
    {
        protected void m_dgvGrid_CancelCommand(object source, GridCommandEventArgs e)
        {
            DisplayError("", m_lblError);
        }

        protected void m_dgvGrid_ItemCreated(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridEditableItem && e.Item.ItemIndex >= 0)
            {
                (e.Item as GridEditableItem)["EditColumn"].Controls[0].Visible = IsSupported(TaskType.EditDesignationDetails);
                (e.Item as GridEditableItem)["DeleteColumn"].Controls[0].Visible = IsSupported(TaskType.DeleteDesignation);
            }
        }

        protected void m_dgvGrid_InsertCommand(object source, GridCommandEventArgs e)
        {
            DisplayError("", m_lblError);
            Hashtable values = new Hashtable();
            GridEditableItem item = e.Item as GridEditableItem;
            item.ExtractValues(values);

            if (!CoreLib.Designation.Add(String(values["Name"])))
            {
                e.Canceled = true;
                DisplayError("Failed to add designation.", m_lblError);
            }
        }

        protected void m_dgvGrid_DeleteCommand(object source, GridCommandEventArgs e)
        {
            DisplayError("", m_lblError);
            GridEditableItem item = e.Item as GridEditableItem;

            if (!CoreLib.Designation.Remove(Designation(item.GetDataKeyValue("ID"))))
            {
                e.Canceled = true;
                DisplayError("Failed to delete designation.", m_lblError);
            }
        }

        protected void m_dgvGrid_UpdateCommand(object source, GridCommandEventArgs e)
        {
            DisplayError("", m_lblError);
            Hashtable values = new Hashtable();
            GridEditableItem item = e.Item as GridEditableItem;
            item.ExtractValues(values);
            m_designation = Designation(item.GetDataKeyValue("ID"));

            if (!m_designation.UpdateName(String(values["Name"])))
            {
                e.Canceled = true;
                DisplayError("Failed to update the designation name.", m_lblError);
            }
        }

        protected void m_dgvGrid_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            m_dgvGrid.DataSource = CoreLib.Designation.All;

            if (!IsSupported(TaskType.AddDesignation))
            {
                m_dgvGrid.MasterTableView.CommandItemDisplay = GridCommandItemDisplay.None;
            }
        }
    }
}