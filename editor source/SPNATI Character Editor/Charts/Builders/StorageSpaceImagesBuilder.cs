﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SPNATI_Character_Editor.Charts.Builders
{
	[Chart(ChartType.Bar, 15)]
	public class StorageSpaceImagesBuilder : IChartDataBuilder
	{
		private List<Tuple<Character, long>> _data;

		public string GetLabel()
		{
			return "Folder Size (MB), only images";
		}

		public void GenerateData()
		{
			List<Tuple<Character, long>> counts = new List<Tuple<Character, long>>();
			foreach (Character c in CharacterDatabase.Characters)
			{
				if (c.FolderName == "human") { continue; }
				long count = 0;
				string folder = Config.GetRootDirectory(c);
				DirectoryInfo directory = new DirectoryInfo(folder);
				foreach (FileInfo file in directory.EnumerateFiles()
					.Where(FilterPaths))
				{
					count += file.Length;
				}
				counts.Add(new Tuple<Character, long>(c, count));
			}

			_data = counts;
			_data.Sort((t1, t2) =>
			{
				return (t2.Item2).CompareTo(t1.Item2);
			});
		}

		private static bool FilterPaths(FileInfo f)
		{
			string extension = f.Extension.ToLower();
			if (extension == ".png" ||
				extension == ".gif" ||
				extension == ".jpg")
			{
				return true;
			}
			return false;
		}

		public List<List<ChartData>> GetSeries(string view)
		{
			List<List<ChartData>> data = new List<List<ChartData>>();
			List<ChartData> series0 = new List<ChartData>();

			data.Add(series0);
			for (int i = 0; i < _data.Count; i++)
			{
				var item = _data[i];
				series0.Add(new ChartData(item.Item1.Label, item.Item2 / 1048576.0f));
			}

			return data;
		}

		public string GetTitle()
		{
			return "Total File Size (MB)";
		}

		public string[] GetViews()
		{
			return new string[] { "All" };
		}
	}
}
