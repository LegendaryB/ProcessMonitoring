using ProcessMonitoring.Events;

namespace ProcessMonitoring
{
    public interface IProcessMonitor : IAsyncDisposable
    {
        event EventHandler<ProcessEventData>? OnProcessStart;
        event EventHandler<ProcessEventData>? OnProcessStop;

        Task Start();
        Task Stop();
    }
}
