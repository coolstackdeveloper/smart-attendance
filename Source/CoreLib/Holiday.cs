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
    /// Holiday
    /// </summary>
    public class Holiday
    {
        #region Static

        /// <summary>
        /// Adapter
        /// </summary>
        internal static HolidaysTableAdapter m_sTA = new HolidaysTableAdapter();

        #endregion Static

        /// <summary>
        /// Table record
        /// </summary>
        internal DataSet.HolidaysRow m_dataRow = null;

        /// <summary>
        /// Initialization
        /// </summary>
        /// <param name="dataRow">Table row</param>
        internal Holiday(BusinessEntity businessEntity, DataSet.HolidaysRow dataRow)
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
        /// Unique name
        /// </summary>
        public string Name
        {
            get
            {
                return m_dataRow.Name;
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
        /// Start
        /// </summary>
        public DateTime End
        {
            get
            {
                return m_dataRow.End;
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
        /// Update
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public bool UpdateDetails(string name, DateTime start, DateTime end)
        {
            lock (BusinessEntity.m_sDS)
            {
                bool success = false;

                try
                {
                    m_dataRow.Name = name;
                    m_dataRow.Start = start;
                    m_dataRow.End = end;
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