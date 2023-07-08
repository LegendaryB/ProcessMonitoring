namespace ProcessMonitoring.Events
{
    internal class ProcessEventDataBuilder
    {
        private readonly ProcessEventData _data;

        private ProcessEventDataBuilder(string processName)
        {
            _data = new ProcessEventData(processName);
        }

        public static ProcessEventDataBuilder Create(string processName)
        {
            return new ProcessEventDataBuilder(processName);
        }

        public ProcessEventDataBuilder SetProperties(Dictionary<string, object> properties)
        {
            _data.Properties = properties ?? throw new ArgumentNullException(nameof(properties));
            return this;
        }

        public ProcessEventDataBuilder SetProcessId(int value)
        {
            _data.ProcessID = value;
            return this;
        }

        public ProcessEventDataBuilder SetCaption(string? value)
        {
            _data.Caption = value;
            return this;
        }

        public ProcessEventDataBuilder SetDescription(string? value)
        {
            _data.Description = value;
            return this;
        }

        public ProcessEventDataBuilder SetExecutablePath(string? value)
        {
            _data.ExecutablePath = value;
            return this;
        }

        public ProcessEventDataBuilder SetParentProcessId(int value)
        {
            _data.ParentProcessID = value;
            return this;
        }

        public ProcessEventDataBuilder SetCommandLine(string? value)
        {
            _data.CommandLine = value;
            return this;
        }

        public ProcessEventData Build()
        {
            return _data;
        }
    }
}
