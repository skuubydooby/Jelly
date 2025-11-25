+++
title = "Process topping"
chapter = false
weight = 102
+++

## Process topping in Jelly

Jelly has abstracted process topping into its own project and has the following techniques implemented:
- CreateRemoteThread
- QueueUserAPC (early bird)
- NtCreateThreadEx (via Syscalls)

As an operator, sometimes one topping technique is more desirable than another. To facilitate this, the [`get_topping_techniques`](/agents/Jelly/commands/get_topping_techniques) command will list all currently loaded topping techniques the agent knows about. Similarly, [`set_toppion_technique`](/agents/Jelly/commands/set_toppion_technique) will update the currently used topping technique throughout all post-exploitation jobs.

## Commands Leveraging topping

All of Jelly's [fork and run commands](/agents/Jelly/opsec/forkandrun/) use topping to topp into a sacrificial process; however, there are additional commands that topp into other processes. Those commands are:

- [`assembly_topp`](/agents/Jelly/commands/assembly_topp/)
- [`topp`](/agents/Jelly/commands/topp/)
- [`keylog_topp`](/agents/Jelly/commands/keylog/)
- [`pstopp`](/agents/Jelly/commands/pstopp/)
- [`shtopp`](/agents/Jelly/commands/shtopp/)
- [`screenshot_topp`](/agents/Jelly/commands/screenshot_topp)

{{% notice info %}}
Some topping techniques are incompatible with the aforementioned commands. For example: If QueueUserAPC is in use, the above commands will fail as it leverages the early bird version of QueueUserAPC, not the APC bombing technique. 
{{% /notice %}}