﻿namespace ProcessMonitoring
{
    internal class Program
    {
        static async Task Main()
        {
            var monitor = ProcessMonitorFactory.Create(ProcessMonitoringStrategy.WMI);
            monitor.OnProcessStart += OnProcessStart;
            monitor.OnProcessStop += OnProcessStop;

            //await monitor.Stop();
            await monitor.Start();

            Console.ReadLine();
        }

        private static void OnProcessStart(object? sender, ProcessEventData data)
        {
            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine(
                $"Process name: {data.ProcessName}\n" +
                $"Process id: {data.ProcessID}\n" +
                $"Parent process id: {data.ParentProcessID}\n" +
                $"Executable path: {data.ExecutablePath}\n" +
                "Properties (key, value):");

            foreach (var property in data.Properties)
                Console.WriteLine($"\t{property.Key}, {property.Value}");

            Console.WriteLine("===================================================================");

            Console.WriteLine();
        }

        private static void OnProcessStop(object? sender, ProcessEventData data)
        {
            Console.ForegroundColor = ConsoleColor.Red;

            Console.WriteLine(
                $"Process name: {data.ProcessName}\n" +
                $"Process id: {data.ProcessID}\n" +
                $"Parent process id: {data.ParentProcessID}\n" +
                $"Executable path: {data.ExecutablePath}\n" +
                "Properties (key, value):");

            foreach (var property in data.Properties)
                Console.WriteLine($"\t{property.Key}, {property.Value}");

            Console.WriteLine("===================================================================");

            Console.WriteLine();
        }
    }
}