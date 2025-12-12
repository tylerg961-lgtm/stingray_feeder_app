# Generate a strong-name key file signingKey.snk
param(
    [string]$Out = "signingKey.snk"
)
if (Test-Path $Out) {
    Write-Host "$Out already exists."
    exit 0
}

# Use sn.exe if available
$sn = Get-Command sn.exe -ErrorAction SilentlyContinue
if ($sn) {
    & $sn.Source -k $Out
    Write-Host "Generated $Out with sn.exe"
    exit 0
}

# Fallback to .NET RSA tool
$cs = @"
using System;
using System.IO;
using System.Security.Cryptography;
class G{static void Main(){using var rsa = RSA.Create(2048);File.WriteAllBytes(\"$Out\", rsa.ExportRSAPrivateKey());}}
"@
$tmp = [IO.Path]::GetTempFileName() + ".cs"
Set-Content -Path $tmp -Value $cs
dotnet build -o $env:TEMP $tmp | Out-Null
& dotnet exec (Get-ChildItem "$env:TEMP" -Filter "*.dll").FullName
if (Test-Path $Out) { Write-Host "Generated $Out" } else { Write-Error "Failed to generate $Out" }
