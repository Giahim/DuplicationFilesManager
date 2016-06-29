using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace DuplicationFilesManager
{
    public class BindingCollection<T> : BindingList<T>
    {
        protected override bool SupportsSortingCore
        {
            get { return true; }
        }
 
        protected override bool SupportsSearchingCore
        {
            get { return true; }
        }
 
        private bool isSortedCore = true;
        protected override bool IsSortedCore
        {
            get
            {
                return isSortedCore;
            }
        }
 
        private ListSortDirection sortDirectionCore = ListSortDirection.Ascending;
        protected override ListSortDirection SortDirectionCore
        {
            get
            {
                return sortDirectionCore;
            }
        }
 
        private PropertyDescriptor sortPropertyCore = null;
        protected override PropertyDescriptor SortPropertyCore
        {
            get
            {
                return sortPropertyCore;
            }
        }
 
        protected override void ApplySortCore(PropertyDescriptor prop, ListSortDirection direction)
        {
            List<T> items = this.Items as List<T>;
 
            if (items != null)
            {
                ObjectPropertyCompare<T> pc = new ObjectPropertyCompare<T>(prop, direction);
                items.Sort(pc);
                isSortedCore = true;
                sortDirectionCore = direction;
                sortPropertyCore = prop;
            }
            else
            {
                isSortedCore = false;
            }
 
            this.OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }
 
        protected override void RemoveSortCore()
        {
            isSortedCore = false;
            this.OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
        }
    }

    class ObjectPropertyCompare<T> : System.Collections.Generic.IComparer<T>
    {

        private PropertyDescriptor _property;
        public PropertyDescriptor property
        {
            get { return _property; }
            set { _property = value; }
        }



        private ListSortDirection _direction;
        public ListSortDirection direction
        {
            get { return _direction; }
            set { _direction = value; }
        }

        public ObjectPropertyCompare()
        { }

        public ObjectPropertyCompare(PropertyDescriptor prop, ListSortDirection direction)
        {
            _property = prop;
            _direction = direction;
        }

        public int Compare(T x, T y)
        {

            object xValue = x.GetType().GetProperty(property.Name).GetValue(x, null);
            object yValue = y.GetType().GetProperty(property.Name).GetValue(y, null);

            int returnValue;

            if (xValue == null && yValue == null)
            {
                returnValue = 0;
            }
            else if (xValue == null)
            {
                returnValue = -1;
            }
            else if (yValue == null)
            {
                returnValue = 1;
            }
            else if (xValue is IComparable)
            {
                returnValue = ((IComparable)xValue).CompareTo(yValue);
            }
            else if (xValue.Equals(yValue))
            {
                returnValue = 0;
            }
            else
            {
                returnValue = xValue.ToString().CompareTo(yValue.ToString());
            }

            if (direction == ListSortDirection.Ascending)
            {
                return returnValue;
            }
            else
            {
                return returnValue * -1;
            }

        }
    }
}
