using Microsoft.Diagnostics.Tracing.Analysis;
using Microsoft.Diagnostics.Tracing.Parsers;
using Microsoft.Diagnostics.Tracing.Session;

using ProcessMonitoring.Events;

using static ProcessMonitoring.ProcessInteractions;

namespace ProcessMonitoring.Monitors
{
    internal class ProcessMonitorViaETW : ProcessMonitor
    {
        private readonly TraceEventSession _session;

        public ProcessMonitorViaETW()
        {
            _session = new TraceEventSession(KernelTraceEventParser.KernelSessionName);
        }

        public override async Task Start()
        {
            _session.EnableKernelProvider(KernelTraceEventParser.Keywords.Process);
            _session.Source.NeedLoadedDotNetRuntimes();

            _session.Source.AddCallbackOnProcessStart(OnProcessStartEvent);
            _session.Source.AddCallbackOnProcessStop(OnProcessStopEvent);

            while (!CancellationToken.IsCancellationRequested)
            {
                _session.Source.Process();

                await Task.Delay(1000);
            }
        }

        private void OnProcessStartEvent(TraceProcess args)
        {
            var process = GetProcessByIdOrDefault(args.ProcessID);

            if (process == null)
                return;

            var parentProcessId = args.ParentID;

            if (parentProcessId == -1 && IsWindowsVersionOrAbove(WindowsVersion512600))
            {
#pragma warning disable CA1416 // Validate platform compatibility
                parentProcessId = GetParentProcessId(args.ProcessID);
#pragma warning restore CA1416 // Validate platform compatibility
            }

            var properties = new Dictionary<string, object>
            {
                { nameof(ProcessEventData.ProcessName), process.ProcessName },
                { nameof(ProcessEventData.ProcessID), args.ProcessID },
                { nameof(ProcessEventData.ParentProcessID), parentProcessId },
                { nameof(args.Is64Bit), args.Is64Bit }
            };

            var data = ProcessEventDataBuilder
                .Create(process.ProcessName)
                .SetProcessId(args.ProcessID)
                .SetParentProcessId(parentProcessId)
                .SetCommandLine(args.CommandLine)
                .SetProperties(properties)
                .Build();

            OnProcessStarted(data);
        }

        private void OnProcessStopEvent(TraceProcess args)
        {
            var parentProcessId = args.ParentID;

            if (parentProcessId == -1 && IsWindowsVersionOrAbove(WindowsVersion512600))
            {
#pragma warning disable CA1416 // Validate platform compatibility
                parentProcessId = GetParentProcessId(args.ProcessID);
#pragma warning restore CA1416 // Validate platform compatibility
            }

            var properties = new Dictionary<string, object>
            {
                { nameof(ProcessEventData.ProcessName), args.Name },
                { nameof(ProcessEventData.ProcessID), args.ProcessID },
                { nameof(ProcessEventData.ParentProcessID), parentProcessId },
                { nameof(args.Is64Bit), args.Is64Bit }
            };

            var data = ProcessEventDataBuilder
                .Create(args.Name)
                .SetProcessId(args.ProcessID)
                .SetParentProcessId(parentProcessId)
                .SetCommandLine(args.CommandLine)
                .SetProperties(properties)
                .Build();

            OnProcessStopped(data);
        }

        public override Task Stop()
        {
            _session.Stop();
            _session.Dispose();

            return base.Stop();
        }

        public override ValueTask DisposeAsync()
        {
            return ValueTask.CompletedTask;
        }
    }
}
