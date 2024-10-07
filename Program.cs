﻿// See https://aka.ms/new-console-template for more information
using System.Diagnostics;
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
    Console.WriteLine("            2024-10-06");
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
        if (args[whereIndex] == "and" || args[whereIndex] == "or")
        {
            if (waitingFor == 0)
            {
                conditionConjunctions.Add(args[whereIndex]);
                //Remove "and" or "or" from args
                removeFieldFromArgs();
                continue;
            }
            else
            {
                err("and/or");
                continue;
            }
        }
        else
        if (acceptedFields.Contains(args[whereIndex]))
        {
            if (waitingFor != 0)
            {
                err(args[whereIndex]);
                continue;
            }
            field = args[whereIndex];
            removeFieldFromArgs();//Remove field from args
            waitingFor = 1;
        }
        else if (acceptedConditions.Contains(args[whereIndex]))
        {
            if (waitingFor != 1)
            {
                err(args[whereIndex]);
                continue;
            }
            condition = args[whereIndex];
            removeFieldFromArgs();//Remove condition from args
            waitingFor = 2;
        }
        else //This is neither field nor condition
        {
            if (waitingFor == 0)
            {
                //Then end of the where clause. We can exit the loop and keep args as is for the next steps
                isWhereClauseCompleted = true;
                continue;
            }
            if (waitingFor != 2)
            {
                err(args[whereIndex]);
                continue;
            }
            value = args[whereIndex];
            if (isFieldsCompleted())
            {
                pathConditions.Add((field, condition, value));
                field = "";
                condition = "";
                value = "";
                waitingFor = 0;
            }
            else
            {
                err(args[whereIndex]);
                continue;
            }
            removeFieldFromArgs();//Remove value from args
            waitingFor = 0;
        }
        if (whereIndex >= args.Length)
        {
            if (waitingFor != 0)
            {
                //Fields  not completed but end of the args
                err("end of args");
                continue;
            }
            else
            {
                isWhereClauseCompleted = true;
            }
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
void printHelp()
{
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine("Bulk Git - Bulk git operations for multiple repositories in a directory");
    Console.WriteLine();
    Console.WriteLine("Usage: bulk-git [command] [options]");
    Console.WriteLine();
    Console.WriteLine("Commands:");
    Console.WriteLine("  pwd         Print current directory");
    Console.WriteLine("  pull        Pull all git repositories in the current directory");
    Console.WriteLine("  branches    List current branches in the current directory");
    Console.ForegroundColor = ConsoleColor.DarkGray;
    Console.WriteLine("    Options:");
    Console.WriteLine("      --all       Also list all other local branches for each repository");
    Console.WriteLine("      --fetch     Fetch all branches from remote before list");
    Console.WriteLine("      -a          [With --all parameter] List all local and remote branches");
    Console.WriteLine("      -r          [With --all parameter] List remote branches only");
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine("  urls        List all urls in the current directory");
    Console.ForegroundColor = ConsoleColor.DarkGray;
    Console.WriteLine("    Options:");
    Console.WriteLine("      --with-directory   Print directory path with urls");
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine("  fetch       Fetch all branches in the current directory");
    Console.WriteLine("  status      Show status of repositories  in the current directory that action needed");
    Console.WriteLine("  switch      Switch all repositories to the given branch");
    Console.ForegroundColor = ConsoleColor.DarkGray;
    Console.WriteLine("    Arguments:");
    Console.WriteLine("      [branchName]   Branch name to switch all repositories");
    Console.WriteLine();
    Console.ForegroundColor = ConsoleColor.DarkGray;
    Console.WriteLine("General Options:");
    Console.WriteLine("  --verbose   Print verbose output");
    Console.WriteLine("  --dir [path]   Set working directory");
    Console.WriteLine("  --where [field] [condition] [value] [and/or] [field] [condition] [value] ...   Filter repositories with conditions");
    Console.WriteLine("    Fields: path, url, branch");
    Console.WriteLine("    Conditions: ct (contains), sw (starts with), ew (ends with), eq (equals), !ct (not contains), !sw (not starts with), !ew (not ends with), !eq (not equals)");
    Console.WriteLine("    Conjunctions: and, or");
    Console.WriteLine("    The comprassion is not case sensitive.");
    Console.WriteLine("    Example: --where path eq \"C:\\Projects\" and branch eq \"master\" or url sw \"http//\"");
    Console.WriteLine();
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("Examples:");
    Console.WriteLine("  bulk-git pull --verbose");
    Console.WriteLine("  bulk-git branches");
    Console.WriteLine("  bulk-git branches --all");
    Console.WriteLine("  bulk-git branches --all --fetch");
    Console.WriteLine("  bulk-git branches -a");
    Console.WriteLine("  bulk-git branches -r");
    Console.WriteLine("  bulk-git branches --all -a");
    Console.WriteLine("  bulk-git branches --all -a --fetch");
    Console.WriteLine("  bulk-git branches --all -r");
    Console.WriteLine("  bulk-git urls --with-directory");
    Console.WriteLine("  bulk-git fetch");
    Console.WriteLine(@"  bulk-git status --dir ""C:\Path to working directory\sources""");
    Console.WriteLine();
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

    if (printVerbose)
    {
        //Console.WriteLine($"Pulling {gitDirectory} for branch ({getCurrentBranchName(gitDirectory)})...");
        process.OutputDataReceived += (sender, data) =>
        {
            if (data == null || data.Data == null || data.Data.Trim() == "")
            {
                return;
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("O: " + data.Data);
            output.AppendLine(data.Data);
            resetColor();
        };
        process.ErrorDataReceived += (sender, data) =>
        {
            if (data == null || data.Data == null || data.Data.Trim() == "")
            {
                return;
            }
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("E: " + data.Data);
            error.AppendLine(data.Data);
            resetColor();
        };
    }
    process.Start();
    if (printVerbose)
    {
        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
    }
    process.WaitForExit(); // İşlem tamamlanana kadar bekle
    process.WaitForExit(300); // Asenkron işleyicilerin tamamlanmasını sağlamak için 300 m.saniye daha bekle

    if (printVerbose)
    {
        return (process.ExitCode, output.ToString(), error.ToString());
    }
    else
    {
        return (process.ExitCode, process.StandardOutput.ReadToEnd(), process.StandardError.ReadToEnd());
    }
}
string getCurrentBranchName(string gitDirectory)
{
    return runCommand("git", $"-C {gitDirectory} rev-parse --abbrev-ref HEAD", gitDirectory, false).StdOut.Trim();
}

string[] _getDirectories()
{
    var r =  Directory.GetDirectories(workingDirectory, ".git", SearchOption.AllDirectories).Select(
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
                switch  (condition.condition)
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
        var uncommitted = runCommand("git", $"-C {gitDirectory} status --porcelain", gitDirectory, false).StdOut.Trim();
        if (uncommitted != "")
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[UC]: {gitDirectory}");
            resetColor();
        }
        var unversioned = runCommand("git", $"-C {gitDirectory} ls-files --others --exclude-standard", gitDirectory, false).StdOut.Trim();
        if (unversioned != "")
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"[UV]: {gitDirectory}");
            resetColor();
        }
        var localCommits = runCommand("git", $"-C {gitDirectory} cherry -v", gitDirectory, false).StdOut.Trim();
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