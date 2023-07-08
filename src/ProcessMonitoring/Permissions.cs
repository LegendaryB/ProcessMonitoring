using System.Runtime.Versioning;
using System.Security.Principal;

namespace ProcessMonitoring
{
    internal static class Permissions
    {
        [SupportedOSPlatform("windows")]
        internal static bool IsAdministrator()
        {
            using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
            {
                var principal = new WindowsPrincipal(identity);
                var hasAdministratorRole = principal.IsInRole(WindowsBuiltInRole.Administrator);

                return hasAdministratorRole;
            }
        }
    }
}
