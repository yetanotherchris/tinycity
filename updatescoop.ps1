param(
	[Parameter(Mandatory=$true)]
	[String]
	$version
)

$search = "*publish/tinycity.exe"
$tinyCityFile = Get-ChildItem -Path "./bin/Release/" -Force -Recurse -File | Where-Object { $_.FullName -like $search }
$filePath = $tinyCityFile[0].FullName

Write-Output "File found: $filePath, getting hash..."
$hash = (Get-FileHash -Path $filePath -Algorithm SHA256).Hash
Write-Output "Hash: $hash"

Write-Output "Creating tinycity.json for version $version..."
Write-Output "{
  `"version`": `"$version`",
  `"architecture`": {
      `"64bit`": {
            `"url`": `"https://github.com/yetanotherchris/tiny-city/releases/download/v$version/tinycity.exe`",
            `"extract_dir`": `"`",
            `"pre_install`": [
                `"Rename-Item \`"$dir\\tinycity-v1.4.0-win-x64.exe\`" \`"tinycity.exe\`"`"
            ],
            `"bin`": [
              `"tinycity.exe`"
            ],
          `"hash`": `"$hash`"
      }
  },
  `"homepage`": `"https://github.com/yetanotherchris/tiny-city`",
  `"license`": `"MIT License`",
  `"description`": `"A console app written that lists and searches your bookmarks files`"
}" | Out-File -FilePath "tinycity.json" -Encoding utf8