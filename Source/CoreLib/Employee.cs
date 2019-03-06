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
    /// Gender type
    /// </summary>
    public enum Gender
    {
        Male, Female
    };

    /// <summary>
    /// Employee
    /// </summary>
    public class Employee
    {
        #region Static

        /// <summary>
        /// 
        /// </summary>
        internal static EmployeesTableAdapter m_sTA = new EmployeesTableAdapter();

        #endregion Static

        /// <summary>
        /// Table record
        /// </summary>
        internal DataSet.EmployeesRow m_dataRow = null;

        /// <summary>
        /// Initialization
        /// </summary>
        /// <param name="businessEntity"></param>
        /// <param name="dataRow"></param>
        internal Employee(BusinessEntity businessEntity, DataSet.EmployeesRow dataRow)
        {
            m_dataRow = dataRow;
            BusinessEntity = businessEntity;
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
        /// Organization wide unique employee code
        /// </summary>
        public string Code
        {
            get
            {
                return m_dataRow.Code;
            }
        }

        /// <summary>
        /// If set to 1, then male else female
        /// </summary>
        public Gender Gender
        {
            get
            {
                return m_dataRow.Gender ? Gender.Female : Gender.Male;
            }
        }

        /// <summary>
        /// Name
        /// </summary>
        public string Name
        {
            get
            {
                return string.Format("{0} {1} {2}", FirstName, MiddleName, LastName);
            }
        }
        
        /// <summary>
        /// First name
        /// </summary>
        public string FirstName
        {
            get
            {
                return m_dataRow.FirstName;
            }
        }

        /// <summary>
        /// Middle name
        /// </summary>
        public string MiddleName
        {
            get
            {
                return m_dataRow.IsMiddleNameNull() ? "" : m_dataRow.MiddleName;
            }
        }

        /// <summary>
        /// Last name
        /// </summary>
        public string LastName
        {
            get
            {
                return m_dataRow.IsLastNameNull() ? "" : m_dataRow.LastName;
            }
        }

        /// <summary>
        /// Joining data
        /// </summary>
        public DateTime JoiningDate
        {
            get
            {
                return m_dataRow.JoiningDate;
            }
        }

        /// <summary>
        /// Type
        /// </summary>
        private EmployeeType m_type = null;

        /// <summary>
        /// Unique name
        /// </summary>
        public EmployeeType Type
        {
            get
            {
                lock (BusinessEntity.m_sDS)
                {
                    if (m_type == null)
                    {
                        m_type = EmployeeType.Find(m_dataRow.EmployeeTypeID);
                    }

                    return m_type;
                }
            }
        }

        /// <summary>
        /// Associated business entity
        /// </summary>
        private Department m_department = null;

        /// <summary>
        /// Department
        /// </summary>
        public Department Department
        {
            get
            {
                lock (BusinessEntity.m_sDS)
                {
                    if (m_department == null)
                    {
                        m_department = Department.Find(m_dataRow.DepartmentID);
                    }

                    return m_department;
                }
            }
        }

        /// <summary>
        /// Associated designation
        /// </summary>
        private Designation m_designation = null;

        /// <summary>
        /// Designation
        /// </summary>
        public Designation Designation
        {
            get
            {
                lock (BusinessEntity.m_sDS)
                {
                    if (m_designation == null)
                    {
                        m_designation = Designation.Find(m_dataRow.DesignationID);
                    }

                    return m_designation;
                }
            }
        }

        /// <summary>
        /// Login
        /// </summary>
        private Login m_login = null;

        /// <summary>
        /// Login info
        /// </summary>
        public Login Login
        {
            get
            {
                lock (BusinessEntity.m_sDS)
                {
                    if (m_login == null)
                    {
                        try
                        {
                            var dataTable = BusinessEntity.m_sDS.Logins;
                            var tempTable = new DataAccess.DataSet.LoginsDataTable();
                            Login.m_sTA.FillByEmployeeID(tempTable, ID);

                            if (tempTable.Count == 1)
                            {
                                dataTable.ImportRow(tempTable[0]);
                                m_login = new Login(this, dataTable[dataTable.Count - 1]);
                            }
                        }
                        catch { }
                    }

                    return m_login;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private List<EmployeeShiftAssignment> m_shiftAssignments = new List<EmployeeShiftAssignment>();

        /// <summary>
        /// 
        /// </summary>
        public List<EmployeeShiftAssignment> ShiftAssignments
        {
            get
            {
                lock (BusinessEntity.m_sDS)
                {
                    if (m_shiftAssignments.Count == 0)
                    {
                        try
                        {
                            var dataTable = BusinessEntity.m_sDS.EmployeeShiftAssignments;

                            foreach (var dataRow in EmployeeShiftAssignment.m_sTA.GetDataByEmployeeID(ID))
                            {
                                dataTable.ImportRow(dataRow);
                                var shift = BusinessEntity.FindShift(dataTable[dataTable.Count - 1].ShiftID);
                                m_shiftAssignments.Add(new EmployeeShiftAssignment(dataTable[dataTable.Count - 1], shift));
                            }
                        }
                        catch (Exception ex)
                        {
                            string e = ex.Message;
                        }
                    }

                    return m_shiftAssignments;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public BusinessEntity BusinessEntity
        {
            get;
            private set;
        }
        /// <summary>
        /// List of leaves
        /// </summary>
        private List<Leave> m_leaves = new List<Leave>();

        /// <summary>
        /// Leaves
        /// </summary>
        public List<Leave> Leaves
        {
            get
            {
                lock (BusinessEntity.m_sDS)
                {
                    if (m_leaves.Count == 0)
                    {
                        try
                        {
                            var dataTable = BusinessEntity.m_sDS.Leaves;

                            foreach (var dataRow in Leave.m_sTA.GetDataByEmployeeID(ID))
                            {
                                dataTable.ImportRow(dataRow);
                                m_leaves.Add(new Leave(this, dataTable[dataTable.Count - 1]));
                            }
                        }
                        catch { }
                    }

                    return m_leaves;
                }
            }
        }

        /// <summary>
        /// Contact details
        /// </summary>
        private List<EmployeeContactDetail> m_contactDetails = new List<EmployeeContactDetail>();

        /// <summary>
        /// Contact details
        /// </summary>
        public List<EmployeeContactDetail> ContactDetails
        {
            get
            {
                lock (BusinessEntity.m_sDS)
                {
                    if (m_contactDetails.Count == 0)
                    {
                        try
                        {
                            var dataTable = BusinessEntity.m_sDS.EmployeeContactDetails;

                            foreach (var dataRow in EmployeeContactDetail.m_sTA.GetDataByEmployeeID(ID))
                            {
                                dataTable.ImportRow(dataRow);
                                m_contactDetails.Add(new EmployeeContactDetail(this, dataTable[dataTable.Count - 1]));
                            }
                        }
                        catch { }
                    }

                    return m_contactDetails;
                }
            }
        }

        /// <summary>
        /// Exceptions
        /// </summary>
        private List<AttendanceException> m_exceptions = new List<AttendanceException>();

        /// <summary>
        /// Shift exceptions
        /// </summary>
        public List<AttendanceException> AttendanceExceptions
        {
            get
            {
                lock (BusinessEntity.m_sDS)
                {
                    if (m_exceptions.Count == 0)
                    {
                        try
                        {
                            var dataTable = BusinessEntity.m_sDS.AttendanceExceptions;

                            foreach (var dataRow in AttendanceException.m_sTA.GetDataByEmployeeID(ID))
                            {
                                dataTable.ImportRow(dataRow);
                                m_exceptions.Add(new AttendanceException(this, dataTable[dataTable.Count - 1]));
                            }
                        }
                        catch { }
                    }

                    return m_exceptions;
                }
            }
        }

        /// <summary>
        /// Address
        /// </summary>
        public string Address
        {
            get
            {
                List<string> result = new List<string>();

                foreach (var detail in ContactDetails)
                {
                    if (!string.IsNullOrEmpty(detail.Value))
                    {
                        result.Add(string.Format("{0}: {1}", detail.Name, detail.Value));
                    }
                }

                return string.Join(", ", result.ToArray());
            }
        }

        /// <summary>
        /// Add exception
        /// </summary>
        /// <returns></returns>
        public bool AddAttendanceException(DateTime start, DateTime end, AttendanceExceptionType type, string remarks)
        {
            lock (BusinessEntity.m_sDS)
            {
                bool success = false;

                if (type != null)
                {
                    remarks = string.IsNullOrEmpty(remarks) ? "" : remarks;
                    var dataRow = BusinessEntity.m_sDS.AttendanceExceptions.AddAttendanceExceptionsRow(start, end, remarks, false, m_dataRow, type.m_dataRow);

                    try
                    {
                        success = 1 == AttendanceException.m_sTA.Update(dataRow);
                    }
                    catch { }

                    if (success)
                    {
                        m_exceptions.Add(new AttendanceException(this, dataRow));
                    }
                    else
                    {
                        BusinessEntity.m_sDS.AttendanceExceptions.RemoveAttendanceExceptionsRow(dataRow);
                    }
                }

                return success;
            }
        }

        /// <summary>
        /// Add a new contact info
        /// </summary>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool AddContactDetail(ProfileField field, string value)
        {
            lock (BusinessEntity.m_sDS)
            {
                bool success = false;

                if (field != null)
                {
                    value = string.IsNullOrEmpty(value) ? "" : value;
                    var dataRow = BusinessEntity.m_sDS.EmployeeContactDetails.AddEmployeeContactDetailsRow(m_dataRow, field.m_dataRow, value);

                    try
                    {
                        success = 1 == EmployeeContactDetail.m_sTA.Update(dataRow);
                    }
                    catch { }

                    if (success)
                    {
                        m_contactDetails.Add(new EmployeeContactDetail(this, dataRow));
                    }
                    else
                    {
                        BusinessEntity.m_sDS.EmployeeContactDetails.RemoveEmployeeContactDetailsRow(dataRow);
                    }
                }

                return success;
            }
        }

        /// <summary>
        /// Add leave
        /// </summary>
        /// <param name="type"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public bool AddLeave(LeaveType type, DateTime start, DateTime end)
        {
            lock (BusinessEntity.m_sDS)
            {
                bool success = false;

                if (type != null)
                {
                    var dataRow = BusinessEntity.m_sDS.Leaves.AddLeavesRow(start, end, false, m_dataRow, type.m_dataRow);

                    try
                    {
                        success = 1 == Leave.m_sTA.Update(dataRow);
                    }
                    catch { }

                    if (success)
                    {
                        m_leaves.Add(new Leave(this, dataRow));
                    }
                    else
                    {
                        BusinessEntity.m_sDS.Leaves.RemoveLeavesRow(dataRow);
                    }
                }

                return success;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool UnassignLogin()
        {
            lock (BusinessEntity.m_sDS)
            {
                bool success = false;

                if (m_login != null)
                {
                    try
                    {
                        success = 1 == Login.m_sTA.DeleteByEmployeeID(ID);
                    }
                    catch { }

                    if (success)
                    {
                        BusinessEntity.m_sDS.Logins.RemoveLoginsRow(m_login.m_dataRow);
                        m_login = null;
                    }
                }

                return success;
            }
        }

        /// <summary>
        /// Assign login name
        /// </summary>
        /// <param name="name"></param>
        /// <param name="password"></param>
        /// <param name="role"></param>
        /// <returns></returns>
        public bool AssignLogin(string name, string password, Role role)
        {
            lock (BusinessEntity.m_sDS)
            {
                bool success = false;

                if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(password) && role != null)
                {
                    var dataRow = BusinessEntity.m_sDS.Logins.AddLoginsRow(name, password.Encrypt(), m_dataRow, role.m_dataRow);

                    try
                    {
                        success = 1 == Login.m_sTA.Update(dataRow);
                    }
                    catch { }

                    if (success)
                    {
                        m_login = new Login(this, dataRow);
                    }
                    else
                    {
                        BusinessEntity.m_sDS.Logins.RemoveLoginsRow(dataRow);
                    }
                }

                return success;
            }
        }

        /// <summary>
        /// Assign shift to employee
        /// </summary>
        /// <param name="employee"></param>
        /// <param name="shift"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public bool AssignShift(Shift shift, DateTime start, DateTime end)
        {
            lock (BusinessEntity.m_sDS)
            {
                bool success = false;

                if (shift != null)
                {
                    var dataRow = BusinessEntity.m_sDS.EmployeeShiftAssignments.AddEmployeeShiftAssignmentsRow(start, end, m_dataRow, shift.m_dataRow);

                    try
                    {
                        success = (1 == EmployeeShiftAssignment.m_sTA.Update(dataRow));
                    }
                    catch { }

                    if (success)
                    {
                        m_shiftAssignments.Add(new EmployeeShiftAssignment(dataRow, shift));
                    }
                    else
                    {
                        BusinessEntity.m_sDS.EmployeeShiftAssignments.RemoveEmployeeShiftAssignmentsRow(dataRow);
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
        public AttendanceException FindAttendanceException(int id)
        {
            lock (BusinessEntity.m_sDS)
            {
                return AttendanceExceptions.Find(f => f.ID == id);
            }
        }

        /// <summary>
        /// Find
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public EmployeeContactDetail FindContactDetail(int id)
        {
            lock (BusinessEntity.m_sDS)
            {
                return ContactDetails.Find(s => s.ID == id);
            }
        }

        /// <summary>
        /// Add a new contact info
        /// </summary>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool RemoveContactDetail(EmployeeContactDetail detail)
        {
            lock (BusinessEntity.m_sDS)
            {
                bool success = false;

                if (detail != null)
                {
                    try
                    {
                        success = 1 == EmployeeContactDetail.m_sTA.DeleteByID(detail.ID);
                    }
                    catch { }

                    if (success)
                    {
                        m_contactDetails.Remove(detail);
                        BusinessEntity.m_sDS.EmployeeContactDetails.RemoveEmployeeContactDetailsRow(detail.m_dataRow);
                    }
                }

                return success;
            }
        }

        /// <summary>
        /// Add a new contact info
        /// </summary>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool RemoveAttendanceException(AttendanceException exception)
        {
            lock (BusinessEntity.m_sDS)
            {
                bool success = false;

                if (exception != null)
                {
                    try
                    {
                        success = 1 == AttendanceException.m_sTA.DeleteByID(exception.ID);
                    }
                    catch { }

                    if (success)
                    {
                        m_exceptions.Remove(exception);
                        BusinessEntity.m_sDS.AttendanceExceptions.RemoveAttendanceExceptionsRow(exception.m_dataRow);
                    }
                }

                return success;
            }
        }

        /// <summary>
        /// Add a new contact info
        /// </summary>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool RemoveShiftAssignment(EmployeeShiftAssignment shiftAssignment)
        {
            lock (BusinessEntity.m_sDS)
            {
                bool success = false;

                if (shiftAssignment != null)
                {
                    try
                    {
                        success = 1 == EmployeeShiftAssignment.m_sTA.DeleteByID(shiftAssignment.ID);
                    }
                    catch { }

                    if (success)
                    {
                        m_shiftAssignments.Remove(shiftAssignment);
                        BusinessEntity.m_sDS.EmployeeShiftAssignments.RemoveEmployeeShiftAssignmentsRow(shiftAssignment.m_dataRow);
                    }
                }

                return success;
            }
        }

        /// <summary>
        /// Add a new designation
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool UpdateDetails(string code, Gender gender,
                                  string firstName, string middleName, string lastName,
                                  DateTime joiningDate,
                                  Department department, Designation designation,
                                  EmployeeType type)
        {
            lock (BusinessEntity.m_sDS)
            {
                bool success = false;

                if (!string.IsNullOrEmpty(code) &&
                    !string.IsNullOrEmpty(firstName) &&
                    department != null && designation != null && type != null)
                {
                    // These are optional
                    lastName = string.IsNullOrEmpty(lastName) ? "" : lastName;
                    middleName = string.IsNullOrEmpty(middleName) ? "" : middleName;
                    m_dataRow.Code = code;
                    m_dataRow.Gender = Gender.Female == gender;
                    m_dataRow.FirstName = firstName;
                    m_dataRow.MiddleName = middleName;
                    m_dataRow.LastName = lastName;
                    m_dataRow.JoiningDate = joiningDate;
                    m_dataRow.DepartmentID = department.ID;
                    m_dataRow.DesignationID = designation.ID;
                    m_dataRow.EmployeeTypeID = type.ID;
                    
                    // For now we shall not expose this feature
                    m_dataRow.SetManagerIDNull();
                    m_dataRow.SetLeavingDateNull();
                    m_dataRow.SetPreviousIDNull();

                    try
                    {
                        success = 1 == Employee.m_sTA.Update(m_dataRow);
                        m_type = type;
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

    public class EmployeeType
    {
        #region Static

        /// <summary>
        /// Adapter
        /// </summary>
        internal static EmployeeTypesTableAdapter m_sTA = new EmployeeTypesTableAdapter();

        /// <summary>
        /// Types
        /// </summary>
        private static List<EmployeeType> m_sEmployeeTypes = new List<EmployeeType>();

        /// <summary>
        /// Gets the contact EmployeeType
        /// </summary>
        public static List<EmployeeType> All
        {
            get
            {
                lock (BusinessEntity.m_sDS)
                {
                    if (m_sEmployeeTypes.Count == 0)
                    {
                        try
                        {
                            var dataTable = BusinessEntity.m_sDS.EmployeeTypes;

                            foreach (var dataRow in m_sTA.GetData())
                            {
                                dataTable.ImportRow(dataRow);
                                m_sEmployeeTypes.Add(new EmployeeType(dataTable[dataTable.Count - 1]));
                            }
                        }
                        catch { }
                    }

                    return m_sEmployeeTypes;
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
                    var dataRow = BusinessEntity.m_sDS.EmployeeTypes.AddEmployeeTypesRow(name);

                    try
                    {
                        success = m_sTA.Update(dataRow) == 1;
                    }
                    catch { }

                    if (!success)
                    {
                        BusinessEntity.m_sDS.EmployeeTypes.RemoveEmployeeTypesRow(dataRow);
                    }
                    else
                    {
                        m_sEmployeeTypes.Add(new EmployeeType(dataRow));
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
        public static bool Remove(EmployeeType employeeType)
        {
            lock (BusinessEntity.m_sDS)
            {
                bool success = false;

                if (employeeType != null)
                {
                    try
                    {
                        success = (m_sTA.DeleteByID(employeeType.ID) == 1);
                    }
                    catch { }

                    if (success)
                    {
                        m_sEmployeeTypes.Remove(employeeType);
                        BusinessEntity.m_sDS.EmployeeTypes.RemoveEmployeeTypesRow(employeeType.m_dataRow);
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
        public static EmployeeType Find(int id)
        {
            return All.Find(t => t.ID == id);
        }

        /// <summary>
        /// Find
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static EmployeeType Find(string name)
        {
            return All.Find(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        #endregion Static

        /// <summary>
        /// Table record
        /// </summary>
        internal DataSet.EmployeeTypesRow m_dataRow = null;

        /// <summary>
        /// Initialization
        /// </summary>
        /// <param name="dataRow"></param>
        private EmployeeType(DataSet.EmployeeTypesRow dataRow)
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
        /// EmployeeType type name
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
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name;
        }
    }

    /// <summary>
    /// Shift assigned to employee
    /// </summary>
    public class EmployeeShiftAssignment
    {
        #region Static

        /// <summary>
        /// Adapter
        /// </summary>
        internal static EmployeeShiftAssignmentsTableAdapter m_sTA = new EmployeeShiftAssignmentsTableAdapter();

        #endregion Static

        /// <summary>
        /// 
        /// </summary>
        internal DataSet.EmployeeShiftAssignmentsRow m_dataRow;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataRow"></param>
        internal EmployeeShiftAssignment(DataSet.EmployeeShiftAssignmentsRow dataRow, Shift shift)
        {
            Shift = shift;
            m_dataRow = dataRow;
        }

        public int ID
        {
            get
            {
                return m_dataRow.ID;
            }
        }

        public Shift Shift
        {
            get;
            private set;
        }

        public DateTime Start
        {
            get
            {
                return m_dataRow.StartDate;
            }
        }

        public DateTime End
        {
            get
            {
                return m_dataRow.EndDate;
            }
        }

        public override string ToString()
        {
            return Shift.Description;
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool UpdateDetails(Shift shift, DateTime start, DateTime end)
        {
            lock (BusinessEntity.m_sDS)
            {
                bool success = false;

                if (shift != null)
                {
                    try
                    {
                        Shift = shift;
                        m_dataRow.StartDate = start;
                        m_dataRow.EndDate = end;
                        m_dataRow.ShiftID = shift.ID;
                        success = 1 == m_sTA.Update(m_dataRow);
                    }
                    catch { }
                }

                if (!success)
                {
                    m_dataRow.RejectChanges();
                }

                return success;
            }
        }
    }
}