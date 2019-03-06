using System;
using System.Linq;
using System.Text;
using System.Data.Linq;
using System.Configuration;
using System.Collections.Generic;

using Utilities;
using DataAccess;
using DataAccess.DataSetTableAdapters;

namespace CoreLib
{
    /// <summary>
    /// Profile types like contacts etc
    /// </summary>
    public class Profile
    {
        #region Static

        /// <summary>
        /// Contact tag in the database
        /// </summary>
        public static readonly string CONTACT_TAG = "Contact";

        /// <summary>
        /// Types
        /// </summary>
        private static List<Profile> m_sProfiles = new List<Profile>();

        /// <summary>
        /// Adapter
        /// </summary>
        internal static ProfilesTableAdapter m_sTA = new ProfilesTableAdapter();

        /// <summary>
        /// Gets the contact profile
        /// </summary>
        public static List<Profile> All
        {
            get
            {
                lock (BusinessEntity.m_sDS)
                {
                    if (m_sProfiles.Count == 0)
                    {
                        try
                        {
                            var dataTable = BusinessEntity.m_sDS.Profiles;

                            foreach (var dataRow in m_sTA.GetData())
                            {
                                dataTable.ImportRow(dataRow);
                                m_sProfiles.Add(new Profile(dataTable[dataTable.Count - 1]));
                            }

                            if (m_sProfiles.Count == 0)
                            {
                                Add(CONTACT_TAG);
                            }
                        }
                        catch { }
                    }

                    return m_sProfiles;
                }
            }
        }

        /// <summary>
        /// Contact profile
        /// </summary>
        private static Profile m_sContact = null;

        /// <summary>
        /// Gets the contact profile
        /// </summary>
        public static Profile Contact
        {
            get
            {
                lock (BusinessEntity.m_sDS)
                {
                    if (m_sContact == null)
                    {
                        m_sContact = Find(CONTACT_TAG);
                    }

                    return m_sContact;
                }
            }
        }

        /// <summary>
        /// Add
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool Add(string name)
        {
            lock (BusinessEntity.m_sDS)
            {
                bool success = false;

                if (!string.IsNullOrEmpty(name))
                {
                    var dataRow = BusinessEntity.m_sDS.Profiles.AddProfilesRow(name);

                    try
                    {
                        success = m_sTA.Update(dataRow) == 1;
                    }
                    catch { }

                    if (!success)
                    {
                        BusinessEntity.m_sDS.Profiles.RemoveProfilesRow(dataRow);
                    }
                    else
                    {
                        m_sProfiles.Add(new Profile(dataRow));
                    }
                }

                return success;
            }
        }

        /// <summary>
        /// Remove
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool Remove(Profile profile)
        {
            lock (BusinessEntity.m_sDS)
            {
                bool success = false;

                if (profile != null && profile != Contact)
                {
                    try
                    {
                        success = (m_sTA.DeleteByID(profile.ID) == 1);
                    }
                    catch { }

                    if (success)
                    {
                        m_sProfiles.Remove(profile);
                        BusinessEntity.m_sDS.Profiles.RemoveProfilesRow(profile.m_dataRow);
                    }
                }

                return success;
            }
        }

        /// <summary>
        /// Find
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static Profile Find(int id)
        {
            return All.Find(t => t.ID == id);
        }

