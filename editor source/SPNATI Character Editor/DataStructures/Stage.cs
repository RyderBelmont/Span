﻿using SPNATI_Character_Editor.IO;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SPNATI_Character_Editor
{
	/// <summary>
	/// Data representation of a single dialogue stage
	/// </summary>
	public class Stage
	{
		public const int Default = 99;

		[XmlAttribute("id")]
		public int Id;

		[XmlSortMethod("SortCases")]
		[XmlElement("case")]
		public List<Case> Cases;

		public Stage()
		{
			Cases = new List<Case>();
		}

		public Stage(int id)
		{
			Id = id;
			Cases = new List<Case>();
		}

		/// <summary>
		/// Sort method for XML output, which differs from editor sort order. This should sort in the same order as make_xml.py
		/// </summary>
		/// <param name="obj1"></param>
		/// <param name="obj2"></param>
		/// <returns></returns>
		public int SortCases(object obj1, object obj2)
		{
			Case c1 = obj1 as Case;
			Case c2 = obj2 as Case;
			Trigger t1 = TriggerDatabase.GetTrigger(c1.Tag);
			Trigger t2 = TriggerDatabase.GetTrigger(c2.Tag);
			int compare = c1.Stages[0].CompareTo(c2.Stages[0]);
			if (compare == 0)
			{
				compare = t1.XmlGroup.CompareTo(t2.XmlGroup);
			}
			if (compare == 0)
			{
				compare = t1.XmlOrder.CompareTo(t2.XmlOrder);
			}
			if (compare == 0)
			{
				//Sort targeted stuff
				string sortKey1 = GetSortKey(c1);
				string sortKey2 = GetSortKey(c2);
				compare = sortKey1.CompareTo(sortKey2);
			}
			return compare;
		}

		/// <summary>
		/// Generates a sort key like make_xml.py does. This is to get the cases sorted the same way make_xml.py does. Otherwise diffs can be tricky
		/// </summary>
		/// <param name="c1"></param>
		/// <returns></returns>
		private string GetSortKey(Case c1)
		{
			List<string> filters = new List<string>();
			if (!string.IsNullOrEmpty(c1.Target))
				filters.Add("target:" + c1.Target);
			if (!string.IsNullOrEmpty(c1.Filter))
				filters.Add("filter:" + c1.Filter);
			if (!string.IsNullOrEmpty(c1.TargetStage))
				filters.Add("targetstage:" + c1.TargetStage);
			if (!string.IsNullOrEmpty(c1.TargetTimeInStage))
				filters.Add("targettimeinstage:" + c1.TargetTimeInStage);
			if (!string.IsNullOrEmpty(c1.AlsoPlaying))
				filters.Add("alsoplaying:" + c1.AlsoPlaying);
			if (!string.IsNullOrEmpty(c1.AlsoPlayingStage))
				filters.Add("alsoplayingstage:" + c1.AlsoPlayingStage);
			if (!string.IsNullOrEmpty(c1.AlsoPlayingHand))
				filters.Add("alsoplayinghand:" + c1.AlsoPlayingHand);
			if (!string.IsNullOrEmpty(c1.TargetHand))
				filters.Add("opphand:" + c1.TargetHand);
			if (!string.IsNullOrEmpty(c1.HasHand))
				filters.Add("hashand:" + c1.HasHand);
			if (!string.IsNullOrEmpty(c1.TotalMales))
				filters.Add("totalmales:" + c1.TotalMales);
			if (!string.IsNullOrEmpty(c1.TotalFemales))
				filters.Add("totalfemales:" + c1.TotalFemales);
			if (!string.IsNullOrEmpty(c1.TargetTimeInStage))
				filters.Add("targettimeinstage:" + c1.TargetTimeInStage);
			if (!string.IsNullOrEmpty(c1.AlsoPlayingTimeInStage))
				filters.Add("alsoplayingtimeinstage:" + c1.AlsoPlayingTimeInStage);
			if (!string.IsNullOrEmpty(c1.TimeInStage))
				filters.Add("timeinstage:" + c1.TimeInStage);
			if (!string.IsNullOrEmpty(c1.ConsecutiveLosses))
				filters.Add("consecutivelosses:" + c1.ConsecutiveLosses);
			if (!string.IsNullOrEmpty(c1.TotalPlaying))
				filters.Add("totalalive:" + c1.TotalPlaying);
			if (!string.IsNullOrEmpty(c1.TotalExposed))
				filters.Add("totalexposed:" + c1.TotalExposed);
			if (!string.IsNullOrEmpty(c1.TotalNaked))
				filters.Add("totalnaked:" + c1.TotalNaked);
			if (!string.IsNullOrEmpty(c1.TotalFinishing))
				filters.Add("totalmasturbating:" + c1.TotalFinishing);
			if (!string.IsNullOrEmpty(c1.TotalFinished))
				filters.Add("totalfinished:" + c1.TotalFinished);
			if (!string.IsNullOrEmpty(c1.TotalRounds))
				filters.Add("totalrounds:" + c1.TotalRounds);
			if (!string.IsNullOrEmpty(c1.SaidMarker))
				filters.Add("saidmarker:" + c1.SaidMarker);
			if (!string.IsNullOrEmpty(c1.NotSaidMarker))
				filters.Add("notsaidmarker:" + c1.NotSaidMarker);
			if (!string.IsNullOrEmpty(c1.AlsoPlayingSaidMarker))
				filters.Add("alsoplayingsaidmarker:" + c1.AlsoPlayingSaidMarker);
			if (!string.IsNullOrEmpty(c1.AlsoPlayingNotSaidMarker))
				filters.Add("alsoplayingnotsaidmarker:" + c1.AlsoPlayingNotSaidMarker);
			if (!string.IsNullOrEmpty(c1.TargetSaidMarker))
				filters.Add("targetsaidmarker:" + c1.TargetSaidMarker);
			if (!string.IsNullOrEmpty(c1.TargetNotSaidMarker))
				filters.Add("targetnotsaidmarker:" + c1.TargetNotSaidMarker);
			if (!string.IsNullOrEmpty(c1.CustomPriority))
				filters.Add("priority:" + c1.CustomPriority);

			return string.Join(",", filters);
		}

		private int CompareString(string s1, string s2)
		{
			if (s1 == null && s2 == null)
				return 0;
			if (s1 == null)
				return 1;
			if (s2 == null)
				return -1;
			return s1.CompareTo(s2);
		}
	}
}
