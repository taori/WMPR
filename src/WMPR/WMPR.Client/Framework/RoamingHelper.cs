using System;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace WMPR.Client.Framework
{
	public static class RoamingHelper
	{
		private const string MasterRoamingSection = "Warcraft Mechanics Performance Reporting";

		public static async Task<string> TryLoadContentAsync(params string[] paths)
		{
			var roamingPath = GetRoamedPath(paths);
			if (!File.Exists(roamingPath))
				return string.Empty;

			using (var stream = new StreamReader(new FileStream(roamingPath, FileMode.Open), Encoding.UTF8))
			{
				return await stream.ReadToEndAsync().ConfigureAwait(false);
			}
		}

		public static async Task TrySaveContentAsync(string content, params string[] paths)
		{
			var roamingPath = GetRoamedPath(paths);

			var folder = Path.GetDirectoryName(roamingPath);
			if (!Directory.Exists(folder) && !string.IsNullOrEmpty(folder))
			{
				Directory.CreateDirectory(folder);
			}

			using (var stream = new StreamWriter(new FileStream(roamingPath, FileMode.Create), Encoding.UTF8))
			{
				await stream.WriteAsync(content).ConfigureAwait(false);
			}
		}

		public static string GetRoamedPath(params string[] paths)
		{
			var combinedPath = Path.Combine(new[]
			{
				System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments, Environment.SpecialFolderOption.Create),
				MasterRoamingSection
			}.Concat(paths).ToArray());

			return combinedPath;
		}
	}
}