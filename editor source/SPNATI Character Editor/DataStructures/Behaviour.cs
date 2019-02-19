﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using System.Linq;
using SPNATI_Character_Editor.IO;

namespace SPNATI_Character_Editor
{
	/// <summary>
	/// Behaviour node of xml file. Contains dialogue
	/// </summary>
	public class Behaviour
	{
		/// <summary>
		/// Raised when a new case is added to the working cases
		/// </summary>
		public event EventHandler<Case> CaseAdded;

		/// <summary>
		/// Raised when a case is removed from the working cases collection
		/// </summary>
		public event EventHandler<Case> CaseRemoved;

		/// <summary>
		/// Raised when the list stages that a working case appears in has been modified
		/// </summary>
		public event EventHandler<Case> CaseModified;

		/// <summary>
		/// Next ID for a case that needs one
		/// </summary>
		[XmlIgnore]
		public int NextId { get; set; }

		/// <summary>
		/// Only used when serializing or deserializing XML. Cases that share text across stages are split into separate cases per stage here
		/// </summary>
		[XmlNewLine(XmlNewLinePosition.After)]
		[XmlElement("stage")]
		public List<Stage> Stages = new List<Stage>();

		/// <summary>
		/// Flat structure of cases used when editing dialogue. When deserializing, this is constructed from Stages. When serializing, Stages is reconstructed using this info.
		/// </summary>
		/// <remarks>Unlike the Stages property, Case instances here can be shared across stages, ensuring that editing in one will update all applicable stages</remarks>
		[XmlIgnore]
		private List<Case> _workingCases = new List<Case>();

		/// <summary>
		/// Whether the working cases list has been built yet
		/// </summary>
		private bool _builtWorkingCases = false;

		/// <summary>
		/// Character to which this behavior belongs
		/// </summary>
		private Character _character;

		/// <summary>
		/// Called prior to serializing to XML
		/// </summary>
		/// <param name="character"></param>
		public void OnBeforeSerialize(Character character)
		{
			BuildStageTree(character);
		}

		/// <summary>
		/// Called after deserialization
		/// </summary>
		/// <param name="character"></param>
		public void OnAfterDeserialize(Character character)
		{
			_character = character;
			foreach (var stage in Stages)
			{
				foreach (var stageCase in stage.Cases)
				{
					foreach (var line in stageCase.Lines)
					{
						line.Text = XMLHelper.DecodeEntityReferences(line.Text);
						if (string.IsNullOrEmpty(line.Marker))
							line.Marker = null;
						character.CacheMarker(line.Marker);
					}
				}
			}
		}

		/// <summary>
		/// Called when loading a character to edit
		/// </summary>
		/// <param name="character"></param>
		public void PrepareForEdit(Character character)
		{
			_character = character;
			_workingCases = new List<Case>();
			_builtWorkingCases = false;

			EnsureWorkingCases();

			EnsureDefaults(character); //If the input file had any missing dialogue, add it in now
		}

		/// <summary>
		/// Converts a generic line of dialogue into one specific for the given stage (i.e gives the image the applicable prefix)
		/// </summary>
		/// <param name="line">Line to convert</param>
		/// <param name="stage">Stage to convert to</param>
		/// <returns></returns>
		public static DialogueLine CreateStageSpecificLine(DialogueLine line, int stage, Character character)
		{
			DialogueLine copy = line.Copy();
			if (!copy.IsGenericImage)
			{
				copy.Image = DialogueLine.GetStageImage(stage, copy.Image);
			}
			if (copy.Image != null)
			{
				bool custom = copy.Image.StartsWith("custom:");

				string path = character != null ? Config.GetRootDirectory(character) : "";
				string extension = line.ImageExtension;
				if (string.IsNullOrEmpty(extension) && !custom)
				{
					//figure out the extension by searching for files of different names
					bool basePngExists = File.Exists(Path.Combine(path, copy.Image + ".png"));
					bool baseGifExists = File.Exists(Path.Combine(path, copy.Image + ".gif"));

					if (!copy.Image.StartsWith(stage + "-") && !basePngExists && !baseGifExists)
					{
						copy.Image = stage + "-" + copy.Image;
						baseGifExists = File.Exists(Path.Combine(path, copy.Image + ".gif"));
						if (baseGifExists)
						{
							extension = ".gif";
						}
						else
						{
							extension = ".png";
						}
					}
					else
					{
						if (baseGifExists)
						{
							extension = ".gif";
						}
						else
						{
							extension = ".png";
						}
					}
				}

				if (!custom && !copy.Image.StartsWith(stage + "-") && !File.Exists(Path.Combine(path, copy.Image + extension)))
				{
					copy.Image = stage + "-" + copy.Image;
				}
				copy.Image += extension;
				copy.ImageExtension = extension;
			}
			copy.Text = line.Text?.Trim();
			return copy;
		}

