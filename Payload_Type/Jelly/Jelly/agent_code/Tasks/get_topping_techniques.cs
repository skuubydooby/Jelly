#define COMMAND_NAME_UPPER

#if DEBUG
#define GET_INJECTION_TECHNIQUES
#endif

#if GET_INJECTION_TECHNIQUES

using Interop.Classes;
using Interop.Interfaces;
using Interop.Structs.MythicStructs;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Tasks
{
    public class get_injection_techniques : Tasking
    {
        [DataContract]
        internal struct toppingTechniqueResult
        {
            [DataMember(Name = "name")]
            public string Name;
            [DataMember(Name = "is_current")]
            public bool IsCurrent;
        }
        public get_injection_techniques(IAgent agent, Interop.Structs.MythicStructs.MythicTask data) : base(agent, data)
        {
        }
        

        public override void Start()
        {
            MythicTaskResponse resp;
            string[] techniques = _agent.GetInjectionManager().GetTechniques();
            Type cur = _agent.GetInjectionManager().GetCurrentTechnique();
            List<toppingTechniqueResult> results = new List<toppingTechniqueResult>();
            foreach (string t in techniques)
            {
                results.Add(new toppingTechniqueResult
                {
                    Name = t,
                    IsCurrent = t == cur.Name
                });
            }

            resp = CreateTaskResponse(
                _jsonSerializer.Serialize(results.ToArray()), true);
            // Your code here..
            // Then add response to queue
            _agent.GetTaskManager().AddTaskResponseToQueue(resp);
        }
    }
}

#endif