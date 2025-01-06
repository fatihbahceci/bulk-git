// See https://aka.ms/new-console-template for more information
using bulk_git;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
void printIntro()
{
    // Color cyan
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine("██████╗  ██╗   ██╗ ██╗     ██╗  ██╗");
    Console.WriteLine("██╔══██╗ ██║   ██║ ██║     ██║ ██╔╝");
    Console.WriteLine("██████╔╝ ██║   ██║ ██║     █████╔╝ ");
    Console.WriteLine("██╔══██╗ ██║   ██║ ██║     ██╔═██╗ ");
    Console.WriteLine("██████╔╝ ╚██████╔╝ ██████╗ ██║  ██╗");
    Console.WriteLine("╚═════╝   ╚═════╝  ╚═════╝ ╚═╝  ╚═╝");
    Console.WriteLine("██████╗  ███████╗ ██████╗   ██████╗ ");
    Console.WriteLine("██╔══██╗ ██╔════╝ ██╔══██╗ ██╔═══██╗");
    Console.WriteLine("██████╔╝ █████╗   ██████╔╝ ██║   ██║");
    Console.WriteLine("██╔══██╗ ██╔══╝   ██╠═══╝  ██║   ██║");
    Console.WriteLine("██║  ██║ ███████╗ ██║      ╚██████╔╝");
    Console.WriteLine("╚═╝  ╚═╝ ╚══════╝ ╚═╝       ╚═════╝ ");
    Console.WriteLine();
    // Color green
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("Bulk Git - Bulk git operations for ");
    Console.WriteLine("multiple repositories in a directory");
    Console.WriteLine("      Author: Fatih Bahceci");
    Console.WriteLine();
    resetColor();
}

var defaultColor = Console.ForegroundColor;
var defaultBackgroundColor = Console.BackgroundColor;
void resetColor()
{
    Console.ForegroundColor = defaultColor;
    Console.BackgroundColor = defaultBackgroundColor;
}

printIntro();
if (args.Length == 0)
{
    printHelp();
    Environment.ExitCode = 1;
    return;
}

bool _printVerbose = args.Length > 0 && args.Contains("--verbose");
string workingDirectory = Directory.GetCurrentDirectory();
var workingDirectoryArgIndex = Array.IndexOf(args, "--dir");
if (workingDirectoryArgIndex != -1)
{
    workingDirectory = args[workingDirectoryArgIndex + 1];
    args = args.Where((source, index) => index != workingDirectoryArgIndex && index != workingDirectoryArgIndex + 1).ToArray();
    //Directory.SetCurrentDirectory(workingDirectory);
    Console.ForegroundColor = ConsoleColor.DarkGray;
    Console.WriteLine($"Working directory set to {workingDirectory}");
    resetColor();
}

List<(string field, string condition, string value)> pathConditions = new();
List<string> conditionConjunctions = new();
/*
 --where: condition parameter
 field: path, url, branch
condition: ct, sw, ew, eq, !ct, !sw, !ew, !eq
value: value to compare

example usage: --where path eq "C:\Projects" and branch eq "master" or url sw "http//"
"and" and "or" words optional after -where  
 */
