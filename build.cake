#tool "nuget:?package=GitVersion.CommandLine"
var target = Argument("target", "Default");
var buildDirectory = Directory("./bin");

Task("Clean")
  .Does(() => {
    CleanDirectory(buildDirectory);
  });

Task("Build")
  .IsDependentOn("Clean")
  .Does(() => {
    var version = GitVersion();
    
    DownloadFile("http://download.red-gate.com/SQLCodeGuardCmdLine.zip", "./bin/SqlCodeGuard.zip");
    Unzip("./bin/SQLCodeGuard.zip", "./bin/SQLCodeGuard");

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

Task("Default")
  .IsDependentOn("Build");

RunTarget(target);