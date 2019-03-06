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
    /// Employee leave
    /// </summary>
    public class Leave
    {
        #region Static

        /// <summary>
        /// 
        /// </summary>
        internal static LeavesTableAdapter m_sTA = new LeavesTableAdapter();

        #endregion Static

        /// <summary>
        /// Table record
        /// </summary>
        private DataSet.LeavesRow m_dataRow = null;

        /// <summary>
        /// Initialization
        /// </summary>
        /// <param name="dataRow"></param>
        internal Leave(Employee employee, DataSet.LeavesRow dataRow)
        {
            m_dataRow = dataRow;
            Employee = employee;
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
        /// Start date
        /// </summary>
        public DateTime Start
        {
            get
            {
                return m_dataRow.Start;
            }
        }

        /// <summary>
        /// End time
        /// </summary>
        public DateTime End
        {
            get
            {
                return m_dataRow.End;
            }
        }

        /// <summary>
        /// Is approved
        /// </summary>
        public bool Approved
        {
            get
            {
                return !m_dataRow.IsApprovedNull() && m_dataRow.Approved;
            }
        }

        /// <summary>
        /// Employee
        /// </summary>
        public Employee Employee
        {
            get;
            private set;
        }

        /// <summary>
        /// Type
        /// </summary>
        private LeaveType m_type = null;

        /// <summary>
        /// 
        /// </summary>
        public LeaveType Type
        {
            get
            {
                lock (BusinessEntity.m_sDS)
                {
                    if (m_type == null)
                    {
                        m_type = LeaveType.Find(m_dataRow.LeaveTypeID);
                    }

                    return m_type;
                }
            }
        }
    }

    public class LeaveType
    {
        #region Static

        /// <summary>
        /// Adapter
        /// </summary>
        internal static LeaveTypesTableAdapter m_sTA = new LeaveTypesTableAdapter();

        /// <summary>
        /// Types
        /// </summary>
        private static List<LeaveType> m_sLeaveTypes = new List<LeaveType>();

        /// <summary>
        /// Gets the contact LeaveType
        /// </summary>
        public static List<LeaveType> All
        {
            get
            {
                lock (BusinessEntity.m_sDS)
                {
                    if (m_sLeaveTypes.Count == 0)
                    {
                        try
                        {
                            var dataTable = BusinessEntity.m_sDS.LeaveTypes;

                            foreach (var dataRow in m_sTA.GetData())
                            {
                                dataTable.ImportRow(dataRow);
                                m_sLeaveTypes.Add(new LeaveType(dataTable[dataTable.Count - 1]));
                            }
                        }
                        catch { }
                    }

                    return m_sLeaveTypes;
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
                    var dataRow = BusinessEntity.m_sDS.LeaveTypes.AddLeaveTypesRow(name);

                    try
                    {
                        success = m_sTA.Update(dataRow) == 1;
                    }
                    catch { }

                    if (!success)
                    {
                        BusinessEntity.m_sDS.LeaveTypes.RemoveLeaveTypesRow(dataRow);
                    }
                    else
                    {
                        m_sLeaveTypes.Add(new LeaveType(dataRow));
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
        public static bool Remove(LeaveType leaveType)
        {
            lock (BusinessEntity.m_sDS)
            {
                bool success = false;

                if (leaveType != null)
                {
                    try
                    {
                        success = (m_sTA.DeleteByID(leaveType.ID) == 1);
                    }
                    catch { }

                    if (success)
                    {
                        m_sLeaveTypes.Remove(leaveType);
                        BusinessEntity.m_sDS.LeaveTypes.RemoveLeaveTypesRow(leaveType.m_dataRow);
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
        public static LeaveType Find(int id)
        {
            return All.Find(t => t.ID == id);
        }

        /// <summary>
        /// Find
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static LeaveType Find(string name)
        {
            return All.Find(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        #endregion Static

        /// <summary>
        /// Table record
        /// </summary>
        internal DataSet.LeaveTypesRow m_dataRow = null;

        /// <summary>
        /// Initialization
        /// </summary>
        /// <param name="dataRow"></param>
        private LeaveType(DataSet.LeaveTypesRow dataRow)
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
        /// LeaveType type name
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

        public override string ToString()
        {
            return Name;
        }
    }
}