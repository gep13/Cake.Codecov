//------------------------------------------------------------------------------
// codecov.cake — exercise script for Cake.Codecov.
//
// HOW TO RUN (from repo root)
//   dotnet build Source/Cake.Codecov/Cake.Codecov.csproj -f net10.0 -c Debug
//   dotnet tool restore
//   dotnet cake codecov.cake
//
// WHAT IT DOES
//   Loads the addin and exercises the Codecov(settings) alias with
//   DryRun=true. A stub coverage XML is created if one isn't already
//   present. No actual upload to codecov.io occurs.
//------------------------------------------------------------------------------

#tool nuget:?package=Codecov&version=1.13.0
#reference "Source/Cake.Codecov/bin/Debug/net10.0/Cake.Codecov.dll"

using Cake.Codecov;

var coverageFile = File("./BuildArtifacts/temp/codecov-stub.xml");

Task("Default")
    .IsDependentOn("Setup-Stub-Coverage")
    .IsDependentOn("Verify-Addin-Loaded")
    .IsDependentOn("Codecov-DryRun");

Task("Setup-Stub-Coverage")
    .Does(() =>
{
    if (!FileExists(coverageFile))
    {
        EnsureDirectoryExists(coverageFile.GetDirectory());
        System.IO.File.WriteAllText(
            coverageFile.Path.FullPath,
            "<?xml version=\"1.0\"?>\n<coverage line-rate=\"1.0\"></coverage>");
    }

    Information("Stub coverage file ready: {0}", coverageFile.Path.FullPath);
});

Task("Verify-Addin-Loaded")
    .Does(() =>
{
    Information("Cake.Codecov addin loaded.");
    Information("CodecovSettings type: {0}", typeof(CodecovSettings).FullName);
});

Task("Codecov-DryRun")
    .Does(() =>
{
    var settings = new CodecovSettings
    {
        Branch = "main",
        Build = "exercise-1",
        Commit = "0000000000000000000000000000000000000000",
        Name = "Cake.Codecov exercise",
        Flags = "exercise",
        DryRun = true,
        Token = "exercise-token",
        Files = new[] { coverageFile.Path.FullPath },
    };

    Information("Calling Codecov(settings) with DryRun=true...");
    Codecov(settings);
    Information("Codecov(settings) completed.");
});

RunTarget("Default");
