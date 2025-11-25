+++
title = "psinject"
chapter = false
weight = 103
hidden = false
+++
 
{{% notice info %}}
Artifacts Generated: Process topp
{{% /notice %}}

## Summary
Execute PowerShell commands in a remote process. Leverages the currently set topping technique to topp the PowerShell loader.

### Arguments (Positional or Popup)
#### PID
Target process ID.

#### Command
PowerShell command to be executed.

## Usage
```
psinject -PID [pid] -Command [command]
```

Example
```
psinject -PID 1234 -Command Get-Process
```


## MITRE ATT&CK Mapping

- T1059
- T1055
