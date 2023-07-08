using ProcessMonitoring.Events;
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

        private void OnProcessStopEventArrived(object sender, EventArrivedEventArgs args)
        {
            try
            {
                var data = default(ProcessEventData);

                var properties = args
                    .NewEvent
                    .Properties
                    .ToDictionary();

                var processName = Convert.ToString(properties.GetValueOrDefault(WMIProcessConstants.ProcessNamePropertyKey));

                if (string.IsNullOrWhiteSpace(processName))
                    return;

                var processId = Convert.ToInt32(properties.GetValueOrDefault(WMIProcessConstants.ProcessIdPropertyKey));
                processId = processId == 0 ? -1 : processId;

                var parentProcessId = Convert.ToInt32(properties.GetValueOrDefault(WMIProcessConstants.ParentProcessIdPropertyKey));
                parentProcessId = parentProcessId == 0 ? -1 : parentProcessId;

                var builder = ProcessEventDataBuilder.Create(processName)
                        .SetProperties(properties)
                        .SetProcessId(processId)
                        .SetParentProcessId(parentProcessId);

                if (processId != -1)
                {
                    _processDetailsObjectSearcher.Query = new ObjectQuery(string.Format(WMIProcessConstants.Win32ProcessDetailsQueryTemplate, processId));

                    using (var results = _processDetailsObjectSearcher.Get())
                    {
                        var result = results.OfType<ManagementObject>().FirstOrDefault();

                        if (result != null)
                        {
                            properties = result.Properties.ToDictionary();

                            var caption = Convert.ToString(properties.GetValueOrDefault(WMIProcessConstants.CaptionPropertyKey));
                            var description = Convert.ToString(properties.GetValueOrDefault(WMIProcessConstants.DescriptionPropertyKey));
                            var executablePath = Convert.ToString(properties.GetValueOrDefault(WMIProcessConstants.ExecutablePathPropertyKey));
                            var commandLine = Convert.ToString(properties.GetValueOrDefault(WMIProcessConstants.CommandLinePropertyKey));

                            builder = builder
                                .SetCaption(caption)
                                .SetDescription(description)
                                .SetExecutablePath(executablePath)
                                .SetCommandLine(commandLine);
                        }
                    }
                }

                data = builder.Build();

                OnProcessStopped(data);
            }
            catch { }
        }

        private void OnProcessStartEventArrived(object sender, EventArrivedEventArgs args)
        {
            try
            {
                var data = default(ProcessEventData);

                var properties = args
                    .NewEvent
                    .Properties
                    .ToDictionary();

                var processName = Convert.ToString(properties.GetValueOrDefault(WMIProcessConstants.ProcessNamePropertyKey));

                if (string.IsNullOrWhiteSpace(processName))
                    return;

                var processId = Convert.ToInt32(properties.GetValueOrDefault(WMIProcessConstants.ProcessIdPropertyKey));
                processId = processId == 0 ? -1 : processId;

                var parentProcessId = Convert.ToInt32(properties.GetValueOrDefault(WMIProcessConstants.ParentProcessIdPropertyKey));
                parentProcessId = parentProcessId == 0 ? -1 : parentProcessId;

                var builder = ProcessEventDataBuilder.Create(processName)
                        .SetProperties(properties)
                        .SetProcessId(processId)
                        .SetParentProcessId(parentProcessId);

                if (processId != -1)
                {
                    _processDetailsObjectSearcher.Query = new ObjectQuery(string.Format(WMIProcessConstants.Win32ProcessDetailsQueryTemplate, processId));

                    using (var results = _processDetailsObjectSearcher.Get())
                    {
                        var result = results.OfType<ManagementObject>().FirstOrDefault();

                        if (result != null)
                        {
                            properties = result.Properties.ToDictionary();

                            var caption = Convert.ToString(properties.GetValueOrDefault(WMIProcessConstants.CaptionPropertyKey));
                            var description = Convert.ToString(properties.GetValueOrDefault(WMIProcessConstants.DescriptionPropertyKey));
                            var executablePath = Convert.ToString(properties.GetValueOrDefault(WMIProcessConstants.ExecutablePathPropertyKey));
                            var commandLine = Convert.ToString(properties.GetValueOrDefault(WMIProcessConstants.CommandLinePropertyKey));

                            builder = builder
                                .SetCaption(caption)
                                .SetDescription(description)
                                .SetExecutablePath(executablePath)
                                .SetCommandLine(commandLine);
                        }
                    }
                }

                data = builder.Build();

                OnProcessStarted(data);
            }
            catch { }
        }

        public override ValueTask DisposeAsync()
        {
            throw new NotImplementedException();
        }
    }
}
