﻿using Desktop;
using System;

namespace SPNATI_Character_Editor.DataStructures
{
	public class ChecklistTask
	{
		public string Text;
		public string HelpText;
		public Action LaunchHandler;
		public LaunchParameters LaunchData;
		public int Value;
		public int MaxValue;

		public bool ProgressBased;

		public ChecklistTask(string text)
		{
			Text = text;
		}

		public override string ToString()
		{
			return Text;
		}
	}
}
