﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using SPNATI_Character_Editor;
using System.Collections.Generic;

namespace UnitTests
{
	[TestClass]
	public class BehaviorTests
	{
		private Character _character;

		[TestInitialize]
		public void Init()
		{
			TriggerDatabase.FakeUsedInStage = UsedInStage;

			_character = new Character();
			for (int i = 0; i < 5; i++)
			{
				_character.Wardrobe.Add(new Clothing());
			}
		}

		private static bool UsedInStage(string tag, Character character, int stage)
		{
			return true;
		}

		[TestMethod]
		public void StageSpecificLine()
		{
			DialogueLine line = new DialogueLine("hey", "Bubba");
			line = Behaviour.CreateStageSpecificLine(line, 4);
			Assert.AreEqual("4-hey.png", line.Image);
			Assert.AreEqual("Bubba", line.Text);
		}

		[TestMethod]
		public void StageSpecificLineHasPrefix()
		{
			DialogueLine line = new DialogueLine("4-hey", "Bubba");
			line = Behaviour.CreateStageSpecificLine(line, 4);
			Assert.AreEqual("4-hey.png", line.Image);
			Assert.AreEqual("Bubba", line.Text);
		}

		[TestMethod]
		public void StageSpecificLineHasExtension()
		{
			DialogueLine line = new DialogueLine("hey.png", "Bubba");
			line = Behaviour.CreateStageSpecificLine(line, 4);
			Assert.AreEqual("4-hey.png", line.Image);
			Assert.AreEqual("Bubba", line.Text);
		}

		[TestMethod]
		public void StageSpecificLineHasPrefixAndExtension()
		{
			DialogueLine line = new DialogueLine("4-hey.png", "Bubba");
			line = Behaviour.CreateStageSpecificLine(line, 4);
			Assert.AreEqual("4-hey.png", line.Image);
			Assert.AreEqual("Bubba", line.Text);
		}

		[TestMethod]
		public void DefaultLine()
		{
			DialogueLine line = new DialogueLine("hey", "Bubba");
			line = Behaviour.CreateDefaultLine(line);
			Assert.AreEqual("hey", line.Image);
			Assert.AreEqual("Bubba", line.Text);
		}

		[TestMethod]
		public void DefaultLineHasPrefix()
		{
			DialogueLine line = new DialogueLine("4-hey", "Bubba");
			line = Behaviour.CreateDefaultLine(line);
			Assert.AreEqual("hey", line.Image);
			Assert.AreEqual("Bubba", line.Text);
		}

		[TestMethod]
		public void DefaultLineHasExtension()
		{
			DialogueLine line = new DialogueLine("hey.png", "Bubba");
			line = Behaviour.CreateDefaultLine(line);
			Assert.AreEqual("hey", line.Image);
			Assert.AreEqual("Bubba", line.Text);
		}

		[TestMethod]
		public void DefaultLineHasPrefixAndExtension()
		{
			DialogueLine line = new DialogueLine("4-hey.png", "Bubba");
			line = Behaviour.CreateDefaultLine(line);
			Assert.AreEqual("hey", line.Image);
			Assert.AreEqual("Bubba", line.Text);
		}

		private Case CreateWorkingCase(Behaviour behavior, string tag, int[] stages, params string[] lines)
		{
			Case c = new Case(tag);
			foreach (string line in lines)
			{
				c.Lines.Add(new DialogueLine("test", line));
			}
			c.Stages.AddRange(stages);
			behavior.WorkingCases.Add(c);
			return c;
		}

		private Case CreateCase(Behaviour behavior, int stageId, string tag, params string[] lines)
		{
			while (behavior.Stages.Count <= stageId)
			{
				behavior.Stages.Add(new Stage(behavior.Stages.Count));
			}
			Stage stage = behavior.Stages[stageId];
			Case stageCase = new Case(tag);
			stage.Cases.Add(stageCase);
			foreach (string line in lines)
			{
				stageCase.Lines.Add(new DialogueLine(stageId + "-test.png", line));
			}
			return stageCase;
		}

		/// <summary>
		/// Case shared between stages
		/// </summary>
		[TestMethod]
		public void StageTreeSharedCase()
		{
			Behaviour behavior = new Behaviour();
			CreateWorkingCase(behavior, "a", new int[] { 1, 2, 3 }, "a1");
			behavior.BuildStageTree(_character);
			Assert.AreEqual(8, behavior.Stages.Count);
			Assert.AreEqual(1, behavior.Stages[1].Cases.Count);
			Assert.AreEqual(1, behavior.Stages[2].Cases.Count);
			Assert.AreEqual(1, behavior.Stages[3].Cases.Count);
			for (int i = 1; i < 4; i++)
			{
				Assert.AreEqual("a", behavior.Stages[i].Cases[0].Tag);
				Assert.AreEqual("a1", behavior.Stages[i].Cases[0].Lines[0].Text);
			}
		}

