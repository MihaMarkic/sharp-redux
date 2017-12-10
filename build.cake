#addin "Cake.FileHelpers"

var configuration = Argument("Configuration", "Release");

var Src = Directory("./src");
var SharpRedux = Src + Directory("./Sharp.Redux/");
var SharpReduxCsproj = SharpRedux + File("Sharp.Redux.csproj");
var SharpVisualizerRedux = Src + Directory("./Sharp.Redux.Visualizer/");
var SharpVisualizerReduxCsproj = SharpVisualizerRedux + File("Sharp.Redux.Visualizer.csproj");
var VisualizerWpf = Src + Directory("./Sharp.Redux.Visualizer.Wpf/");
var VisualizerWpfCsproj = VisualizerWpf + File("Sharp.Redux.Visualizer.Wpf.csproj");
var visualizerWpfNuSpec = VisualizerWpf + File("Sharp.Redux.Visualizer.Wpf.nuspec");

var Test = Directory("./test/");
var SharpReduxTest = Test + File("Sharp.Redux.Test/Sharp.Redux.Test.csproj");
var SharpReduxVisualizerTest = Test + File("Sharp.Redux.Visualizer.Test/Sharp.Redux.Visualizer.Test.csproj");

var solution = Src + File("Sharp.Redux.sln");

var nupkg = Directory("./nupkg");

var target = Argument("target", "Default");

Task("Restore")
	.Does(() => {
 		DotNetCoreRestore(solution);
	});

Task("Build")
	.IsDependentOn("Restore")
	.Does (() =>
	{
		MSBuild (solution, new MSBuildSettings {
			Configuration = configuration,
			Verbosity = Verbosity.Minimal,
			ToolVersion = MSBuildToolVersion.VS2017
		});
});

Task("UnitTest")
	.IsDependentOn("Build")
	.Does(() =>
	{
		//NUnit3(CakeTestDockerAssembly);
		DotNetCoreTest(SharpReduxTest, new DotNetCoreTestSettings {
			Configuration = configuration,
			NoBuild = true
		});
	});

Task("Pack")
	.IsDependentOn("UnitTest")
	.Does (() =>
{
	CreateDirectory(nupkg);
	foreach (var project in new []{ SharpReduxCsproj, SharpVisualizerReduxCsproj })
	{
		DotNetCorePack (project, new DotNetCorePackSettings { 
			Configuration = configuration,
			OutputDirectory = nupkg,
			NoBuild = true
		});	
	}
	// separately pack wpf visualizer since it's not a .netstandard or .netcore project
	NuGetPack (visualizerWpfNuSpec, new NuGetPackSettings { 
		Version = "1.0.0-alpha1",
		Verbosity = NuGetVerbosity.Normal,
		OutputDirectory = nupkg,
		BasePath = VisualizerWpf,
		Symbols=true,
	});	
});

RunTarget (target);
