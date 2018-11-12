
//////////////////////////////////////////////////////////////////////
// TOOLS / ADDINS
//////////////////////////////////////////////////////////////////////

#tool paket:?package=GitVersion.CommandLine
#tool paket:?package=gitreleasemanager
#tool paket:?package=vswhere
#addin paket:?package=Cake.Figlet
#addin paket:?package=Cake.Paket

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
if (string.IsNullOrWhiteSpace(target))
{
    target = "Default";
}

var configuration = Argument("configuration", "Release");
if (string.IsNullOrWhiteSpace(configuration))
{
    configuration = "Release";
}

var verbosity = Argument("verbosity", Verbosity.Normal);
if (string.IsNullOrWhiteSpace(configuration))
{
    verbosity = Verbosity.Normal;
}

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

var repoName = "MahApps.Metro.SimpleChildWindow";
var local = BuildSystem.IsLocalBuild;

// Set build version
if (local == false
    || verbosity == Verbosity.Verbose)
{
    GitVersion(new GitVersionSettings { OutputType = GitVersionOutput.BuildServer });
}
GitVersion gitVersion = GitVersion(new GitVersionSettings { OutputType = GitVersionOutput.Json });

var latestInstallationPath = VSWhereProducts("*", new VSWhereProductSettings { Version = "[\"15.0\",\"16.0\"]" }).FirstOrDefault();
var msBuildPath = latestInstallationPath.CombineWithFilePath("./MSBuild/15.0/Bin/MSBuild.exe");

var isPullRequest = AppVeyor.Environment.PullRequest.IsPullRequest;
var branchName = gitVersion.BranchName;
var isDevelopBranch = StringComparer.OrdinalIgnoreCase.Equals("dev", branchName);
var isReleaseBranch = StringComparer.OrdinalIgnoreCase.Equals("master", branchName);
var isTagged = AppVeyor.Environment.Repository.Tag.IsTag;

// Version
var nugetVersion = isReleaseBranch ? gitVersion.MajorMinorPatch : gitVersion.NuGetVersion;

// Directories and Paths
var solution = "./src/MahApps.Metro.SimpleChildWindow.sln";
var publishDir = "./Publish";

// Define global marcos.
Action Abort = () => { throw new Exception("a non-recoverable fatal error occurred."); };

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup(context =>
{
    if (!IsRunningOnWindows())
    {
        throw new NotImplementedException($"{repoName} will only build on Windows because it's not possible to target WPF and Windows Forms from UNIX.");
    }

    Information(Figlet(repoName));

    Information("Informational Version  : {0}", gitVersion.InformationalVersion);
    Information("SemVer Version         : {0}", gitVersion.SemVer);
    Information("AssemblySemVer Version : {0}", gitVersion.AssemblySemVer);
    Information("MajorMinorPatch Version: {0}", gitVersion.MajorMinorPatch);
    Information("NuGet Version          : {0}", gitVersion.NuGetVersion);
    Information("IsLocalBuild           : {0}", local);
    Information("Branch                 : {0}", branchName);
    Information("Configuration          : {0}", configuration);
    Information("MSBuildPath            : {0}", msBuildPath);
});

Teardown(context =>
{
    // Executed AFTER the last task.
});

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .ContinueOnError()
    .Does(() =>
{
    var directoriesToDelete = GetDirectories("./**/obj").Concat(GetDirectories("./**/bin")).Concat(GetDirectories("./**/Publish"));
    DeleteDirectories(directoriesToDelete, new DeleteDirectorySettings { Recursive = true, Force = true });
});

Task("Restore")
    .Does(() =>
{
    PaketRestore();

    var msBuildSettings = new MSBuildSettings { ToolPath = msBuildPath, ArgumentCustomization = args => args.Append("/m") };
    MSBuild(solution, msBuildSettings
            //.SetConfiguration(configuration)
            .SetVerbosity(Verbosity.Minimal)
            .WithTarget("restore")
            );
});

Task("Build")
    .Does(() =>
{
    var msBuildSettings = new MSBuildSettings { ToolPath = msBuildPath, ArgumentCustomization = args => args.Append("/m") };
    MSBuild(solution, msBuildSettings
                .SetMaxCpuCount(0)
                .SetConfiguration(configuration)
                .SetVerbosity(Verbosity.Normal)
                //.WithRestore() only with cake 0.28.x            
                .WithProperty("AssemblyVersion", gitVersion.AssemblySemVer)
                .WithProperty("FileVersion", gitVersion.AssemblySemFileVer)
                .WithProperty("InformationalVersion", gitVersion.InformationalVersion)
                );
});

Task("Pack")
    .WithCriteria(() => !isPullRequest)
    .Does(() =>
{
	EnsureDirectoryExists(Directory(publishDir));

	// PaketPack(publishDir, new PaketPackSettings { Version = isReleaseBranch ? gitVersion.MajorMinorPatch : gitVersion.NuGetVersion });

    var msBuildSettings = new MSBuildSettings { ToolPath = msBuildPath };
    var project = "./src/MahApps.Metro.SimpleChildWindow/MahApps.Metro.SimpleChildWindow.csproj";

    MSBuild(project, msBuildSettings
      .SetConfiguration(configuration)
      .SetVerbosity(Verbosity.Normal)
      .WithTarget("pack")
      .WithProperty("PackageOutputPath", "../../" + publishDir)
      .WithProperty("RepositoryBranch", branchName)
      .WithProperty("RepositoryCommit", gitVersion.Sha)
      //.WithProperty("Description", "The goal of MahApps.Metro is to allow devs to quickly and easily cobble together a 'Modern' UI for their WPF apps (>= .Net 4.5), with minimal effort.")
      .WithProperty("Version", isReleaseBranch ? gitVersion.MajorMinorPatch : gitVersion.NuGetVersion)
      .WithProperty("AssemblyVersion", gitVersion.AssemblySemVer)
      .WithProperty("FileVersion", gitVersion.AssemblySemFileVer)
      .WithProperty("InformationalVersion", gitVersion.InformationalVersion)
    );
});

Task("Zip")
    .Does(() =>
{
	EnsureDirectoryExists(Directory(publishDir));
    Zip("./src/MahApps.Metro.SimpleChildWindow.Demo/bin/" + configuration, publishDir + "/MahApps.Metro.SimpleChildWindow-v" + gitVersion.NuGetVersion + ".zip");
});

Task("CreateRelease")
    .WithCriteria(() => !isTagged)
    .Does(() =>
{
    var username = EnvironmentVariable("GITHUB_USERNAME");
    if (string.IsNullOrEmpty(username))
    {
        throw new Exception("The GITHUB_USERNAME environment variable is not defined.");
    }

    var token = EnvironmentVariable("GITHUB_TOKEN");
    if (string.IsNullOrEmpty(token))
    {
        throw new Exception("The GITHUB_TOKEN environment variable is not defined.");
    }

    GitReleaseManagerCreate(username, token, "punker76", repoName, new GitReleaseManagerCreateSettings {
        Milestone         = gitVersion.MajorMinorPatch,
        Name              = gitVersion.AssemblySemFileVer,
        Prerelease        = isDevelopBranch,
        TargetCommitish   = branchName,
        WorkingDirectory  = "."
    });
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore")
    .IsDependentOn("Build");

Task("appveyor")
    .IsDependentOn("Default")
    .IsDependentOn("Pack")
    .IsDependentOn("Zip");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
