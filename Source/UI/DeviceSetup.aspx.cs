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
    [CoreLib.AllowedTasks(CoreLib.TaskType.AddDevice, CoreLib.TaskType.DeleteDevice,
                            CoreLib.TaskType.EditDeviceDetails, CoreLib.TaskType.ViewDeviceDetails)]
    public partial class DeviceSetup : BasePage
    {
        protected void m_dgvGrid_ItemCreated(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridEditableItem && e.Item.ItemIndex >= 0)
            {
                (e.Item as GridEditableItem)["EditColumn"].Controls[0].Visible = IsSupported(TaskType.EditDeviceDetails);
                (e.Item as GridEditableItem)["DeleteColumn"].Controls[0].Visible = IsSupported(TaskType.DeleteDevice);
            }
        }

        protected void m_dgvGrid_ItemDataBound(object source, GridItemEventArgs e)
        {
            var item = e.Item as GridEditableItem;

            // Already in edit mode but not a new insert
            if (item != null &&
                item.IsInEditMode && !item.OwnerTableView.IsItemInserted) // Edit or Insert
            {
                m_device = Device((e.Item as GridEditableItem).GetDataKeyValue("ID"), Entity(m_cboEntities.SelectedValue));
                RadioButtonList rList = e.Item.FindControl("m_rblTypes") as RadioButtonList;
                rList.SelectedIndex = -1;

                foreach (ListItem rItem in rList.Items)
                {
                    rItem.Selected = false;

                    if (rItem.Value.Equals(m_device.Type.ToString(), StringComparison.OrdinalIgnoreCase))
                    {
                        rItem.Selected = true;
                        rList.DataBind();
                        break;
                    }
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

            RadioButtonList rList = e.Item.FindControl("m_rblTypes") as RadioButtonList;
            DeviceType type = (DeviceType)Enum.Parse(typeof(DeviceType), rList.SelectedValue);

            if (!m_businessEntity.AddDevice(String(values["Name"]), String(values["Address"]), String(values["SubnetMask"]), String(values["GatewayIP"]), String(values["MACAddress"]), type))
            {
                e.Canceled = true;
                DisplayError("Failed to add device.", m_lblError);
            }
        }

        protected void m_dgvGrid_DeleteCommand(object source, GridCommandEventArgs e)
        {
            DisplayError("", m_lblError);
            GridEditableItem item = e.Item as GridEditableItem;
            m_businessEntity = Entity(m_cboEntities.SelectedValue);

            if (!m_businessEntity.RemoveDevice(Device(item.GetDataKeyValue("ID"), m_businessEntity)))
            {
                e.Canceled = true;
                DisplayError("Failed to delete device.", m_lblError);
            }
        }

        protected void m_dgvGrid_UpdateCommand(object source, GridCommandEventArgs e)
        {
            DisplayError("", m_lblError);
            Hashtable values = new Hashtable();
            GridEditableItem item = e.Item as GridEditableItem;
            item.ExtractValues(values);
            m_businessEntity = Entity(m_cboEntities.SelectedValue);
            m_device = Device(item.GetDataKeyValue("ID"), m_businessEntity);

            RadioButtonList rList = e.Item.FindControl("m_rblTypes") as RadioButtonList;
            DeviceType type = (DeviceType)Enum.Parse(typeof(DeviceType), rList.SelectedValue);

            if (!m_device.UpdateDetails(String(values["Name"]), String(values["Address"]), String(values["SubnetMask"]), String(values["GatewayIP"]), String(values["MACAddress"]), type))
            {
                e.Canceled = true;
                DisplayError("Failed to update the device details.", m_lblError);
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
                m_dgvGrid.DataSource = m_businessEntity.Devices;
            }

            if (!IsSupported(TaskType.AddDevice))
            {
                m_dgvGrid.MasterTableView.CommandItemDisplay = GridCommandItemDisplay.None;
            }
        }
    }
}