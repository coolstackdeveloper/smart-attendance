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
    /// Department
    /// </summary>
    public class Department
    {
        #region Static

        /// <summary>
        /// Adapter
        /// </summary>
        internal static DepartmentsTableAdapter m_sTA = new DepartmentsTableAdapter();

        /// <summary>
        /// Types
        /// </summary>
        private static List<Department> m_sDepartments = new List<Department>();

        /// <summary>
        /// Gets the contact Department
        /// </summary>
        public static List<Department> All
        {
            get
            {
                lock (BusinessEntity.m_sDS)
                {
                    if (m_sDepartments.Count == 0)
                    {
                        try
                        {
                            var dataTable = BusinessEntity.m_sDS.Departments;

                            foreach (var dataRow in m_sTA.GetData())
                            {
                                dataTable.ImportRow(dataRow);
                                m_sDepartments.Add(new Department(dataTable[dataTable.Count - 1]));
                            }
                        }
                        catch { }
                    }

                    return m_sDepartments;
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
                    var dataRow = BusinessEntity.m_sDS.Departments.AddDepartmentsRow(name);

                    try
                    {
                        success = m_sTA.Update(dataRow) == 1;
                    }
                    catch { }

                    if (!success)
                    {
                        BusinessEntity.m_sDS.Departments.RemoveDepartmentsRow(dataRow);
                    }
                    else
                    {
                        m_sDepartments.Add(new Department(dataRow));
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
        public static bool Remove(Department department)
        {
            lock (BusinessEntity.m_sDS)
            {
                bool success = false;

                if (department != null)
                {
                    try
                    {
                        success = (m_sTA.DeleteByID(department.ID) == 1);
                    }
                    catch { }

                    if (success)
                    {
                        m_sDepartments.Remove(department);
                        BusinessEntity.m_sDS.Departments.RemoveDepartmentsRow(department.m_dataRow);
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
        public static Department Find(int id)
        {
            return All.Find(t => t.ID == id);
        }

        /// <summary>
        /// Find
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Department Find(string name)
        {
            return All.Find(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        #endregion Static

        /// <summary>
        /// Table record
        /// </summary>
        internal DataSet.DepartmentsRow m_dataRow = null;

        /// <summary>
        /// Initialization
        /// </summary>
        /// <param name="dataRow"></param>
        private Department(DataSet.DepartmentsRow dataRow)
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
        /// Department type name
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