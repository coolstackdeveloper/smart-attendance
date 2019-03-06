using System;
using System.Linq;
using System.Text;
using System.Data.Linq;
using System.Reflection;
using System.Collections.Generic;

using Utilities;
using DataAccess;
using DataAccess.DataSetTableAdapters;

namespace CoreLib
{
    /// <summary>
    /// ID must match with the DB schema
    /// </summary>
    public enum TaskType
    {
        None,
        [TaskAttribute("Add Company")]
        AddCompany,
        [TaskAttribute("Edit Company Details")]
        EditCompanyDetails = 1,
        [TaskAttribute("View Company Details")]
        ViewCompanyDetails,
        [TaskAttribute("Delete Company")]
        DeleteCompany,
        [TaskAttribute("Add Branch")]
        AddBranch,
        [TaskAttribute("Delete Branch")]
        DeleteBranch,
        [TaskAttribute("Edit Branch Details")]
        EditBranchDetails,
        [TaskAttribute("View Branch Details")]
        ViewBranchDetails,
        [TaskAttribute("Add Department")]
        AddDepartment,
        [TaskAttribute("Delete Department")]
        DeleteDepartment,
        [TaskAttribute("Edit Department Details")]
        EditDepartmentDetails,
        [TaskAttribute("View Department Details")]
        ViewDepartmentDetails,
        [TaskAttribute("Add Designation")]
        AddDesignation,
        [TaskAttribute("Delete Designation")]
        DeleteDesignation,
        [TaskAttribute("Edit Designation Details")]
        EditDesignationDetails,
        [TaskAttribute("View Designation Details")]
        ViewDesignationDetails,
        [TaskAttribute("Add Device")]
        AddDevice,
        [TaskAttribute("Delete Device")]
        DeleteDevice,
        [TaskAttribute("Edit Device Details")]
        EditDeviceDetails,
        [TaskAttribute("View Device Details")]
        ViewDeviceDetails,
        [TaskAttribute("Add Shift")]
        AddShift,
        [TaskAttribute("Delete Shift")]
        DeleteShift,
        [TaskAttribute("Edit Shift Details")]
        EditShiftDetails,
        [TaskAttribute("View Shift Details")]
        ViewShiftDetails,
        [TaskAttribute("Add Shift Exception")]
        AddShiftException,
        [TaskAttribute("Delete Shift Exception")]
        DeleteShiftException,
        [TaskAttribute("Edit Shift Exception Details")]
        EditShiftExceptionDetails,
        [TaskAttribute("View Shift Exception Details")]
        ViewShiftExceptionDetails,
        [TaskAttribute("Add Shift Exception Type")]
        AddShiftExceptionType,
        [TaskAttribute("Delete Shift Exception Type")]
        DeleteShiftExceptionType,
        [TaskAttribute("Edit Shift Exception Type Details")]
        EditShiftExceptionTypeDetails,
        [TaskAttribute("View Shift Exception Type Details")]
        ViewShiftExceptionTypeDetails,
        [TaskAttribute("Add Role")]
        AddRole,
        [TaskAttribute("Delete Role")]
        DeleteRole,
        [TaskAttribute("Edit Role Details")]
        EditRoleDetails,
        [TaskAttribute("View Role Details")]
        ViewRoleDetails,
        [TaskAttribute("Add Contact Field")]
        AddContactField,
        [TaskAttribute("Delete Contact Field")]
        DeleteContactField,
        [TaskAttribute("Edit Contact Field Details")]
        EditContactFieldDetails,
        [TaskAttribute("View Contact Field Details")]
        ViewContactFieldDetails,
        [TaskAttribute("Add Holiday")]
        AddHoliday,
        [TaskAttribute("Delete Holiday")]
        DeleteHoliday,
        [TaskAttribute("Edit Holiday Details")]
        EditHolidayDetails,
        [TaskAttribute("View Holiday Details")]
        ViewHolidayDetails,
        [TaskAttribute("Add Employee Type")]
        AddEmployeeType,
        [TaskAttribute("Delete Employee Type")]
        DeleteEmployeeType,
        [TaskAttribute("Edit Employee Type Details")]
        EditEmployeeTypeDetails,
        [TaskAttribute("View Employee Type Details")]
        ViewEmployeeTypeDetails,
        [TaskAttribute("Add Employee")]
        AddEmployee,
        [TaskAttribute("Delete Employee")]
        DeleteEmployee,
        [TaskAttribute("Edit Employee Details")]
        EditEmployeeDetails,
        [TaskAttribute("View Employee Details")]
        ViewEmployeeDetails,
        [TaskAttribute("Add Employee Shift")]
        AddEmployeeShift,
        [TaskAttribute("Delete Employee Shift")]
        DeleteEmployeeShift,
        [TaskAttribute("Edit Employee Shift Details")]
        EditEmployeeShiftDetails,
        [TaskAttribute("View Employee Shift Details")]
        ViewEmployeeShiftDetails,
        [TaskAttribute("Add Entry Type")]
        AddAttendanceExceptionType,
        [TaskAttribute("Delete Entry Type")]
        DeleteAttendanceExceptionType,
        [TaskAttribute("Edit Entry Type Details")]
        EditAttendanceExceptionTypeDetails,
        [TaskAttribute("View Entry Type Details")]
        ViewAttendanceExceptionTypeDetails,
        [TaskAttribute("Add Entry")]
        AddAttendanceException,
        [TaskAttribute("Delete Entry")]
        DeleteAttendanceException,
        [TaskAttribute("Edit Entry Details")]
        EditAttendanceExceptionDetails,
        [TaskAttribute("View Entry Details")]
        ViewAttendanceExceptionDetails,
    }

