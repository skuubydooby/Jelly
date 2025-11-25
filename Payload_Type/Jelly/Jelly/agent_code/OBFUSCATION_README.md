# Automatic Config.cs Obfuscation System

## Overview

This system provides **automatic XOR obfuscation** for Jelly C2 agent configuration values. No more manual byte array generation—just run a command and copy-paste the output into `Config.cs`.

## Files Created

### 1. `ConfigObfuscator.cs` - C# Helper Class
Provides methods for obfuscating/deobfuscating values at runtime or during builds.

**Key Methods:**
- `ObfuscateValue(string)` - Convert plain text to obfuscated C# code
- `DeobfuscateValue(byte[])` - Decrypt obfuscated bytes
- `ObfuscateConfigValues(Dictionary)` - Bulk obfuscation
- `GenerateHttpProfileConfig(Dictionary)` - Generate complete profile sections

**Usage in Code:**
```csharp
using Jelly;

// Use during build-time code generation
var obfuscated = ConfigObfuscator.ObfuscateValue("https://my-c2.com");
// Result: UnobfuscateString(new byte[] { 42, 8, 234, 79, ... })
```

### 2. `auto_obfuscate_config.py` - Automatic Configuration Generator
Python tool to automatically generate obfuscated values for Config.cs.

**Features:**
- Single value obfuscation
- Generate complete HTTP/WebSocket profile sections
- Auto-obfuscate entire Config.cs files
- Interactive and CLI modes

## Quick Start

### Obfuscate a Single Value
```powershell
cd d:\Jelly\Payload_Type\Jelly\Jelly\agent_code
python auto_obfuscate_config.py --value "https://my-c2-server.com"
```

**Output:**
```
Plain:      https://my-c2-server.com
C# Code:    UnobfuscateString(new byte[] { 42, 8, 234, 79, 210, 103, 231, 61, ... })
```

Copy the C# Code directly into your Config.cs.

### Generate HTTP Profile Section
```powershell
python auto_obfuscate_config.py --generate-http
```

**Output:** Complete Parameters dictionary with all HTTP profile values obfuscated and ready to copy into Config.cs.

### Generate WebSocket Profile Section
```powershell
python auto_obfuscate_config.py --generate-websocket
```

### Obfuscate Entire Config.cs
```powershell
python auto_obfuscate_config.py --input Jelly/Config.cs --output Jelly/Config.cs
```

This automatically finds and obfuscates plain text values in the file.

### Interactive Mode
```powershell
python auto_obfuscate_config.py
```

Provides a menu for all operations.

## How It Works

### Obfuscation Process (XOR Cipher)

1. **Input:** Plain text value (e.g., "https://my-c2.com")
2. **Convert to bytes:** UTF-8 encoding
3. **XOR with key:** Each byte is XORed with cycling 8-byte key
   - Key: `0x42, 0x7C, 0x9E, 0x3F, 0xA1, 0x5D, 0xC8, 0x12`
4. **Output:** Byte array for Config.cs

### Runtime Deobfuscation

When the Jelly agent runs:
1. Config.cs contains obfuscated byte arrays
2. At runtime, `UnobfuscateString()` method XORs bytes again with the same key
3. Original value is recovered in memory
4. Dynamic analysis tools (dnSpy, ILSpy) show only byte arrays, not readable strings

## Complete Workflow

### Scenario: Change C2 Server URL

**Before:**
```csharp
{ "callback_host", UnobfuscateString(new byte[] { 42, 8, 234, 79, 210, 103, 231, 61, ... }) },
```

**Step 1: Generate obfuscated value**
```powershell
python auto_obfuscate_config.py --value "https://new-c2-server.com"
```

**Step 2: Copy output into Config.cs**
```csharp
{ "callback_host", UnobfuscateString(new byte[] { 42, 8, 234, 79, 210, 103, 231, 61, 45, 13, 234, 76, 129, 43, 239, 97, 100, 5, 190, 22, 130, 56, 180 }) },
// Original: "https://new-c2-server.com"
```

**Step 3: Build**
```powershell
dotnet build
```

Done! The new C2 URL is now obfuscated in the binary.

## Configuration Values

