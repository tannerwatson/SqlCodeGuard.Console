#tool "nuget:?package=GitVersion.CommandLine"
var target = Argument("Target", "Default");
var nuGetApiKey = Argument("NuGetApiKey", "SET_API_KEY");
var buildDirectory = Directory("./bin");

Task("Clean")
  .Does(() => {
    CleanDirectory(buildDirectory);
  });

Task("Version")
  .Does(() => {
    GitVersion(new GitVersionSettings { OutputType = GitVersionOutput.BuildServer});
  });

Task("Build")
  .IsDependentOn("Clean")
  .IsDependentOn("Version")
  .Does(() => {
    DownloadFile("http://download.red-gate.com/SQLCodeGuardCmdLine.zip", "./bin/SqlCodeGuard.zip");
    Unzip("./bin/SQLCodeGuard.zip", "./bin/SQLCodeGuard");
  });

Task("Package")
  .IsDependentOn("Build")
  .Does(() => {
    var version = GitVersion();

    var nuGetPackSettings = new NuGetPackSettings {
      Id = "SqlCodeGuard.Console",
      Version = version.NuGetVersionV2,
      Title = "SqlCodeGuard.Console",
      Authors = new[] {"Tanner Watson"},
      Description = "Packaged RedGate's SQL Code Guard",
      ProjectUrl = new Uri("https://github.com/tannerwatson/SqlCodeGuard.Console/"),
      Copyright = "Copyright (c) .NET Foundation and contributors",
      Tags = new [] {"cake", "sqlcodeguard", "build", "redgate"},
      LicenseUrl = new Uri("https://github.com/tannerwatson/SqlCodeGuard.Console/blob/master/LICENSE"),
      RequireLicenseAcceptance = false,
      Files = new [] {
        new NuSpecContent {Source = "./License.rtf", Target = "tools"},
        new NuSpecContent {Source = "./Microsoft.SqlServer.TransactSql.ScriptDom.dll", Target = "tools"},
        new NuSpecContent {Source = "./SqlCodeGuard30.API.dll", Target = "tools"},
        new NuSpecContent {Source = "./SqlCodeGuard30.Cmd.exe", Target = "tools"},
        new NuSpecContent {Source = "./SqlCodeGuard30.Core.dll", Target = "tools"},
        new NuSpecContent {Source = "./SqlCodeGuard30.Ouroboros.dll", Target = "tools"},
      },
      BasePath = Directory("./bin/SqlCodeGuard"),
      OutputDirectory = buildDirectory
      };
  
    NuGetPack(nuGetPackSettings);
  });

Task("Publish")
  .IsDependentOn("Package")
  .Does(() => {
    var version = GitVersion();
    var package = "./bin/SqlCodeGuard.Console." + version.NuGetVersionV2 + ".nupkg";

    NuGetPush(package, new NuGetPushSettings {
     Source = "https://api.nuget.org/v2/package/",
     ApiKey = nuGetApiKey
    });

  });

Task("Default")
  .IsDependentOn("Build");

RunTarget(target);