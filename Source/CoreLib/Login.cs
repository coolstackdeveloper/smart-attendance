using System;
using System.Linq;
using System.Text;
using System.Security;
using System.Data.Linq;
using System.Web.Security;
using System.Security.Principal;
using System.Collections.Generic;

using Utilities;
using DataAccess;
using DataAccess.DataSetTableAdapters;

namespace CoreLib
{
    /// <summary>
    /// Employee login
    /// </summary>
    public class Login
    {
        #region Static

        /// <summary>
        /// Adapter
        /// </summary>
        internal static LoginsTableAdapter m_sTA = new LoginsTableAdapter();

        /// <summary>
        /// Adapter
        /// </summary>
        internal static LoginViewTableAdapter m_sTAView = new LoginViewTableAdapter();

        /// <summary>
        /// 
        /// </summary>
        internal static QueriesTableAdapter m_sTAQuesries = new QueriesTableAdapter();

        /// <summary>
        /// Authenticate
        /// </summary>
        /// <param name="name"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static Login Authenticate(string name, string password)
        {
            lock (BusinessEntity.m_sDS)
            {
                Login result = null;

                try
                {
                    password = string.IsNullOrEmpty(password) ? "" : password;
                    var dataTable = Login.m_sTAView.GetDataByName(name, password.Encrypt());

                    if (dataTable.Count > 0)
                    {
                        var dataRow = dataTable[0];
                        BusinessEntity entity = BusinessEntity.Find(dataRow.BusinessEntityID);
                        Employee employee = entity.FindEmployee(dataRow.EmployeeID);
                        result = employee.Login;
                    }
                }
                catch { }

                return result;
            }
        }

        /// <summary>
        /// Authenticate
        /// </summary>
        /// <param name="name"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public static bool ChangeCredential(string name, string existingPassword, string newPassword)
        {
            lock (BusinessEntity.m_sDS)
            {
                bool success = false;

                try
                {
                    if (!string.IsNullOrEmpty(newPassword) && !string.IsNullOrEmpty(existingPassword))
                    {
                        var dataTable = Login.m_sTAView.GetDataByName(name, existingPassword.Encrypt());

                        if (dataTable.Count > 0)
                        {
                            success = 1 == Login.m_sTA.ChangePassword(newPassword.Encrypt(), name);
                        }
                    }
                }
                catch { }

                return success;
            }
        }

        #endregion Static

        /// <summary>
        /// Record
        /// </summary>
        internal DataSet.LoginsRow m_dataRow = null;

        /// <summary>
        /// Initialization
        /// </summary>
        /// <param name="dataRow"></param>
        internal Login(Employee employee, DataSet.LoginsRow dataRow)
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
        /// Name
        /// </summary>
        public string Password
        {
            get
            {
                return m_dataRow.Password.Decrypt();
            }
        }

        /// <summary>
        /// Associated employee
        /// </summary>
        public Employee Employee
        {
            get;
            private set;
        }

        /// <summary>
        /// Role
        /// </summary>
        public Role Role
        {
            get
            {
                lock (BusinessEntity.m_sDS)
                {
                    return Role.Find(m_dataRow.RoleID);
                }
            }
        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="newName"></param>
        /// <returns></returns>
        public bool UpdateDetails(string newName, Role role, string password)
        {
            lock (BusinessEntity.m_sDS)
            {
                bool success = false;

                if (!string.IsNullOrEmpty(newName) && !string.IsNullOrEmpty(password) && role != null)
                {
                    try
                    {
                        m_dataRow.Name = newName;
                        m_dataRow.RoleID = role.ID;
                        m_dataRow.Password = password.Encrypt();

                        success = 1 == m_sTA.Update(m_dataRow);

                        // Check if any row in the table already has admin role...
                        if (BusinessEntity.m_sDS.Logins.Count(r => r.RoleID == Role.Administrator.ID) == 0)
                        {
                            BusinessEntity.Initialized = false;
                        }
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