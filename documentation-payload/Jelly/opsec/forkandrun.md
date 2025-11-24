+++
title = "Fork and Run Commands"
chapter = false
weight = 102
+++

## What is Fork and Run?

"Fork and Run" is an agent architecture that spawns sacrificial processes in a suspended state to inject shellcode into.

## Fork and Run in Jelly

Jelly uses the fork and run architecture for a variety of jobs. These jobs will all first spawn a new process specified by the [`spawnto_x86`](/agents/Jelly/commands/spawnto_x86) or [`spawnto_x64`](/agents/Jelly/commands/spawnto_x64) commands. The parent process of these new processes is specified by the [`ppid`](/agents/Jelly/commands/ppid/) command. Once the process is spawned, Jelly will use the currently set topping technique to inject into the remote process.

The following commands use the fork and run architecture:

- [`execute_assembly`](/agents/Jelly/commands/execute_assembly/)
- [`mimikatz`](/agents/Jelly/commands/mimikatz/)
- [`powerpick`](/agents/Jelly/commands/powerpick/)
- [`printspoofer`](/agents/Jelly/commands/printspoofer/)
- [`pth`](/agents/Jelly/commands/pth/)
- [`dcsync`](/agents/Jelly/commands/pth/)
- [`spawn`](/agents/Jelly/commands/spawn/)
- [`execute_pe`](/agents/Jelly/commands/execute_pe/)