using System.Reflection;

namespace EcommerceWebApi.Utilities
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SearchableAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Property)]
    public class SortableAttribute : Attribute { }

    [AttributeUsage(AttributeTargets.Property)]
    public class UpdatableAttribute : Attribute { }

    public class QueryFilter
    {
        public string? SearchBy { get; set; } = null!;

        public string? Search { get; set; } = null!;

        public string? SortBy { get; set; } = null!;

        public bool IsSortAscending { get; set; }
    }

    public static class QueryHelper
    {
        public static IEnumerable<T> SearchObjects<T>(
            IEnumerable<T> objects,
            string? searchBy,
            string? search,
            StringComparison comparisonType
        )
        {
            if (string.IsNullOrEmpty(searchBy) || string.IsNullOrEmpty(search))
            {
                return objects;
            }

            // Get the property info using reflection
            var propertyInfo = typeof(T).GetProperty(
                searchBy,
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance
            );

            // Check if the property has the Searchable attribute
            if (
                propertyInfo == null
                || !propertyInfo.GetCustomAttributes(typeof(SearchableAttribute), false).Any()
            )
            {
                return objects;
            }

            // Search the objects
            return objects.Where(obj =>
            {
                var propertyValue = propertyInfo.GetValue(obj);
                if (propertyValue == null)
                {
                    return false;
                }
                var propertyValueString = propertyValue.ToString();
                return propertyValueString != null
                    && propertyValueString.Contains(search, comparisonType);
            });
        }

        public static IEnumerable<T> SortObjects<T>(
            IEnumerable<T> objects,
            string? sortBy,
            bool isSortAscending
        )
        {
            if (string.IsNullOrEmpty(sortBy))
            {
                return objects;
            }

            // Get the property info using reflection
            var propertyInfo = typeof(T).GetProperty(
                sortBy,
                BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance
            );

            // Check if the property has the Sortable attribute
            if (
                propertyInfo == null
                || !propertyInfo.GetCustomAttributes(typeof(SortableAttribute), false).Any()
            )
            {
                return objects;
            }

            // Sort the objects based on the property value
            return isSortAscending
                ? objects.OrderBy(p => propertyInfo.GetValue(p))
                : objects.OrderByDescending(p => propertyInfo.GetValue(p));
        }
    }
}
