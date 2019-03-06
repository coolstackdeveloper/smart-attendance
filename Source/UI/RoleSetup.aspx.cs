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
    [CoreLib.AllowedTasks(CoreLib.TaskType.AddRole, CoreLib.TaskType.DeleteRole,
                            CoreLib.TaskType.EditRoleDetails, CoreLib.TaskType.ViewRoleDetails)]
    public partial class RoleSetup : BasePage
    {
        protected void m_btnDelete_Click(object sender, EventArgs e)
        {
            var btnDelete = sender as LinkButton;
            m_role = Role(m_hidden.Value);

            if (m_role.RemoveTask(Task(btnDelete.CommandArgument)))
            {
                m_dgvGrid.Rebind();
            }
            else
            {
                DisplayError("Failed to delete the requested task.", m_lblError);
            }
        }

        protected void m_cboTasks_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            RadComboBox m_cboTasks = sender as RadComboBox;
            m_role = Role(m_hidden.Value);

            if (m_role != null)
            {
                if (m_role.AddTask(Task(Long(e.Value))))
                {
                    m_dgvGrid.Rebind();
                }
                else
                {
                    DisplayError("Failed to add requested task.", m_lblError);
                }
            }
        }

        protected void m_cboTasks_ItemsRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
        {
            RadComboBox m_cboTasks = sender as RadComboBox;
            m_role = Role(m_hidden.Value);

            if (m_role != null)
            {
                foreach (var task in CoreLib.Task.All)
                {
                    if (m_role.FindTask(task.ID) == null)
                    {
                        m_cboTasks.Items.Add(new RadComboBoxItem(task.Name, task.ID.ToString()));
                    }
                }
            }
        }

        protected void m_chkTask_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox chkTask = sender as CheckBox;
            m_role = Role(m_hidden.Value);
            m_role.RemoveTask(Task(chkTask.Attributes["TaskID"]));
        }

        protected void m_dgvGrid_ItemDataBound(object source, GridItemEventArgs e)
        {
            var item = e.Item as GridEditableItem;

            // Already in edit mode but not a new insert
            if (item != null &&
                item.IsInEditMode && !item.OwnerTableView.IsItemInserted) // Edit or Insert
            {
                m_role = Role((e.Item as GridEditableItem).GetDataKeyValue("ID"));
                m_hidden.Value = m_role.ID.ToString();
                Repeater repeater = e.Item.FindControl("m_rpTaskList") as Repeater;
                repeater.DataSource = CoreLib.Task.All;
                repeater.DataBind();

                foreach (RepeaterItem rItem in repeater.Items)
                {
                    CheckBox chkTask = rItem.FindControl("m_chkTask") as CheckBox;
                    var btnDelete = rItem.FindControl("m_btnDelete") as LinkButton;
                    var task = Task(btnDelete.CommandArgument);
                    chkTask.Attributes.Add("TaskID", task.ID.ToString()); 
                    chkTask.AutoPostBack = chkTask.Checked = m_role.Tasks.Contains(task);
                }
            }
        }

        protected void m_dgvGrid_ItemCreated(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridEditableItem && e.Item.ItemIndex == 0)
            {
                (e.Item as GridEditableItem)["EditColumn"].Controls[0].Visible = false;
                (e.Item as GridEditableItem)["DeleteColumn"].Controls[0].Visible = false;
            }

            if (e.Item is GridEditableItem && e.Item.ItemIndex > 0)
            {
                (e.Item as GridEditableItem)["EditColumn"].Controls[0].Visible = IsSupported(TaskType.EditRoleDetails);
                (e.Item as GridEditableItem)["DeleteColumn"].Controls[0].Visible = IsSupported(TaskType.DeleteRole);
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

            if (!CoreLib.Role.Add(String(values["Name"])))
            {
                e.Canceled = true;
                DisplayError("Failed to add role.", m_lblError);
            }
        }

        protected void m_dgvGrid_DeleteCommand(object source, GridCommandEventArgs e)
        {
            DisplayError("", m_lblError);
            GridEditableItem item = e.Item as GridEditableItem;

            if (!CoreLib.Role.Remove(Role(item.GetDataKeyValue("ID"))))
            {
                e.Canceled = true;
                DisplayError("Failed to delete role.", m_lblError);
            }
        }

        protected void m_dgvGrid_UpdateCommand(object source, GridCommandEventArgs e)
        {
            DisplayError("", m_lblError);
            Hashtable values = new Hashtable();
            GridEditableItem item = e.Item as GridEditableItem;
            item.ExtractValues(values);
            m_role = Role(item.GetDataKeyValue("ID"));

            if (!m_role.UpdateName(String(values["Name"])))
            {
                e.Canceled = true;
                DisplayError("Failed to update the role name.", m_lblError);
            }
            else
            {
                Repeater repeater = e.Item.FindControl("m_rpTaskList") as Repeater;

                foreach (RepeaterItem rItem in repeater.Items)
                {
                    CheckBox chkTask = rItem.FindControl("m_chkTask") as CheckBox;

                    if (chkTask.Checked)
                    {
                        var btnDelete = rItem.FindControl("m_btnDelete") as LinkButton;
                        var task = Task(btnDelete.CommandArgument);
                        m_role.AddTask(task);
                    }
                }
            }
        }

        protected void m_dgvGrid_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            m_dgvGrid.DataSource = CoreLib.Role.All;

            if (!IsSupported(TaskType.AddRole))
            {
                m_dgvGrid.MasterTableView.CommandItemDisplay = GridCommandItemDisplay.None;
            }
        }
    }
}