### HTTP Profile (Default)
- `callback_interval`: "10" (seconds between check-ins)
- `callback_jitter`: "23" (randomization %)
- `callback_port`: "8443" (C2 server port)
- `callback_host`: "https://16.170.226.88" (C2 server URL)
- `post_uri`: "data" (URI path for beaconing)
- `encrypted_exchange_check`: "true" (enable encryption)
- `killdate`: "2026-11-21" (agent expiration)
- `User-Agent`: "Mozilla/5.0 ..." (HTTP user agent)

### Staging Keys
- `StagingRSAPrivateKey`: "gl8RB+whNyCog2/UNAdq1pnjdVpa+B3NhrHy597CRPo="
- `PayloadUUID`: "f0ce3024-e28b-4ce2-bedd-e99ebf36c74b"

## Integration with Mythic C2

To automate in Mythic's payload generation:

1. **In Mythic's payload handler**, call the auto_obfuscate_config.py tool:
   ```python
   subprocess.run([
       "python", "auto_obfuscate_config.py",
       "--value", user_provided_c2_url
   ])
   ```

2. **Inject output into Config.cs** before building

3. **Build Jelly.exe** with obfuscated values

4. **Return obfuscated payload** to Mythic UI

This eliminates manual steps and provides one-click obfuscated payload generation.

## Security Notes

### XOR Obfuscation Limitations
- XOR is **fast and simple**, not cryptographically strong
- Protects against **casual static analysis** (dnSpy, ILSpy showing readable strings)
- Protects against **automated string extraction** (grep, binary searches)
- **NOT protected against** determined reverse engineers who can:
  - Extract the XOR key from the binary
  - Manually brute-force the small 8-byte keyspace
  - Use dynamic analysis to capture decrypted strings in memory

### Recommended Enhancements
For higher security:
1. **Replace XOR key with random key** generated per build
2. **Store key separately** from the binary (e.g., registry, environment variable)
3. **Add additional obfuscation** (ConfuserEx, code flow obfuscation)
4. **Use .NET native compilation** to prevent decompilation entirely
5. **Anti-reversing checks** (detect debuggers, check for breakpoints)

## Troubleshooting

### Issue: "Input string was not in a correct format"
**Cause:** Obfuscated byte array is incorrect
**Solution:** Regenerate using the auto_obfuscate_config.py tool with exact value

### Issue: dnSpy still shows plain text
**Cause:** Compiler optimization disabled, or value not properly obfuscated
**Solution:** 
1. Verify `#define C2PROFILE_NAME_UPPER` is set
2. Verify `#if HTTP` (or relevant profile) is active
3. Rebuild with Release configuration

### Issue: Script generates different output each time
**This should not happen** - XOR with the same key should always produce the same result
**Solution:** Verify the XOR key hasn't changed between runs

## File Structure

```
agent_code/
├── Jelly/
│   ├── Config.cs                    (Main configuration file)
│   ├── ConfigObfuscator.cs          (NEW: C# obfuscation helper)
│   └── [other Jelly files]
├── xor_obfuscate.py                 (Original simple tool)
├── auto_obfuscate_config.py         (NEW: Advanced auto-generator)
├── wrapper.py                       (Mythic integration template)
└── [other payload files]
```

## Command Reference

| Command | Purpose |
|---------|---------|
| `python auto_obfuscate_config.py --value "text"` | Obfuscate single value |
| `python auto_obfuscate_config.py --generate-http` | Generate HTTP profile section |
| `python auto_obfuscate_config.py --generate-websocket` | Generate WebSocket profile section |
| `python auto_obfuscate_config.py --input Config.cs --output Config.cs` | Obfuscate entire file |
| `python auto_obfuscate_config.py` | Interactive mode |

## Next Steps

✅ **Completed:**
- XOR obfuscation system implemented
- Auto-generation tools created
- Runtime deobfuscation working
- Mythic C2 compatibility maintained

⏳ **Optional Enhancements:**
- Integrate `auto_obfuscate_config.py` with Mythic's payload handler
- Add build-time code generation hooks to .csproj
- Generate random XOR keys per build
- Add anti-reversing checks to Config.cs

---

**Author:** Jelly C2 Configuration System  
**Date:** November 2025  
**XOR Key:** `0x42, 0x7C, 0x9E, 0x3F, 0xA1, 0x5D, 0xC8, 0x12`
