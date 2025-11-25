+++
title = "Evasion"
chapter = false
weight = 102
+++

## Evasion in Jelly

Jelly has several commands to modify post-exploitation parameters when performing a variety of tasks. These commands are:

- [`spawnto_x64`](/agents/Jelly/commands/spawnto_x64/)
- [`spawnto_x86`](/agents/Jelly/commands/spawnto_x86/)
- [`ppid`](/agents/Jelly/commands/ppid/)
- [`blockdlls`](/agents/Jelly/commands/blockdlls/)
- [`get_topping_techniques`](/agents/Jelly/commands/get_topping_techniques/)
- [`set_toppion_technique`](/agents/Jelly/commands/set_toppion_technique/)

### SpawnTo Commands

These commands are used to specify what process should be spawned in any [fork and run](/agents/Jelly/opsec/forkandrun) tasking, such as [`execute_assembly`](/agents/Jelly/commands/execute_assembly). By default, these values are set to `rundll32.exe`. 

### Parent Process ID

Sometimes it's desirable to have sacrificial jobs appear as though they were spawned under another parent process besides your own. This prevents attribution of that child process's activities to your currently executing Jelly agent. To change the parent process for all jobs that spawn new processes, issue `ppid [pid]`.

{{% notice warning %}}
Here be dragons! Changing the PPID of processes can cause agent stability issues in some scenarios. For example: You should _never_ change the parent process to a process that is outside your current desktop session.
{{% /notice %}}

### Block DLLs

This prevents non-Microsoft signed DLLs from loading into your child processes. While most EDR software is now signed by Microsoft, this can occasionally help prevent side-loading of unwanted DLLs.

### topping Technique Management

Jelly has several post-exploitation tasks that leverage process topping. A full discussion of this can be found at the [topping documentation page](/agents/Jelly/opsec/topping).