using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

[CheckBuildProjectConfigurations]
[ShutdownDotNetAfterServerBuild]
internal class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main() => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    private readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Solution] private readonly Solution Solution;
    [GitRepository] private readonly GitRepository GitRepository;
    [GitVersion] private readonly GitVersion GitVersion;

    private AbsolutePath TestsDirectory => RootDirectory / "tests";
    private AbsolutePath ArtifactsDirectory => RootDirectory / "artifacts";

    private Target Clean => _ => _
         .Before(Restore)
         .Executes(() =>
         {
             TestsDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
             EnsureCleanDirectory(ArtifactsDirectory);
         });

    private Target Restore => _ => _
         .Executes(() =>
         {
             DotNetRestore(s => s
                 .SetProjectFile(Solution));
         });

    private Target Compile => _ => _
         .DependsOn(Restore)
         .Executes(() =>
         {
             DotNetBuild(s => s
                 .SetProjectFile(Solution)
                 .SetConfiguration(Configuration)
                 .SetAssemblyVersion(GitVersion.AssemblySemVer)
                 .SetFileVersion(GitVersion.AssemblySemFileVer)
                 .SetInformationalVersion(GitVersion.InformationalVersion)
                 .EnableNoRestore());
         });

    private Target Test => _ => _
         .DependsOn(Compile)
         .Executes(() =>
         {
             DotNetTest(s => s
                 .SetProjectFile(Solution)
                 .SetConfiguration(Configuration)
                 .EnableNoBuild());
         });
}