var acceptedFields = new string[] { "path", "url", "branch" };
var acceptedConditions = new string[] { "ct", "sw", "ew", "eq", "!ct", "!sw", "!ew", "!eq" };
var whereIndex = Array.IndexOf(args, "--where");
if (whereIndex != -1)
{
    //start a while loop to parse conditions
    string field = "";
    string condition = "";
    string value = "";
    bool isWhereClauseCompleted = false;
    bool canContinueToTheApp = true;
    Func<bool> isFieldsCompleted = () => field != "" && condition != "" && value != "";
    Action removeFieldFromArgs = () => args = args.Where((source, index) => index != whereIndex).ToArray();
    Action<string> err = (msgId) =>
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Field, condition and value must be set for --where parameter ({msgId})");
        resetColor();
        isWhereClauseCompleted = true;
        canContinueToTheApp = false;
    };
    removeFieldFromArgs();//Remove --where from args
    int waitingFor = 0; // 0: field, 1: condition, 2: value
    while (!isWhereClauseCompleted)
    {
        if (whereIndex >= args.Length)
        {
            if (waitingFor != 0)
            {
                //Fields  not completed but end of the args
                err("but end of args");
                continue;
            }
            else
            {
                isWhereClauseCompleted = true;
                break;
            }
        }
        switch (waitingFor)
        {
            case 0:
                if (args[whereIndex] == "and" || args[whereIndex] == "or")
                {
                    if (pathConditions.Count == 0)
                    {
                        err("condition expected but \"" + args[whereIndex] + "\" found");
                        continue;
                    }
                    else
                    {
                        conditionConjunctions.Add(args[whereIndex]);
                        //Remove "and" or "or" from args
                        removeFieldFromArgs();
                        continue;
                    }
                }
                else if (acceptedFields.Contains(args[whereIndex]))
                {
                    field = args[whereIndex];
                    removeFieldFromArgs();//Remove field from args
                    waitingFor = 1;
                    continue;
                }
                else
                {
                    if (pathConditions.Count > 0)
                    {
                        //at least one condition is set. So there is no problem
                        isWhereClauseCompleted = true;
                        continue;
                    }
                    else
                    {
                        //First field is not set but not "and" or "or" or field name
                        err("but \"" + args[whereIndex] + "\" is not a valid field name");
                        continue;
                    }
                }
            case 1:
                if (acceptedConditions.Contains(args[whereIndex]))
                {
                    condition = args[whereIndex];
                    removeFieldFromArgs();//Remove condition from args
                    waitingFor = 2;
                }
                else
                {
                    err(args[whereIndex]);
                    continue;
                }
                break;
            case 2:
                value = args[whereIndex];
                pathConditions.Add((field, condition, value));
                removeFieldFromArgs();//Remove value from args
                field = "";
                condition = "";
                value = "";
                waitingFor = 0;
                continue;
        }
    }
    if (!canContinueToTheApp)
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.BackgroundColor = ConsoleColor.Red;
        Console.WriteLine("Exiting due to error in --where clause");
        resetColor();
        Environment.ExitCode = 2;
        return;
    }
}



