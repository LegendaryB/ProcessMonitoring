namespace ProcessMonitoring.Extensions
{
    internal static class DictionaryExtensions
    {
        internal static T? GetTypedValueOrDefault<T>(this Dictionary<string, object> dictionary, string key)
            where T : IConvertible
        {
            if (!dictionary.TryGetValue(key, out var value))
                return default;

            return (T)Convert.ChangeType(value, typeof(T));
        }

        internal static bool TryGetTypedValue<T>(this Dictionary<string, object> dictionary, string key, out T? value)
            where T : IConvertible
        {
            value = default;

            if (!dictionary.TryGetValue(key, out var propertyValue))
                return false;

            value = (T?)Convert.ChangeType(propertyValue, typeof(T));

            return value != null;
        }
    }
}
