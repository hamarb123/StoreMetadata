using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace DotnetInstallerLocator
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.Write("Please enter the loctation for the information file: ");
			string contents = File.ReadAllText(Console.ReadLine());
			string[] vs = contents.Split('\n');
			string gitRepo = vs[0].TrimEnd('\r');
			string outputFile = vs[1];
			Process.Start(new ProcessStartInfo("git", "pull") { WorkingDirectory = gitRepo }).WaitForExit();
			string dotnetRuntimeFolder = Path.Combine(gitRepo, "manifests", "m", "Microsoft", "dotnetRuntime");
			List<(Version version, string arch, string url, string switches)> li = new();
			foreach (var folder_ in Directory.EnumerateDirectories(dotnetRuntimeFolder, "*", SearchOption.TopDirectoryOnly))
			{
				foreach (var folder in Directory.EnumerateDirectories(folder_, "*", SearchOption.TopDirectoryOnly))
				{
					string manifest = null;
					string installer = null;
					foreach (var file in Directory.EnumerateFiles(folder, "*", SearchOption.TopDirectoryOnly))
					{
						string name = Path.GetFileName(file);
						if (name.Contains("locale")) continue;
						if (name.EndsWith(".installer.yaml")) installer = file;
						else if (name.EndsWith(".yaml")) manifest = file;
					}

					//sketchy yaml parsing:

					string[] versionArr = File.ReadAllText(manifest).Split('\n').Select((x) => x = x.Trim()).Where((x) => x.StartsWith("PackageVersion:")).Select((x) => x[("PackageVersion:".Length)..].Trim()).ToArray();
					if (versionArr.Length != 1) throw new Exception($"Found more or less than one version in file '{ manifest }': { string.Join(", ", versionArr) }");
					string version = versionArr[0];

					string[] architectureArr = File.ReadAllText(installer).Split('\n').Select((x) => x = x.Trim()).Where((x) => x.StartsWith("Architecture:")).Select((x) => x[("Architecture:".Length)..].Trim()).ToArray();
					if (architectureArr.Length != 1) throw new Exception($"Found more or less than one architecture in file '{ manifest }': { string.Join(", ", versionArr) }");
					string architecture = architectureArr[0];

					string[] urlArr = File.ReadAllText(installer).Split('\n').Select((x) => x = x.Trim()).Where((x) => x.StartsWith("InstallerUrl:")).Select((x) => x[("InstallerUrl:".Length)..].Trim()).ToArray();
					if (urlArr.Length != 1) throw new Exception($"Found more or less than one url in file '{ manifest }': { string.Join(", ", versionArr) }");
					string url = urlArr[0];

					string[] switchesArr = File.ReadAllText(installer).Split('\n').Select((x) => x = x.Trim()).Where((x) => x.StartsWith("SilentWithProgress:")).Select((x) => x[("SilentWithProgress:".Length)..].Trim()).ToArray();
					if (switchesArr.Length != 1) throw new Exception($"Found more or less than one switch (for SilentWithProgress) in file '{ manifest }': { string.Join(", ", versionArr) }");
					string switches = switchesArr[0];

					li.Add((Version.Parse(version), architecture, url, switches));
				}
			}
			li = li.OrderBy((x) => (x.version, x.arch)).GroupBy((x) => (new Version(x.version.Major, x.version.Minor), x.arch)).Select((g) => g.OrderByDescending((x) => x.version).First()).ToList();
			StringBuilder sb = new();
			foreach (var item in li)
			{
				sb.Append(new Version(item.version.Major, item.version.Minor).ToString() + " " + item.arch + " " + item.url + " " + item.switches);
				sb.Append('\n');
			}
			string str = sb.ToString();
			Console.WriteLine("Does this look correct:\n" + str);
			if (Console.ReadKey().Key == ConsoleKey.Y) File.WriteAllText(outputFile, str);
		}
	}
}
