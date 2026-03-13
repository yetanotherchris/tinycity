---
name: tinycity
description: Search, list, import, and export bookmarks using the tinycity CLI. Use this skill when the user wants to find, browse, or manage their browser bookmarks from the terminal.
---

# tinycity

Run tinycity commands on behalf of the user to search, list, import, and export bookmarks.

## Installation check

Before running any tinycity command, check whether it is installed:

```bash
tinycity --version
```

If the command is not found, install it based on the platform:

**macOS/Linux (Homebrew):**
```bash
brew tap yetanotherchris/tinycity https://github.com/yetanotherchris/tinycity
brew install tinycity
```

**Windows (Scoop):**
```powershell
scoop bucket add tinycity https://github.com/yetanotherchris/tinycity
scoop install tinycity
```

**Linux (wget fallback):**
```bash
wget -O tinycity "https://github.com/yetanotherchris/tinycity/releases/latest/download/tinycity"
chmod +x tinycity
sudo mv tinycity /usr/local/bin/
```

**macOS (curl fallback):**
```bash
curl -o tinycity -L "https://github.com/yetanotherchris/tinycity/releases/latest/download/tinycity"
chmod +x tinycity
sudo mv tinycity /usr/local/bin/
```

**Windows (PowerShell fallback):**
```powershell
Invoke-WebRequest -Uri "https://github.com/yetanotherchris/tinycity/releases/latest/download/tinycity.exe" -OutFile "tinycity.exe"
```

After installing, run `tinycity config` to set up bookmark sources before the first use.

## Commands

### Search bookmarks
```bash
tinycity search "<query>"
tinycity search "<query>" --urls        # also search inside URLs
tinycity search "<query>" --launch      # open the first result in the browser
tinycity search "<query>" --export      # save results to exported-bookmarks.md
```

### List all bookmarks
```bash
tinycity ls
tinycity ls --export
```

### Configure sources
```bash
tinycity config                                    # show current config
tinycity config --add-source chrome
tinycity config --add-source brave
tinycity config --add-source edge
tinycity config --add-source opera
tinycity config --add-source /path/to/file.md
tinycity config --add-source /path/to/bookmarks.html
tinycity config --remove-source chrome
```

### Export bookmarks
```bash
tinycity export --source all --directory ~/backup
tinycity export --source chrome --directory ~/backup
```

### Import bookmarks
```bash
tinycity import --target chrome --directory ~/backup
tinycity import --target all --directory ~/backup
```

## Behaviour

- Map the user's natural language request to the appropriate command.
- If the user says "find", "look for", or "search", use `tinycity search`.
- If the user says "show all" or "list", use `tinycity ls`.
- If the user says "open" or "launch", use `tinycity search "<query>" --launch`.
- If the user says "export" or "back up", use `tinycity export`.
- If the user says "import" or "restore", use `tinycity import`.
- If results are empty, tell the user and suggest checking sources with `tinycity config`.
- Do not guess or fabricate bookmark data — always run the command and return the real output.
- Ensure the results you display match the tiny results, which should include the hyperlinks.
  - For example searching for Google Drive bookmarks, the result would show "Google Drive (drive.google.com)" and "Google Drive" would be a hyperlink to the full url.