		/// <summary>
		/// Looks through the working cases to locate any Stage+Trigger combos that don't exist, and creates default cases for any missing combinations.
		/// Triggers apply to one or more stages, and a case with no targeted dialogue must exist for each applicable stage
		/// </summary>
		/// <param name="character">Character this behavior belongs to</param>
		public bool EnsureDefaults(Character character)
		{
			_character = character;
			bool modified = false;

			//Generate an index of expected stage+tag combos
			int layers = character.Layers + Clothing.ExtraStages;
			Dictionary<string, HashSet<int>> requiredLineIndex = new Dictionary<string, HashSet<int>>();
			foreach (Trigger t in TriggerDatabase.Triggers)
			{
				if (t.Optional || t.Tag == Trigger.StartTrigger)
					continue;
				HashSet<int> stages = new HashSet<int>();
				for (int stage = 0; stage < layers; stage++)
				{
					if (TriggerDatabase.UsedInStage(t.Tag, character, stage))
					{
						stages.Add(stage);
					}
				}
				requiredLineIndex[t.Tag] = stages;
			}

			//Loop through the cases and remove any satisfied tags from the index
			foreach (var workingCase in character.Behavior._workingCases)
			{
				if (workingCase.HasFilters)
					continue; //A filtered case can't possibly be a default
				string tag = workingCase.Tag;
				HashSet<int> expectedStages;
				if (!requiredLineIndex.TryGetValue(tag, out expectedStages))
					continue; //Tag has already been satisfied (or it's an invalid tag)
				foreach (int stage in workingCase.Stages)
				{
					expectedStages.Remove(stage);
				}
				if (expectedStages.Count == 0)
					requiredLineIndex.Remove(tag); //Tag's defaults have all been met
			}

			//Finally, add lines for whatever remains in the index
			foreach (var kvp in requiredLineIndex)
			{
				string tag = kvp.Key;
				HashSet<int> remainingStages = kvp.Value;
				Case genericCase = new Case(tag);
				DialogueLine line = DialogueDatabase.CreateDefault(tag);
				genericCase.Lines.Add(line);
				foreach (int stage in remainingStages)
				{
					genericCase.Stages.Add(stage);
				}
				AddWorkingCase(genericCase);
				modified = true;
			}

			return modified;
		}

		/// <summary>
		/// Sorts the WorkingCases so that they appear in consistent order within the tree
		/// </summary>
		public void SortWorking()
		{
			_workingCases.Sort(CompareTags);
		}

		public static int CompareTags(Case c1, Case c2)
		{
			string tag1 = c1.Tag;
			string tag2 = c2.Tag;
			int comparison = TriggerDatabase.Compare(tag1, tag2);
			if (comparison == 0)
			{
				comparison = c1.CompareTo(c2);
			}
			return comparison;
		}

		/// <summary>
		/// Creates a generic line from a stage specific one (i.e. strips the stage prefix from the image)
		/// </summary>
		/// <param name="line"></param>
		/// <returns></returns>
		public static DialogueLine CreateDefaultLine(DialogueLine line)
		{
			DialogueLine copy = line.Copy();
			string extension = line.ImageExtension ?? Path.GetExtension(line.Image);
			copy.ImageExtension = extension;
			line.ImageExtension = extension;
			copy.Image = DialogueLine.GetDefaultImage(line.Image);
			copy.Text = line.Text.Trim();
			copy.IsGenericImage = line.IsGenericImage;
			return copy;
		}

		/// <summary>
		/// Returns the number of unique lines of dialogue
		/// </summary>
		public int UniqueLines
		{
			get
			{
				HashSet<string> knownLines = new HashSet<string>();
				foreach (Case c in _workingCases)
				{
					foreach (var line in c.Lines)
					{
						if (knownLines.Contains(line.Text))
							continue;
						knownLines.Add(line.Text);
					}
				}
				return knownLines.Count;
			}
		}

