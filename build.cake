var target = Argument("Target", "Default");
var nuGetApiKey = Argument("NuGetApiKey", "NOT SET");
var buildDirectory = Directory("./bin");
var version = Argument("Version", "0.0.1");

Task("Clean")
  .Does(() => {
    CleanDirectory(buildDirectory);
  });

Task("Build")
  .IsDependentOn("Clean")
  .Does(() => {
    DownloadFile("https://files.red-gate.com/messages/vtBLIs2P07QqpKyB4DPyih/attachments/S93YraiTbbZyRoeD79yEZ4/download/SCG4.p3.zip", "./bin/SqlCodeGuard.zip");
    Unzip("./bin/SQLCodeGuard.zip", "./bin/SQLCodeGuard");
  });

Task("Package")
  .IsDependentOn("Build")
  .Does(() => {
    var nuGetPackSettings = new NuGetPackSettings {
      Id = "SqlCodeGuard.Console",
      Version = version,
      Title = "SqlCodeGuard.Console",
      Authors = new[] {"Tanner Watson"},
      Description = "Packaged RedGate's SQL Code Guard command line tools.",
      ProjectUrl = new Uri("https://github.com/tannerwatson/SqlCodeGuard.Console/"),
      Tags = new [] {"cake", "sqlcodeguard", "build", "redgate"},
      RequireLicenseAcceptance = false,
      Files = new [] {
        new NuSpecContent {Source = "./License.rtf", Target = "tools"},
        new NuSpecContent {Source = "./Microsoft.SqlServer.TransactSql.ScriptDom.dll", Target = "tools"},
        new NuSpecContent {Source = "./SampleProject.msbuild", Target = "tools"},
        new NuSpecContent {Source = "./SqlCodeGuard30.API.dll", Target = "tools"},
        new NuSpecContent {Source = "./SqlCodeGuard30.Cmd.exe", Target = "tools"},
        new NuSpecContent {Source = "./SqlCodeGuard30.Core.dll", Target = "tools"},
        new NuSpecContent {Source = "./SqlCodeGuard30.MSBuild.dll", Target = "tools"},
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
    var package = File($"./bin/SqlCodeGuard.Console.{version}.nupkg");

    NuGetPush(package, new NuGetPushSettings {
     Source = "https://api.nuget.org/v3/index.json",
     ApiKey = nuGetApiKey
    });
  });

Task("Default")
  .IsDependentOn("Package");

RunTarget(target);