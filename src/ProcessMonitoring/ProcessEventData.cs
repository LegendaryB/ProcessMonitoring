namespace ProcessMonitoring
{
    public class ProcessEventData
    {
        public Dictionary<string, object> Properties { get; private set; }

        public string ProcessName { get; private set; }
        public int ProcessID { get; internal set; } = -1;
        public int ParentProcessID { get; internal set; } = -1;

        public string? ExecutablePath { get; internal set; }

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
