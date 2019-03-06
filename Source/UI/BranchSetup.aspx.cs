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
    [CoreLib.AllowedTasks(CoreLib.TaskType.AddBranch, CoreLib.TaskType.DeleteBranch,
                          CoreLib.TaskType.EditBranchDetails, CoreLib.TaskType.ViewBranchDetails)]
    public partial class BranchSetup : BasePage
    {
        protected void m_btnDelete_Click(object sender, EventArgs e)
        {
            LinkButton btnDelete = sender as LinkButton;
            m_businessEntity = BusinessEntity.Find(Int(m_hidden.Value));
            var contactDetail = ContactDetail(btnDelete.CommandArgument, m_businessEntity);

            if (m_businessEntity.RemoveContactDetail(contactDetail))
            {
                m_dgvGrid.Rebind();
            }
            else
            {
                DisplayError("Failed to delete the requested contact field.", m_lblError);
            }
        }

        protected void m_cboEntities_ItemsRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
        {
            foreach (var entity in BusinessEntity.All.Where( t => t.Type == BusinessEntityType.Company))
            {
                var cboItem = new RadComboBoxItem(entity.Name, entity.ID.ToString());
                cboItem.ImageUrl = "Images/Headquater.png";
                m_cboEntities.Items.Add(cboItem);
            }
        }

        protected void m_cboEntities_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            m_dgvGrid.Rebind();
        }

        protected void m_dgvGrid_DeleteCommand(object source, GridCommandEventArgs e)
        {
            DisplayError("", m_lblError);
            GridEditableItem item = e.Item as GridEditableItem;
            m_businessEntity = BusinessEntity.Find(Int(item.GetDataKeyValue("ID")));
            
            if (!CoreLib.BusinessEntity.Remove(m_businessEntity))
            {
                e.Canceled = true;
                DisplayError("Failed to delete branch.", m_lblError);
            }
        }

        protected void m_dgvGrid_ItemCreated(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridEditableItem && e.Item.ItemIndex >= 0)
            {
                (e.Item as GridEditableItem)["EditColumn"].Controls[0].Visible = IsSupported(TaskType.EditBranchDetails);
                (e.Item as GridEditableItem)["DeleteColumn"].Controls[0].Visible = IsSupported(TaskType.DeleteBranch);
            }
        }

        protected void m_dgvGrid_ItemDataBound(object source, GridItemEventArgs e)
        {
            var item = e.Item as GridEditableItem;

            // Already in edit mode but not a new insert
            if (item != null &&
                item.IsInEditMode) // Edit or Insert
            {
                if (!item.OwnerTableView.IsItemInserted)
                {
                    Repeater repeater = e.Item.FindControl("m_rpCompanyContactInfo") as Repeater;
                    m_businessEntity = BusinessEntity.Find(Int(item.GetDataKeyValue("ID")));
                    m_hidden.Value = m_businessEntity.ID.ToString();
                    repeater.DataSource = m_businessEntity.ContactDetails;
                    repeater.DataBind();
                }
                else
                {
                    var cboContactDetails = e.Item.FindControl("m_cboContactDetails") as RadComboBox;
                    cboContactDetails.Enabled = false;
                }
            }
        }

        protected void m_dgvGrid_CancelCommand(object source, GridCommandEventArgs e)
        {
            DisplayError("", m_lblError);
        }

        protected void m_dgvGrid_UpdateCommand(object source, GridCommandEventArgs e)
        {
            DisplayError("", m_lblError);
            Hashtable values = new Hashtable();
            GridEditableItem item = e.Item as GridEditableItem;
            item.ExtractValues(values);
            m_businessEntity = BusinessEntity.Find(Int(item.GetDataKeyValue("ID")));

            if (!m_businessEntity.UpdateName(String(values["Name"])))
            {
                e.Canceled = true;
                DisplayError("Failed to update the branch name.", m_lblError);
            }

            Repeater repeater = e.Item.FindControl("m_rpCompanyContactInfo") as Repeater;

            foreach (RepeaterItem rItem in repeater.Items)
            {
                var txtValue = rItem.FindControl("m_txtData") as TextBox;
                var btnDelete = rItem.FindControl("m_btnDelete") as LinkButton;
                var contactDetail = ContactDetail(btnDelete.CommandArgument, m_businessEntity);

                if (!contactDetail.UpdateValue(txtValue.Text))
                {
                    e.Canceled = true;
                    DisplayError("Failed to update the one or more contact information.", m_lblError);
                }
            }
        }

        protected void m_dgvGrid_InsertCommand(object source, GridCommandEventArgs e)
        {
            DisplayError("", m_lblError);
            Hashtable values = new Hashtable();
            GridEditableItem item = e.Item as GridEditableItem;
            item.ExtractValues(values);
            m_businessEntity = Entity(m_cboEntities.SelectedValue);

            if (!CoreLib.BusinessEntity.Add(String(values["Name"]), m_businessEntity, BusinessEntityType.Branch))
            {
                e.Canceled = true;
                DisplayError("Failed to add branch.", m_lblError);
            }
        }

        protected void m_dgvGrid_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            m_businessEntity = Entity(m_cboEntities.SelectedValue);

            if (m_businessEntity != null)
            {
                m_dgvGrid.DataSource = BusinessEntity.All.Where(b => b.Parent == m_businessEntity);
            }

            if (!IsSupported(TaskType.AddBranch))
            {
                m_dgvGrid.MasterTableView.CommandItemDisplay = GridCommandItemDisplay.None;
            }
        }

        protected void m_cboContactDetails_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            RadComboBox cboContactFields = sender as RadComboBox;
            cboContactFields.Text = e.Text;
            m_businessEntity = BusinessEntity.Find(Int(m_hidden.Value));

            if (m_businessEntity.AddContactDetail(CoreLib.Profile.Contact.FindField(Int(e.Value)), string.Empty))
            {
                m_dgvGrid.Rebind();
            }
            else
            {
                DisplayError("Failed to add requested contact field.", m_lblError);
            }
        }

        protected void m_cboContactDetails_ItemsRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
        {
            RadComboBox cboContactFields = sender as RadComboBox;

            foreach (var profileField in CoreLib.Profile.Contact.Fields)
            {
                cboContactFields.Items.Add(new RadComboBoxItem(profileField.Name, profileField.ID.ToString()));
            }
        }
    }
}