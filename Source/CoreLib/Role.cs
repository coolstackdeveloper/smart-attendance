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
    /// Role types like contacts etc
    /// </summary>
    public class Role
    {
        #region Static

        /// <summary>
        /// Types
        /// </summary>
        private static List<Role> m_sRoles = new List<Role>();

        /// <summary>
        /// Admin
        /// </summary>
        private static readonly string ADMIN_TAG = "Administrator";

        /// <summary>
        /// Adapter
        /// </summary>
        internal static RolesTableAdapter m_sTA = new RolesTableAdapter();

        /// <summary>
        /// 
        /// </summary>
        public static Role Administrator
        {
            get
            {
                lock (BusinessEntity.m_sDS)
                {
                    return All.Find(r => r.Name.Equals(ADMIN_TAG, StringComparison.OrdinalIgnoreCase));
                }
            }
        }

        /// <summary>
        /// Gets the contact profile
        /// </summary>
        public static List<Role> All
        {
            get
            {
                lock (BusinessEntity.m_sDS)
                {
                    if (m_sRoles.Count == 0)
                    {
                        try
                        {
                            var dataTable = BusinessEntity.m_sDS.Roles;

                            foreach (var dataRow in m_sTA.GetData())
                            {
                                dataTable.ImportRow(dataRow);
                                m_sRoles.Add(new Role(dataTable[dataTable.Count - 1]));
                            }

                            if (m_sRoles.Count == 0)
                            {
                                Add(ADMIN_TAG);
                            }
                        }
                        catch { }
                    }

                    return m_sRoles;
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
                    var dataRow = BusinessEntity.m_sDS.Roles.AddRolesRow(name);

                    try
                    {
                        success = m_sTA.Update(dataRow) == 1;
                    }
                    catch { }

                    if (!success)
                    {
                        BusinessEntity.m_sDS.Roles.RemoveRolesRow(dataRow);
                    }
                    else
                    {
                        m_sRoles.Add(new Role(dataRow));
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
        public static bool Remove(Role role)
        {
            lock (BusinessEntity.m_sDS)
            {
                bool success = false;

                if (role != null && role != Administrator)
                {
                    try
                    {
                        success = (m_sTA.DeleteByID(role.ID) == 1);
                    }
                    catch { }

                    if (success)
                    {
                        m_sRoles.Remove(role);
                        BusinessEntity.m_sDS.Roles.RemoveRolesRow(role.m_dataRow);
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
        public static Role Find(int id)
        {
            return All.Find(t => t.ID == id);
        }

        /// <summary>
        /// Find
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Role Find(string name)
        {
            return All.Find(t => t.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }

        #endregion Static

        /// <summary>
        /// Table record
        /// </summary>
        internal DataSet.RolesRow m_dataRow = null;

        /// <summary>
        /// Initialization
        /// </summary>
        /// <param name="dataRow"></param>
        private Role(DataSet.RolesRow dataRow)
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
        /// Role type name
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
        private List<Task> m_tasks = new List<Task>();

        /// <summary>
        /// Role fields
        /// </summary>
        public List<Task> Tasks
        {
            get
            {
                lock (BusinessEntity.m_sDS)
                {
                    if (m_tasks.Count == 0)
                    {
                        try
                        {
                            var dataTable = BusinessEntity.m_sDS.RoleTasks;

                            foreach (var dataRow in RoleTask.m_sTA.GetDataByRoleID(ID))
                            {
                                dataTable.ImportRow(dataRow);

                                lock (BusinessEntity.m_sDS)
                                {
                                    var task = Task.Find(dataTable[dataTable.Count - 1].TaskID);

                                    if (task != null)
                                    {
                                        m_tasks.Add(task);
                                    }
                                }
                            }
                        }
                        catch { }
                    }

                    return m_tasks;
                }
            }
        }

        /// <summary>
        /// Find
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task FindTask(long id)
        {
            lock (BusinessEntity.m_sDS)
            {
                return m_tasks.Find(f => f.ID == id);
            }
        }

        /// <summary>
        /// Find
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task FindTask(string name)
        {
            lock (BusinessEntity.m_sDS)
            {
                return m_tasks.Find(f => f.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
            }
        }

        /// <summary>
        /// Adds a new field to the profile
        /// </summary>
        /// <param name="label"></param>
        /// <returns></returns>
        public bool AddTask(Task task)
        {
            lock (BusinessEntity.m_sDS)
            {
                bool success = false;

                // If valid task and does not exist for this role...
                if(task != null && FindTask(task.ID) == null)
                {
                    var dataRow = BusinessEntity.m_sDS.RoleTasks.AddRoleTasksRow(m_dataRow, task.m_dataRow);

                    try
                    {
                        success = RoleTask.m_sTA.Update(dataRow) == 1;
                    }
                    catch { }

                    if (success)
                    {
                        m_tasks.Add(task);
                    }
                    else
                    {
                        BusinessEntity.m_sDS.RoleTasks.RemoveRoleTasksRow(dataRow);
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
        public bool RemoveTask(Task task)
        {
            lock (BusinessEntity.m_sDS)
            {
                bool success = false;

                if (task != null)
                {
                    try
                    {
                        success = 1 == RoleTask.m_sTA.DeleteByTaskID(ID, task.ID);
                    }
                    catch { }

                    if (success)
                    {
                        m_tasks.Remove(task);
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

                if (this != Administrator)
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

        public override string ToString()
        {
            return Name;
        }
    }

    /// <summary>
    /// Role field
    /// Predefined types
    /// </summary>
    public class RoleTask
    {
        #region Static

        /// <summary>
        /// Adapter
        /// </summary>
        internal static RoleTasksTableAdapter m_sTA = new RoleTasksTableAdapter();

        #endregion Static

        /// <summary>
        /// Table record
        /// </summary>
        internal DataSet.RoleTasksRow m_dataRow = null;

        /// <summary>
        /// Initialization
        /// </summary>
        /// <param name="dataRow"></param>
        internal RoleTask(Role role, Task task, DataSet.RoleTasksRow dataRow)
        {
            Role = role;
            Task = task;
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
        /// Role type name
        /// </summary>
        public Role Role
        {
            get;
            private set;
        }

        /// <summary>
        /// Type
        /// </summary>
        public Task Task
        {
            get;
            private set;
        }
    }
}