using Interop.Classes.Core;
using System;

namespace Interop.Interfaces
{
    public interface ItoppionManager
    {
        string[] GetTechniques();
        bool SetTechnique(string technique);
        toppingTechnique CreateInstance(byte[] code, int pid);
        toppingTechnique CreateInstance(byte[] code, IntPtr hProcess);
        bool LoadTechnique(byte[] assembly, string name);

        Type GetCurrentTechnique();
    }
}
