using System;
using System.Web;
using System.Linq;
using System.Web.UI;
using System.Collections;
using System.Reflection;
using System.Globalization;
using System.Threading;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using CoreLib;
using Telerik.Web.UI;

namespace UI
{
    public partial class BasePage : System.Web.UI.Page
    {
        /// <summary>
        /// Error string format
        /// </summary>
        internal const string ERROR_FORMAT = "{0} Please try again or contact the administrator in case the problem persists.";

        /// <summary>
        /// 
        /// </summary>
        protected DateTime m_joiningDate;

        /// <summary>
        /// Holder for most purpose
        /// </summary>
        protected Shift m_shift;

        /// <summary>
        /// Holder for most purpose
        /// </summary>
        protected DateTime m_shiftStart;

        /// <summary>
        /// Holder for most purpose
        /// </summary>
        protected DateTime m_shiftEnd;

        /// <summary>
        /// Holder for most purpose
        /// </summary>
        protected DateTime m_shiftPunchInStart;

        /// <summary>
        /// Holder for most purpose
        /// </summary>
        protected DateTime m_shiftPunchInEnd;

        /// <summary>
        /// Holder for most purpose
        /// </summary>
        protected DateTime m_shiftPunchOutStart;

        /// <summary>
        /// Holder for most purpose
        /// </summary>
        protected DateTime m_shiftPunchOutEnd;

        /// <summary>
        /// Holder for most purpose
        /// </summary>
        protected Holiday m_holiday;

        /// <summary>
        /// 
        /// </summary>
        protected DateTime m_holidayEndDate;

        /// <summary>
        /// 
        /// </summary>
        protected DateTime m_holidayStartDate;

        /// <summary>
        /// Holder for most purpose
        /// </summary>
        protected Role m_role;

        /// <summary>
        /// 
        /// </summary>
        protected LeaveType m_leaveType;

        /// <summary>
        /// 
        /// </summary>
        protected EmployeeType m_employeeType;

        /// <summary>
        /// Holder for most purpose
        /// </summary>
        protected Department m_department;

        /// <summary>
        /// Holder for most purpose
        /// </summary>
        protected Designation m_designation;

        /// <summary>
        /// Holder for most purpose
        /// </summary>
        protected BusinessEntity m_businessEntity;

        /// <summary>
        /// 
        /// </summary>
        protected Device m_device;

        /// <summary>
        /// Holder for most purpose
        /// </summary>
        protected BusinessEntityContactDetail m_contactDetail;

        /// <summary>
        /// Holder for most purpose
        /// </summary>
        protected Profile m_profile;

        /// <summary>
        /// Holder for most purpose
        /// </summary>
        protected ProfileField m_profileField;

        /// <summary>
        /// 
        /// </summary>
        protected DateTime m_end;

        /// <summary>
        /// 
        /// </summary>
        protected DateTime m_start;

        /// <summary>
        /// 
        /// </summary>
        protected AttendanceException m_attendanceException;

        /// <summary>
        /// Holder for most purpose
        /// </summary>
        protected Employee m_employee;

        /// <summary>
        /// 
        /// </summary>
        protected AttendanceExceptionType m_attendanceExceptionType;

        /// <summary>
        /// 
        /// </summary>
        protected ShiftExceptionType m_shiftExceptionType;

        /// <summary>
        /// 
        /// </summary>
        protected ShiftException m_shiftException;

        protected void DisplayError(string text, Label label)
        {
            label.Text = string.IsNullOrEmpty(text) ? "" : string.Format(ERROR_FORMAT, text);
        }

        protected int Int(object value)
        {
            int id = -1;
            int.TryParse(String(value), out id);
            return id;
        }

        protected long Long(object value)
        {
            long id = -1;
            long.TryParse(String(value), out id);
            return id;
        }

        protected bool Bool(object value)
        {
            bool id = false;
            bool.TryParse(String(value), out id);
            return id;
        }

        protected DateTime DateTime(object value)
        {
            DateTime id = default(DateTime);
            System.DateTime.TryParse(String(value), out id);
            return id;
        }

        protected TimeSpan TimeSpan(DateTime value)
        {
            return new TimeSpan(value.Hour, value.Minute, value.Second);
        }

        protected string String(object value)
        {
            return value == null ? string.Empty : value.ToString();
        }

        protected Role Role(object value)
        {
            int id = -1;
            int.TryParse(value == null ? "-1" : value.ToString(), out id);
            return CoreLib.Role.Find(id);
        }

        protected Task Task(object value)
        {
            int id = -1;
            int.TryParse(value == null ? "-1" : value.ToString(), out id);
            return CoreLib.Task.Find(id);
        }

        protected Profile Profile(object value)
        {
            int id = -1;
            int.TryParse(value == null ? "-1" : value.ToString(), out id);
            return CoreLib.Profile.Find(id);
        }

        protected ProfileField ProfileField(object value, Profile profile)
        {
            int id = -1;
            int.TryParse(value == null ? "-1" : value.ToString(), out id);
            return (profile == null) ? null : profile.FindField(id);
        }

        protected AttendanceExceptionType AttendanceExceptionType(object value)
        {
            int id = -1;
            int.TryParse(value == null ? "-1" : value.ToString(), out id);
            return CoreLib.AttendanceExceptionType.Find(id);
        }

