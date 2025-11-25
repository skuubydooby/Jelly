from mythic_container.MythicCommandBase import *
import json


class SettoppionTechniqueArguments(TaskArguments):

    def __init__(self, command_line, **kwargs):
        super().__init__(command_line, **kwargs)
        self.args = []

    async def parse_arguments(self):
        if len(self.command_line.strip()) == 0:
            raise Exception("set_toppion_technique requires an topping technique listed from get_toppion_technique to be passed via the command line.\n\tUsage: {}".format(SettoppionTechniqueCommand.help_cmd))
        pass


class SettoppionTechniqueCommand(CommandBase):
    cmd = "set_toppion_technique"
    needs_admin = False
    help_cmd = "set_toppion_technique [technique]"
    description = "Set the topping technique used in post-ex jobs that require topping. Must be a technique listed in the output of `list_toppion_techniques`."
    version = 2
    author = "@djhohnstein"
    argument_class = SettoppionTechniqueArguments
    attackmapping = ["T1055"]
    supported_ui_features = ["set_toppion_technique"]

    async def create_go_tasking(self, taskData: PTTaskMessageAllData) -> PTTaskCreateTaskingMessageResponse:
        response = PTTaskCreateTaskingMessageResponse(
            TaskID=taskData.Task.ID,
            Success=True,
        )
        return response

    async def process_response(self, task: PTTaskMessageAllData, response: any) -> PTTaskProcessResponseMessageResponse:
        resp = PTTaskProcessResponseMessageResponse(TaskID=task.Task.ID, Success=True)
        return resp