#define COMMAND_NAME_UPPER

#if DEBUG
#define SET_toppION_TECHNIQUE
#endif

#if SET_toppION_TECHNIQUE

using Interop.Classes;
using Interop.Interfaces;
using Interop.Structs.MythicStructs;

namespace Tasks
{
    public class set_toppion_technique : Tasking
    {
        public set_toppion_technique(IAgent agent, Interop.Structs.MythicStructs.MythicTask data) : base(agent, data)
        {
        }


        public override void Start()
        {
            MythicTaskResponse resp;
            if (_agent.GettoppionManager().SetTechnique(_data.Parameters))
            {
                resp = CreateTaskResponse($"Set topping technique to {_data.Parameters}", true);
            }
            else
            {
                resp = CreateTaskResponse($"Unknown technique: {_data.Parameters}", true, "error");
            }

            // Your code here..
            // Then add response to queue
            _agent.GetTaskManager().AddTaskResponseToQueue(resp);
        }
    }
}

#endif