		/// <summary>
		/// Multiple cases going to the same stages
		/// </summary>
		[TestMethod]
		public void StageTreeCrossStages()
		{
			Behaviour behavior = new Behaviour();
			CreateWorkingCase(behavior, "a", new int[] { 1, 2, 3 }, "a1");
			CreateWorkingCase(behavior, "b", new int[] { 1, 3 }, "b1");
			behavior.BuildStageTree(_character);
			Assert.AreEqual(8, behavior.Stages.Count);
			Assert.AreEqual(2, behavior.Stages[1].Cases.Count);
			Assert.AreEqual(1, behavior.Stages[2].Cases.Count);
			Assert.AreEqual(2, behavior.Stages[3].Cases.Count);
		}

		[TestMethod]
		public void StageTreeMergeDialogue()
		{
			Behaviour behavior = new Behaviour();
			CreateWorkingCase(behavior, "a", new int[] { 0 }, "a1");
			CreateWorkingCase(behavior, "a", new int[] { 0 }, "a2");
			behavior.BuildStageTree(_character);
			Assert.AreEqual(2, behavior.Stages[0].Cases[0].Lines.Count);
		}

		[TestMethod]
		public void StageTreeIntegration()
		{
			Behaviour behavior = new Behaviour();
			CreateWorkingCase(behavior, "a", new int[] { 1, 2, 3 }, "z");
			CreateWorkingCase(behavior, "b", new int[] { 1, 3 }, "y");
			CreateWorkingCase(behavior, "b", new int[] { 4 }, "x");
			CreateWorkingCase(behavior, "b", new int[] { 3 }, "x");
			CreateWorkingCase(behavior, "c", new int[] { 1, 2 }, "w");
			behavior.BuildStageTree(_character);
			Assert.AreEqual(3, behavior.Stages[1].Cases.Count);
			Assert.AreEqual("a", behavior.Stages[1].Cases[0].Tag);
			Assert.AreEqual("b", behavior.Stages[1].Cases[1].Tag);
			Assert.AreEqual("c", behavior.Stages[1].Cases[2].Tag);
			Assert.AreEqual(2, behavior.Stages[2].Cases.Count);
			Assert.AreEqual("a", behavior.Stages[2].Cases[0].Tag);
			Assert.AreEqual("c", behavior.Stages[2].Cases[1].Tag);
			Assert.AreEqual(2, behavior.Stages[3].Cases.Count);
			Assert.AreEqual("a", behavior.Stages[3].Cases[0].Tag);
			Assert.AreEqual("b", behavior.Stages[3].Cases[1].Tag);
			Assert.AreEqual(2, behavior.Stages[3].Cases[1].Lines.Count);
			Assert.AreEqual(1, behavior.Stages[4].Cases.Count);
			Assert.AreEqual("b", behavior.Stages[4].Cases[0].Tag);
		}

		[TestMethod]
		public void SplitSharedCase()
		{
			Behaviour behavior = new Behaviour();
			CreateCase(behavior, 1, "a", "a1");
			CreateCase(behavior, 2, "a", "a1");
			CreateCase(behavior, 3, "a", "a1");
			behavior.BuildWorkingCases(_character);
			Assert.AreEqual(1, behavior.WorkingCases.Count);
			Assert.AreEqual(3, behavior.WorkingCases[0].Stages.Count);
		}

		[TestMethod]
		public void SplitSharedCaseExtraLine()
		{
			Behaviour behavior = new Behaviour();
			CreateCase(behavior, 1, "a", "a1", "a2");
			CreateCase(behavior, 2, "a", "a1", "a2");
			CreateCase(behavior, 3, "a", "a1");
			behavior.BuildWorkingCases(_character);
			Assert.AreEqual(2, behavior.WorkingCases.Count);
		}

		[TestMethod]
		public void SplitSharedCaseSharedLines()
		{
			Behaviour behavior = new Behaviour();
			CreateCase(behavior, 1, "a", "a1", "a2", "a3");
			CreateCase(behavior, 2, "a", "a1", "a2");
			CreateCase(behavior, 3, "a", "a1", "a2");
			behavior.BuildWorkingCases(_character);
			Assert.AreEqual(2, behavior.WorkingCases.Count);
		}

