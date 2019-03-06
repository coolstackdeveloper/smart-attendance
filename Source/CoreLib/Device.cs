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
    /// Device type
    /// </summary>
    public enum DeviceType 
    { 
        Entry, Exit, Both
    };

    /// <summary>
    /// FP device for an entity
    /// </summary>
    public class Device
    {
        #region Static

        /// <summary>
        /// 
        /// </summary>
        internal static DevicesTableAdapter m_sTA = new DevicesTableAdapter();

        #endregion Static

        /// <summary>
        /// Table record
        /// </summary>
        internal DataSet.DevicesRow m_dataRow = null;

        /// <summary>
        /// Initialization
        /// </summary>
        /// <param name="businessEntity"></param>
        /// <param name="dataRow"></param>
        internal Device(BusinessEntity businessEntity, DataSet.DevicesRow dataRow)
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
        /// Type
        /// </summary>
        public DeviceType Type
        {
            get
            {
                DeviceType type = DeviceType.Both;

                if (!m_dataRow.IsDirectionNull())
                {
                    type = m_dataRow.Direction ? DeviceType.Entry : DeviceType.Exit;
                }

                return type;
            }
        }

        /// <summary>
        /// IP address
        /// </summary>
        public string Address
        {
            get
            {
                return m_dataRow.IPAddress;
            }
        }

        /// <summary>
        /// IP address
        /// </summary>
        public string SubnetMask
        {
            get
            {
                return m_dataRow.SubnetMask;
            }
        }

        /// <summary>
        /// IP address
        /// </summary>
        public string GatewayIP
        {
            get
            {
                return m_dataRow.GatewayIP;
            }
        }

        /// <summary>
        /// IP address
        /// </summary>
        public string MACAddress
        {
            get
            {
                return m_dataRow.MACAddress;
            }
        }
        
        /// <summary>
        /// Associated entity
        /// </summary>
        public BusinessEntity BusinessEntity
        {
            get;
            private set;
        }

        /// <summary>
        /// Device list
        /// </summary>
        private List<DeviceLog> m_deviceLogs = new List<DeviceLog>();

        /// <summary>
        /// List of devices
        /// </summary>
        public List<DeviceLog> DeviceLogs
        {
            get
            {
                lock (BusinessEntity.m_sDS)
                {
                    if (m_deviceLogs.Count == 0)
                    {
                        try
                        {
                            var dataTable = BusinessEntity.m_sDS.DeviceLogs;

                            foreach (var dataRow in DeviceLog.m_sTA.GetDataByDeviceID(ID))
                            {
                                dataTable.ImportRow(dataRow);
                                m_deviceLogs.Add(new DeviceLog(this, dataTable[dataTable.Count - 1]));
                            }
                        }
                        catch { }
                    }

                    return m_deviceLogs;
                }
            }
        }

        /// <summary>
        /// Device list
        /// </summary>
        private List<DeviceFP> m_deviceFPs = new List<DeviceFP>();

        /// <summary>
        /// List of devices
        /// </summary>
        public List<DeviceFP> DeviceFPs
        {
            get
            {
                lock (BusinessEntity.m_sDS)
                {
                    if (m_deviceFPs.Count == 0)
                    {
                        try
                        {
                            var dataTable = BusinessEntity.m_sDS.DeviceFPs;

                            foreach (var dataRow in DeviceFP.m_sTA.GetDataByDeviceID(ID))
                            {
                                dataTable.ImportRow(dataRow);
                                m_deviceFPs.Add(new DeviceFP(this, dataTable[dataTable.Count - 1]));
                            }
                        }
                        catch { }
                    }

                    return m_deviceFPs;
                }
            }
        }

        /// <summary>
        /// Adds a new device
        /// </summary>
        /// <param name="name"></param>
        /// <param name="address"></param>
        /// <returns></returns>
        public bool AddDeviceLog(Employee employee, DateTime checkInOut)
        {
            lock (BusinessEntity.m_sDS)
            {
                bool success = false;

                if (employee != null)
                {
                    var dataRow = BusinessEntity.m_sDS.DeviceLogs.AddDeviceLogsRow(checkInOut, m_dataRow, employee.m_dataRow);

                    try
                    {
                        success = 1 == DeviceLog.m_sTA.Update(dataRow);
                    }
                    catch { }

                    if (success)
                    {
                        m_deviceLogs.Add(new DeviceLog(this, dataRow));
                    }
                    else
                    {
                        BusinessEntity.m_sDS.DeviceLogs.RemoveDeviceLogsRow(dataRow);
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
        public bool RemoveDeviceLog(DeviceLog deviceLog)
        {
            lock (BusinessEntity.m_sDS)
            {
                bool success = false;

                if (deviceLog != null)
                {
                    try
                    {
                        success = 1 == DeviceLog.m_sTA.DeleteByID(deviceLog.ID);
                    }
                    catch { }

                    if (success)
                    {
                        m_deviceLogs.Remove(deviceLog);
                        BusinessEntity.m_sDS.DeviceLogs.RemoveDeviceLogsRow(deviceLog.m_dataRow);
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
        public bool AddDeviceFP(Employee employee, string fpData)
        {
            lock (BusinessEntity.m_sDS)
            {
                bool success = false;

                if (employee != null && !string.IsNullOrEmpty(fpData))
                {
                    var dataRow = BusinessEntity.m_sDS.DeviceFPs.AddDeviceFPsRow(fpData, m_dataRow, employee.m_dataRow);

                    try
                    {
                        success = 1 == DeviceFP.m_sTA.Update(dataRow);
                    }
                    catch { }

                    if (success)
                    {
                        m_deviceFPs.Add(new DeviceFP(this, dataRow));
                    }
                    else
                    {
                        BusinessEntity.m_sDS.DeviceFPs.RemoveDeviceFPsRow(dataRow);
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
        public bool RemoveDeviceFP(DeviceFP deviceFP)
        {
            lock (BusinessEntity.m_sDS)
            {
                bool success = false;

                if (deviceFP != null)
                {
                    try
                    {
                        success = 1 == DeviceFP.m_sTA.DeleteByID(deviceFP.ID);
                    }
                    catch { }

                    if (success)
                    {
                        m_deviceFPs.Remove(deviceFP);
                        BusinessEntity.m_sDS.DeviceFPs.RemoveDeviceFPsRow(deviceFP.m_dataRow);
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
        public bool UpdateDetails(string name, string address, string subnet, string gateway, string mac, DeviceType type)
        {
            lock (BusinessEntity.m_sDS)
            {
                bool success = false;

                try
                {
                    m_dataRow.SetDirectionNull(); // Default to both
                    m_dataRow.Name = name;
                    m_dataRow.IPAddress = address;
                    m_dataRow.SubnetMask = subnet;
                    m_dataRow.GatewayIP = gateway;
                    m_dataRow.MACAddress = mac;

                    if (type != DeviceType.Both)
                    {
                        m_dataRow.Direction = (type == DeviceType.Entry);
                    }

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

    /// <summary>
    /// FP device log for an entity
    /// </summary>
    public class DeviceFP
    {
        #region Static

        /// <summary>
        /// 
        /// </summary>
        internal static DeviceFPsTableAdapter m_sTA = new DeviceFPsTableAdapter();

        #endregion Static

        /// <summary>
        /// Table record
        /// </summary>
        internal DataSet.DeviceFPsRow m_dataRow = null;

        /// <summary>
        /// Initialization
        /// </summary>
        /// <param name="businessEntity"></param>
        /// <param name="dataRow"></param>
        internal DeviceFP(Device device, DataSet.DeviceFPsRow dataRow)
        {
            Device = device;
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
        /// Checkin/out data based on device
        /// </summary>
        public string Data
        {
            get
            {
                return m_dataRow.Data;
            }
        }

        /// <summary>
        /// Employee code
        /// </summary>
        public string EmployeeCode
        {
            get
            {
                return m_dataRow.EmployeeCode;
            }
        }

        /// <summary>
        /// Associated entity
        /// </summary>
        public Device Device
        {
            get;
            private set;
        }
    }

    /// <summary>
    /// FP device log for an entity
    /// </summary>
    public class DeviceLog
    {
        #region Static

        /// <summary>
        /// 
        /// </summary>
        internal static DeviceLogsTableAdapter m_sTA = new DeviceLogsTableAdapter();

        #endregion Static

        /// <summary>
        /// Table record
        /// </summary>
        internal DataSet.DeviceLogsRow m_dataRow = null;

        /// <summary>
        /// Initialization
        /// </summary>
        /// <param name="businessEntity"></param>
        /// <param name="dataRow"></param>
        internal DeviceLog(Device device, DataSet.DeviceLogsRow dataRow)
        {
            Device = device;
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
        /// Checkin/out data based on device
        /// </summary>
        public DateTime Data
        {
            get
            {
                return m_dataRow.Data;
            }
        }

        /// <summary>
        /// Employee code
        /// </summary>
        public string EmployeeCode
        {
            get
            {
                return m_dataRow.EmployeeCode;
            }
        }

        /// <summary>
        /// Associated entity
        /// </summary>
        public Device Device
        {
            get;
            private set;
        }
    }
}