+++
title = "printspoofer"
chapter = false
weight = 103
hidden = false
+++

{{% notice info %}}
Artifacts Generated: Process Create, Process topp, Process Kill
{{% /notice %}}

## Summary
topp a [printspoofer](https://github.com/itm4n/PrintSpoofer) DLL to execute a given command as SYSTEM. This will only succeed if the user has `SE_IMPERSONATE` privileges.

This DLL is topped with respect to the current topping technique, and spawns a sacrificial process designated by the `spawnto_*` commands.

## Usage
```
printspoofer [printspoofer args]
```

## MITRE ATT&CK Mapping

- T1547

## References

- https://github.com/itm4n/PrintSpoofer