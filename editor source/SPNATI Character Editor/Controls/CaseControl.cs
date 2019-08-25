﻿using Desktop;
using Desktop.CommonControls;
using Desktop.Skinning;
using SPNATI_Character_Editor.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace SPNATI_Character_Editor.Controls
{
	public partial class CaseControl : UserControl, IMacroEditor, ISkinnedPanel, ISkinControl
	{
		private const string FavoriteConditionsSetting = "FavoritedConditions";

		private Character _character;
		private CharacterEditorData _editorData;
		private Case _selectedCase;
		private Stage _selectedStage;
		private bool _populatingCase;
		private List<DialogueLine> _lineClipboard = new List<DialogueLine>();
		private Case _trackedCase;

		public event EventHandler<DialogueLine> TextUpdated;
		public event EventHandler<int> HighlightRow;

		public CaseControl()
		{
			InitializeComponent();
		}

		public int PreviewStage
		{
			get { return _selectedStage == null ? _selectedCase.Stages[0] : _selectedStage.Id; }
		}

		public void OnUpdateSkin(Skin skin)
		{
			BackColor = skin.Background.GetColor(VisualState.Normal, false, Enabled);
		}

		private void CaseControl_Load(object sender, EventArgs e)
		{
			tableConditions.RecordFilter = FilterTargets;
			lstAddTags.RecordType = typeof(Tag);
			lstRemoveTags.RecordType = typeof(Tag);
			gridStages.CheckedChanged += Check_CheckedChanged;
			gridStages.LayerSelected += GridStages_LayerSelected;
			gridDialogue.TextUpdated += GridDialogue_TextUpdated;
		}

		public void Activate()
		{
			CreateStageCheckboxes();
		}

		public void SetCharacter(Character character)
		{
			_character = character;
			_editorData = CharacterDatabase.GetEditorData(_character);
			tableConditions.Context = character;
			CreateStageCheckboxes();
		}

		public Case GetCase()
		{
			return _selectedCase;
		}

		public void SetCase(Stage stage, Case workingCase)
		{
			if (_selectedCase != null)
			{
				tabsConditions.SelectedIndexChanged -= tabsConditions_SelectedIndexChanged;
				tabsConditions.SelectedIndex = 0;
				for (int i = tabsConditions.TabPages.Count - 1; i > 0; i--)
				{
					tabsConditions.TabPages.RemoveAt(i);
				}
			}
			_selectedStage = stage;
			_selectedCase = workingCase;
			TrackCase(_selectedCase);
			if (_selectedCase != null)
			{
				tabConditions.Enabled = true;
				foreach (Case alternative in _selectedCase.AlternativeConditions)
				{
					AddAlternateTab();
				}
				tabsConditions.SelectedIndexChanged += tabsConditions_SelectedIndexChanged;
			}
			PopulateCase();
		}

		private void CasePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			_character.IsDirty = true;
		}

		public void UpdateStages()
		{
			if (_character == null) { return; }
			CreateStageCheckboxes();
			PopulateStageCheckboxes();
		}

		public void UpdateMacros()
		{
			tableConditions.AddMacros();
		}

		public bool AutoOpenConditions
		{
			set { tableConditions.RunInitialAddEvents = value; }
		}

		private void GridDialogue_TextUpdated(object sender, int e)
		{
			TextUpdated?.Invoke(this, gridDialogue.GetLine(e));
		}

		public void SaveFavorites()
		{
			//Update favorite conditions
			List<string> favorites = tableConditions.GetFavorites();
			Config.Set(FavoriteConditionsSetting, string.Join("|", favorites));
		}

		public void Save()
		{
			SaveCase();
		}


		#region Event handlers
		/// <summary>
		/// Raised when a Stage checkbox is clicked
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Check_CheckedChanged(object sender, EventArgs e)
		{
			if (_populatingCase)
				return;
			_populatingCase = true;
			HashSet<int> stages = GetSelectedStages();
			_selectedCase.ClearStages();
			_selectedCase.AddStages(stages);
			UpdatePreviewStage(stages, -1);
			_populatingCase = false;
		}

		private void GridStages_LayerSelected(object sender, int layer)
		{
			UpdatePreviewStage(GetSelectedStages(), layer);
		}

		private void UpdatePreviewStage(HashSet<int> stages, int desiredStage)
		{
			if (_selectedStage != null && stages.Count > 0)
			{
				if (desiredStage == -1)
				{
					desiredStage = stages.Min();
				}
				_selectedStage = new Stage(desiredStage);
				gridDialogue.SetStage(_selectedStage, stages);
				gridStages.SetPreviewStage(_selectedStage.Id);
			}
		}

		/// <summary>
		/// Copies the current case's lines to the clipboard
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void cmdCopyAll_Click(object sender, EventArgs e)
		{
			if (_selectedCase == null)
				return;
			_lineClipboard = gridDialogue.CopyLines();
			Shell.Instance.SetStatus(string.Format("Lines from {0} copied to the clipboard.", _selectedCase));
		}

		/// <summary>
		/// Pastes the lines in the clipboard into the selected case, either replacing or appending to the existing lines
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void cmdPasteAll_Click(object sender, EventArgs e)
		{
			if (_selectedCase == null || _lineClipboard.Count == 0)
				return;

			if (!gridDialogue.IsEmpty)
			{
				DialogResult result = MessageBox.Show("Do you want to overwrite the existing lines?", "Paste Lines", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
				if (result == DialogResult.Cancel)
					return;
				else if (result == DialogResult.Yes)
				{
					gridDialogue.Clear();
				}
			}
			gridDialogue.PasteLines(_lineClipboard);
		}

		private void gridDialogue_HighlightRow(object sender, int index)
		{
			HighlightRow?.Invoke(this, index);
		}

		private void gridDialogue_KeyDown(object sender, KeyEventArgs e)
		{
			OnKeyDown(e);
		}
		#endregion

		public bool FindReplace(FindArgs args)
		{
			return gridDialogue.FindReplace(args);
		}

		public PoseMapping GetImage(int index)
		{
			return gridDialogue.GetImage(index);
		}
		public DialogueLine GetLine(int index)
		{
			return gridDialogue.GetLine(index);
		}

		public void ClearSelection()
		{
			gridDialogue.ClearSelection();
		}

		public int FindText(string text, int startIndex, FindArgs args)
		{
			return gridDialogue.FindText(text, startIndex, args);
		}

		public void SelectTextInRow(int rowIndex, int startIndex, int length)
		{
			gridDialogue.SelectTextInRow(rowIndex, startIndex, length);
		}

		/// <summary>
		/// Loads the newly selected case into the dialogue fields
		/// </summary>
		private void PopulateCase()
		{
			if (_selectedCase == null)
			{
				return;
			}
			_populatingCase = true;
			Case stageCase = _selectedCase;

			PopulateStageCheckboxes();

			TriggerDefinition caseTrigger = TriggerDatabase.GetTrigger(stageCase.Tag);

			#region Case-wide settings
			//Tag combo box
			cboCaseTags.Items.Clear();
			TriggerDefinition currentTrigger = TriggerDatabase.GetTrigger(_selectedCase.Tag);
			if (_selectedStage != null)
			{
				TriggerDefinition selection = null;
				foreach (string tag in TriggerDatabase.GetTags())
				{
					TriggerDefinition t = TriggerDatabase.GetTrigger(tag);
					if (currentTrigger.HasTarget && currentTrigger.HasTarget != t.HasTarget)
					{
						continue;
					}
					if (tag == _selectedCase.Tag)
						selection = t;
					cboCaseTags.Items.Add(t);
				}
				cboCaseTags.SelectedItem = selection;
				cboCaseTags.Enabled = true;
			}
			else
			{
				cboCaseTags.Enabled = false;
			}

			//Help text
			lblHelpText.Text = caseTrigger.HelpText;

			//Available variables
			List<string> vars = new List<string>();
			foreach (Variable globalVar in VariableDatabase.GlobalVariables)
			{
				vars.Add($"~{globalVar.Name}~");
			}
			foreach (string variable in caseTrigger.AvailableVariables)
			{
				vars.Add($"~{variable}~");
			}
			toolTip1.SetToolTip(lblAvailableVars, string.Format("Variables: {0}", string.Join(" ", vars)));

			#endregion

			txtNotes.Text = _editorData.GetNote(_selectedCase);
			CaseLabel label = _editorData.GetLabel(_selectedCase);
			txtFolder.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
			txtFolder.AutoCompleteSource = AutoCompleteSource.CustomSource;
			txtFolder.AutoCompleteCustomSource = _editorData.Folders;
			if (label == null)
			{
				txtLabel.Text = null;
				txtFolder.Text = null;
				SetColorButton(null);
			}
			else
			{
				txtLabel.Text = label.Text;
				txtFolder.Text = label.Folder;
				SetColorButton(label.ColorCode);
			}

			if (caseTrigger.HasTarget)
			{
				tableConditions.RecordFilter = null;
			}
			else
			{
				tableConditions.RecordFilter = FilterTargets;
			}
			bool firstPopulation = (tableConditions.Data == null);
			PopulateConditionTable(_selectedCase);

			if (firstPopulation)
			{
				List<string> favorites = new List<string>();
				string favoritesData = Config.GetString(FavoriteConditionsSetting);
				if (!string.IsNullOrEmpty(favoritesData))
				{
					foreach (string key in favoritesData.Split('|'))
					{
						if (!string.IsNullOrEmpty(key))
						{
							favorites.Add(key);
						}
					}
				}
				tableConditions.SetFavorites(favorites);
			}


			GUIHelper.SetNumericBox(valPriority, _selectedCase.CustomPriority);

			var stages = GetSelectedStages();
			gridDialogue.SetData(_character, _selectedStage, _selectedCase, stages);

			PopulateTagsTab();

			_populatingCase = false;
		}

		private void PopulateConditionTable(Case workingCase)
		{
			tableConditions.Data = workingCase;
			AddSpeedButtons(tableConditions, workingCase?.Tag);
		}

		public static void AddSpeedButtons(PropertyTable table, string tag)
		{
			if (tag == null) { return; }
			TriggerDefinition caseTrigger = TriggerDatabase.GetTrigger(tag);

			//Game-wide
			table.AddSpeedButton("Game", "Background", (data) => { return AddVariableTest("~background~", data); });

			//Player variables

			table.AddSpeedButton("Player", "Collectible (+)", (data) => { return AddVariableTest("~_.collectible.*~", data); });
			table.AddSpeedButton("Player", "Collectible (Counter) (+)", (data) => { return AddVariableTest("~_.collectible.*.counter~", data); });
			table.AddSpeedButton("Player", "Costume (+)", (data) => { return AddVariableTest("~_.costume~", data); });
			table.AddSpeedButton("Player", "Largest Lead (+)", (data) => { return AddVariableTest("~_.biggestlead~", data); });
			table.AddSpeedButton("Player", "Layer Difference (+)", (data) => { return AddVariableTest("~_.diff~", data); });
			table.AddSpeedButton("Player", "Marker (+)", (data) => { return AddVariableTest("~_.marker.*~", data); });
			table.AddSpeedButton("Player", "Marker (Persistent) (+)", (data) => { return AddVariableTest("~_.persistent.*~", data); });
			table.AddSpeedButton("Player", "Place (+)", (data) => { return AddVariableTest("~_.lead~", data); });
			table.AddSpeedButton("Player", "Relative Position (+)", (data) => { return AddVariableTest("~_.position~", data); });
			table.AddSpeedButton("Player", "Slot (+)", (data) => { return AddVariableTest("~_.slot~", data); });
			table.AddSpeedButton("Player", "Stage (+)", (data) => { return AddVariableTest("~_.stage~", data); });
			table.AddSpeedButton("Player", "Tag (+)", (data) => { return AddVariableTest("~_.tag.*~", data); });

			//Table-wide
			table.AddSpeedButton("Table", "Human Name", (data) => { return AddVariableTest("~player~", data); });
			if (caseTrigger.AvailableVariables.Contains("cards"))
			{
				table.AddSpeedButton("Self", "Cards Exchanged", (data) => { return AddVariableTest("~cards~", data); });
			}
			if (caseTrigger.HasTarget)
			{
				if (caseTrigger.AvailableVariables.Contains("clothing"))
				{
					table.AddSpeedButton("Clothing", "Clothing Position", (data) => { return AddVariableTest("~clothing.position~", data); });
					table.AddSpeedButton("Clothing", "Clothing Type", (data) => { return AddVariableTest("~clothing.type~", data); });
				}
				table.AddSpeedButton("Target", "Gender", (data) => { return AddVariableTest("~target.gender~", data); });
				table.AddSpeedButton("Target", "Size", (data) => { return AddVariableTest("~target.size~", data); });
			}
		}

		private static string AddVariableTest(string variable, object data)
		{
			Case theCase = data as Case;
			theCase.Expressions.Add(new ExpressionTest(variable, ""));
			return "Expressions";
		}

		private void PopulateTagsTab()
		{
			lstAddTags.Clear();
			lstRemoveTags.Clear();
			string[] addTags = (_selectedCase.AddCharacterTags ?? "").Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
			string[] removeTags = (_selectedCase.RemoveCharacterTags ?? "").Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
			lstAddTags.SelectedItems = addTags;
			lstRemoveTags.SelectedItems = removeTags;
		}

		private HashSet<int> GetSelectedStages()
		{
			HashSet<int> selectedStages = new HashSet<int>();
			for (int i = 0; i < _character.Layers + Clothing.ExtraStages; i++)
			{
				if (gridStages.GetChecked(i))
				{
					selectedStages.Add(i);
				}
			}
			return selectedStages;
		}

		/// <summary>
		/// Sets the checked state for each stage for the current case
		/// </summary>
		private void CreateStageCheckboxes()
		{
			gridStages.SetData(_character, null, -1);
		}

		private bool FilterTargets(PropertyRecord record)
		{
			if (record == null)
			{
				return false;
			}
			if (record.Group == "Target")
			{
				return false;
			}
			return true;
		}

		/// <summary>
		/// Puts the data in the fields into the selected case object
		/// </summary>
		/// <param name="switchingCases">True when saving within the context of switching selected cases</param>
		/// <returns>True if cases were changed in such a way that the dialogue tree needs to be regenerated</returns>
		private void SaveCase()
		{
			if (_selectedCase == null)
			{
				return;
			}

			SaveNotes();
			var c = _selectedCase;
			if (c.Tag != TriggerDefinition.StartTrigger)
			{
				string newTag = "";
				TriggerDefinition trigger = cboCaseTags.SelectedItem as TriggerDefinition;
				if (trigger != null)
				{
					newTag = trigger.Tag;
				}
				c.Tag = newTag;
				foreach (Case alternate in c.AlternativeConditions)
				{
					alternate.Tag = newTag;
				}

				//Figure out the stages
				List<int> oldStages = new List<int>();
				oldStages.AddRange(c.Stages);

				tableConditions.Save();

				c.CustomPriority = GUIHelper.ReadNumericBox(valPriority);
			}

			//Lines
			gridDialogue.Save();

			SaveTagsTab();

			_character.Behavior.ApplyChanges(_selectedCase);
		}

		/// <summary>
		/// Updates the stage checkboxes to match the selected case's stages
		/// </summary>
		private void PopulateStageCheckboxes()
		{
			int stageId = -1;
			if (_selectedStage != null)
			{
				stageId = _selectedStage.Id;
			}
			gridStages.SetData(_character, _selectedCase, stageId);
		}

		private void SaveTagsTab()
		{
			string[] addTags = lstAddTags.SelectedItems;
			string[] removeTags = lstRemoveTags.SelectedItems;
			if (addTags.Length > 0)
			{
				_selectedCase.AddCharacterTags = string.Join(",", addTags);
			}
			if (removeTags.Length > 0)
			{
				_selectedCase.RemoveCharacterTags = string.Join(",", removeTags);
			}
		}

		public void SelectLine(DialogueLine line)
		{
			if (line != null)
			{
				gridDialogue.SelectLine(line);
			}
		}

		private void txtNotes_Validated(object sender, EventArgs e)
		{
			SaveNotes();
		}

		private void SaveNotes()
		{
			if (_selectedCase == null)
			{
				return;
			}
			_editorData.SetNote(_selectedCase, txtNotes.Text);
			_editorData.SetLabel(_selectedCase, txtLabel.Text, cmdColorCode.Tag?.ToString(), txtFolder.Text);
		}

		#region Macro editing
		private void tableConditions_EditingMacro(object sender, MacroArgs args)
		{
			args.SetEditor(this);
		}

		public bool ShowHelp
		{
			get
			{
				return MacroManager.ShowMacroHelp;
			}
		}

		public SkinnedBackgroundType PanelType
		{
			get { return SkinnedBackgroundType.Background; }
		}

		public string GetHelpText()
		{
			return MacroManager.HelpText;
		}

		public object CreateData()
		{
			Case tag = new Case(_selectedCase.Tag);
			tag.AddStages(_selectedCase.Stages);
			return tag;
		}

		public object GetRecordContext()
		{
			return _character;
		}

		public Func<PropertyRecord, bool> GetRecordFilter(object data)
		{
			Case tag = data as Case;
			TriggerDefinition trigger = TriggerDatabase.GetTrigger(tag.Tag);
			if (trigger.HasTarget)
			{
				return FilterTargets;
			}
			else
			{
				return null;
			}
		}
		#endregion

		private void tableConditions_MacroChanged(object sender, MacroArgs e)
		{
			Config.SaveMacros("Case");
		}

		private void cboCaseTags_SelectedIndexChanged(object sender, EventArgs e)
		{
			TriggerDefinition tag = cboCaseTags.SelectedItem as TriggerDefinition;
			if (tag != null)
			{
				lblHelpText.Text = tag.HelpText;
			}
		}

		public void AddSpeedButtons(PropertyTable table)
		{
			AddSpeedButtons(table, _selectedCase?.Tag);
		}

		private void cmdColorCode_Click(object sender, EventArgs e)
		{
			ColorCode color = RecordLookup.DoLookup(typeof(ColorCode), "", false, null) as ColorCode;
			if (color != null)
			{
				SetColorButton(color.Key);
			}
		}

		private void SetColorButton(string colorCode)
		{
			ColorCode code = Definitions.Instance.Get<ColorCode>(colorCode);
			if (code == null)
			{
				cmdColorCode.BackColor = SkinManager.Instance.CurrentSkin.Background.Normal;
				cmdColorCode.Tag = null;
			}
			else
			{
				cmdColorCode.BackColor = code.GetColor();
				cmdColorCode.Tag = colorCode;
			}
		}

		private void AlternativeConditions_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
		{
			switch (e.Action)
			{
				case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
					AddAlternateTab();
					break;
				case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
					tabsConditions.TabPages.RemoveAt(e.OldStartingIndex + 1);
					for (int i = e.OldStartingIndex + 1; i < tabsConditions.TabPages.Count; i++)
					{
						tabsConditions.TabPages[i].Text = "Set " + (i + 1);
					}
					break;
			}
		}

		private void stripConditions_AddButtonClicked(object sender, EventArgs e)
		{
			if (_selectedCase == null) { return; }
			Case alternate = new Case(_selectedCase.Tag);
			_selectedCase.AlternativeConditions.Add(alternate);
			_selectedCase.NotifyPropertyChanged(nameof(_selectedCase.AlternativeConditions));
			AddAlternateTab();
			tabsConditions.SelectedIndex = _selectedCase.AlternativeConditions.Count;
		}

		private void AddAlternateTab()
		{
			tabsConditions.TabPages.Add($"Set {(tabsConditions.TabPages.Count + 1)}");
		}

		private void stripConditions_CloseButtonClicked(object sender, EventArgs e)
		{
			if (_selectedCase == null) { return; }
			int index = tabsConditions.SelectedIndex - 1;
			if (index >= 0)
			{
				_selectedCase.AlternativeConditions.RemoveAt(index);
				_selectedCase.NotifyPropertyChanged(nameof(_selectedCase.AlternativeConditions));
				tabsConditions.TabPages.RemoveAt(index + 1);
				for (int i = index + 1; i < tabsConditions.TabPages.Count; i++)
				{
					tabsConditions.TabPages[i].Text = "Set " + (i + 1);
				}
				tabsConditions.SelectedIndex = index < tabsConditions.TabPages.Count - 1 ? index + 1 : index;
			}
		}

		private void tabsConditions_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (_selectedCase == null) { return; }
			int index = tabsConditions.SelectedIndex;
			Case desiredCase = _selectedCase;
			if (index > 0)
			{
				desiredCase = _selectedCase.AlternativeConditions[index - 1];
			}
			TrackCase(desiredCase);
			PopulateConditionTable(desiredCase);
		}

		private void TrackCase(Case c)
		{
			if (_trackedCase != null)
			{
				_trackedCase.PropertyChanged -= CasePropertyChanged;
			}
			_trackedCase = c;
			if (_trackedCase != null)
			{
				_trackedCase.PropertyChanged += CasePropertyChanged;
			}
		}
	}
}
