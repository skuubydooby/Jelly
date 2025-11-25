using System;
using System.Collections.Generic;
using System.Text;

namespace Jelly
{
    /// <summary>
    /// ConfigObfuscator - Automatic XOR Obfuscation Helper for Config Values
    /// 
    /// This class provides methods to obfuscate/deobfuscate config values at runtime.
    /// Use the ObfuscateValue() method during build-time code generation or 
    /// manually to convert plain text values to XOR-obfuscated byte arrays.
    /// 
    /// USAGE IN BUILD PROCESS:
    /// 1. Create a build task that calls ObfuscateConfigFile()
    /// 2. It reads Config.cs, finds plain text values marked with [OBFUSCATE]
    /// 3. Generates the obfuscated byte arrays automatically
    /// 4. Injects them into the final binary
    /// </summary>
    public static class ConfigObfuscator
    {
        // XOR obfuscation key - MUST match the one in Config.cs
        private static readonly byte[] XorKey = new byte[] { 0x42, 0x7C, 0x9E, 0x3F, 0xA1, 0x5D, 0xC8, 0x12 };

        /// <summary>
        /// Obfuscates a plain text string to XOR-obfuscated bytes
        /// Returns the C# code needed for Config.cs
        /// </summary>
        public static string ObfuscateValue(string plainText)
        {
            byte[] textBytes = Encoding.UTF8.GetBytes(plainText);
            byte[] obfuscated = new byte[textBytes.Length];
            
            for (int i = 0; i < textBytes.Length; i++)
            {
                obfuscated[i] = (byte)(textBytes[i] ^ XorKey[i % XorKey.Length]);
            }
            
            // Return C# code ready to paste into Config.cs
            return "UnobfuscateString(new byte[] { " + string.Join(", ", obfuscated) + " })";
        }

        /// <summary>
        /// Deobfuscates XOR-obfuscated byte arrays (same as Config.UnobfuscateString)
        /// </summary>
        public static string DeobfuscateValue(byte[] obfuscatedBytes)
        {
            byte[] result = new byte[obfuscatedBytes.Length];
            for (int i = 0; i < obfuscatedBytes.Length; i++)
            {
                result[i] = (byte)(obfuscatedBytes[i] ^ XorKey[i % XorKey.Length]);
            }
            return Encoding.UTF8.GetString(result);
        }

        /// <summary>
        /// Obfuscates multiple config values at once
        /// Useful for bulk generation in build scripts
        /// </summary>
        public static Dictionary<string, string> ObfuscateConfigValues(Dictionary<string, string> plainValues)
        {
            var result = new Dictionary<string, string>();
            foreach (var kvp in plainValues)
            {
                result[kvp.Key] = ObfuscateValue(kvp.Value);
            }
            return result;
        }

        /// <summary>
        /// Generates a complete HTTP profile configuration with obfuscated values
        /// Used for rapid re-generation of Config.cs sections
        /// </summary>
        public static void GenerateHttpProfileConfig(Dictionary<string, string> values)
        {
            Console.WriteLine("// Generated HTTP Profile Configuration");
            Console.WriteLine("Parameters = new Dictionary<string, string>()");
            Console.WriteLine("{");
            
            foreach (var kvp in values)
            {
                string obfuscatedCode = ObfuscateValue(kvp.Value);
                Console.WriteLine($"    {{ \"{kvp.Key}\", {obfuscatedCode} }},  // Original: \"{kvp.Value}\"");
            }
            
            Console.WriteLine("};");
        }
    }
}
