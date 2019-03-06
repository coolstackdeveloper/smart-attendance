using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

namespace Utilities
{
    /// <summary>
    /// Make sure you make the child class constructor private
    /// </summary>
    /// <typeparam name="T">Generic type for which the singleton class has to be implemented</typeparam>
    public class Singleton<T> where T : class
    {
        /// <summary>
        /// TODO
        /// </summary>
        static Singleton()
        {
            Instance = (T)Activator.CreateInstance(typeof(T), true);
        }

        /// <summary>
        /// Singleton instance
        /// </summary>
        public static T Instance
        {
            get;
            private set;
        }
    }
}