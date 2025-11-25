# Encryption helper script to encrypt config values
# Usage: .\encrypt_config.ps1 -PlainText "value_to_encrypt"

param(
    [string]$PlainText = ""
)

# Must match the key in Config.cs
$encryptionKey = @(
    0x7a, 0x4d, 0x1b, 0x8e, 0x3c, 0x9f, 0x2d, 0x5a, 
    0x6b, 0x1e, 0x4f, 0x9c, 0x2a, 0x7d, 0x3b, 0x8c,
    0x5e, 0x1a, 0x6f, 0x4d, 0x2c, 0x9b, 0x3d, 0x7a,
    0x1c, 0x8f, 0x5b, 0x2e, 0x6d, 0x4a, 0x9e, 0x3f
)

if ([string]::IsNullOrEmpty($PlainText)) {
    Write-Host "Usage: .\encrypt_config.ps1 -PlainText `"text_to_encrypt`""
    exit
}

[System.Reflection.Assembly]::LoadWithPartialName("System.Security") | Out-Null

$aes = [System.Security.Cryptography.Aes]::Create()
$aes.Key = [byte[]]$encryptionKey
$aes.Mode = [System.Security.Cryptography.CipherMode]::CBC
$aes.Padding = [System.Security.Cryptography.PaddingMode]::PKCS7

$plainTextBytes = [System.Text.Encoding]::UTF8.GetBytes($PlainText)
$encryptor = $aes.CreateEncryptor()
$encryptedData = $encryptor.TransformFinalBlock($plainTextBytes, 0, $plainTextBytes.Length)

# Prepend IV to encrypted data
$iv = $aes.IV
$fullEncryptedData = $iv + $encryptedData

$encryptedBase64 = [Convert]::ToBase64String($fullEncryptedData)
Write-Host "Encrypted: $encryptedBase64"

$encryptor.Dispose()
$aes.Dispose()
