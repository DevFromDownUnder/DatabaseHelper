using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DatabaseHelper.Extensions
{
    public static class EnumerableExtensions
    {
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> source)
        {
            if (source == null)
            {
                return new ObservableCollection<T>();
            }

            return new ObservableCollection<T>(source);
        }
    }
}