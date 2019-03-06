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
    [CoreLib.AllowedTasks(CoreLib.TaskType.AddCompany, CoreLib.TaskType.DeleteCompany,
                          CoreLib.TaskType.EditCompanyDetails, CoreLib.TaskType.ViewCompanyDetails)]
    public partial class CompanySetup : BasePage
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

        protected void m_dgvGrid_DeleteCommand(object source, GridCommandEventArgs e)
        {
            DisplayError("", m_lblError);
            GridEditableItem item = e.Item as GridEditableItem;
            m_businessEntity = BusinessEntity.Find(Int(item.GetDataKeyValue("ID")));
            
            if (!CoreLib.BusinessEntity.Remove(m_businessEntity))
            {
                e.Canceled = true;
                DisplayError("Failed to delete company.", m_lblError);
            }
        }

        protected void m_dgvGrid_ItemCreated(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridEditableItem && e.Item.ItemIndex >= 0)
            {
                (e.Item as GridEditableItem)["EditColumn"].Controls[0].Visible = IsSupported(TaskType.EditCompanyDetails);
                (e.Item as GridEditableItem)["DeleteColumn"].Controls[0].Visible = IsSupported(TaskType.DeleteCompany);
            }
        }

        protected void m_dgvGrid_ItemDataBound(object source, GridItemEventArgs e)
        {
            var item = e.Item as GridEditableItem;

            // Already in edit mode but not a new insert
            if (item != null && item.IsInEditMode) // Edit or Insert
            {
                if (!item.OwnerTableView.IsItemInserted)
                {
                    Repeater repeater = e.Item.FindControl("m_rpCompanyContactInfo") as Repeater;
                    m_businessEntity = BusinessEntity.Find(Int(item.GetDataKeyValue("ID")));

                    var txtName = e.Item.FindControl("m_txtCompName") as TextBox;
                    txtName.Text = m_businessEntity.Name;

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
            var txtName = e.Item.FindControl("m_txtCompName") as TextBox;
            
            if (!m_businessEntity.UpdateName(txtName.Text))
            {
                e.Canceled = true;
                DisplayError("Failed to update the company name.", m_lblError);
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
            var txtName = e.Item.FindControl("m_txtCompName") as TextBox;

            if (!CoreLib.BusinessEntity.Add(txtName.Text, null, BusinessEntityType.Company))
            {
                e.Canceled = true;
                DisplayError("Failed to add company.", m_lblError);
            }
        }

        protected void m_dgvGrid_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            m_dgvGrid.DataSource = BusinessEntity.All.Where(s => s.Type == BusinessEntityType.Company);

            if (!IsSupported(TaskType.AddCompany))
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