# tinycity
A console app written in C# that lists and searches your bookmarks files, displaying them in the terminal and optionally launching them in your default browser. 
It doesn't manage your bookmarks (adding/editing/deleting), it just lists and searches through them.

The terminal output includes links that can be clicked, using the Spectre console library. It supports the following bookmarks:

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

```powershell
Invoke-WebRequest -Uri "https://github.com/yetanotherchris/tinycity/releases/latest/download/tinycity.exe" -OutFile "tinycity.exe"
```
```bash
wget -O tinycity "https://github.com/yetanotherchris/tinycity/releases/latest/download/tinycity"
```
```bash
curl -o tinycity -L "https://github.com/yetanotherchris/tinycity/releases/latest/download/tinycity"
```

Scoop.sh on Windows:
```
scoop bucket add tinycity https://github.com/yetanotherchris/tinycity/
scoop install tinycity
```

You can also download the latest release directly from the [Releases page](https://github.com/yetanotherchris/tinycity/releases).

## Usage

```
USAGE:
    tinycity [OPTIONS] <COMMAND>

OPTIONS:
    -h, --help       Prints help information
    -v, --version    Prints version information

COMMANDS:
    search <query>    Search the bookmarks
    ls                List all bookmarks
    config            Configure bookmark sources
```

### Examples
```
./tinycity config # show the current config
./tinycity config --help
./tinycity ls
./tinycity search "google.com" -urls #search using the url
./tinycity q "gmail" # q is a shortcut for search
./tinycity search "openrouter" --launch
./tinycity config -b brave
./tinycity config -a more-bookmarks.md
./tinycity config --html-bookmark-file "$($env:USERPROFILE)/bookmarks.html"
```

If you clone the source using `git clone` (requires [.NET 9](https://dotnet.microsoft.com/en-us/download/dotnet/9.0) or later):

```
dotnet run --
dotnet run -- ls
dotnet run -- search "google" -urls
dotnet run -- search "gmail"
dotnet run -- search "openrouter" --launch
```

### Why the tinycity name?
The name was generated using a name generator.