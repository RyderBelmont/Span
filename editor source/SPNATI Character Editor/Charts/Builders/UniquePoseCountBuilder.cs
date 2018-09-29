﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace SPNATI_Character_Editor.Charts.Builders
{
	[Chart(ChartType.Bar, 12)]
	public class UniquePoseCountBuilder : IChartDataBuilder
	{
		private List<Tuple<Character, int>> _data;
		private Regex _regex = new Regex(@"^[0-9]*-");

		public string GetLabel()
		{
			return "Unique Poses";
		}

		private bool Filter(string filename)
		{
			string ext = Path.GetExtension(filename);
			if (ext != ".png" && ext != ".gif")
				return false;
			//exclude epilogue images. Note these might not actually be epilogue images, but it's a best guess
			string name = Path.GetFileNameWithoutExtension(filename);
			if (!_regex.IsMatch(name))
				return false;
			return true;
		}

		public void GenerateData()
		{
			List<Tuple<Character, int>> counts = new List<Tuple<Character, int>>();
			foreach (Character c in CharacterDatabase.Characters)
			{
				HashSet<string> poses = new HashSet<string>();
				string folder = Config.GetRootDirectory(c);
				foreach (string filename in Directory.EnumerateFiles(folder))
				{
					string name = Path.GetFileNameWithoutExtension(filename);
					int hyphen = name.IndexOf("-");
					name = name.Substring(hyphen + 1);
					poses.Add(name);
				}
				counts.Add(new Tuple<Character, int>(c, poses.Count));
			}

			_data = counts;
			_data.Sort((t1, t2) =>
			{
				return (t2.Item2).CompareTo(t1.Item2);
			});
		}

		public List<List<ChartData>> GetSeries(string view)
		{
			List<List<ChartData>> data = new List<List<ChartData>>();
			List<ChartData> series0 = new List<ChartData>();

			data.Add(series0);
			for (int i = 0; i < _data.Count; i++)
			{
				var item = _data[i];
				series0.Add(new ChartData(item.Item1.Label, item.Item2));
			}

			return data;
		}

		public string GetTitle()
		{
			return "Unique poses (based on image names)";
		}

		public string[] GetViews()
		{
			return new string[] { "All" };
		}
	}
}
