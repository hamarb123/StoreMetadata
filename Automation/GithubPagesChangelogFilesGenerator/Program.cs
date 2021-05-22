using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace GithubPagesChangelogFilesGenerator
{
	class Program
	{
		static void Main(string[] args)
		{
			var path = Path.GetFullPath(args[0]);
			var project = args[1];
			if (project == "2dcraft")
			{
				Run(path + Path.DirectorySeparatorChar + "2dcraft" + Path.DirectorySeparatorChar + "changelog", "v2/hamarb123/2dcraft/changelogs", new string[0], new string[] { "snapshot.txt" });
			}
			else if (project == "store")
			{
				Run(path + Path.DirectorySeparatorChar + "store" + Path.DirectorySeparatorChar + "changelog", "v2/hamarb123/store/changelogs", new string[0], new string[0]);
			}
			else if (project == "chemistrylearner")
			{
				Console.WriteLine("Not implemented"); //likely to never be implemented
			}
			else if (project == "utils")
			{
				Run(path + Path.DirectorySeparatorChar + "utils" + Path.DirectorySeparatorChar + "changelog", "v2/hamarb123/utils/changelogs", new string[0], new string[0]);
			}
			else Console.WriteLine("Unsupported");
		}

		static void Run(string changelogHtmlDir, string changelogTxtDir, string[] ignoreNames, string[] dontRename)
		{
			string gitRepoRoot = Directory.GetParent(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)).Parent.Parent.Parent.Parent.FullName; //StoreMetadata folder
			var txtDir = Path.Combine(gitRepoRoot, changelogTxtDir.Replace('/', Path.DirectorySeparatorChar));
			Directory.Delete(changelogHtmlDir, true);
			Directory.CreateDirectory(changelogHtmlDir);
			foreach (var file in Directory.EnumerateFiles(txtDir, "*.txt", SearchOption.AllDirectories))
			{
				var name = Path.GetFileName(file);
				if (name.StartsWith("._")) continue;
				if (ignoreNames.Contains(name)) continue;
				var text = File.ReadAllText(file).Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;");
				text = @"<pre style=""width: 100%; font-family: Comfortaa; font-size: 17pt; white-space: pre-wrap; -ms-word-wrap: break-word;"">" + text + "</pre>";
				File.WriteAllBytes(Path.Combine(changelogHtmlDir, (dontRename.Contains(name) ? "" : "v") + name.Replace(".txt", ".html")), Encoding.UTF8.GetBytes(text));
			}
		}
	}
}