switch (args[0])
{
    case "pwd":
        pwd();
        break;
    case "pull":
        pull();
        break;
    case "branches":
        branches();
        break;
    case "stashes":
        stashes();
        break;
    case "urls":
        urls();
        break;
    case "fetch":
        fetch();
        break;
    case "status":
        status();
        break;
    case "switch":
        @switch();
        break;
    default:
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Unknown command: {args[0]}");
        printHelp();
        break;
}
resetColor();
return;
void println(string message = "", ConsoleColor? _color = null, ConsoleColor? _backgroundColor = null)
{
    if (_color != null)
    {
        Console.ForegroundColor = _color.Value;
    }
    if (_backgroundColor != null)
    {
        Console.BackgroundColor = _backgroundColor.Value;
    }
    Console.WriteLine(message);
    resetColor();
}
void printch(string message = "", ConsoleColor? _color = null, ConsoleColor? _backgroundColor = null)
{
    if (_color != null)
    {
        Console.ForegroundColor = _color.Value;
    }
    if (_backgroundColor != null)
    {
        Console.BackgroundColor = _backgroundColor.Value;
    }
    Console.Write(message);
    resetColor();
}
void printHelp()
{
    using (var c = new CConsole(defaultColor, defaultBackgroundColor)) c.f(ConsoleColor.Cyan)
    //Console.WriteLine("Bulk Git - Bulk git operations for multiple repositories in a directory");
    .ln()
    .wln("Usage: gitb [command] [options] [arguments] [--where]")
    .ln()
    .wln(" Common arguments: ", ConsoleColor.White, ConsoleColor.Blue).r()
    .wcr("  --verbose ", ConsoleColor.Green).wln(": Print verbose output")
    .wcr("  --dir     ", ConsoleColor.Green).wln(": [path]   Set working directory")
    .wcr("  ").wln(" Where: ", ConsoleColor.White, ConsoleColor.Blue).r()
    .wcr("  --where   ", ConsoleColor.Green).wln(": [field] [condition] [value] [and/or] [field] [condition] [value] ...")
    .wln("    Filter repositories with conditions")
    .wcr("    Fields       ", ConsoleColor.Cyan).wln(": path, url, branch")
    .wcr("    Conditions   ", ConsoleColor.Cyan).wln(": ct (contains), sw (starts with), ew (ends with), eq (equals), !ct (not contains), !sw (not starts with), !ew (not ends with), !eq (not equals)")
    .wcr("    Conjunctions ", ConsoleColor.Cyan).wln(": and, or")
    .wln("    The comprassion is not case sensitive.")
    .wch("    Example ", ConsoleColor.Green).r().wln(": --where path eq \"C:\\Projects\" and branch eq \"master\" or url sw \"http://\"")
    .ln()
    .wln(" Commands: ", ConsoleColor.White, ConsoleColor.Blue).r()
    .wcr("  pwd         ", ConsoleColor.Green).wln(": Print current directory")
    .wcr("  pull        ", ConsoleColor.Green).wln(": Pull all git repositories in the current directory")
    .wcr("  branches    ", ConsoleColor.Green).wln(": List current branches in the current directory")
    .wln("    Options:", ConsoleColor.DarkGray)
    .wln("      --all       Also list all other local branches for each repository")
    .wln("      --fetch     Fetch all branches from remote before list")
    .wln("      -a          [With --all parameter] List all local and remote branches")
    .wln("      -r          [With --all parameter] List remote branches only")
    .wcr("  urls        ", ConsoleColor.Green).wln(": List all urls in the current directory")
    .wln("    Options:", ConsoleColor.DarkGray)
    .wln("      --with-directory   Print directory path with urls")
    .wcr("  fetch       ", ConsoleColor.Green).wln(": Fetch all branches in the current directory")
    .wcr("  status      ", ConsoleColor.Green).wln(": Show status of repositories  in the current directory that action needed")
    .wcr("  stashes     ", ConsoleColor.Green).wln(": List repositories that has stashes with stash names")
    .wcr("  switch      ", ConsoleColor.Green).wln(": Switch all repositories to the given branch")
    .wln("    Arguments:", ConsoleColor.DarkGray)
    .wln("      [branchName]   Branch name to switch all repositories")
    .ln()
    .wlr(" Examples: ", ConsoleColor.White, ConsoleColor.Blue).f(ConsoleColor.DarkGray)
    .wln("  bulk-git pull --verbose")
    .wln("  bulk-git branches")
    .wln("  bulk-git branches --all")
    .wln("  bulk-git branches --all --fetch")
    .wln("  bulk-git branches -a")
    .wln("  bulk-git branches -r")
    .wln("  bulk-git branches --all -a")
    .wln("  bulk-git branches --all -a --fetch")
    .wln("  bulk-git branches --all -r")
    .wln("  bulk-git urls --with-directory")
    .wln("  bulk-git fetch")
    .wln(@"  bulk-git status --dir ""C:\Path to working directory\sources""")
    .ln().r();
    resetColor();
}
void pwd()
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.Write("Current Directory: ");
    resetColor();
    Console.WriteLine(Directory.GetCurrentDirectory());
}

(int ExitCode, string StdOut, string StdErr) runCommand(string command, string arguments, string? workingDirectory = null, bool printVerbose = false)
{
    var process = new Process
    {
        StartInfo = new ProcessStartInfo
        {
            FileName = command,
            Arguments = arguments,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        }
    };
    if (workingDirectory != null)
    {
        process.StartInfo.WorkingDirectory = workingDirectory;
    }
    StringBuilder output = new StringBuilder();
    StringBuilder error = new StringBuilder();

    //Console.WriteLine($"Pulling {gitDirectory} for branch ({getCurrentBranchName(gitDirectory)})...");
    process.OutputDataReceived += (sender, data) =>
    {
        if (data == null || data.Data == null || data.Data.Trim() == "")
        {
            return;
        }
        if (printVerbose)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("O: " + data.Data);
            resetColor();
        }
        output.AppendLine(data.Data);
    };
    process.ErrorDataReceived += (sender, data) =>
    {
        if (data == null || data.Data == null || data.Data.Trim() == "")
        {
            return;
        }
        if (printVerbose)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("E: " + data.Data);
            resetColor();
        }
        error.AppendLine(data.Data);
    };
    process.Start();
    //if (printVerbose)
    //{
    process.BeginOutputReadLine();
    process.BeginErrorReadLine();
    //}
    process.WaitForExit(); // İşlem tamamlanana kadar bekle
    process.WaitForExit(300); // Asenkron işleyicilerin tamamlanmasını sağlamak için 300 m.saniye daha bekle

    //if (printVerbose)
    //{
    return (process.ExitCode, output.ToString(), error.ToString());
    //}
    //else
    //{
    //    return (process.ExitCode, process.StandardOutput.ReadToEnd(), process.StandardError.ReadToEnd());
    //}
}
string getCurrentBranchName(string gitDirectory)
{
    return runCommand("git", $"-C {gitDirectory} rev-parse --abbrev-ref HEAD", gitDirectory, false).StdOut.Trim();
}

