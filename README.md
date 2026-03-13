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
  export             Export bookmarks to local filesystem.
  import             Import bookmarks from local filesystem.
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
tinycity export --source all --directory ~/backup
tinycity export --source chrome --directory ~/backup
tinycity import --target chrome --directory ~/backup
tinycity import --target all --directory ~/backup

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

## Agent skill

Tinycity includes a `SKILL.md` compatible with [vercel-labs/skills](https://github.com/vercel-labs/skills), which lets AI agents (Claude Code, GitHub Copilot, etc.) run tinycity commands on your behalf.

Install the skill into your agent environment:

```bash
npx skills install https://github.com/yetanotherchris/tinycity
```

Once installed, you can ask your agent things like:
- "Search my bookmarks for GitHub"
- "List all my bookmarks"
- "Open the first result for gmail"
- "Export my Chrome bookmarks to ~/backup"

The skill will install tinycity automatically if it is not already present.

### Why the tinycity name?
The name was generated using a name generator.