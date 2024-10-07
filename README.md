# bulk-git

```
██████╗  ██╗   ██╗ ██╗     ██╗  ██╗
██╔══██╗ ██║   ██║ ██║     ██║ ██╔╝
██████╔╝ ██║   ██║ ██║     █████╔╝
██╔══██╗ ██║   ██║ ██║     ██╔═██╗
██████╔╝ ╚██████╔╝ ██████╗ ██║  ██╗
╚═════╝   ╚═════╝  ╚═════╝ ╚═╝  ╚═╝
██████╗  ███████╗ ██████╗   ██████╗
██╔══██╗ ██╔════╝ ██╔══██╗ ██╔═══██╗
██████╔╝ █████╗   ██████╔╝ ██║   ██║
██╔══██╗ ██╔══╝   ██╠═══╝  ██║   ██║
██║  ██║ ███████╗ ██║      ╚██████╔╝
╚═╝  ╚═╝ ╚══════╝ ╚═╝       ╚═════╝

Bulk Git - Bulk git operations for
multiple repositories in a directory
      Author: Fatih Bahceci


Usage: gitb [command] [options] [arguments] [where conditions]

Commands:
  pwd         Print current directory
  pull        Pull all git repositories in the current directory
  branches    List current branches in the current directory
    Options:
      --all       Also list all other local branches for each repository
      --fetch     Fetch all branches from remote before list
      -a          [With --all parameter] List all local and remote branches
      -r          [With --all parameter] List remote branches only
  urls        List all urls in the current directory
    Options:
      --with-directory   Print directory path with urls
  fetch       Fetch all branches in the current directory
  status      Show status of repositories  in the current directory that action needed
  switch      Switch all repositories to the given branch
    Arguments:
      [branchName]   Branch name to switch all repositories

General Options:
  --verbose   Print verbose output
  --dir [path]   Set working directory
  --where [field] [condition] [value] [and/or] [field] [condition] [value] ...   Filter repositories with conditions
    Fields: path, url, branch
    Conditions: ct (contains), sw (starts with), ew (ends with), eq (equals), !ct (not contains), !sw (not starts with), !ew (not ends with), !eq (not equals)
    Conjunctions: and, or
    The comprassion is not case sensitive.
    Example: --where path eq "C:\Projects" and branch eq "master" or url sw "http//"

Examples:
  bulk-git pull --verbose
  bulk-git branches
  bulk-git branches --all
  bulk-git branches --all --fetch
  bulk-git branches -a
  bulk-git branches -r
  bulk-git branches --all -a
  bulk-git branches --all -a --fetch
  bulk-git branches --all -r
  bulk-git urls --with-directory
  bulk-git fetch
  bulk-git status --dir "C:\Path to working directory\sources"
```