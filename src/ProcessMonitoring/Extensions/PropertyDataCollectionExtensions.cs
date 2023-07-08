using System.Management;
using System.Runtime.Versioning;

namespace ProcessMonitoring.Extensions
{
    [SupportedOSPlatform("windows")]
    internal static class PropertyDataCollectionExtensions
    {
        internal static Dictionary<string, object> ToDictionary(this PropertyDataCollection propertyDataCollection)
        {
            return propertyDataCollection
                .OfType<PropertyData>()
                .ToDictionary(pd => pd.Name, pd => pd.Value);
        }
    }
}