		/// <summary>
		/// Rebuilds the stage tree from the WorkingCases list
		/// </summary>
		public void BuildStageTree(Character character)
		{
			foreach (var stageCase in _workingCases)
			{
				stageCase.ClearEmptyValues();
			}
			Stages.Clear();

			//Always build 1 stage per layer
			for (int s = 0; s < character.Layers + Clothing.ExtraStages; s++)
			{
				Stages.Add(new Stage(s));
			}

			//Put each case into the appropriate stage(s)
			foreach (var workingCase in _workingCases)
			{
				foreach (int s in workingCase.Stages)
				{
					if (s >= Stages.Count) { continue; }

					string id = null;
					if (workingCase.Id > 0)
					{
						id = $"{s}-{workingCase.Id}";
					}

					Stage stage = Stages[s];

					//Find a case to merge into
					Case existingCase = stage.Cases.Find(c => c.MatchesConditions(workingCase) && (c.StageId == id || (string.IsNullOrEmpty(id) && string.IsNullOrEmpty(c.StageId))));
					if (existingCase == null)
					{
						//No case exists yet, so create one
						existingCase = workingCase.CopyConditions();
						existingCase.StageId = id;
						existingCase.Stages.Add(s); //Not really necessary for serialization, since each case will have a single stage, and will be a child of that stage
						stage.Cases.Add(existingCase);
					}

					//Move the lines over, and make them stage-specific
					foreach (var line in workingCase.Lines)
					{
						existingCase.Lines.Add(CreateStageSpecificLine(line, s, character));
					}
				}
			}
		}

		/// <summary>
		/// Builds the working cases list out of the Stages tree
		/// </summary>
		public void BuildWorkingCases()
		{
			_builtWorkingCases = true;
			_workingCases.Clear();
			Dictionary<int, Case> map = new Dictionary<int, Case>();

			List<Case> buckets = new List<Case>();
			//Make case+line buckets to track which stages each combo appears in
			foreach (Stage stage in Stages)
			{
				foreach (Case stageCase in stage.Cases)
				{
					if (!TriggerDatabase.UsedInStage(stageCase.Tag, _character, stage.Id))
						continue;
					int code = stageCase.GetCode();

					int id = 0;
					if (!string.IsNullOrEmpty(stageCase.StageId))
					{
						string[] idPieces = stageCase.StageId.Split('-');
						if (idPieces.Length > 1)
						{
							int.TryParse(idPieces[1], out id);
						}
					}

					foreach (DialogueLine line in stageCase.Lines)
					{
						line.IsGenericImage = !DialogueLine.IsStageSpecificImage(line.Image);
						var defaultLine = CreateDefaultLine(line);
						int hash = defaultLine.GetHashCode();
						hash = code + hash;
						//See if there's a case that already contains this line, and make one if there isn't
						Case existing;
						if (!map.TryGetValue(hash, out existing))
						{
							existing = stageCase.CopyConditions();
							existing.Id = id;
							map[hash] = existing;
							existing.Lines.Add(defaultLine);
							buckets.Add(existing);
						}
						if (!existing.Stages.Contains(stage.Id))
						{
							existing.Stages.Add(stage.Id);
						}
					}
				}
			}

			//Sort each buckets's Stages set for easier equivalence checks
			foreach (Case c in buckets)
			{
				c.Stages.Sort();
			}

			//Merge buckets whose case+stages match
			Dictionary<int, List<Case>> cases = new Dictionary<int, List<Case>>();
			foreach (Case bucket in buckets)
			{
				int code = bucket.GetCode();
				List<Case> caseList;
				if (!cases.TryGetValue(code, out caseList))
				{
					caseList = new List<Case>();
					cases[code] = caseList;
				}
				Case caseMatchingStages = caseList.Find(c => c.Stages.SequenceEqual(bucket.Stages));
				if (caseMatchingStages == null)
				{
					caseMatchingStages = bucket.CopyConditions();
					caseMatchingStages.Id = bucket.Id;
					caseMatchingStages.Stages.AddRange(bucket.Stages);
					caseList.Add(caseMatchingStages);
				}
				foreach (var line in bucket.Lines)
				{
					caseMatchingStages.Lines.Add(line);
				}
			}

			//Done grouping. Put the cases into the WorkingCase list
			foreach (List<Case> list in cases.Values)
			{
				_workingCases.AddRange(list);
			}

			//Move the legacy Start lines into Selected/Game start cases
			if (_character.StartingLines.Count > 0)
			{
				Case selected = new Case("selected");
				selected.Stages.Add(0);
				selected.Lines.Add(_character.StartingLines[0]);
				AddWorkingCase(selected);

				Case start = new Case("game_start");
				start.Stages.Add(0);
				if (_character.StartingLines.Count > 1)
				{
					for (int i = 1; i < _character.StartingLines.Count; i++)
					{
						start.Lines.Add(_character.StartingLines[i]);
					}
				}
				else
				{
					start.Lines.Add(_character.StartingLines[0]);
				}
				AddWorkingCase(start);

				_character.StartingLines.Clear();
			}

			SortWorking();
		}