    /// <summary>
    /// Task
    /// </summary>
    public class Task
    {
        #region Static

        /// <summary>
        /// Adapter
        /// </summary>
        internal static TasksTableAdapter m_sTA = new TasksTableAdapter();

        /// <summary>
        /// Types
        /// </summary>
        private static List<Task> m_sTasks = new List<Task>();

        /// <summary>
        /// Gets the contact Task
        /// </summary>
        public static List<Task> All
        {
            get
            {
                lock (BusinessEntity.m_sDS)
                {
                    if (m_sTasks.Count == 0)
                    {
                        try
                        {
                            var dataTable = BusinessEntity.m_sDS.Tasks;

                            foreach (var dataRow in m_sTA.GetData())
                            {
                                dataTable.ImportRow(dataRow);
                                m_sTasks.Add(new Task(dataTable[dataTable.Count - 1]));
                            }

                            foreach (TaskType taskType in Enum.GetValues(typeof(TaskType)))
                            {
                                var attribute = taskType.Attribute();

                                if(attribute != null)
                                {
                                    if (!m_sTasks.Exists(t => t.Name.Equals(attribute.Description, StringComparison.OrdinalIgnoreCase)))
                                    {
                                        Add(taskType);
                                    }
                                }
                            }
                        }
                        catch { }
                    }

                    return m_sTasks;
                }
            }
        }

        /// <summary>
        /// Add
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool Add(TaskType taskType)
        {
            lock (BusinessEntity.m_sDS)
            {
                bool success = false;
                var attribute = taskType.Attribute();

                if (attribute != null)
                {
                    var dataRow = BusinessEntity.m_sDS.Tasks.AddTasksRow(attribute.Description);

                    try
                    {
                        success = m_sTA.Update(dataRow) == 1;
                    }
                    catch { }

                    if (!success)
                    {
                        BusinessEntity.m_sDS.Tasks.RemoveTasksRow(dataRow);
                    }
                    else
                    {
                        m_sTasks.Add(new Task(dataRow));
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
        public static bool Remove(Task task)
        {
            lock (BusinessEntity.m_sDS)
            {
                bool success = false;

                if (task != null)
                {
                    try
                    {
                        success = (m_sTA.DeleteByID(task.ID) == 1);
                    }
                    catch { }

                    if (success)
                    {
                        m_sTasks.Remove(task);
                        BusinessEntity.m_sDS.Tasks.RemoveTasksRow(task.m_dataRow);
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
        public static Task Find(long id)
        {
            return All.Find(t => t.ID == id);
        }

        /// <summary>
        /// Find
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Task Find(TaskType taskType)
        {
            Task task = null;
            var attribute = taskType.Attribute();

            if (attribute != null)
            {
                task = Find(attribute.Description);
            }

            return task;
        }

        /// <summary>
        /// Find
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Task Find(string name)
        {
            return All.Find(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        #endregion Static

        /// <summary>
        /// Table record
        /// </summary>
        internal DataSet.TasksRow m_dataRow = null;

        /// <summary>
        /// Initialization
        /// </summary>
        /// <param name="dataRow"></param>
        private Task(DataSet.TasksRow dataRow)
        {
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
        /// Task type name
        /// </summary>
        public string Name
        {
            get
            {
                return m_dataRow.Name;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private TaskType m_type = TaskType.None;

        /// <summary>
        /// Type
        /// </summary>
        public TaskType Type
        {
            get
            {
                if (m_type == TaskType.None)
                {
                    foreach (TaskType taskType in Enum.GetValues(typeof(TaskType)))
                    {
                        var attribute = taskType.Attribute();

                        if (attribute != null && Name.Equals(attribute.Description, StringComparison.OrdinalIgnoreCase))
                        {
                            m_type = taskType;
                            break;
                        }
                    }
                }

                return m_type;
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class AllowedTasksAttribute : Attribute
    {
        public AllowedTasksAttribute(params TaskType[] taskTypes)
        {
            TaskTypes = new List<TaskType>(taskTypes);
        }

        public List<TaskType> TaskTypes
        {
            get;
            private set;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class TaskAttribute : Attribute
    {
        /// <summary>
        /// Initialization
        /// </summary>
        /// <param name="taskType"></param>
        /// <param name="taskName"></param>
        /// <param name="description"></param>
        public TaskAttribute(string description)
        {
            Description = description;
        }

        /// <summary>
        /// Task description
        /// </summary>
        public string Description
        {
            get;
            private set;
        }
    }

    public static class TaskExtension
    {
        public static TaskAttribute Attribute(this TaskType taskType)
        {
            TaskAttribute attribute = null;
            FieldInfo fInfo = taskType.GetType().GetField(taskType.ToString());

            if (fInfo != null)
            {
                attribute = fInfo.GetCustomAttributes(typeof(TaskAttribute), true).FirstOrDefault() as TaskAttribute;
            }

            return attribute;
        }
    }
}