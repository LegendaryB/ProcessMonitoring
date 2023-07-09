using Microsoft.Diagnostics.Tracing.Analysis;
using Microsoft.Diagnostics.Tracing.Parsers;
using Microsoft.Diagnostics.Tracing.Session;

using System.Management;

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
            HandleProcessEvent(args, ProcessEventType.Start);
        }

        private void OnProcessStopEvent(TraceProcess args)
        {
            HandleProcessEvent(args, ProcessEventType.Stop);
        }

        private void HandleProcessEvent(TraceProcess args, ProcessEventType eventType)
        {
            try
            {
                if (!TryGetProcessEventData(args, out var data) || data == null)
                    return;

                if (eventType == ProcessEventType.Start)
                    OnProcessStarted(data);
                else
                    OnProcessStopped(data);
            }
            catch { }
        }

        private static bool TryGetProcessEventData(TraceProcess args, out ProcessEventData? data)
        {
            data = null;

            if (!TryGetProcessName(args, out var processName))
                return false;

            var parentProcessId = args.ParentID;

            if (parentProcessId == -1 && IsWindowsVersionOrAbove(WindowsVersion512600))
            {
#pragma warning disable CA1416 // Validate platform compatibility
                parentProcessId = GetParentProcessId(args.ProcessID);
#pragma warning restore CA1416 // Validate platform compatibility
            }

            data = new ProcessEventData(processName)
            {
                ProcessID = args.ProcessID,
                ParentProcessID = parentProcessId
            };

            data.Properties.Add(nameof(ProcessEventData.ProcessName), processName);
            data.Properties.Add(nameof(ProcessEventData.ProcessID), args.ProcessID);
            data.Properties.Add(nameof(ProcessEventData.ParentProcessID), parentProcessId);
            data.Properties.Add(nameof(args.CommandLine), args.CommandLine);
            data.Properties.Add(nameof(args.Is64Bit), args.Is64Bit);

            return true;
        }

        private static bool TryGetProcessName(TraceProcess args, out string processName)
        {
            processName = "";

            if (!string.IsNullOrWhiteSpace(args.Name))
            {
                processName = args.Name;
                return true;
            }

            if (!string.IsNullOrWhiteSpace(args.ImageFileName))
            {
                processName = args.ImageFileName.Replace(".exe", string.Empty);
                return true;
            }

            var process = GetProcessByIdOrDefault(args.ProcessID);

            if (process != null)
            {
                processName = process.ProcessName;
                return true;
            }

            return false;
        }

        public override Task Stop()
        {
            _session.Stop();
            _session.Dispose();

            return base.Stop();
        }

        public override async ValueTask DisposeAsync()
        {
            await Stop();
        }
    }
}