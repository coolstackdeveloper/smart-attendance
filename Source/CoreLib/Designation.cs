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
    /// Designation
    /// </summary>
    public class Designation
    {
        #region Static

        /// <summary>
        /// Adapter
        /// </summary>
        internal static DesignationsTableAdapter m_sTA = new DesignationsTableAdapter();

        /// <summary>
        /// Types
        /// </summary>
        private static List<Designation> m_sDesignations = new List<Designation>();

        /// <summary>
        /// Gets the contact Designation
        /// </summary>
        public static List<Designation> All
        {
            get
            {
                lock (BusinessEntity.m_sDS)
                {
                    if (m_sDesignations.Count == 0)
                    {
                        try
                        {
                            var dataTable = BusinessEntity.m_sDS.Designations;

                            foreach (var dataRow in m_sTA.GetData())
                            {
                                dataTable.ImportRow(dataRow);
                                m_sDesignations.Add(new Designation(dataTable[dataTable.Count - 1]));
                            }
                        }
                        catch { }
                    }

                    return m_sDesignations;
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

                if (!string.IsNullOrEmpty(name) && Find(name) == null)
                {
                    var dataRow = BusinessEntity.m_sDS.Designations.AddDesignationsRow(name);

                    try
                    {
                        success = m_sTA.Update(dataRow) == 1;
                    }
                    catch { }

                    if (!success)
                    {
                        BusinessEntity.m_sDS.Designations.RemoveDesignationsRow(dataRow);
                    }
                    else
                    {
                        m_sDesignations.Add(new Designation(dataRow));
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
        public static bool Remove(Designation designation)
        {
            lock (BusinessEntity.m_sDS)
            {
                bool success = false;

                if (designation != null)
                {
                    try
                    {
                        success = (m_sTA.DeleteByID(designation.ID) == 1);
                    }
                    catch { }

                    if (success)
                    {
                        m_sDesignations.Remove(designation);
                        BusinessEntity.m_sDS.Designations.RemoveDesignationsRow(designation.m_dataRow);
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
        public static Designation Find(int id)
        {
            return All.Find(t => t.ID == id);
        }

        /// <summary>
        /// Find
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Designation Find(string name)
        {
            return All.Find(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        #endregion Static

        /// <summary>
        /// Table record
        /// </summary>
        internal DataSet.DesignationsRow m_dataRow = null;

        /// <summary>
        /// Initialization
        /// </summary>
        /// <param name="dataRow"></param>
        private Designation(DataSet.DesignationsRow dataRow)
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
        /// Designation type name
        /// </summary>
        public string Name
        {
            get
            {
                return m_dataRow.Name;
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

        /// <summary>
        /// Description
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }
    }
}