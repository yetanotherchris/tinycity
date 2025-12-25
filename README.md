# tinycity
Tinycity is terminal application written in C# that lists, searches, imports, and exports your bookmarks files, displaying them in the terminal and optionally launching them in your default browser.

It doesn't manage your bookmarks (adding/editing/deleting), it just lists, searches, imports, and exports them.

The terminal output includes links that can be clicked. It supports the following bookmarks:

- Chrome, Brave, Opera and Edge bookmarks (Chromium JSON bookmark format)
- Markdown files with links.
- Bookmark export HTML (Netscape) format which all browsers export their bookmarks as.

## Download

[![GitHub Release](https://img.shields.io/github/v/release/yetanotherchris/tinycity?logo=github&sort=semver)](https://github.com/yetanotherchris/tinycity/releases/latest/)

**Package managers**

Scoop on Windows:
```powershell
scoop bucket add tinycity https://github.com/yetanotherchris/tinycity
scoop install tinycity
```

Homebrew on macOS/Linux:
```bash
brew tap yetanotherchris/tinycity https://github.com/yetanotherchris/tinycity
brew install tinycity
```

**Via your terminal**

Powershell  
```powershell
Invoke-WebRequest -Uri "https://github.com/yetanotherchris/tinycity/releases/latest/download/tinycity.exe" -OutFile "tinycity.exe"
```

Bash  
```bash
wget -O tinycity "https://github.com/yetanotherchris/tinycity/releases/latest/download/tinycity"
```
```bash
curl -o tinycity -L "https://github.com/yetanotherchris/tinycity/releases/latest/download/tinycity"
```

You can also download the latest release directly from the [Releases page](https://github.com/yetanotherchris/tinycity/releases).

## Usage

```
Description:
  A command line tool for searching, importing, and exporting bookmarks

Usage:
  tinycity [command] [options]

Options:
  --version       Show version information
  -?, -h, --help  Show help and usage information

Commands:
  q, search <query>  Search the bookmarks.
  list, ls           List all bookmarks.
  config             Configure bookmark sources.
  export             Export bookmarks to S3 or local filesystem.
  import             Import bookmarks from S3 or local filesystem.
```

### Examples
```
# Configuration
tinycity config
tinycity config --add-source chrome
tinycity config --add-source chrome --directory /home/user/.var/app/io.github.ungoogled_software.ungoogled_chromium/config/chromium/Default
tinycity config --add-source more-bookmarks.md
tinycity config --remove-source brave

# Search
tinycity search "google.com" --urls
tinycity q "gmail"
tinycity search "openrouter" --launch

# List
tinycity ls
tinycity ls --export

# Export/Import
tinycity export --type local --source all --directory ~/backup
tinycity export --type remote --source chrome --bucket my-bucket --s3-endpoint https://s3.amazonaws.com
tinycity import --type local --target chrome --directory ~/backup

tinycity export --type remote --source all --s3-endpoint youraccount-id.r2.cloudflarestorage.com--s3-access-key key123 --s3-secret-key keyxyz --bucket yours3bucket --save-credentials
tinycity import --type remote --source chrome --s3-endpoint s3.eu-central-003.backblazeb2.com --s3-access-key key123 --s3-secret-key keyxyz --bucket yours3bucket --save-credentials

```

#### Local development
If you clone the source using `git clone` (requires [.NET 10](https://dotnet.microsoft.com/en-us/download/dotnet/10.0) or later):

```
dotnet run --
dotnet run -- ls
dotnet run -- search "google" -urls
dotnet run -- search "gmail"
dotnet run -- search "openrouter" --launch
```

### Why the tinycity name?
The name was generated using a name generator.