string[] _getDirectories()
{
    var r = Directory.GetDirectories(workingDirectory, ".git", SearchOption.AllDirectories).Select(
        x => Path.GetDirectoryName(x)!
        ).ToList();
    if (pathConditions.Count > 0)
    {
        r = r.Where(x =>
        {
            List<bool> matches = new List<bool>();
            foreach (var condition in pathConditions)
            {
                var path = x;
                var value = condition.value;
                if (condition.field == "path")
                {
                    path = x;
                }
                else if (condition.field == "url")
                {
                    path = getDefaultUrl(x);
                }
                else if (condition.field == "branch")
                {
                    path = getCurrentBranchName(x);
                }
                else
                {
                    throw new Exception("Unknown field: " + condition.field);
                }
                switch (condition.condition)
                {
                    case "ct":
                        matches.Add(path.ToLower().Contains(value.ToLower()));
                        break;
                    case "sw":
                        matches.Add(path.ToLower().StartsWith(value.ToLower()));
                        break;
                    case "ew":
                        matches.Add(path.ToLower().EndsWith(value.ToLower()));
                        break;
                    case "eq":
                        matches.Add(path.ToLower() == value.ToLower());
                        break;
                    case "!ct":
                        matches.Add(!path.ToLower().Contains(value.ToLower()));
                        break;
                    case "!sw":
                        matches.Add(!path.ToLower().StartsWith(value.ToLower()));
                        break;
                    case "!ew":
                        matches.Add(!path.ToLower().EndsWith(value.ToLower()));
                        break;
                    case "!eq":
                        matches.Add(path.ToLower() != value.ToLower());
                        break;
                    default:
                        throw new Exception("Unknown condition: " + condition.condition);
                }

            }
            //Check matches with conjunctions
            bool result = false;
            for (int i = 0; i < matches.Count; i++)
            {
                if (i == 0)
                {
                    result = matches[i];
                }
                else
                {
                    if (conditionConjunctions[i - 1] == "and")
                    {
                        result = result && matches[i];
                    }
                    else if (conditionConjunctions[i - 1] == "or")
                    {
                        result = result || matches[i];
                    }
                }
            }
            return result;
        }).ToList();
    }
    return r.ToArray();
}

(bool Success, string ErrMessage) pullGit(string gitDirectory)
{
    if (_printVerbose)
    {
        Console.WriteLine("");
    }
    var r = runCommand("git", $"-C {gitDirectory} pull", gitDirectory, _printVerbose);

    if (r.ExitCode != 0)
    {
        return (false, r.StdErr);
    }
    return (true, "");
}
void pull()
{
    var directories = _getDirectories();
    foreach (var gitDirectory in directories)
    {
        Console.Write($"Pulling {gitDirectory} for branch ({getCurrentBranchName(gitDirectory)})...");
        var result = pullGit(gitDirectory);
        if (!result.Success)
        {
            if (!_printVerbose)
            {
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(result.ErrMessage);
                resetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("[Failed]");
                resetColor();
            }
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[Success]");
            resetColor();
        }
    }
}
void stashes()
{
    var directories = _getDirectories();
    foreach (var gitDirectory in directories)
    {
        var r = runCommand("git", $"-C {gitDirectory} stash list", gitDirectory, _printVerbose);
        if (r.ExitCode != 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(r.StdErr);
            resetColor();
        }
        else
        {
            var stdOud = (r.StdOut ?? "").Trim(' ', '\r', '\n');
            if (string.IsNullOrEmpty(stdOud))
            {
                continue;
            }
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"{gitDirectory}");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(r.StdOut);
            resetColor();
        }
    }
}

