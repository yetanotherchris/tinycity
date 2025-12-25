# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

Tinycity is a .NET 10 CLI application that searches and displays bookmarks from various sources (Chrome/Brave/Edge/Opera bookmarks, Markdown files, HTML bookmark exports). It uses Native AOT compilation for cross-platform single-file executables.

## Build and Run Commands

**Development:**
```bash
dotnet run -- [command]
dotnet run -- ls
dotnet run -- search "google" -urls
dotnet run -- search "gmail"
dotnet run -- config
```

**Build:**
```bash
dotnet build
dotnet restore
```

**Publish (Native AOT):**
```bash
# Windows
dotnet publish tinycity.csproj -c Release -r win-x64 -p:PublishAot=true

# Linux
dotnet publish tinycity.csproj -c Release -r linux-x64 -p:PublishAot=true

# macOS (Intel)
dotnet publish tinycity.csproj -c Release -r osx-x64 -p:PublishAot=true

# macOS (ARM)
dotnet publish tinycity.csproj -c Release -r osx-arm64 -p:PublishAot=true
```

## Architecture

**Command Pattern:**
- Commands are in `Commands/` directory
- Each command handler inherits from `BaseCommandHandler<TSettings>`
- Settings classes in `Commands/Settings/` define command-line options
- Uses System.CommandLine for CLI parsing

**Dependency Injection:**
- IoC setup in `Program.cs:SetupIoC()`
- Services registered: `ChromeBookmarks`, `MarkdownBookmarks`, `HtmlBookmarks`, `BookmarkAggregator`, command handlers

**Bookmark Engines:**
- `BookmarkEngines/` contains parsers for different bookmark formats
- `ChromeBookmarks`: Parses Chromium JSON format (Chrome, Brave, Edge, Opera)
- `MarkdownBookmarks`: Extracts links from Markdown files
- `HtmlBookmarks`: Parses Netscape HTML bookmark format
- `BookmarkAggregator`: Combines all bookmark sources into single list

**Configuration:**
- `TinyCitySettings`: Manages app configuration stored in JSON
- Windows: `%LOCALAPPDATA%\tinycity\config.json`
- Linux: `~/.config/tinycity/config.json`
- Auto-detects browser bookmark file paths using `BrowserKnownPaths.cs`

**Data Model:**
- `Model/BookmarkNode.cs`: Core bookmark structure (mirrors Chromium JSON format)
- Uses System.Text.Json with source generation (`TinyCityJsonContext.cs`)

**Key Technologies:**
- Spectre.Console: Terminal UI and markup
- System.CommandLine: CLI framework
- AngleSharp: HTML parsing
- Markdig: Markdown parsing
- Native AOT: PublishAot=true for performance and single-file deployment

## Important Platform Handling

`BrowserKnownPaths.cs` contains OS-specific paths for browser bookmarks. Not all browsers are implemented on all platforms (e.g., Edge on macOS/Linux throws `NotImplementedException`).

## Release Process

Uses GitVersion for semantic versioning. GitHub Actions workflow builds for all platforms when tags matching `v*.*.*` are pushed. The workflow:
1. Builds Native AOT binaries for linux-x64, win-x64, osx-x64, osx-arm64
2. Creates GitHub release with all binaries
3. Updates scoop (tinycity.json) and homebrew (Formula/tinycity.rb) manifests via PowerShell scripts

## File Structure Notes

- `Program.cs`: Entry point, IoC setup, command registration
- `TinyCitySettings.cs`: Configuration management
- `ExtraArgumentHandler.cs`: Handles `--extra` flag for debug timing info
