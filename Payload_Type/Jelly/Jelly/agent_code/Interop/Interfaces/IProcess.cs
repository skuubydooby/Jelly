using Interop.Structs.Structs;
using System;

namespace Interop.Interfaces
{
    public interface IProcess
    {
        bool topp(byte[] code, string arguments = "");
        void WaitForExit();
        void WaitForExit(int milliseconds);

        bool Start();
        bool StartWithCredentials(ApolloLogonInformation logonInfo);

        bool StartWithCredentials(IntPtr hToken);

    }
}
