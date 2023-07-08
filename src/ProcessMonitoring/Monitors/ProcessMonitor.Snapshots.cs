using ProcessMonitoring.Events;

using System.Diagnostics;

using static ProcessMonitoring.ProcessInteractions;

namespace ProcessMonitoring.Monitors
{
    internal partial class ProcessMonitorViaSnapshots : ProcessMonitor
    {
        private readonly ProcessEqualityComparer _processEqualityComparer;
        private List<Process> _processListSnapshot;

        public ProcessMonitorViaSnapshots()
        {
            _processEqualityComparer = new ProcessEqualityComparer();
            _processListSnapshot = new List<Process>();
        }

        public override async Task Start()
        {
            _processListSnapshot = Process
                .GetProcesses()
                .ToList();

            await RunAsync()
                .ConfigureAwait(false);
        }

        private async Task RunAsync()
        {
            while (!CancellationToken.IsCancellationRequested)
            {
                await Task.WhenAll(
                    UpdateProcessListSnapshot(),
                    Task.Delay(1000, CancellationToken)
                );
            }
        }

        private List<Process> GetProcessListSnapshotDifferences(List<Process> currentProcessListSnapshot)
        {
            var differences = new List<Process> ();

            var stoppedProcesses = _processListSnapshot.Except(
                    currentProcessListSnapshot,
                    _processEqualityComparer);

            var startedProcesses = currentProcessListSnapshot.Except(
                _processListSnapshot,
                _processEqualityComparer);

            differences.AddRange(stoppedProcesses);
            differences.AddRange(startedProcesses);

            return differences;
        }

        private async Task UpdateProcessListSnapshot()
        {
            await Task.Run(() =>
            {
                var currentProcessListSnapshot = Process.GetProcesses().ToList();
                var processListSnapshotDifferences = GetProcessListSnapshotDifferences(currentProcessListSnapshot);

                foreach (var process in processListSnapshotDifferences)
                {
                    var builder = ProcessEventDataBuilder.Create(process.ProcessName)
                            .SetProcessId(process.Id);

                    var parentProcessId = -1;

                    if (IsWindowsVersionOrAbove(WindowsVersion512600))
                    {
#pragma warning disable CA1416 // Validate platform compatibility
                        parentProcessId = GetParentProcessId(process.Id);
#pragma warning restore CA1416 // Validate platform compatibility
                    }

                    builder.SetParentProcessId(parentProcessId);

                    var data = builder.Build();

                    if (process.HasExited)
                        OnProcessStopped(data);
                    else
                        OnProcessStarted(data);
                }

                _processListSnapshot = currentProcessListSnapshot;

            }, CancellationToken);
        }

        public override ValueTask DisposeAsync()
        {
            return ValueTask.CompletedTask;
        }
    }
}