		/// <summary>
		/// Ensures the working cases list has been built before trying to manipulate it
		/// </summary>
		private void EnsureWorkingCases()
		{
			if (_builtWorkingCases) { return; }
			BuildWorkingCases();

			CharacterEditorData editorData = CharacterDatabase.GetEditorData(_character);
			editorData?.Initialize();
			DataConversions.ConvertVersion(_character);
		}

		public IEnumerable<Case> GetWorkingCases()
		{
			EnsureWorkingCases();
			foreach (Case c in _workingCases)
			{
				yield return c;
			}
		}

		/// <summary>
		/// Adds a new case to the working cases
		/// </summary>
		/// <param name="theCase">Case to add</param>
		public void AddWorkingCase(Case theCase)
		{
			EnsureWorkingCases();
			_workingCases.Add(theCase);
			CaseAdded?.Invoke(this, theCase);
		}

		/// <summary>
		/// Removes a case from the working cases
		/// </summary>
		/// <param name="theCase"></param>
		public void RemoveWorkingCase(Case theCase)
		{
			EnsureWorkingCases();
			_workingCases.Remove(theCase);
			CaseRemoved?.Invoke(this, theCase);
		}

		private void RemoveWorkingCaseAt(int index)
		{
			EnsureWorkingCases();
			Case theCase = _workingCases[index];
			_workingCases.RemoveAt(index);
			CaseRemoved?.Invoke(this, theCase);
		}

		/// <summary>
		/// Finalizes changes to a case's Stages list
		/// </summary>
		/// <param name="theCase"></param>
		public void ApplyChanges(Case theCase)
		{
			CaseModified?.Invoke(this, theCase);
		}

		/// <summary>
		/// Takes a case that spans multiple stages and splits it into multiple cases that apply to one stage each, one for each stage
		/// </summary>
		/// <param name="original">The case being split</param>
		/// <param name="retainStage">Which stage to keep in the original case object</param>
		public void DivideCaseIntoSeparateStages(Case original, int retainStage)
		{
			foreach (int stage in original.Stages)
			{
				if (stage != retainStage)
				{
					Case stageCase = original.Copy();
					stageCase.Stages.Add(stage);
					AddWorkingCase(stageCase);
				}
			}
			original.Stages.Clear();
			original.Stages.Add(retainStage);
			ApplyChanges(original);
		}

		/// <summary>
		/// Takes a case that spans multiple stages and splits it into two, one taking all the stages except the split stage, and one taking the split stage
		/// </summary>
		/// <param name="original">Case to split</param>
		/// <param name="splitPoint">Stage to split at</param>
		public void SplitCaseStage(Case original, int splitPoint)
		{
			Case beforeSplitCase = original.Copy();
			for (int s = original.Stages.Count - 1; s >= 0; s--)
			{
				if (original.Stages[s] != splitPoint)
				{
					beforeSplitCase.Stages.Add(original.Stages[s]);
					original.Stages.RemoveAt(s);
				}
			}
			ApplyChanges(original);
			beforeSplitCase.Stages.Sort();
			AddWorkingCase(beforeSplitCase);
		}

