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
    [CoreLib.AllowedTasks(CoreLib.TaskType.AddContactField, CoreLib.TaskType.DeleteContactField,
                            CoreLib.TaskType.EditContactFieldDetails, CoreLib.TaskType.ViewContactFieldDetails)]
    public partial class ProfileFieldSetup : BasePage
    {
        protected void m_dgvGrid_ItemCreated(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridEditableItem && e.Item.ItemIndex >= 0)
            {
                (e.Item as GridEditableItem)["EditColumn"].Controls[0].Visible = IsSupported(TaskType.EditContactFieldDetails);
                (e.Item as GridEditableItem)["DeleteColumn"].Controls[0].Visible = IsSupported(TaskType.DeleteContactField);
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

            if (!CoreLib.Profile.Contact.AddField(String(values["Name"])))
            {
                e.Canceled = true;
                DisplayError("Failed to add contact Field.", m_lblError);
            }
        }

        protected void m_dgvGrid_DeleteCommand(object source, GridCommandEventArgs e)
        {
            DisplayError("", m_lblError);
            GridEditableItem item = e.Item as GridEditableItem;

            if (!CoreLib.Profile.Contact.RemoveField(ProfileField(item.GetDataKeyValue("ID"), CoreLib.Profile.Contact)))
            {
                e.Canceled = true;
                DisplayError("Failed to delete contact Field.", m_lblError);
            }
        }

        protected void m_dgvGrid_UpdateCommand(object source, GridCommandEventArgs e)
        {
            DisplayError("", m_lblError);
            Hashtable values = new Hashtable();
            GridEditableItem item = e.Item as GridEditableItem;
            item.ExtractValues(values);
            m_profileField = ProfileField(item.GetDataKeyValue("ID"), CoreLib.Profile.Contact);

            if (!m_profileField.UpdateName(String(values["Name"])))
            {
                e.Canceled = true;
                DisplayError("Failed to update the contact field name.", m_lblError);
            }
        }

        protected void m_dgvGrid_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            m_dgvGrid.DataSource = CoreLib.Profile.Contact.Fields;

            if (!IsSupported(TaskType.AddContactField))
            {
                m_dgvGrid.MasterTableView.CommandItemDisplay = GridCommandItemDisplay.None;
            }
        }
    }
}