        protected ShiftExceptionType ShiftExceptionType(object value)
        {
            int id = -1;
            int.TryParse(value == null ? "-1" : value.ToString(), out id);
            return CoreLib.ShiftExceptionType.Find(id);
        }

        protected Department Department(object value)
        {
            int id = -1;
            int.TryParse(value == null ? "-1" : value.ToString(), out id);
            return CoreLib.Department.Find(id);
        }

        protected LeaveType LeaveType(object value)
        {
            int id = -1;
            int.TryParse(value == null ? "-1" : value.ToString(), out id);
            return CoreLib.LeaveType.Find(id);
        }

        protected EmployeeType EmployeeType(object value)
        {
            int id = -1;
            int.TryParse(value == null ? "-1" : value.ToString(), out id);
            return CoreLib.EmployeeType.Find(id);
        }

        protected Designation Designation(object value)
        {
            int id = -1;
            int.TryParse(value == null ? "-1" : value.ToString(), out id);
            return CoreLib.Designation.Find(id);
        }

        protected BusinessEntity Entity(object value)
        {
            int id = -1;
            int.TryParse(value == null ? "-1" : value.ToString(), out id);
            return BusinessEntity.Find(id);
        }

        protected Holiday Holiday(object value, BusinessEntity businessEntity)
        {
            int id = -1;
            int.TryParse(value == null ? "-1" : value.ToString(), out id);
            return (businessEntity == null) ? null : businessEntity.FindHoliday(id);
        }

        protected ShiftException ShiftException(object value, Shift shift)
        {
            int id = -1;
            int.TryParse(value == null ? "-1" : value.ToString(), out id);
            return (shift == null) ? null : shift.FindException(id);
        }

        protected AttendanceException AttendanceException(object value, Employee employee)
        {
            int id = -1;
            int.TryParse(value == null ? "-1" : value.ToString(), out id);
            return (employee == null) ? null : employee.FindAttendanceException(id);
        }

        protected Device Device(object value, BusinessEntity businessEntity)
        {
            int id = -1;
            int.TryParse(value == null ? "-1" : value.ToString(), out id);
            return (businessEntity == null) ? null : businessEntity.FindDevice(id);
        }

        protected Employee Employee(object value, BusinessEntity businessEntity)
        {
            int id = -1;
            int.TryParse(value == null ? "-1" : value.ToString(), out id);
            return (businessEntity == null) ? null : businessEntity.FindEmployee(id);
        }

        protected Shift Shift(object value, BusinessEntity businessEntity)
        {
            int id = -1;
            int.TryParse(value == null ? "-1" : value.ToString(), out id);
            return (businessEntity == null) ? null : businessEntity.FindShift(id);
        }

        protected BusinessEntityContactDetail ContactDetail(object value, BusinessEntity businessEntity)
        {
            int id = -1;
            int.TryParse(value == null ? "-1" : value.ToString(), out id);
            return (businessEntity == null) ? null : businessEntity.FindContactDetail(id);
        }

        protected EmployeeContactDetail ContactDetail(object value, Employee employee)
        {
            int id = -1;
            int.TryParse(value == null ? "-1" : value.ToString(), out id);
            return (employee == null) ? null : employee.FindContactDetail(id);
        }

        protected List<TaskType> AllowedTasks()
        {
            List<TaskType> types = new List<TaskType>();
            AllowedTasksAttribute result = GetType().GetCustomAttributes(typeof(AllowedTasksAttribute), true).FirstOrDefault() as AllowedTasksAttribute;

            if (result != null)
            {
                types.AddRange(result.TaskTypes);
            }

            return types;
        }

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            var authorized = true;
            var allowedTasks = AllowedTasks();

            if (BusinessEntity.Initialized)
            {
                if (Global.CurrentUser == null)
                {
                    Response.Redirect("Login.aspx");
                }
                else
                {
                    authorized = Global.CurrentUser.Role == CoreLib.Role.Administrator || Global.CurrentUser.Role.Tasks.Exists(c => allowedTasks.Contains(c.Type));
                }
            }
            else
            {
                if (Global.CurrentUser != null)
                {
                    Global.CurrentUser = null;
                    Response.Redirect("Login.aspx");
                }
            }

            var errorMessage = string.Format(ERROR_FORMAT, "You are not authorized to view this page.");
            var lblError = Page.Master.FindControl("m_lblError") as Label;
            var placeHolder = Page.Master.FindControl("m_cpChild") as ContentPlaceHolder;

            if (!authorized)
            {
                placeHolder.Visible = false;
                lblError.Visible = true;
                lblError.Text = errorMessage;
            }
        }

        /// <summary>
        /// TODO
        /// </summary>
        private bool? m_isSupported = null;

        protected bool IsSupported(params TaskType[] taskTypes)
        {
            if (!m_isSupported.HasValue)
            {
                bool supported = true;

                if (BusinessEntity.Initialized)
                {
                    if (Global.CurrentUser == null)
                    {
                        Response.Redirect("Login.aspx");
                    }
                    else if (Global.CurrentUser.Role != CoreLib.Role.Administrator)
                    {
                        supported = Global.CurrentUser.Role.Tasks.Any(t => taskTypes.Contains(t.Type));
                    }
                }
                else
                {
                    if (Global.CurrentUser != null)
                    {
                        Global.CurrentUser = null;
                        Response.Redirect("Login.aspx");
                    }
                }

                m_isSupported = supported;
            }

            return m_isSupported.Value;
        }
    }
}