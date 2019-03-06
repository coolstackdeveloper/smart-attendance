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
    /// Shift
    /// </summary>
    public class Shift
    {
        #region Static

        /// <summary>
        /// 
        /// </summary>
        internal static ShiftsTableAdapter m_sTA = new ShiftsTableAdapter();

        #endregion Static

        /// <summary>
        /// Table record
        /// </summary>
        internal DataSet.ShiftsRow m_dataRow = null;

        /// <summary>
        /// Initialization
        /// </summary>
        /// <param name="dataRow">Table row</param>
        internal Shift(BusinessEntity businessEntity, DataSet.ShiftsRow dataRow)
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
        /// Name
        /// </summary>
        public string Name
        {
            get
            {
                return m_dataRow.Name;
            }
        }

        /// <summary>
        /// Description
        /// </summary>
        public string Description
        {
            get
            {
                var timeDifference = (EndTime - StartTime).TotalHours;
                return string.Format("{0} [{1}-{2}] [{3} Hours]", Name, StartTime.ToShortTimeString(), EndTime.ToShortTimeString(), timeDifference);
            }
        }

        /// <summary>
        /// Start
        /// </summary>
        public DateTime StartTime
        {
            get
            {
                return m_dataRow.StartTime;
            }
        }

        /// <summary>
        /// Start
        /// </summary>
        public DateTime EndTime
        {
            get
            {
                return m_dataRow.EndTime;
            }
        }

        /// <summary>
        /// Start
        /// </summary>
        public DateTime PunchInStartTime
        {
            get
            {
                return m_dataRow.PunchInStartTime;
            }
        }

        /// <summary>
        /// Start
        /// </summary>
        public DateTime PunchInEndTime
        {
            get
            {
                return m_dataRow.PunchInEndTime;
            }
        }

        /// <summary>
        /// Start
        /// </summary>
        public DateTime PunchOutStartTime
        {
            get
            {
                return m_dataRow.PunchOutStartTime;
            }
        }

        /// <summary>
        /// Start
        /// </summary>
        public DateTime PunchOutEndTime
        {
            get
            {
                return m_dataRow.PunchOutEndTime;
            }
        }

        /// <summary>
        /// Name
        /// </summary>
        public BusinessEntity BusinessEntity
        {
            get;
            private set;
        }

        /// <summary>
        /// Exceptions
        /// </summary>
        private List<ShiftException> m_exceptions = new List<ShiftException>();

        /// <summary>
        /// Shift exceptions
        /// </summary>
        public List<ShiftException> Exceptions
        {
            get
            {
                lock (BusinessEntity.m_sDS)
                {
                    if (m_exceptions.Count == 0)
                    {
                        try
                        {
                            var dataTable = BusinessEntity.m_sDS.ShiftExceptions;

                            foreach (var dataRow in ShiftException.m_sTA.GetDataByShiftID(ID))
                            {
                                dataTable.ImportRow(dataRow);
                                m_exceptions.Add(new ShiftException(this, dataTable[dataTable.Count - 1]));
                            }
                        }
                        catch { }
                    }

                    return m_exceptions;
                }
            }
        }

        /// <summary>
        /// Add
        /// </summary>
        /// <param name="name"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        public bool AddException(DateTime start, DateTime end, DayOfWeek? weekday, ShiftExceptionType type)
        {
            lock (BusinessEntity.m_sDS)
            {
                bool success = false;

                if (type != null)
                {
                    var dataRow = BusinessEntity.m_sDS.ShiftExceptions.AddShiftExceptionsRow(start, end, null , m_dataRow, type.m_dataRow);
                    dataRow.SetWeekDayNull();

                    if (weekday.HasValue)
                    {
                        dataRow.WeekDay = weekday.ToString();
                    }

                    try
                    {
                        success = 1 == ShiftException.m_sTA.Update(dataRow);
                    }
                    catch { }

                    if (success)
                    {
                        m_exceptions.Add(new ShiftException(this, dataRow));
                    }
                    else
                    {
                        BusinessEntity.m_sDS.ShiftExceptions.RemoveShiftExceptionsRow(dataRow);
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
        public bool RemoveException(ShiftException exception)
        {
            lock (BusinessEntity.m_sDS)
            {
                bool success = false;

                if (exception != null)
                {
                    try
                    {
                        success = 1 == ShiftException.m_sTA.DeleteByID(exception.ID);
                    }
                    catch { }

                    if (success)
                    {
                        m_exceptions.Remove(exception);
                        BusinessEntity.m_sDS.ShiftExceptions.RemoveShiftExceptionsRow(exception.m_dataRow);
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
        public ShiftException FindException(int id)
        {
            lock (BusinessEntity.m_sDS)
            {
                return Exceptions.Find(f => f.ID == id);
            }
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool UpdateDetails(string name, DateTime shiftStart, DateTime shiftEnd, DateTime punchInStart, DateTime punchInEnd, DateTime punchOutStart, DateTime punchOutEnd)
        {
            lock (BusinessEntity.m_sDS)
            {
                bool success = false;

                try
                {
                    m_dataRow.Name = name;
                    m_dataRow.StartTime = shiftStart;
                    m_dataRow.EndTime = shiftEnd;
                    m_dataRow.PunchInStartTime = punchInStart;
                    m_dataRow.PunchInEndTime = punchInEnd;
                    m_dataRow.PunchOutStartTime = punchOutStart;
                    m_dataRow.PunchOutEndTime = punchOutEnd;
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

    /// <summary>
    /// Shift exception
    /// </summary>
    public class ShiftException
    {
        #region Static

        /// <summary>
        /// 
        /// </summary>
        internal static ShiftExceptionsTableAdapter m_sTA = new ShiftExceptionsTableAdapter();

        #endregion Static

        /// <summary>
        /// Table record
        /// </summary>
        internal DataSet.ShiftExceptionsRow m_dataRow = null;

        /// <summary>
        /// Initialization
        /// </summary>
        /// <param name="dataRow">Table row</param>
        internal ShiftException(Shift shift, DataSet.ShiftExceptionsRow dataRow)
        {
            Shift = shift;
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
        /// Week day exception
        /// </summary>
        public DayOfWeek? WeekDay
        {
            get
            {
                return m_dataRow.IsWeekDayNull() ? (DayOfWeek?)null : (DayOfWeek)Enum.Parse(typeof(DayOfWeek), m_dataRow.WeekDay);
            }
        }

        /// <summary>
        /// Start
        /// </summary>
        public DateTime StartTime
        {
            get
            {
                return m_dataRow.StartTime;
            }
        }

        /// <summary>
        /// Start
        /// </summary>
        public DateTime EndTime
        {
            get
            {
                return m_dataRow.EndTime;
            }
        }

        /// <summary>
        /// Shift
        /// </summary>
        public Shift Shift
        {
            get;
            private set;
        }

        /// <summary>
        /// Type
        /// </summary>
        private ShiftExceptionType m_type = null;

        /// <summary>
        /// Type
        /// </summary>
        public ShiftExceptionType Type
        {
            get
            {
                lock (BusinessEntity.m_sDS)
                {
                    if (m_type == null)
                    {
                        m_type = ShiftExceptionType.Find(m_dataRow.ShiftExceptionTypeID);
                    }

                    return m_type;
                }
            }
        }

        /// <summary>
        /// Add
        /// </summary>
        /// <param name="name"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        public bool UpdateDetails(DateTime start, DateTime end, DayOfWeek? weekday, ShiftExceptionType type)
        {
            lock (BusinessEntity.m_sDS)
            {
                bool success = false;

                if (type != null)
                {
                    m_dataRow.StartTime = start;
                    m_dataRow.EndTime = end;
                    m_dataRow.SetWeekDayNull();

                    if (weekday.HasValue)
                    {
                        m_dataRow.WeekDay = weekday.Value.ToString();
                    }

                    m_dataRow.ShiftExceptionTypeID = type.ID;

                    try
                    {
                        success = 1 == ShiftException.m_sTA.Update(m_dataRow);
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
    /// Shift exception types
    /// </summary>
    public class ShiftExceptionType
    {
        #region Static

        /// <summary>
        /// Adapter
        /// </summary>
        internal static ShiftExceptionTypesTableAdapter m_sTA = new ShiftExceptionTypesTableAdapter();

        /// <summary>
        /// Types
        /// </summary>
        private static List<ShiftExceptionType> m_sShiftExceptionTypes = new List<ShiftExceptionType>();

        /// <summary>
        /// Gets the contact ShiftExceptionType
        /// </summary>
        public static List<ShiftExceptionType> All
        {
            get
            {
                lock (BusinessEntity.m_sDS)
                {
                    if (m_sShiftExceptionTypes.Count == 0)
                    {
                        try
                        {
                            var dataTable = BusinessEntity.m_sDS.ShiftExceptionTypes;

                            foreach (var dataRow in m_sTA.GetData())
                            {
                                dataTable.ImportRow(dataRow);
                                m_sShiftExceptionTypes.Add(new ShiftExceptionType(dataTable[dataTable.Count - 1]));
                            }
                        }
                        catch { }
                    }

                    return m_sShiftExceptionTypes;
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
                    var dataRow = BusinessEntity.m_sDS.ShiftExceptionTypes.AddShiftExceptionTypesRow(name);

                    try
                    {
                        success = m_sTA.Update(dataRow) == 1;
                    }
                    catch { }

                    if (!success)
                    {
                        BusinessEntity.m_sDS.ShiftExceptionTypes.RemoveShiftExceptionTypesRow(dataRow);
                    }
                    else
                    {
                        m_sShiftExceptionTypes.Add(new ShiftExceptionType(dataRow));
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
        public static bool Remove(ShiftExceptionType shiftExceptionType)
        {
            lock (BusinessEntity.m_sDS)
            {
                bool success = false;

                if (shiftExceptionType != null)
                {
                    try
                    {
                        success = (m_sTA.DeleteByID(shiftExceptionType.ID) == 1);
                    }
                    catch { }

                    if (success)
                    {
                        m_sShiftExceptionTypes.Remove(shiftExceptionType);
                        BusinessEntity.m_sDS.ShiftExceptionTypes.RemoveShiftExceptionTypesRow(shiftExceptionType.m_dataRow);
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
        public static ShiftExceptionType Find(int id)
        {
            return All.Find(t => t.ID == id);
        }

        /// <summary>
        /// Find
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ShiftExceptionType Find(string name)
        {
            return All.Find(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        #endregion Static

        /// <summary>
        /// Table record
        /// </summary>
        internal DataSet.ShiftExceptionTypesRow m_dataRow = null;

        /// <summary>
        /// Initialization
        /// </summary>
        /// <param name="dataRow"></param>
        private ShiftExceptionType(DataSet.ShiftExceptionTypesRow dataRow)
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
        /// ShiftExceptionType type name
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