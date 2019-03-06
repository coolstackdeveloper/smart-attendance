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
    /// Attendance exception types
    /// </summary>
    public class AttendanceExceptionType
    {
        #region Static

        /// <summary>
        /// Adapter
        /// </summary>
        internal static AttendanceExceptionTypesTableAdapter m_sTA = new AttendanceExceptionTypesTableAdapter();

        /// <summary>
        /// Types
        /// </summary>
        private static List<AttendanceExceptionType> m_sAttendanceExceptionTypes = new List<AttendanceExceptionType>();

        /// <summary>
        /// Gets the contact AttendanceExceptionType
        /// </summary>
        public static List<AttendanceExceptionType> All
        {
            get
            {
                lock (BusinessEntity.m_sDS)
                {
                    if (m_sAttendanceExceptionTypes.Count == 0)
                    {
                        try
                        {
                            var dataTable = BusinessEntity.m_sDS.AttendanceExceptionTypes;

                            foreach (var dataRow in m_sTA.GetData())
                            {
                                dataTable.ImportRow(dataRow);
                                m_sAttendanceExceptionTypes.Add(new AttendanceExceptionType(dataTable[dataTable.Count - 1]));
                            }
                        }
                        catch { }
                    }

                    return m_sAttendanceExceptionTypes;
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
                    var dataRow = BusinessEntity.m_sDS.AttendanceExceptionTypes.AddAttendanceExceptionTypesRow(name);

                    try
                    {
                        success = m_sTA.Update(dataRow) == 1;
                    }
                    catch { }

                    if (!success)
                    {
                        BusinessEntity.m_sDS.AttendanceExceptionTypes.RemoveAttendanceExceptionTypesRow(dataRow);
                    }
                    else
                    {
                        m_sAttendanceExceptionTypes.Add(new AttendanceExceptionType(dataRow));
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
        public static bool Remove(AttendanceExceptionType type)
        {
            lock (BusinessEntity.m_sDS)
            {
                bool success = false;

                if (type != null)
                {
                    try
                    {
                        success = (m_sTA.DeleteByID(type.ID) == 1);
                    }
                    catch { }

                    if (success)
                    {
                        m_sAttendanceExceptionTypes.Remove(type);
                        BusinessEntity.m_sDS.AttendanceExceptionTypes.RemoveAttendanceExceptionTypesRow(type.m_dataRow);
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
        public static AttendanceExceptionType Find(int id)
        {
            return All.Find(t => t.ID == id);
        }

        /// <summary>
        /// Find
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static AttendanceExceptionType Find(string name)
        {
            return All.Find(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        #endregion Static

        /// <summary>
        /// Table record
        /// </summary>
        internal DataSet.AttendanceExceptionTypesRow m_dataRow = null;

        /// <summary>
        /// Initialization
        /// </summary>
        /// <param name="dataRow"></param>
        private AttendanceExceptionType(DataSet.AttendanceExceptionTypesRow dataRow)
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
        /// AttendanceExceptionType type name
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

    /// <summary>
    /// Attendance exception
    /// </summary>
    public class AttendanceException
    {
        #region Static

        /// <summary>
        /// Adapter
        /// </summary>
        internal static AttendanceExceptionsTableAdapter m_sTA = new AttendanceExceptionsTableAdapter();

        #endregion Static

        /// <summary>
        /// Table record
        /// </summary>
        internal DataSet.AttendanceExceptionsRow m_dataRow = null;

        /// <summary>
        /// Initialization
        /// </summary>
        /// <param name="dataRow">Table row</param>
        internal AttendanceException(Employee employee, DataSet.AttendanceExceptionsRow dataRow)
        {
            m_dataRow = dataRow;
            Employee = employee;
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
        /// Start
        /// </summary>
        public DateTime Start
        {
            get
            {
                return m_dataRow.Start;
            }
        }

        /// <summary>
        /// End
        /// </summary>
        public DateTime End
        {
            get
            {
                return m_dataRow.End;
            }
        }

        /// <summary>
        /// Approved
        /// </summary>
        public bool Approved
        {
            get
            {
                return m_dataRow.IsApprovedNull() ? false : m_dataRow.Approved;
            }
        }

        /// <summary>
        /// Remarks
        /// </summary>
        public string Remarks
        {
            get
            {
                return m_dataRow.Remarks;
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
        private AttendanceExceptionType m_type = null;

        /// <summary>
        /// Type
        /// </summary>
        public AttendanceExceptionType Type
        {
            get
            {
                lock (BusinessEntity.m_sDS)
                {
                    if (m_type == null)
                    {
                        m_type = AttendanceExceptionType.Find(m_dataRow.AttendanceExceptionTypeID);
                    }

                    return m_type;
                }
            }
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool UpdateDetails(string remarks, DateTime start, DateTime end, bool approved)
        {
            lock (BusinessEntity.m_sDS)
            {
                bool success = false;

                try
                {
                    m_dataRow.Start = start;
                    m_dataRow.End = end;
                    m_dataRow.Approved = approved;
                    m_dataRow.Remarks = remarks;
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