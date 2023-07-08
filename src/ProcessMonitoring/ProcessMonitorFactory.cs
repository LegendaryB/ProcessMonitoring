using ProcessMonitoring.Monitors;

using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace ProcessMonitoring
{
    public static class ProcessMonitorFactory
    {
        public static IProcessMonitor Create(ProcessMonitoringStrategy strategy)
        {
            if (strategy == ProcessMonitoringStrategy.WMI)
            {
                if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    throw new NotSupportedException("Process monitoring via WMI is only available on the Windows platform!");

                if (!Permissions.IsAdministrator())
                    throw new InvalidOperationException("Process monitoring via WMI requires administrator privileges!");
            }

            if (strategy == ProcessMonitoringStrategy.ETW)
            {
                if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                    throw new NotSupportedException("Process monitoring via ETW is only available on the Windows platform!");

                if (!Permissions.IsAdministrator())
                    throw new InvalidOperationException("Process monitoring via ETW requires administrator privileges!");
            }

            switch (strategy)
            {
                case ProcessMonitoringStrategy.WMI:
                    return CreateWMIProcessMonitor();
                case ProcessMonitoringStrategy.ETW:
                    return CreateETWProcessMonitor();
                case ProcessMonitoringStrategy.Snapshots:
                    return CreateSnapshotProcessMonitor();
                default:
                    throw new ArgumentException(null, nameof(strategy));
            }
        }

        [SupportedOSPlatform("windows")]
        private static IProcessMonitor CreateETWProcessMonitor()
        {
            return new ProcessMonitorViaETW();
        }

        private static IProcessMonitor CreateSnapshotProcessMonitor()
        {
            return new ProcessMonitorViaSnapshots();
        }

        [SupportedOSPlatform("windows")]
        private static IProcessMonitor CreateWMIProcessMonitor()
        {
            return new ProcessMonitorViaWMI();
        }
    }
}
