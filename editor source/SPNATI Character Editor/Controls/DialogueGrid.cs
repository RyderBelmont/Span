﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace SPNATI_Character_Editor.Controls
{
	public partial class DialogueGrid : UserControl
	{
		private Case _selectedCase;
		private Stage _selectedStage;
		private Character _selectedCharacter;
		private ImageLibrary _imageLibrary;
		private bool _populatingCase;
		private int _selectedRow;

		#region Events
		public new event EventHandler<KeyEventArgs> KeyDown;
		public event EventHandler<int> HighlightRow;
		#endregion

		public DialogueGrid()
		{
			InitializeComponent();
		}

		public void SetData(Character character, Stage stage, Case c, HashSet<int> selectedStages, ImageLibrary imageLibrary)
		{
			_selectedCharacter = character;
			_selectedStage = stage;
			_selectedCase = c;
			_imageLibrary = imageLibrary;
			_populatingCase = true;
			for (int i = 0; i < gridDialogue.Rows.Count; i++)
			{
				DataGridViewRow row = gridDialogue.Rows[i];
				row.Cells["ColImage"].Value = null;
			}

			UpdateAvailableImagesForCase(selectedStages, false);

			//Populate lines
			gridDialogue.Rows.Clear();
			List<DialogueLine> lines = (_selectedCase.Tag == Trigger.StartTrigger ? _selectedCharacter.StartingLines : _selectedCase.Lines);
			foreach (DialogueLine line in lines)
			{
				AddLineToDialogueGrid(line);
			}
			if (lines.Count > 0)
				SelectRow(0);
			_populatingCase = false;
		}

		public void Save()
		{
			List<DialogueLine> lines = (_selectedCase.Tag == Trigger.StartTrigger ? _selectedCharacter.StartingLines : _selectedCase.Lines);
			lines.Clear();
			for (int i = 0; i < gridDialogue.Rows.Count; i++)
			{
				DialogueLine line = ReadLineFromDialogueGrid(i);
				if (line != null)
				{
					lines.Add(line);
					_selectedCharacter.CacheMarker(line.Marker);
				}
			}
		}

		private void SelectRow(int index)
		{
			_selectedRow = index;
			HighlightRow?.Invoke(this, index);
		}

		/// <summary>
		/// Converts a row in the dialogue grid into a DialogueLine
		/// </summary>
		/// <param name="rowIndex"></param>
		/// <returns></returns>
		private DialogueLine ReadLineFromDialogueGrid(int rowIndex)
		{
			DataGridViewRow row = gridDialogue.Rows[rowIndex];
			string image = row.Cells["ColImage"].Value?.ToString();
			string text = row.Cells["ColText"].Value?.ToString();
			string silent = row.Cells["ColSilent"].Value?.ToString();
			string marker = row.Cells["ColMarker"].Value?.ToString();
			if (silent == "")
				text = "";
			if (text == "~silent~")
			{
				text = "";
				silent = "";
			}
			if (text == null)
				return null;
			CharacterImage img = _imageLibrary.Find(image);
			string extension = img != null ? img.FileExtension : ".png";
			DialogueLine line = new DialogueLine(DialogueLine.GetDefaultImage(image) + extension, text);
			line.IsSilent = silent;
			line.Marker = string.IsNullOrEmpty(marker) ? null : marker;
			return line;
		}

		/// <summary>
		/// Updates the Image column in the dialogue grid to contain only images that exist for all currently selected stages
		/// </summary>
		/// <param name="retainValue"></param>
		public void UpdateAvailableImagesForCase(HashSet<int> selectedStages, bool retainValue)
		{
			List<object> values = new List<object>();
			if (retainValue)
			{
				//save off values
				for (int i = 0; i < gridDialogue.Rows.Count; i++)
				{
					DataGridViewRow row = gridDialogue.Rows[i];
					values.Add(row.Cells["ColImage"].Value);
				}
			}

			int stageId = _selectedStage == null ? 0 : _selectedStage.Id;
			DataGridViewComboBoxColumn col = gridDialogue.Columns["ColImage"] as DataGridViewComboBoxColumn;
			col.Items.Clear();
			List<CharacterImage> images = new List<CharacterImage>();
			if (_selectedStage == null)
			{
				images.AddRange(_imageLibrary.GetImages(0));
				images.AddRange(_imageLibrary.GetImages(-1));
				foreach (var image in images)
				{
					col.Items.Add(image);
				}
			}
			else
			{
				images.AddRange(_imageLibrary.GetImages(stageId));
				images.AddRange(_imageLibrary.GetImages(-1));

				foreach (var image in images)
				{
					string name = DialogueLine.GetDefaultImage(image.Name);
					//Filter out the ones that don't appear in every selected stage
					bool allExist = true;
					if (!image.IsGeneric)
					{
						foreach (int stage in selectedStages)
						{
							if (_imageLibrary.Find(stage + "-" + name) == null)
							{
								allExist = false;
								break;
							}
						}

					}
					if (allExist)
						col.Items.Add(image);
				}
			}

			col.DisplayMember = "DefaultName";

			if (retainValue)
			{
				//restore values
				for (int i = 0; i < gridDialogue.Rows.Count; i++)
				{
					DataGridViewRow row = gridDialogue.Rows[i];

					//Make sure the value is still valid
					bool found = false;
					foreach (var item in col.Items)
					{
						CharacterImage img = item as CharacterImage;
						CharacterImage oldImg = values[i] as CharacterImage;
						if ((oldImg == null && img.DefaultName == values[i]?.ToString()) || (oldImg != null && oldImg.DefaultName == img.DefaultName))
						{
							row.Cells["ColImage"].Value = item;
							found = true;
							break;
						}
					}
					if (!found)
					{
						row.Cells["ColImage"].Value = null;
					}
				}
			}
		}

		private void gridDialogue_KeyDown(object sender, KeyEventArgs e)
		{
			KeyDown?.Invoke(this, e);
		}

		private void gridDialogue_CellEnter(object sender, DataGridViewCellEventArgs e)
		{
			SelectRow(e.RowIndex);
		}

		private void gridDialogue_CellParsing(object sender, DataGridViewCellParsingEventArgs e)
		{
			if (_selectedCase == null)
				return;
			if (e.ColumnIndex == 1)
			{
				//Validate variables in text
				string text = e.Value?.ToString();
				if (string.IsNullOrEmpty(text))
					return;
				Regex varRegex = new Regex(@"~\w*~", RegexOptions.IgnoreCase);
				List<string> invalidVars = new List<string>();
				e.Value = varRegex.Replace(text, (match) =>
				{
					string var = match.Value;
					if (var == "~clothes~")
					{
						var = "~clothing~";
					}
					return var;
				});
				e.ParsingApplied = true;
			}
		}

		private void gridDialogue_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
		{
			if (_selectedCase == null || e.FormattedValue == null || _populatingCase)
				return;

			Regex varRegex = new Regex(@"~\w*~", RegexOptions.IgnoreCase);
			List<string> invalidVars = DialogueLine.GetInvalidVariables(_selectedCase.Tag, e.FormattedValue.ToString());
			if (invalidVars.Count > 0)
			{
				MessageBox.Show(string.Format("The following variables are invalid for this line: {0}", string.Join(",", invalidVars)));
				e.Cancel = true;
			}
		}

		private void gridDialogue_CellValueChanged(object sender, DataGridViewCellEventArgs e)
		{
			if (e.ColumnIndex == 0)
			{
				SelectRow(e.RowIndex);
			}
		}

		private void gridDialogue_CurrentCellDirtyStateChanged(object sender, EventArgs e)
		{
			if (gridDialogue.IsCurrentCellDirty)
			{
				gridDialogue.CommitEdit(DataGridViewDataErrorContexts.Commit);
			}
		}

		public bool IsEmpty
		{
			get
			{
				return gridDialogue.Rows.Count == 0 || string.IsNullOrEmpty(gridDialogue.Rows[0].Cells["ColText"].Value?.ToString());
			}
		}

		public void SetFocus()
		{
			if (ActiveControl != gridDialogue && ActiveControl != gridDialogue.EditingControl)
				gridDialogue.Select();
			if (gridDialogue.EditingControl != null && ActiveControl != gridDialogue.EditingControl)
				gridDialogue.EditingControl.Select();
		}

		/// <summary>
		/// Gets the image at a row
		/// </summary>
		/// <param name="row"></param>
		/// <returns></returns>
		public string GetImage(int index)
		{
			DataGridViewRow row = gridDialogue.Rows[index];
			string image = row.Cells["ColImage"].Value?.ToString();
			return image;
		}

		public List<DialogueLine> CopyLines()
		{
			List<DialogueLine> lines = new List<DialogueLine>();
			for (int i = 0; i < gridDialogue.Rows.Count; i++)
			{
				var line = ReadLineFromDialogueGrid(i);
				if (line != null)
					lines.Add(line);
			}
			return lines;
		}

		public void Clear()
		{
			gridDialogue.Rows.Clear();
		}

		public void PasteLines(List<DialogueLine> lines)
		{
			foreach (var line in lines)
			{
				AddLineToDialogueGrid(line);
			}
		}

		/// <summary>
		/// Adds a row to the dialogue grid
		/// </summary>
		/// <param name="line">Line to populate the row with</param>
		private void AddLineToDialogueGrid(DialogueLine line)
		{
			string imageKey = line.Image;
			CharacterImage image = _imageLibrary.Find(imageKey);
			if (image == null && _selectedCase.Stages.Count > 0)
			{
				int stage = _selectedCase.Stages[0];
				if (_selectedStage != null)
					stage = _selectedStage.Id;
				imageKey = string.Format("{0}-{1}", stage, imageKey);
				image = _imageLibrary.Find(imageKey);
			}

			DataGridViewRow row = gridDialogue.Rows[gridDialogue.Rows.Add()];
			DataGridViewComboBoxCell imageCell = row.Cells["ColImage"] as DataGridViewComboBoxCell;
			SetImage(imageCell, imageKey);
			DataGridViewCell textCell = row.Cells["ColText"];
			textCell.Value = line.Text;
			DataGridViewCheckBoxCell silentCell = row.Cells["ColSilent"] as DataGridViewCheckBoxCell;
			silentCell.FalseValue = null;
			silentCell.TrueValue = "";
			silentCell.Value = line.IsSilent;

			DataGridViewCell markerCell = row.Cells["ColMarker"];
			markerCell.Value = line.Marker;
		}

		/// <summary>
		/// Sets the image cell of a dialogue row
		/// </summary>
		/// <param name="cell"></param>
		/// <param name="key"></param>
		private void SetImage(DataGridViewComboBoxCell cell, string key)
		{
			string defaultKey = Path.GetFileNameWithoutExtension(DialogueLine.GetDefaultImage(key));
			foreach (var item in cell.Items)
			{
				CharacterImage image = item as CharacterImage;
				if (image != null && image.DefaultName == defaultKey)
				{
					cell.Value = item;
					return;
				}
			}
		}

		#region Find/replace
		/// <summary>
		/// Gets the text selection start of a grid's TextBox cell, if there is one
		/// </summary>
		/// <param name="grid">Grid to look at</param>
		/// <returns>Selection start index, or -1 if there is none</returns>
		private int GetSelectionStart(DataGridView grid)
		{
			if (grid.CurrentCell == null)
				return -1;
			if (grid.EditingControl != null && grid.EditingControl is TextBox)
			{
				TextBox box = (TextBox)grid.EditingControl;
				return box.SelectionStart;
			}
			return -1;
		}

		private void ReplaceText(string replacement)
		{
			if (gridDialogue.EditingControl != null && gridDialogue.EditingControl is TextBox)
			{
				TextBox box = (TextBox)gridDialogue.EditingControl;
				int start = box.SelectionStart;
				box.SelectedText = replacement;
				box.SelectionStart = start;
				box.SelectionLength = replacement.Length;
			}
		}

		/// <summary>
		/// Looks for a find match in a string of text
		/// </summary>
		/// <param name="text">Text to search</param>
		/// <param name="args">Search args</param>
		/// <returns>Index in the string where the match begins, or -1 if no match</returns>
		public int FindText(string text, int startIndex, FindArgs args)
		{
			if (string.IsNullOrEmpty(text) || startIndex >= text.Length)
				return -1;
			string pattern = args.FindText;
			if (args.WholeWords)
			{
				pattern = string.Format(@"\b{0}\b", pattern);
			}

			Regex regex = new Regex(pattern, !args.MatchCase ? RegexOptions.IgnoreCase : RegexOptions.None);
			text = text.Substring(startIndex); //only search from the startIndex on
			Match match = regex.Match(text);
			if (match.Success)
			{
				return match.Index + startIndex;
			}
			return -1;
		}

		public void SelectTextInRow(int rowIndex, int startIndex, int length)
		{
			DataGridViewRow row = gridDialogue.Rows[rowIndex];
			gridDialogue.Select();
			gridDialogue.CurrentCell = row.Cells["ColText"];
			gridDialogue.BeginEdit(false);
			SelectText(startIndex, length);
		}

		private void SelectText(int start, int length)
		{
			TextBox textbox = (TextBox)gridDialogue.EditingControl;
			if (textbox != null)
			{
				textbox.SelectionStart = start;
				textbox.SelectionLength = length;
			}
		}

		public void ClearSelection()
		{
			_selectedRow = 0;
			TextBox box = gridDialogue.EditingControl as TextBox;
			if (box != null)
			{
				box.SelectionStart = 0;
				box.SelectionLength = 0;
			}
		}

		public bool FindReplace(FindArgs args)
		{
			int startLine = _selectedRow;
			int startIndex = GetSelectionStart(gridDialogue);
			if (args.DoReplace && startIndex >= 0)
			{
				//Back up a space when replacing so the highlighted word gets replaced
				startIndex--;
			}
			bool firstIteration = (startIndex == -1);
			for (int l = startLine; l < gridDialogue.Rows.Count; l++)
			{
				DataGridViewRow row = gridDialogue.Rows[l];
				string text = row.Cells["ColText"].Value?.ToString();
				if (!string.IsNullOrEmpty(text))
				{
					int index = -1;
					do
					{
						index = FindText(text, startIndex + 1, args);
						if (index >= 0)
						{
							//highlight it
							if (ActiveControl != gridDialogue.EditingControl || gridDialogue.CurrentCell != row.Cells["ColText"])
							{
								gridDialogue.Select();
								gridDialogue.CurrentCell = row.Cells["ColText"];
								gridDialogue.BeginEdit(false);
							}
							SelectText(index, args.FindText.Length);

							args.Success = true;

							if (args.DoReplace)
							{
								ReplaceText(args.ReplaceText);
								args.ReplaceCount++;
								text = row.Cells["ColText"].Value?.ToString();
							}

							if (!args.ReplaceAll)
								return true;
						}
						firstIteration = true;
					}
					while (index >= 0);
				}
				startIndex = -1;
			}
			return false;
		}
		#endregion
	}
}
