namespace ProcessMonitoring.Events
{
    public class ProcessEventData
    {
        public Dictionary<string, object> Properties { get; internal set; }

        public string ProcessName { get; internal set; }
        public int ProcessID { get; internal set; } = -1;
        public int ParentProcessID { get; internal set; } = -1;
        public string? Caption { get; internal set; }
        public string? Description { get; internal set; }
        public string? ExecutablePath { get; internal set; }
        public string? CommandLine { get; internal set; }

        internal ProcessEventData(string processName) 
        {
            ProcessName = processName;
            Properties = new Dictionary<string, object>();
        }

        public override string ToString()
        {
            return $"Process name: {ProcessName}, Process ID: {ProcessID}";
        }
    }
}
