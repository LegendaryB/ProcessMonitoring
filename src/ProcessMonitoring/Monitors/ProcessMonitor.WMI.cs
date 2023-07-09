using ProcessMonitoring.Extensions;

using System.Management;
using System.Runtime.Versioning;

namespace ProcessMonitoring.Monitors
{
    [SupportedOSPlatform("windows")]
    internal class ProcessMonitorViaWMI : ProcessMonitor
    {
        private readonly ManagementEventWatcher _processStartTraceWatcher;
        private readonly ManagementEventWatcher _processStopTraceWatcher;

        private readonly ManagementObjectSearcher _processDetailsObjectSearcher;

        public ProcessMonitorViaWMI()
        {
            _processStartTraceWatcher = new ManagementEventWatcher(
                WMIProcessConstants.Win32ProcessStartTraceQuery);

            _processStopTraceWatcher = new ManagementEventWatcher(
                WMIProcessConstants.Win32ProcessStopTraceQuery);

            _processDetailsObjectSearcher = new ManagementObjectSearcher();
        }

        public override Task Start()
        {
            _processStartTraceWatcher.EventArrived += OnProcessStartEventArrived;
            _processStartTraceWatcher?.Start();

            _processStopTraceWatcher.EventArrived += OnProcessStopEventArrived;
            _processStopTraceWatcher?.Start();

            return Task.CompletedTask;
        }

        public override Task Stop()
        {
            _processStartTraceWatcher.EventArrived -= OnProcessStartEventArrived;
            _processStartTraceWatcher?.Stop();

            _processStopTraceWatcher.EventArrived -= OnProcessStopEventArrived;
            _processStopTraceWatcher?.Stop();

            return base.Stop();
        }

        private void OnProcessStartEventArrived(object sender, EventArrivedEventArgs args)
        {
            HandleProcessEvent(args, ProcessEventType.Start);
        }

        private void OnProcessStopEventArrived(object sender, EventArrivedEventArgs args)
        {
            HandleProcessEvent(args, ProcessEventType.Stop);
        }

        private void HandleProcessEvent(EventArrivedEventArgs args, ProcessEventType eventType)
        {
            try
            {
                if (!TryGetProcessEventData(args, out var data) || data == null)
                    return;

                if (data.ProcessID != -1)
                    QueryForProcessDetails(data);

                if (eventType == ProcessEventType.Start)
                    OnProcessStarted(data);
                else
                    OnProcessStopped(data);
            }
            catch { }
        }

        private static bool TryGetProcessEventData(EventArrivedEventArgs args, out ProcessEventData? data)
        {
            data = null;

            var properties = GetEventPropertiesOrDefault(args);

            if (properties == null)
                return false;

            var processName = GetProcessName(properties);

            if (string.IsNullOrWhiteSpace(processName))
                return false;

            data = new ProcessEventData(processName)
            {
                ProcessID = GetProcessID(properties),
                ParentProcessID = GetParentProcessID(properties)
            };

            foreach (var property in properties)
                data.Properties.Add(property.Key, property.Value);

            return true;
        }

        private static Dictionary<string, object>? GetEventPropertiesOrDefault(EventArrivedEventArgs args)
        {
            var properties = args?
                    .NewEvent?
                    .Properties?
                    .ToDictionary();

            return properties == null || properties.Count == 0 ?
                null :
                properties;
        }

        private static string? GetProcessName(Dictionary<string, object> properties)
        {
            return properties.GetTypedValueOrDefault<string>(WMIProcessConstants.ProcessNamePropertyKey);
        }

        private static int GetProcessID(Dictionary<string, object> properties)
        {
            return properties.GetTypedValueOrDefault<int>(WMIProcessConstants.ProcessIDPropertyKey);
        }

        private static int GetParentProcessID(Dictionary<string, object> properties)
        {
            return properties.GetTypedValueOrDefault<int>(WMIProcessConstants.ParentProcessIDPropertyKey);
        }

        private void QueryForProcessDetails(ProcessEventData data)
        {
            _processDetailsObjectSearcher.Query = new ObjectQuery(string.Format(WMIProcessConstants.Win32ProcessDetailsQueryTemplate, data.ProcessID));

            using (var results = _processDetailsObjectSearcher.Get())
            {
                var result = results.OfType<ManagementObject>().FirstOrDefault();

                if (result != null)
                {
                    var properties = result.Properties.ToDictionary();

                    if (properties.TryGetTypedValue<string>(WMIProcessConstants.CaptionPropertyKey, out var caption) && caption != null)
                        data.Properties.Add(WMIProcessConstants.CaptionPropertyKey, caption);

                    if (properties.TryGetTypedValue<string>(WMIProcessConstants.DescriptionPropertyKey, out var description) && description != null)
                        data.Properties.Add(WMIProcessConstants.DescriptionPropertyKey, description);

                    if (properties.TryGetTypedValue<string>(WMIProcessConstants.ExecutablePathPropertyKey, out var executablePath) && executablePath != null)
                        data.ExecutablePath = executablePath;

                    if (properties.TryGetTypedValue<string>(WMIProcessConstants.CommandLinePropertyKey, out var commandLine) && commandLine != null)
                        data.Properties.Add(WMIProcessConstants.CommandLinePropertyKey, commandLine);
                }
            }
        }

        public override async ValueTask DisposeAsync()
        {
            await Stop();
        }
    }
}
