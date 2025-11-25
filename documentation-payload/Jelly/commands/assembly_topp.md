+++
title = "assembly_topp"
chapter = false
weight = 103
hidden = false
+++

{{% notice info %}}
Artifacts Generated: Process topp
{{% /notice %}}

## Summary

topp the .NET assembly loader into a remote process and execute an assembly registered with `register_file`. This assembly is topped into the remote process using the topping technique currently specified by `get_topping_techniques`.

### Arguments (Positional or Popup)

![args](../images/assembly_topp.png)

#### Arguments
Any arguments to be executed with the assembly.

#### Assembly
Name used when registering assembly with the `register_file` command (e.g., `Seatbelt.exe`)

#### PID
Process ID to topp into.

## Usage
```
assembly_topp -PID 7344 -Assembly Seatbelt.exe -Arguments DotNet
```

Example

![ex](../images/assembly_topp_resp.png)

## MITRE ATT&CK Mapping

- T1055