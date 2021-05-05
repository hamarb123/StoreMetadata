using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using ICSharpCode.SharpZipLib.BZip2;
using SharpCompress.Archives.Tar;

namespace Compressor
{
	class Program
	{
		static void Main(string[] args)
		{
			bool notLower = false;
			if (args.Length > 1 && args[1] == "true") notLower = true;
			List<KeyValuePair<string, byte[]>> files = new();
			List<string> dirs = new();
			string arg0 = Path.GetFullPath(args[0]); //input directory
			void DoDirectory(string path)
			{
				foreach (var item in Directory.EnumerateFiles(path, "*", SearchOption.TopDirectoryOnly).Select((x) => x[(arg0.Length + 1)..].Replace('\\', '/')))
				{
					var name = item.Split('/')[^1];
					if (name.StartsWith("._")) continue;
					if (name == ".DS_Store") continue;
					if (name == "Thumbs.db") continue;
					var file = arg0 + Path.DirectorySeparatorChar + item.Replace('/', Path.DirectorySeparatorChar);
					var itemName = item;
					if (!notLower) itemName = itemName.ToLowerInvariant();
					files.Add(new(itemName, File.ReadAllBytes(file)));
				}
				foreach (var item in Directory.EnumerateDirectories(path, "*", SearchOption.TopDirectoryOnly).Select((x) => x[(arg0.Length + 1)..].Replace('\\', '/')))
				{
					var itemName = item;
					if (!notLower) itemName = itemName.ToLowerInvariant();
					dirs.Add(itemName);
					DoDirectory(arg0 + Path.DirectorySeparatorChar + item.Replace('/', Path.DirectorySeparatorChar));
				}
			}
			DoDirectory(arg0);
			dirs.Sort();
			files.Sort((x, y) => x.Key.CompareTo(y.Key));
			using MemoryStream ms = new();
			TarArchive archive = TarArchive.Create();
			foreach (var item in files)
			{
				MemoryStream ms2 = new(item.Value);
				var entry = archive.AddEntry(item.Key, ms2, item.Value.LongLength, DateTime.UnixEpoch);
			}
			archive.SaveTo(ms, new SharpCompress.Writers.WriterOptions(SharpCompress.Common.CompressionType.None));
			ms.Position = 0;
			using MemoryStream ms_bz = new();
			BZip2.Compress(ms, ms_bz, false, 9);
			File.WriteAllBytes(arg0 + ".tar.bz2", ms_bz.ToArray());
			Console.WriteLine("Updated " + arg0);
		}
	}
}