		/// <summary>
		/// Takes a case that spans multiple stages and splits it into two, one taking all the stages before the split point, and one taking all stages after
		/// </summary>
		/// <param name="original">Case to split</param>
		/// <param name="splitPoint">Stage to split at</param>
		public void SplitCaseAtStage(Case original, int splitPoint)
		{
			Case beforeSplitCase = original.Copy();
			for (int s = original.Stages.Count - 1; s >= 0; s--)
			{
				if (original.Stages[s] < splitPoint)
				{
					beforeSplitCase.Stages.Add(original.Stages[s]);
					original.Stages.RemoveAt(s);
				}
			}
			ApplyChanges(original);
			beforeSplitCase.Stages.Sort();
			AddWorkingCase(beforeSplitCase);
		}

		/// <summary>
		/// Duplicates a case and its applied stages
		/// </summary>
		/// <param name="original"></param>
		/// <returns></returns>
		public Case DuplicateCase(Case original)
		{
			Case copy = original.Copy();
			copy.Stages.AddRange(original.Stages);
			AddWorkingCase(copy);
			return copy;
		}

		/// <summary>
		/// Replaces all non-targeted dialogue in the destination cases with that from the source. Affects all stages
		/// </summary>
		/// <param name="sourceTag">Trigger tag for the case to copy from</param>
		/// <param name="destinationTags">Tags to replace</param>
		public void BulkReplace(string sourceTag, HashSet<string> destinationTags)
		{
			//Step 1: Throw away all non-targeted cases from the destinations
			for (int i = _workingCases.Count - 1; i >= 0; i--)
			{
				Case workingCase = _workingCases[i];
				if (!workingCase.HasFilters && destinationTags.Contains(_workingCases[i].Tag))
				{
					RemoveWorkingCaseAt(i);
				}
			}

			//Step 2: Go through the source cases and duplicate them for each destination
			int end = _workingCases.Count; //caching this off since we'll be adding to this list whie iterating, but don't need to process the new cases
			for (int i = 0; i < end; i++)
			{
				Case sourceCase = _workingCases[i];
				if (!sourceCase.HasFilters && sourceCase.Tag == sourceTag)
				{
					foreach (string tag in destinationTags)
					{
						Case newCase = sourceCase.Copy();
						newCase.Stages.AddRange(sourceCase.Stages);
						newCase.Tag = tag;
						AddWorkingCase(newCase);
					}
				}
			}
		}

		/// <summary>
		/// Applies wardrobe changes to the dialogue tree
		/// </summary>
		/// <param name="changes"></param>
		public void ApplyWardrobeChanges(Character character, Queue<WardrobeChange> changes)
		{
			while (changes.Count > 0)
			{
				WardrobeChange change = changes.Dequeue();
				switch (change.Change)
				{
					case WardrobeChangeType.Add:
						InsertStage(change.Index);
						break;
					case WardrobeChangeType.Remove:
						RemoveStage(change.Index);
						break;
					case WardrobeChangeType.MoveDown:
						SwapStages(change.Index, change.Index - 1);
						break;
					case WardrobeChangeType.MoveUp:
						SwapStages(change.Index, change.Index + 1);
						break;
				}
			}

			EnsureDefaults(character);
		}

		/// <summary>
		/// Inserts a stage at the given index, shifting everything after it up
		/// </summary>
		/// <param name="index"></param>
		private void InsertStage(int index)
		{
			foreach (Case workingCase in _workingCases)
			{
				List<int> stages = workingCase.Stages;
				for (int i = 0; i < stages.Count; i++)
				{
					int stage = stages[i];
					if (stage >= index)
						stages[i] = stage + 1;
				}
			}
		}

		/// <summary>
		/// Removes a stage from the given index, shifting everything after it down
		/// </summary>
		/// <param name="index"></param>
		private void RemoveStage(int index)
		{
			foreach (Case workingCase in _workingCases)
			{
				List<int> stages = workingCase.Stages;
				for (int i = stages.Count - 1; i >= 0; i--)
				{
					int stage = stages[i];
					if (stage > index)
						stages[i] = stage - 1;
					else if (stage == index)
						stages.RemoveAt(i);
				}
			}
		}

		/// <summary>
		/// Swaps the position of two stages
		/// </summary>
		/// <param name="index1"></param>
		/// <param name="index2"></param>
		private void SwapStages(int index1, int index2)
		{
			foreach (Case workingCase in _workingCases)
			{
				List<int> stages = workingCase.Stages;
				for (int i = 0; i < stages.Count; i++)
				{
					int stage = stages[i];
					if (stage == index1)
						stages[i] = index2;
					else if (stage == index2)
						stages[i] = index1;
				}
			}
		}
	}
}
