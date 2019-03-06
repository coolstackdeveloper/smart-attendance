using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Utilities
{
    /// <summary>
    /// List with event support
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ListWithEvents<T> : Collection<T>
    {
        /// <summary>
        /// Allowed change types
        /// </summary>
        public enum ChangeType
        {
            Added, Removed, Modified
        };

        /// <summary>
        /// Event triggered
        /// </summary>
        public event Action<ListWithEvents<T>, ChangeEventArgs> Modified;

        /// <summary>
        /// Event arguments
        /// </summary>
        public class ChangeEventArgs : EventArgs
        {
            public T Item;
            public int Index;
            public ChangeType ChangeType;
        }

        /// <summary>
        /// On item added
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        protected override void InsertItem(int index, T item)
        {
            base.InsertItem(index, item);
            NotifyChange(ChangeType.Added, item, index);
        }

        /// <summary>
        /// On item removal
        /// </summary>
        /// <param name="index"></param>
        protected override void RemoveItem(int index)
        {
            T item = this.ElementAtOrDefault(index);

            base.RemoveItem(index);
            NotifyChange(ChangeType.Removed, item, index);
        }

        /// <summary>
        /// On item updated
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        protected override void SetItem(int index, T item)
        {
            base.SetItem(index, item);
            NotifyChange(ChangeType.Modified, item, index);
        }

        /// <summary>
        /// Notify change
        /// </summary>
        /// <param name="changeType"></param>
        /// <param name="item"></param>
        /// <param name="index"></param>
        private void NotifyChange(ChangeType changeType, T item, int index)
        {
            if (Modified != null)
            {
                ChangeEventArgs args = new ChangeEventArgs()
                {
                    Item = item,
                    Index = index,
                    ChangeType = changeType
                };

                Modified(this, args);
            }
        }
    }
}
