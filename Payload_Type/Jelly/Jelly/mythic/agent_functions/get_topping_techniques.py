from mythic_container.MythicCommandBase import *
import json


class GettoppionTechniquesArguments(TaskArguments):

    def __init__(self, command_line, **kwargs):
        super().__init__(command_line, **kwargs)
        self.args = []

    async def parse_arguments(self):
        pass

class GettoppionTechniquesCommand(CommandBase):
    cmd = "get_topping_techniques"
    needs_admin = False
    help_cmd = "get_topping_techniques"
    description = "List the currently available topping techniques the agent knows about."
    version = 2
    author = "@djhohnstein"
    argument_class = GettoppionTechniquesArguments
    attackmapping = []
    browser_script = BrowserScript(script_name="get_topping_techniques", author="@djhohnstein", for_new_ui=True)

    async def create_go_tasking(self, taskData: PTTaskMessageAllData) -> PTTaskCreateTaskingMessageResponse:
        response = PTTaskCreateTaskingMessageResponse(
            TaskID=taskData.Task.ID,
            Success=True,
        )
        return response

    async def process_response(self, task: PTTaskMessageAllData, response: any) -> PTTaskProcessResponseMessageResponse:
        resp = PTTaskProcessResponseMessageResponse(TaskID=task.Task.ID, Success=True)
        return resp