		[TestMethod]
		public void TreatsConditionsAsSeparateCases()
		{
			Behaviour behavior = new Behaviour();
			CreateCase(behavior, 0, "a", "a1", "a2");
			Case conditioned = CreateCase(behavior, 0, "a", "a1", "a2");
			conditioned.Filter = "x";
			behavior.BuildWorkingCases(_character);
			Assert.AreEqual(2, behavior.WorkingCases.Count);
		}

		[TestMethod]
		public void SplitIntegration()
		{
			Behaviour behavior = new Behaviour();
			CreateCase(behavior, 1, "a", "a1", "a2");
			CreateCase(behavior, 1, "b", "b1");
			CreateCase(behavior, 1, "c", "b1");
			CreateCase(behavior, 2, "a", "a1", "a2", "a3");
			CreateCase(behavior, 2, "b", "b2", "b3");
			CreateCase(behavior, 3, "a", "a1", "a2");
			CreateCase(behavior, 3, "b", "b1", "b2");
			behavior.BuildWorkingCases(_character);
			Assert.AreEqual(6, behavior.WorkingCases.Count);
		}

		[TestMethod]
		public void BackAndForth()
		{
			Behaviour behavior = new Behaviour();
			CreateCase(behavior, 1, "a", "a1", "a2");
			CreateCase(behavior, 1, "b", "b1");
			CreateCase(behavior, 1, "c", "b1");
			CreateCase(behavior, 2, "a", "a1", "a2", "a3");
			CreateCase(behavior, 2, "b", "b2", "b3");
			CreateCase(behavior, 3, "a", "a1", "a2");
			CreateCase(behavior, 3, "b", "b1", "b2");
			behavior.BuildWorkingCases(_character);
			behavior.BuildStageTree(_character);
			Assert.AreEqual(3, behavior.Stages[1].Cases.Count);
			Assert.AreEqual(2, behavior.Stages[1].Cases[0].Lines.Count);
			Assert.AreEqual(1, behavior.Stages[1].Cases[1].Lines.Count);
			Assert.AreEqual(1, behavior.Stages[1].Cases[2].Lines.Count);
			Assert.AreEqual(2, behavior.Stages[2].Cases.Count);
			Assert.AreEqual(3, behavior.Stages[2].Cases[0].Lines.Count);
			Assert.AreEqual(2, behavior.Stages[2].Cases[1].Lines.Count);
			Assert.AreEqual(2, behavior.Stages[3].Cases.Count);
			Assert.AreEqual(2, behavior.Stages[3].Cases[0].Lines.Count);
			Assert.AreEqual(2, behavior.Stages[3].Cases[1].Lines.Count);
		}

		[TestMethod]
		public void ReplaceReplaces()
		{
			Behaviour behavior = new Behaviour();
			CreateWorkingCase(behavior, "a", new int[] { 0, 1, 2 }, "a1");
			CreateWorkingCase(behavior, "b", new int[] { 0, 1, 2 }, "c");
			HashSet<string> dest = new HashSet<string>();
			dest.Add("b");
			behavior.BulkReplace("a", dest);
			Assert.AreEqual(2, behavior.WorkingCases.Count);
			Assert.AreEqual("a1", behavior.WorkingCases[1].Lines[0].Text);
		}

		[TestMethod]
		public void ReplaceReplacesMultiple()
		{
			Behaviour behavior = new Behaviour();
			CreateWorkingCase(behavior, "a", new int[] { 0, 1, 2 }, "a1");
			CreateWorkingCase(behavior, "b", new int[] { 0, 1, 2 }, "c");
			HashSet<string> dest = new HashSet<string>();
			dest.Add("b");
			dest.Add("c");
			dest.Add("d");
			behavior.BulkReplace("a", dest);
			Assert.AreEqual(4, behavior.WorkingCases.Count);
			Assert.AreEqual("a1", behavior.WorkingCases[1].Lines[0].Text);
			Assert.AreEqual("a1", behavior.WorkingCases[2].Lines[0].Text);
			Assert.AreEqual("a1", behavior.WorkingCases[3].Lines[0].Text);
		}

		[TestMethod]
		public void ReplaceIgnoresConditions()
		{
			Behaviour behavior = new Behaviour();
			CreateWorkingCase(behavior, "a", new int[] { 0, 1, 2 }, "a1");
			Case c = CreateWorkingCase(behavior, "b", new int[] { 0, 1, 2 }, "c");
			c.Filter = "filter";
			HashSet<string> dest = new HashSet<string>();
			dest.Add("b");
			behavior.BulkReplace("a", dest);
			Assert.AreEqual(3, behavior.WorkingCases.Count);
			Assert.AreEqual("c", behavior.WorkingCases[1].Lines[0].Text);
		}
	}
}
