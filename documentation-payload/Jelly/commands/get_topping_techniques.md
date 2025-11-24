+++
title = "get_topping_techniques"
chapter = false
weight = 103
hidden = false
+++

## Summary
Retrieve a list of available topping techniques the agent can use.

## Usage
```
get_topping_techniques
```

## Detailed Summary
The `get_topping_techniques` command displays the various process topping techniques the agent is capable of using for post-exploitation jobs. You can see the current technique being used by an agent with the [`get_topping_techniques`](/agents/Jelly/commands/get_topping_techniques/) command. The technique can also be changed using the [`set_injection_technique`](/agents/Jelly/commands/set_injection_technique/) command.

You are encouraged to create your own topping technique and submit a new pull request!

### Available techniques

#### CreateRemoteThread
"Classic" process topping technique that uses the `VirtualAllocEx`, `WriteProcessMemory` and `CreateRemoteThread` Windows APIs to execute shellcode in a specified process.

#### Early-Bird QueueUserAPC
Works for all jobs spawning sacrificial processes, but mileage may vary for topping-type commands. Calls `VirtualAllocEx`, `WriteProcessMemory`, `QueueUserAPC` and `ResumeThread` calls.

#### NtCreateThreadEx
Leverages syscalls from the NTDLL library to directly invoke shellcode associated with `NtOpenProcess`, `NtClose`, `NtDuplicateObject`, `NtAllocateVirtualMemory`, `NtProtectVirtualMemory`, `NtWriteVirtualMemory`, and `NtCreateThreadEx`


![get_topping_techniques](../images/get_topping_techniques.png)