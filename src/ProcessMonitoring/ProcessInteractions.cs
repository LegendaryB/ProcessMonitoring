using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

using Windows.Win32;
using Windows.Win32.Foundation;
using Windows.Win32.System.Diagnostics.ToolHelp;

namespace ProcessMonitoring
{
    internal static class ProcessInteractions
    {
        internal static readonly Version WindowsVersion512600 = Version.Parse("5.1.2600");

        internal static Process? GetProcessByIdOrDefault(int processId)
        {
            var process = Process
                .GetProcesses()
                .FirstOrDefault(p => p.Id == processId);

            return process;
        }

        internal static bool IsWindowsVersionOrAbove(Version windowsVersion)
        {
            return
                RuntimeInformation.IsOSPlatform(OSPlatform.Windows) &&
                Environment.OSVersion.Version >= windowsVersion;
        }

        [SupportedOSPlatform("windows5.1.2600")]
        internal static int GetParentProcessId(int processId)
        {
            try
            {
                var pid = (uint)processId;
                var pe32 = new PROCESSENTRY32
                {
                    dwSize = (uint)Marshal.SizeOf(typeof(PROCESSENTRY32))
                };

                var hSnapshot = PInvoke.CreateToolhelp32Snapshot_SafeHandle(CREATE_TOOLHELP_SNAPSHOT_FLAGS.TH32CS_SNAPPROCESS, pid);

                if (hSnapshot.IsInvalid)
                    return -1;

                if (!PInvoke.Process32First(hSnapshot, ref pe32))
                {
                    var error = (WIN32_ERROR)Marshal.GetLastWin32Error();

                    if (error == WIN32_ERROR.ERROR_NO_MORE_FILES)
                        return -1;
                }

                do
                {
                    if (pe32.th32ProcessID == pid)
                        return (int)pe32.th32ParentProcessID;
                } while (PInvoke.Process32Next(hSnapshot, ref pe32));
            }
            catch { }

            return -1;
        }
    }
}
