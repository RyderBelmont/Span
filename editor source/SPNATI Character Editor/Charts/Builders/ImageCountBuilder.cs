﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace SPNATI_Character_Editor.Charts.Builders
{
	[Chart(ChartType.Bar, 10)]
	public class ImageCountBuilder : IChartDataBuilder
	{
		private List<Tuple<Character, int>> _data;
		private Regex _regex = new Regex(@"^[0-9]*-");

		public string GetLabel()
		{
			return "Images (Total)";
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "filename")]
		public void GenerateData()
		{
			List<Tuple<Character, int>> counts = new List<Tuple<Character, int>>();
			foreach (Character c in CharacterDatabase.Characters)
			{
				if (c.FolderName == "human") { continue; }
				int lines, poses;
				c.GetUniqueLineAndPoseCount(out lines, out poses);
				counts.Add(new Tuple<Character, int>(c, poses));
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
			return "Images Per Character (Used in Dialogue)";
		}

		public string[] GetViews()
		{
			return new string[] { "All" };
		}
	}
}
