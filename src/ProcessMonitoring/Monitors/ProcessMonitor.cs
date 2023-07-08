using ProcessMonitoring.Events;

namespace ProcessMonitoring.Monitors
{
    internal abstract class ProcessMonitor : IProcessMonitor
    {
        public event EventHandler<ProcessEventData>? OnProcessStart;
        public event EventHandler<ProcessEventData>? OnProcessStop;

        private readonly CancellationTokenSource _cancellationTokenSource;

        protected CancellationToken CancellationToken;

        protected ProcessMonitor() 
        { 
            _cancellationTokenSource = new CancellationTokenSource();
        }

        public abstract Task Start();

        public virtual Task Stop()
        {
            _cancellationTokenSource.Cancel();

            return Task.CompletedTask;
        }

        protected virtual void OnProcessStarted(ProcessEventData eventData)
        {
            OnProcessStart?.Invoke(this, eventData);
        }

        protected virtual void OnProcessStopped(ProcessEventData eventData)
        {
            OnProcessStop?.Invoke(this, eventData);
        }

        public abstract ValueTask DisposeAsync();
    }
}
