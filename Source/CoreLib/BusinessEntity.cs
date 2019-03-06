using System;
using System.Linq;
using System.Text;
using System.Data.Linq;
using System.Collections;
using System.Collections.Generic;

using Utilities;
using DataAccess;
using DataAccess.DataSetTableAdapters;

namespace CoreLib
{
    /// <summary>
    /// Entity type
    /// </summary>
    public enum BusinessEntityType
    {
        Company = 1, Branch
    }

    /// <summary>
    /// Entity class
    /// </summary>
    public class BusinessEntity
    {
        #region Static

        /// <summary>
        /// Data set
        /// </summary>
        internal static DataAccess.DataSet m_sDS = new DataSet();

        /// <summary>
        /// Types
        /// </summary>
        private static List<BusinessEntity> m_sBusinessEntities = new List<BusinessEntity>();

        /// <summary>
        /// Adapter
        /// </summary>
        internal static BusinessEntitiesTableAdapter m_sTA = new BusinessEntitiesTableAdapter();

        /// <summary>
        /// Gets the contact Department
        /// </summary>
        public static List<BusinessEntity> All
        {
            get
            {
                lock (BusinessEntity.m_sDS)
                {
                    if (m_sBusinessEntities.Count == 0)
                    {
                        try
                        {
                            var dataTable = m_sDS.BusinessEntities;

                            foreach (var dataRow in m_sTA.GetData())
                            {
                                dataTable.ImportRow(dataRow);
                                m_sBusinessEntities.Add(new BusinessEntity(dataTable[dataTable.Count - 1]));
                            }
                        }
                        catch { }
                    }

                    return m_sBusinessEntities;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private static bool m_sInitialized = false;

        /// <summary>
        /// Initialized
        /// </summary>
        public static bool Initialized
        {
            get
            {
                lock (BusinessEntity.m_sDS)
                {
                    if (!m_sInitialized)
                    {
                        // At least one entity
                        // At least one employee with login assigned
                        // For those employee, they must be administrator
                        int? value = Login.m_sTAQuesries.IsAdminInitialized();
                        m_sInitialized = value.HasValue ? value.Value > 0 : false;
                    }

                    return m_sInitialized;

                }
            }
            internal set
            {
                lock (BusinessEntity.m_sDS)
                {
                    m_sInitialized = value;
                }
            }
        }

        /// <summary>
        /// Add
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool Add(string name, BusinessEntity parent, BusinessEntityType type)
        {
            lock (BusinessEntity.m_sDS)
            {
                bool success = false;

                if (parent != null)
                {
                    // Ensure that the parent and the child does not have the same name...
                    success = !parent.Name.Equals(name, StringComparison.OrdinalIgnoreCase);

                    // Ensure none of the sibiings has the same name already...
                    success = success && !parent.Children.Exists(d => d.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
                }
                else if (type == BusinessEntityType.Company && parent == null)
                {
                    // Ensure that companies at the root with the same does not exist...
                    success = All.Where(s => s.Parent == null).Count(f => f.Name.Equals(name, StringComparison.OrdinalIgnoreCase)) == 0;
                }

                if (success)
                {
                    success = false; // Reset
                    var dataRow = m_sDS.BusinessEntities.AddBusinessEntitiesRow(name, (parent == null) ? null : parent.m_dataRow, (byte)type);

                    try
                    {
                        success = m_sTA.Update(dataRow) == 1;
                    }
                    catch { }

                    if (!success)
                    {
                        m_sDS.BusinessEntities.RemoveBusinessEntitiesRow(dataRow);
                    }
                    else
                    {
                        m_sBusinessEntities.Add(new BusinessEntity(dataRow));
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
        public static bool Remove(BusinessEntity businessEntity)
        {
            lock (BusinessEntity.m_sDS)
            {
                bool success = false;

                if (businessEntity != null)
                {
                    try
                    {
                        success = (m_sTA.DeleteByID(businessEntity.ID) == 1);
                    }
                    catch { }

                    if (success)
                    {
                        m_sBusinessEntities.Remove(businessEntity);
                        m_sDS.BusinessEntities.RemoveBusinessEntitiesRow(businessEntity.m_dataRow);
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
        public static BusinessEntity Find(int id)
        {
            return All.Find(t => t.ID == id);
        }

        /// <summary>
        /// Find
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static BusinessEntity Find(string name)
        {
            return All.Find(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        #endregion Static

        /// <summary>
        /// Table record
        /// </summary>
        internal DataSet.BusinessEntitiesRow m_dataRow = null;

        /// <summary>
        /// Initialization
        /// </summary>
        /// <param name="dataRow"></param>
        private BusinessEntity(DataSet.BusinessEntitiesRow dataRow)
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
        /// Parent
        /// </summary>
        private BusinessEntity m_parent = null;

        /// <summary>
        /// Parent ID 
        /// </summary>
        public BusinessEntity Parent
        {
            get
            {
                lock (BusinessEntity.m_sDS)
                {
                    if (m_parent == null && !m_dataRow.IsParentIDNull())
                    {
                        m_parent = All.Find(b => b.ID == m_dataRow.ParentID);
                    }

                    return m_parent;
                }
            }
        }

        /// <summary>
        /// TODO
        /// </summary>
        public BusinessEntityType Type
        {
            get
            {
                return m_dataRow.TypeID == (byte)BusinessEntityType.Company ? BusinessEntityType.Company : BusinessEntityType.Branch;
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
        /// Children
        /// </summary>
        public List<BusinessEntity> Children
        {
            get
            {
                return All.Where(c => c.Parent == this).ToList();
            }
        }

        /// <summary>
        /// Contact details
        /// </summary>
        private List<BusinessEntityContactDetail> m_contactDetails = new List<BusinessEntityContactDetail>();

        /// <summary>
        /// Contact details
        /// </summary>
        public List<BusinessEntityContactDetail> ContactDetails
        {
            get
            {
                lock (BusinessEntity.m_sDS)
                {
                    if (m_contactDetails.Count == 0)
                    {
                        try
                        {
                            var dataTable = m_sDS.BusinessEntityContactDetails;

                            foreach (var dataRow in BusinessEntityContactDetail.m_sTA.GetDataByBusinessEntityID(ID))
                            {
                                dataTable.ImportRow(dataRow);
                                m_contactDetails.Add(new BusinessEntityContactDetail(this, dataTable[dataTable.Count - 1]));
                            }
                        }
                        catch { }
                    }

                    return m_contactDetails;
                }
            }
        }

        /// <summary>
        /// Device list
        /// </summary>
        private List<Device> m_devices = new List<Device>();

        /// <summary>
        /// List of devices
        /// </summary>
        public List<Device> Devices
        {
            get
            {
                lock (BusinessEntity.m_sDS)
                {
                    if (m_devices.Count == 0)
                    {
                        try
                        {
                            var dataTable = m_sDS.Devices;

                            foreach (var dataRow in Device.m_sTA.GetDataByBusinessEntityID(ID))
                            {
                                dataTable.ImportRow(dataRow);
                                m_devices.Add(new Device(this, dataTable[dataTable.Count - 1]));
                            }
                        }
                        catch { }
                    }

                    return m_devices;
                }
            }
        }

        /// <summary>
        /// Holiday list
        /// </summary>
        private List<Holiday> m_holidays = new List<Holiday>();

        /// <summary>
        /// List of holidays
        /// </summary>
        public List<Holiday> Holidays
        {
            get
            {
                lock (BusinessEntity.m_sDS)
                {
                    if (m_holidays.Count == 0)
                    {
                        try
                        {
                            var dataTable = m_sDS.Holidays;

                            foreach (var dataRow in Holiday.m_sTA.GetDataByBusinessEntityID(ID))
                            {
                                dataTable.ImportRow(dataRow);
                                m_holidays.Add(new Holiday(this, dataTable[dataTable.Count - 1]));
                            }
                        }
                        catch { }
                    }

                    return m_holidays;
                }
            }
        }

        /// <summary>
        /// Shift list
        /// </summary>
        private List<Shift> m_shifts = new List<Shift>();

        /// <summary>
        /// List of shifts
        /// </summary>
        public List<Shift> Shifts
        {
            get
            {
                lock (BusinessEntity.m_sDS)
                {
                    if (m_shifts.Count == 0)
                    {
                        try
                        {
                            var dataTable = m_sDS.Shifts;

                            foreach (var dataRow in Shift.m_sTA.GetDataByBusinessEntityID(ID))
                            {
                                dataTable.ImportRow(dataRow);
                                m_shifts.Add(new Shift(this, dataTable[dataTable.Count - 1]));
                            }
                        }
                        catch { }
                    }

                    return m_shifts;
                }
            }
        }

        /// <summary>
        /// List of employees
        /// </summary>
        private List<Employee> m_employees = new List<Employee>();

        /// <summary>
        /// List of employees
        /// </summary>
        public List<Employee> Employees
        {
            get
            {
                lock (BusinessEntity.m_sDS)
                {
                    if (m_employees.Count == 0)
                    {
                        try
                        {
                            var dataTable = m_sDS.Employees;

                            foreach (var dataRow in Employee.m_sTA.GetDataByBusinessEntityID(ID))
                            {
                                dataTable.ImportRow(dataRow);
                                m_employees.Add(new Employee(this, dataTable[dataTable.Count - 1]));
                            }
                        }
                        catch { }
                    }

                    return m_employees;
                }
            }
        }

        /// <summary>
        /// Add a new designation
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool AddEmployee(string code, Gender gender,
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

                    var dataRow = m_sDS.Employees.AddEmployeesRow(code, gender == Gender.Female,
                                                                    firstName, middleName, lastName,
                                                                    joiningDate, default(DateTime), null, type.m_dataRow,
                                                                    null, m_dataRow, department.m_dataRow, designation.m_dataRow);

                    // For now we shall not expose this feature
                    dataRow.SetManagerIDNull();
                    dataRow.SetLeavingDateNull();
                    dataRow.SetPreviousIDNull();

                    try
                    {
                        success = 1 == Employee.m_sTA.Update(dataRow);
                    }
                    catch { }

                    if (success)
                    {
                        m_employees.Add(new Employee(this, dataRow));
                    }
                    else
                    {
                        m_sDS.Employees.RemoveEmployeesRow(dataRow);
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
        public bool RemoveEmployee(Employee employee)
        {
            lock (BusinessEntity.m_sDS)
            {
                bool success = false;

                if (employee != null)
                {
                    try
                    {
                        success = 1 == Employee.m_sTA.DeleteByID(employee.ID);
                    }
                    catch { }

                    if (success)
                    {
                        m_employees.Remove(employee);
                        BusinessEntity.m_sDS.Employees.RemoveEmployeesRow(employee.m_dataRow);
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
                    var dataRow = m_sDS.BusinessEntityContactDetails.AddBusinessEntityContactDetailsRow(m_dataRow, field.m_dataRow, value);

                    try
                    {
                        success = 1 == BusinessEntityContactDetail.m_sTA.Update(dataRow);
                    }
                    catch { }

                    if (success)
                    {
                        m_contactDetails.Add(new BusinessEntityContactDetail(this, dataRow));
                    }
                    else
                    {
                        m_sDS.BusinessEntityContactDetails.RemoveBusinessEntityContactDetailsRow(dataRow);
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
        public bool RemoveContactDetail(BusinessEntityContactDetail detail)
        {
            lock (BusinessEntity.m_sDS)
            {
                bool success = false;

                if (detail != null)
                {
                    try
                    {
                        success = 1 == BusinessEntityContactDetail.m_sTA.DeleteByID(detail.ID);
                    }
                    catch { }

                    if (success)
                    {
                        m_contactDetails.Remove(detail);
                        m_sDS.BusinessEntityContactDetails.RemoveBusinessEntityContactDetailsRow(detail.m_dataRow);
                    }
                }

                return success;
            }
        }

        /// <summary>
        /// Adds a new device
        /// </summary>
        /// <param name="name"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        public bool AddDevice(string name, string address, string subnet, string gateway, string mac, DeviceType type)
        {
            lock (BusinessEntity.m_sDS)
            {
                bool success = false;

                if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(address))
                {
                    var dataRow = m_sDS.Devices.AddDevicesRow(name, address, m_dataRow, true, subnet, gateway, mac);

                    try
                    {
                        dataRow.SetDirectionNull();

                        if (type != DeviceType.Both)
                        {
                            dataRow.Direction = (type == DeviceType.Entry);
                        }

                        success = 1 == Device.m_sTA.Update(dataRow);
                    }
                    catch { }

                    if (success)
                    {
                        m_devices.Add(new Device(this, dataRow));
                    }
                    else
                    {
                        m_sDS.Devices.RemoveDevicesRow(dataRow);
                    }
                }

                return success;
            }
        }

        /// <summary>
        /// Adds a new device
        /// </summary>
        /// <param name="name"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        public bool RemoveDevice(Device device)
        {
            lock (BusinessEntity.m_sDS)
            {
                bool success = false;

                if (device != null)
                {
                    try
                    {
                        success = 1 == Device.m_sTA.DeleteByID(device.ID);
                    }
                    catch { }

                    if (success)
                    {
                        m_devices.Remove(device);
                        m_sDS.Devices.RemoveDevicesRow(device.m_dataRow);
                    }
                }

                return success;
            }
        }

        /// <summary>
        /// Add holiday
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool AddHoliday(string name, DateTime start, DateTime end)
        {
            lock (BusinessEntity.m_sDS)
            {
                bool success = false;

                if (!string.IsNullOrEmpty(name))
                {
                    var dataRow = m_sDS.Holidays.AddHolidaysRow(start, end, m_dataRow, name);

                    try
                    {
                        success = 1 == Holiday.m_sTA.Update(dataRow);
                    }
                    catch { }

                    if (success)
                    {
                        m_holidays.Add(new Holiday(this, dataRow));
                    }
                    else
                    {
                        m_sDS.Holidays.RemoveHolidaysRow(dataRow);
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
        public bool RemoveHoliday(Holiday holiday)
        {
            lock (BusinessEntity.m_sDS)
            {
                bool success = false;

                if (holiday != null)
                {
                    try
                    {
                        success = 1 == Holiday.m_sTA.DeleteByID(holiday.ID);
                    }
                    catch { }

                    if (success)
                    {
                        m_holidays.Remove(holiday);
                        BusinessEntity.m_sDS.Holidays.RemoveHolidaysRow(holiday.m_dataRow);
                    }
                }

                return success;
            }
        }

        /// <summary>
        /// Add holiday
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool AddShift(string name, DateTime shiftStart, DateTime shiftEnd, DateTime punchInStart, DateTime punchInEnd, DateTime punchOutStart, DateTime punchOutEnd)
        {
            lock (BusinessEntity.m_sDS)
            {
                bool success = false;

                if (!string.IsNullOrEmpty(name))
                {
                    var dataRow = BusinessEntity.m_sDS.Shifts.AddShiftsRow(name, shiftStart, shiftEnd, punchInStart, punchInEnd, punchOutStart, punchOutEnd, m_dataRow);

                    try
                    {
                        success = 1 == Shift.m_sTA.Update(dataRow);
                    }
                    catch { }

                    if (success)
                    {
                        m_shifts.Add(new Shift(this, dataRow));
                    }
                    else
                    {
                        BusinessEntity.m_sDS.Shifts.RemoveShiftsRow(dataRow);
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
        public bool RemoveShift(Shift shift)
        {
            lock (BusinessEntity.m_sDS)
            {
                bool success = false;

                if (shift != null)
                {
                    try
                    {
                        success = 1 == Shift.m_sTA.DeleteByID(shift.ID);
                    }
                    catch { }

                    if (success)
                    {
                        m_shifts.Remove(shift);
                        BusinessEntity.m_sDS.Shifts.RemoveShiftsRow(shift.m_dataRow);
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
        public Shift FindShift(int id)
        {
            lock (BusinessEntity.m_sDS)
            {
                return Shifts.Find(s => s.ID == id);
            }
        }

        /// <summary>
        /// Find
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Holiday FindHoliday(int id)
        {
            lock (BusinessEntity.m_sDS)
            {
                return Holidays.Find(s => s.ID == id);
            }
        }

        /// <summary>
        /// Find
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Device FindDevice(int id)
        {
            lock (BusinessEntity.m_sDS)
            {
                return Devices.Find(s => s.ID == id);
            }
        }

        /// <summary>
        /// Find
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public BusinessEntityContactDetail FindContactDetail(int id)
        {
            lock (BusinessEntity.m_sDS)
            {
                return ContactDetails.Find(s => s.ID == id);
            }
        }

        /// <summary>
        /// Find
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Employee FindEmployee(long id)
        {
            lock (BusinessEntity.m_sDS)
            {
                return Employees.Find(s => s.ID == id);
            }
        }

        /// <summary>
        /// Find
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Employee FindEmployee(string id)
        {
            lock (BusinessEntity.m_sDS)
            {
                return Employees.Find(s => s.Code.Equals(id, StringComparison.OrdinalIgnoreCase));
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
}