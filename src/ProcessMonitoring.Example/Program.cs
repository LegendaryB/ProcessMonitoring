using ProcessMonitoring.Events;

using System.Diagnostics;

namespace ProcessMonitoring
{
    //class ProcessInfo
    //{
    //    public int Id { get; set; }
    //    public string Name { get; set; }
    //}

    //class Program
    //{
    //    static void Main(string[] args)
    //    {
    //var processes = Process.GetProcesses().Select(p => new ProcessInfo
    //{
    //    Name = p.ProcessName,
    //    Id = p.Id
    //}).ToDictionary(p => p.Id);

    //using (var session = new TraceEventSession(KernelTraceEventParser.KernelSessionName))
    //{
    //    session.EnableKernelProvider(KernelTraceEventParser.Keywords.Process);
    //    var parser = session.Source.Kernel;

    //    parser.ProcessStart += e => {
    //        Console.ForegroundColor = ConsoleColor.Green;
    //        Console.WriteLine($"{e.TimeStamp}.{e.TimeStamp.Millisecond:D3}: Process {e.ProcessID} ({e.ProcessName}) Created by {e.ParentID}: {e.CommandLine}");
    //        processes.Add(e.ProcessID, new ProcessInfo { Id = e.ProcessID, Name = e.ProcessName });
    //    };
    //    parser.ProcessStop += e => {
    //        Console.ForegroundColor = ConsoleColor.Red;
    //        Console.WriteLine($"{e.TimeStamp}.{e.TimeStamp.Millisecond:D3}: Process {e.ProcessID} {TryGetProcessName(e)} Exited");
    //    };

    //parser.ImageLoad += e => {
    //    Console.ForegroundColor = ConsoleColor.Yellow;
    //    var name = TryGetProcessName(e);
    //    Console.WriteLine($"{e.TimeStamp}.{e.TimeStamp.Millisecond:D3}: Image Loaded: {e.FileName} into process {e.ProcessID} ({name}) Size=0x{e.ImageSize:X}");
    //};

    //parser.ImageUnload += e => {
    //    Console.ForegroundColor = ConsoleColor.DarkYellow;
    //    var name = TryGetProcessName(e);
    //    Console.WriteLine($"{e.TimeStamp}.{e.TimeStamp.Millisecond:D3}: Image Unloaded: {e.FileName} from process {e.ProcessID} ({name})");
    //};

    //    Task.Run(() => session.Source.Process());
    //    Thread.Sleep(TimeSpan.FromSeconds(60));
    //}

    //string TryGetProcessName(TraceEvent evt)
    //{
    //    if (!string.IsNullOrEmpty(evt.ProcessName))
    //        return evt.ProcessName;
    //    return processes.TryGetValue(evt.ProcessID, out var info) ? info.Name : string.Empty;
    //        //}
    //    }
    //}


    internal class Program
    {
        static async Task Main(string[] args)
        {
            var monitor = ProcessMonitorFactory.Create(ProcessMonitoringStrategy.ETW);
            monitor.OnProcessStart += Monitor_OnProcessStart;
            monitor.OnProcessStop += Monitor_OnProcessStop;

            //await monitor.Stop();
            await monitor.Start();

            Console.ReadLine();
        }

        private static void Monitor_OnProcessStop(object? sender, ProcessEventData data)
        {
            Console.ForegroundColor = ConsoleColor.Red;

            Console.WriteLine(
                $"Process name: {data.ProcessName}\n" +
                $"Caption: {data.Caption}\n" +
                $"Description: {data.Description}\n" +
                $"ExecutablePath: {data.ExecutablePath}\n" +
                $"CommandLine: {data.CommandLine}\n" +
                "===================================================================");

            Console.WriteLine();
        }

        private static void Monitor_OnProcessStart(object? sender, ProcessEventData data)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(
                $"Process name: {data.ProcessName}\n" +
                //$"Parent process name: {Process.GetProcessById(data.ParentProcessId).ProcessName}\n" +
                $"Caption: {data.Caption}\n" +
                $"Description: {data.Description}\n" +
                $"ExecutablePath: {data.ExecutablePath}\n" +
                $"CommandLine: {data.CommandLine}\n" +
                "===================================================================");

            Console.WriteLine();
        }
    }
}