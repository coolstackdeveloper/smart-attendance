using System;
using System.Linq;
using System.Text;
using System.Data.Linq;
using System.Collections.Generic;

using Utilities;
using DataAccess;
using DataAccess.DataSetTableAdapters;

namespace CoreLib
{
    /// <summary>
    /// Contact details
    /// </summary>
    public class BusinessEntityContactDetail
    {
        #region Static

        /// <summary>
        /// 
        /// </summary>
        internal static BusinessEntityContactDetailsTableAdapter m_sTA = new BusinessEntityContactDetailsTableAdapter();

        #endregion Static

        /// <summary>
        /// Table record
        /// </summary>
        internal DataSet.BusinessEntityContactDetailsRow m_dataRow = null;

        /// <summary>
        /// Intialization
        /// </summary>
        /// <param name="businessEntity"></param>
        internal BusinessEntityContactDetail(BusinessEntity businessEntity, DataSet.BusinessEntityContactDetailsRow dataRow)
        {
            m_dataRow = dataRow;
            BusinessEntity = businessEntity;
        }

        /// <summary>
        /// ID
        /// </summary>
        public int ID
        {
            get
            {
                return m_dataRow.ID;
            }
        }

        /// <summary>
        /// Field
        /// </summary>
        private ProfileField m_field = null;

        /// <summary>
        /// Field
        /// </summary>
        public string Name
        {
            get
            {
                lock (BusinessEntity.m_sDS)
                {
                    bool s = false;

                    if (m_field == null)
                    {
                        m_field = Profile.Contact.FindField(m_dataRow.ProfileFieldID);
                        s = Profile.Contact.Fields.Count == 0;
                    }

                    return (m_field == null) ? "INVALID" : m_field.Name;
                }
            }
        }

        /// <summary>
        /// Value
        /// </summary>
        public string Value
        {
            get
            {
                return m_dataRow.ProfileFieldValue;
            }
        }

        /// <summary>
        /// Associated entity
        /// </summary>
        public BusinessEntity BusinessEntity
        {
            get;
            private set;
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="newName"></param>
        /// <returns></returns>
        public bool UpdateValue(string newValue)
        {
            lock (BusinessEntity.m_sDS)
            {
                bool success = false;

                try
                {
                    m_dataRow.ProfileFieldValue = newValue;
                    success = 1 == m_sTA.Update(m_dataRow);
                }
                catch { }

                if (!success)
                {
                    m_dataRow.RejectChanges();
                }

                return success;
            }
        }
    }
}