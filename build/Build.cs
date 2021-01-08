using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.Coverlet;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Utilities.Collections;

using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.Tools.Git.GitTasks;

[CheckBuildProjectConfigurations]
[UnsetVisualStudioEnvironmentVariables]
class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode


    public static int Main () => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Parameter("Feed url for publishing nuget packages", Name = "feed")] 
    readonly string NugetFeed;

    [Parameter("API Key for publishing nuget packages", Name = "apikey")] 
    readonly string NugetApiKey;

    [Required] [Solution] readonly Solution Solution;
    [Required] [GitVersion(NoFetch = true, Framework = "net5.0")] GitVersion CurrentVersion;

    AbsolutePath SourceDirectory => RootDirectory / "src";
    AbsolutePath TestsDirectory => RootDirectory / "test";
    AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";

    Target Clean => _ => _
        .DependentFor(Restore)
        .Before(Restore)
        .Executes(() =>
        {
            SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            TestsDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            EnsureCleanDirectory(ArtifactsDirectory);
        });

    Target Restore => _ => _
        .Executes(() =>
        {
            DotNetRestore(s => s
                .SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            DotNetBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration)
                .SetAssemblyVersion(CurrentVersion.AssemblySemVer)
                .SetFileVersion(CurrentVersion.AssemblySemFileVer)
                .SetInformationalVersion(CurrentVersion.InformationalVersion)
                .EnableNoRestore()
                .SetNoRestore(true)
            );
        });

    Target Test => _ => _
        .DependsOn(Compile)
        .Produces(ArtifactsDirectory / "*.trx")
        .Produces(ArtifactsDirectory / "*.xml")
        .Executes(() =>
        {   
            DotNetTest(s => s
                .SetConfiguration(Configuration)
                .SetNoBuild(InvokedTargets.Contains(Compile))
                .ResetVerbosity()
                .SetResultsDirectory(ArtifactsDirectory)
                .EnableCollectCoverage()
                .SetCoverletOutputFormat(CoverletOutputFormat.opencover)
                    .When(IsServerBuild, _ => _.EnableUseSourceLink())
                .SetNoRestore(true)
                .SetNoBuild(true)
                .CombineWith(Solution.GetProjects("*.Tests"), (_, v) => _
                    .SetProjectFile(v)
                    .SetLogger($"trx;LogFileName={v.Name}.trx")
                    .SetCoverletOutput(ArtifactsDirectory / $"{v.Name}.coverage.xml")
                )
            );
        });

    Target Pack => _ => _
        .DependsOn(Test)
        .Produces(ArtifactsDirectory / "*.nupkg")
        .Executes(() =>
        {
            DotNetPack( s => s
                .SetProject(Solution)
                .SetNoBuild(true)
                .SetConfiguration(Configuration)
                .SetOutputDirectory(ArtifactsDirectory)
                .SetVersion(CurrentVersion.NuGetVersionV2)
            );
        });

    Target Publish => _ => _
        .DependsOn(Pack)
        .Consumes(Pack)
        .OnlyWhenStatic(() => GitHasCleanWorkingCopy())
        .OnlyWhenDynamic(() => GitCurrentBranch() == "master")
        .Requires(() => NugetApiKey)
        .Requires(() => NugetFeed)
        .Requires(() => Configuration.Equals(Configuration.Release))
        .Executes(() =>
        {
            DotNetNuGetPush(_ => _
                .SetSource(NugetFeed)
                .SetApiKey(NugetApiKey)
                .CombineWith(ArtifactsDirectory.GlobFiles("*.nupkg"), (_, v) => _
                    .SetTargetPath(v)
                )
            );
        });    
} 