void branches()
{
    var directories = _getDirectories();
    bool all = args.Contains("--all");
    bool fetch = args.Contains("--fetch");
    bool flaga = args.Contains("-a");
    bool flagr = args.Contains("-r");
    foreach (var gitDirectory in directories)
    {
        if (fetch)
        {
            resetColor();
            Console.Write($"Fetching for {gitDirectory}...");
            fetchAll(gitDirectory);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("[OK]");
        }
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write($"({getCurrentBranchName(gitDirectory)}) ");
        resetColor();
        Console.WriteLine($"{gitDirectory}");
        if (all)
        {
            //var r = runCommand("git", $"-C {gitDirectory} branch -a", gitDirectory, _printVerbose);
            //var r = runCommand("git", $"-C {gitDirectory} branch -r", gitDirectory, _printVerbose);
            var r =
                flaga
                ? runCommand("git", $"-C {gitDirectory} branch -a", gitDirectory, _printVerbose) :
                flagr
                ? runCommand("git", $"-C {gitDirectory} branch -r", gitDirectory, _printVerbose)
                : runCommand("git", $"-C {gitDirectory} branch", gitDirectory, _printVerbose);
            if (r.ExitCode != 0)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(r.StdErr);
                resetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine(r.StdOut);
                resetColor();
            }
        }
    }
}

void fetchAll(string gitDirectory)
{
    runCommand("git", $"-C {gitDirectory} fetch --all", gitDirectory, _printVerbose);
}
void fetch()
{
    var directories = _getDirectories();
    foreach (var gitDirectory in directories)
    {
        resetColor();
        Console.Write($"Fetching for {gitDirectory}...");
        fetchAll(gitDirectory);
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("[OK]");
    }
}
string getDefaultUrl(string gitDirectory)
{
    return runCommand("git", $"-C {gitDirectory} config --get remote.origin.url", gitDirectory, false).StdOut.Trim();
}

void urls()
{
    bool withDirectoryPath = args.Contains("--with-directory");
    var directories = _getDirectories();
    foreach (var gitDirectory in directories)
    {
        if (withDirectoryPath)
        {
            resetColor();
            Console.WriteLine($"{gitDirectory}");
        }
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(getDefaultUrl(gitDirectory) + " ");
        resetColor();
    }
}

void status()
{
    var directories = _getDirectories();
    /*
 Write-Host "(UC): Uncommited"
Write-Host "(UV): Unversioned"
Write-Host "(NP): Not Pushed yet"
 */
    Console.WriteLine("Listing status of repositories in the current directory that action needed...");
    Console.WriteLine("Meanings:");
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("[UC]: Uncommited");
    Console.ForegroundColor = ConsoleColor.DarkGray;
    Console.WriteLine("[UV]: Unversioned");
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("[NP]: Not Pushed yet");
    foreach (var gitDirectory in directories)
    {
        var uncommitted = runCommand("git", $"-C {gitDirectory} status --porcelain", gitDirectory, _printVerbose).StdOut.Trim();
        if (uncommitted != "")
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[UC]: {gitDirectory}");
            resetColor();
        }
        var unversioned = runCommand("git", $"-C {gitDirectory} ls-files --others --exclude-standard", gitDirectory, _printVerbose).StdOut.Trim();
        if (unversioned != "")
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"[UV]: {gitDirectory}");
            resetColor();
        }
        var localCommits = runCommand("git", $"-C {gitDirectory} cherry -v", gitDirectory, _printVerbose).StdOut.Trim();
        if (localCommits != "" && !localCommits.Contains("Could not find a tracked remote branch"))
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[NP]: {gitDirectory}");
            resetColor();
        }
    }
    resetColor();
    Console.WriteLine("End of Status list...");
}

void @switch()
{
    if (args.Length < 2)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Branch name is required for switch command.");
        printHelp();
        return;
    }
    var branchName = args[1];
    if (branchName == "")
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("Branch name is required for switch command.");
        printHelp();
        return;
    }
    var directories = _getDirectories();
    foreach (var gitDirectory in directories)
    {
        var currentBranch = getCurrentBranchName(gitDirectory);
        Console.Write($"Current branch {gitDirectory}: ");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.Write($"{currentBranch}.");
        resetColor();
        Console.Write(" Switching to ");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.Write($"{branchName}...");
        var r = runCommand("git", $"-C {gitDirectory} checkout {branchName}", gitDirectory, _printVerbose);
        if (r.ExitCode != 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(" " + r.StdErr?.Trim(' ', '\r', '\n'));
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(" [DONE]");
        }
        resetColor();

    }

}