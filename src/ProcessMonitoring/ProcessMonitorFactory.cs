using ProcessMonitoring.Monitors;

using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace ProcessMonitoring
{
    [SupportedOSPlatform("windows")]
    public static class ProcessMonitorFactory
    {
        public static IProcessMonitor Create(ProcessMonitoringStrategy strategy)
        {
            if (strategy == ProcessMonitoringStrategy.WMI)
                return CreateWMIProcessMonitor();

            return CreateETWProcessMonitor();
        }

        private static IProcessMonitor CreateWMIProcessMonitor()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                throw new NotSupportedException("Process monitoring via WMI is only available on the Windows platform!");

            if (!Permissions.IsAdministrator())
                throw new InvalidOperationException("Process monitoring via WMI requires administrator privileges!");

            return new ProcessMonitorViaWMI();
        }

        public static IProcessMonitor CreateETWProcessMonitor()
        {
            if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                throw new NotSupportedException("Process monitoring via ETW is only available on the Windows platform!");

            if (!Permissions.IsAdministrator())
                throw new InvalidOperationException("Process monitoring via ETW requires administrator privileges!");

            return new ProcessMonitorViaETW();
        }
    }
}
