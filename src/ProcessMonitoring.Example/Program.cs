using ProcessMonitoring.Events;

namespace ProcessMonitoring
{
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