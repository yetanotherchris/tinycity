param(
    [Parameter(Mandatory = $true)]
    [string]
    $Version
)

# Look for the renamed Windows executable in the current directory
$searchPattern = "tinycity-v$Version-win-x64.exe"
$tinycityFile = Get-ChildItem -Path $PSScriptRoot -File | Where-Object { $_.Name -eq $searchPattern } | Select-Object -First 1

if (-not $tinycityFile) {
    throw "Unable to locate $searchPattern in the current directory."
}

$filePath = $tinycityFile.FullName
Write-Output "File found: $filePath, getting hash..."
$hash = (Get-FileHash -Path $filePath -Algorithm SHA256).Hash
Write-Output "Hash: $hash"

$manifest = @{
    version = $Version
    architecture = @{
        '64bit' = @{
            url = "https://github.com/yetanotherchris/tinycity/releases/download/v$Version/tinycity-v$Version-win-x64.exe"
            bin = @("tinycity.exe")
            hash = $hash
            extract_dir = ""
            pre_install = @("Rename-Item `"`$dir\tinycity-v$Version-win-x64.exe`" `"tinycity.exe`"")
        }
    }
    homepage = "https://github.com/yetanotherchris/tinycity"
    license = "MIT License"
    description = "Ask any large language model from your terminal via OpenAI-compatible APIs."
}

Write-Output "Creating tinycity.json for version $Version..."
$manifest | ConvertTo-Json -Depth 5 | Out-File -FilePath "tinycity.json" -Encoding utf8