        /// <summary>
        /// Find
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Profile Find(string name)
        {
            return All.Find(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        #endregion Static

        /// <summary>
        /// Table record
        /// </summary>
        private DataSet.ProfilesRow m_dataRow = null;

        /// <summary>
        /// Initialization
        /// </summary>
        /// <param name="dataRow"></param>
        private Profile(DataSet.ProfilesRow dataRow)
        {
            m_dataRow = dataRow;
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
        /// Profile type name
        /// </summary>
        public string Name
        {
            get
            {
                return m_dataRow.Name;
            }
        }

        /// <summary>
        /// Fields
        /// </summary>
        private List<ProfileField> m_fields = new List<ProfileField>();

        /// <summary>
        /// Profile fields
        /// </summary>
        public List<ProfileField> Fields
        {
            get
            {
                lock (BusinessEntity.m_sDS)
                {
                    if (m_fields.Count == 0)
                    {
                        try
                        {
                            var dataTable = BusinessEntity.m_sDS.ProfileFields;

                            foreach (var dataRow in ProfileField.m_sTA.GetDataByProfileID(ID))
                            {
                                dataTable.ImportRow(dataRow);
                                m_fields.Add(new ProfileField(this, dataTable[dataTable.Count - 1]));
                            }

                            if (m_fields.Count == 0)
                            {
                                foreach (SettingsProperty property in Settings.Default.Properties)
                                {
                                    Profile.Contact.AddField(Settings.Default[property.Name] as string);
                                }
                            }
                        }
                        catch { }
                    }

                    return m_fields;
                }
            }
        }

        /// <summary>
        /// Find
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ProfileField FindField(long id)
        {
            lock (BusinessEntity.m_sDS)
            {
                return Fields.Find(f => f.ID == id);
            }
        }

        /// <summary>
        /// Find
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ProfileField FindField(string name)
        {
            lock (BusinessEntity.m_sDS)
            {
                return Fields.Find(f => f.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            }
        }

        /// <summary>
        /// Adds a new field to the profile
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        public bool AddField(string name)
        {
            lock (BusinessEntity.m_sDS)
            {
                bool success = false;

                if (!string.IsNullOrEmpty(name))
                {
                    var dataRow = BusinessEntity.m_sDS.ProfileFields.AddProfileFieldsRow(name, m_dataRow);

                    try
                    {
                        success = ProfileField.m_sTA.Update(dataRow) == 1;
                    }
                    catch { }

                    if (success)
                    {
                        m_fields.Add(new ProfileField(this, dataRow));
                    }
                    else
                    {
                        BusinessEntity.m_sDS.ProfileFields.RemoveProfileFieldsRow(dataRow);
                    }
                }

                return success;
            }
        }

        /// <summary>
        /// Remove
        /// </summary>
        /// <param name="name"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        public bool RemoveField(ProfileField field)
        {
            lock (BusinessEntity.m_sDS)
            {
                bool success = false;

                if (field != null)
                {
                    try
                    {
                        success = 1 == ProfileField.m_sTA.DeleteByID(field.ID);
                    }
                    catch { }

                    if (success)
                    {
                        m_fields.Remove(field);
                        BusinessEntity.m_sDS.ProfileFields.RemoveProfileFieldsRow(field.m_dataRow);
                    }
                }

                return success;
            }
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="newName"></param>
        /// <returns></returns>
        public bool UpdateName(string newName)
        {
            lock (BusinessEntity.m_sDS)
            {
                bool success = false;

                if (this != Contact)
                {
                    try
                    {
                        m_dataRow.Name = newName;
                        success = 1 == m_sTA.Update(m_dataRow);
                    }
                    catch { }

                    if (!success)
                    {
                        m_dataRow.RejectChanges();
                    }
                }

                return success;
            }
        }
    }

    /// <summary>
    /// Profile field
    /// Predefined types
    /// </summary>
    public class ProfileField
    {
        #region Static

        /// <summary>
        /// Adapter
        /// </summary>
        internal static ProfileFieldsTableAdapter m_sTA = new ProfileFieldsTableAdapter();

        #endregion Static

        /// <summary>
        /// Table record
        /// </summary>
        internal DataSet.ProfileFieldsRow m_dataRow = null;

        /// <summary>
        /// Initialization
        /// </summary>
        /// <param name="dataRow"></param>
        internal ProfileField(Profile profile, DataSet.ProfileFieldsRow dataRow)
        {
            Profile = profile;
            m_dataRow = dataRow;
        }

        /// <summary>
        /// ID
        /// </summary>
        public long ID
        {
            get
            {
                return m_dataRow.ID;
            }
        }

        /// <summary>
        /// Profile type name
        /// </summary>
        public string Name
        {
            get
            {
                return m_dataRow.Name;
            }
        }

        /// <summary>
        /// Type
        /// </summary>
        public Profile Profile
        {
            get;
            private set;
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="newName"></param>
        /// <returns></returns>
        public bool UpdateName(string newName)
        {
            lock (BusinessEntity.m_sDS)
            {
                bool success = false;

                try
                {
                    m_dataRow.Name = newName;
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