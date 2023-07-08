<div align="center">

[![forthebadge](https://forthebadge.com/images/badges/fuck-it-ship-it.svg)](https://forthebadge.com)
[![forthebadge](https://forthebadge.com/images/badges/made-with-c-sharp.svg)](https://forthebadge.com)

[![GitHub license](https://img.shields.io/github/license/LegendaryB/ProcessMonitoring.svg?longCache=true&style=flat-square)](https://github.com/LegendaryB/ProcessMonitoring/blob/main/LICENSE)

Library to monitor process creation/destruction powered by C#.
</div><br>

## 🎯 Features
* Monitoring via ETW - Windows only, requires administrator privileges.
* Monitoring via WMI - Windows only, requires administrator privileges.
* Monitoring via process list snapshots - should work on windows and linux.

## 📝 Usage

### Retrieve a `IProcessMonitor` instance from the static `ProcessMonitorFactory`

```csharp
// Possible monitor strategies are: ETW, WMI, Snapshots
var monitor = ProcessMonitorFactory.Create(ProcessMonitoringStrategy.ETW);
```

### Listening for the `ProcessStart` event
```csharp
monitor.OnProcessStart += MonitorOnProcessStart;

private static void Monitor_OnProcessStart(object? sender, ProcessEventData data)
{
    Console.WriteLine(
        $"Process name: {data.ProcessName}\n" +
        $"Caption: {data.Caption}\n" +
        $"Description: {data.Description}\n" +
        $"ExecutablePath: {data.ExecutablePath}\n" +
        $"CommandLine: {data.CommandLine}\n");
}
```

### Listening for the `ProcessStop` event
```csharp
monitor.OnProcessStop += MonitorOnProcessStop;

private static void Monitor_OnProcessStop(object? sender, ProcessEventData data)
{
    Console.WriteLine(
        $"Process name: {data.ProcessName}\n" +
        $"Caption: {data.Caption}\n" +
        $"Description: {data.Description}\n" +
        $"ExecutablePath: {data.ExecutablePath}\n" +
        $"CommandLine: {data.CommandLine}\n");
}
```
