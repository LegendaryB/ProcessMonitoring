namespace ProcessMonitoring.Monitors
{
    internal static class WMIProcessConstants
    {
        internal const string Win32ProcessStartTraceQuery = "SELECT * FROM Win32_ProcessStartTrace";
        internal const string Win32ProcessStopTraceQuery = "SELECT * FROM Win32_ProcessStopTrace";

        internal const string ProcessNamePropertyKey = "ProcessName";
        internal const string ProcessIDPropertyKey = "ProcessID";
        internal const string ParentProcessIDPropertyKey = "ParentProcessID";
        internal const string CaptionPropertyKey = "Caption";
        internal const string CommandLinePropertyKey = "CommandLine";
        internal const string ExecutablePathPropertyKey = "ExecutablePath";
        internal const string DescriptionPropertyKey = "Description";

        internal static readonly string Win32ProcessDetailsQueryTemplate = $"SELECT {CaptionPropertyKey}, {CommandLinePropertyKey}, {ExecutablePathPropertyKey}, {DescriptionPropertyKey} FROM Win32_Process WHERE ProcessId = " + "{0}";

    }
}
