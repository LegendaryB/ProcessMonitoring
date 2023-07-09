<div align="center">

[![forthebadge](https://forthebadge.com/images/badges/fuck-it-ship-it.svg)](https://forthebadge.com)
[![forthebadge](https://forthebadge.com/images/badges/made-with-c-sharp.svg)](https://forthebadge.com)

[![GitHub license](https://img.shields.io/github/license/LegendaryB/ProcessMonitoring.svg?longCache=true&style=flat-square)](https://github.com/LegendaryB/ProcessMonitoring/blob/main/LICENSE.txt)

Library to monitor process creation/destruction on Windows powered by C#.
</div><br>

## 🎯 Features
* Monitoring via ETW - requires administrator privileges.
* Monitoring via WMI - requires administrator privileges.

## 📝 Usage

#### Retrieve a `IProcessMonitor` instance from the static `ProcessMonitorFactory`

```csharp
// Possible monitor strategies are: ETW (Event Tracing Windows) and WMI (Windows Management Instrumentation)
var monitor = ProcessMonitorFactory.Create(ProcessMonitoringStrategy.ETW);

// OR
monitor = ProcessMonitorFactory.CreateWMIProcessMonitor();

// OR
monitor = ProcessMonitorFactory.CreateETWProcessMonitor();
```

#### Listening for the `ProcessStart` event
```csharp
monitor.OnProcessStart += MonitorOnProcessStart;
monitor.Start();

private static void Monitor_OnProcessStart(object? sender, ProcessEventData data)
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
```

#### Listening for the `ProcessStop` event
```csharp
monitor.OnProcessStop += MonitorOnProcessStop;
monitor.Start();

private static void Monitor_OnProcessStop(object? sender, ProcessEventData data)
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
```
