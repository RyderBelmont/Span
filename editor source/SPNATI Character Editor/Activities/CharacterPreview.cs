﻿using Desktop;
using Desktop.Skinning;
using SPNATI_Character_Editor.Controls.Reference;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SPNATI_Character_Editor.Activities
{
	[Activity(typeof(Character), -1, Width = 251, Pane = WorkspacePane.Sidebar)]
	[Activity(typeof(Costume), -1, Width = 251, Pane = WorkspacePane.Sidebar)]
	public partial class CharacterPreview : Activity
	{
		private Character _character;

		public CharacterPreview()
		{
			InitializeComponent();
		}

		public override string Caption
		{
			get { return "Overview"; }
		}

		protected override void OnInitialize()
		{
			_character = Record as Character;
			if (_character != null)
			{
				_character.PrepareForEdit();
				_character.Behavior.CaseAdded += WorkingCasesChanged;
				_character.Behavior.CaseRemoved += WorkingCasesChanged;
				_character.Behavior.CaseModified += WorkingCasesChanged;
				chkText.Checked = Config.GetBoolean("PreviewText");
			}
			else
			{
				lblSkin.Visible = false;
				cboSkin.Visible = false;
				chkText.Visible = false;
			}
			SubscribeWorkspace<DialogueLine>(WorkspaceMessages.PreviewLine, UpdatePreview);
			SubscribeWorkspace<CharacterImage>(WorkspaceMessages.UpdatePreviewImage, UpdatePreviewImage);
			SubscribeWorkspace<List<string>>(WorkspaceMessages.UpdateMarkers, UpdateMarkers);
			UpdateLineCount();
		}

		protected override void OnActivate()
		{
			PopulateSkinCombo();
		}

		private void PopulateSkinCombo()
		{
			if (_character == null) { return; }

			SkinLink previous = cboSkin.SelectedItem as SkinLink;

			cboSkin.Items.Clear();
			cboSkin.Items.Add("- Default - ");
			foreach (AlternateSkin alt in _character.Metadata.AlternateSkins)
			{
				foreach (SkinLink link in alt.Skins)
				{
					cboSkin.Items.Add(link);
				}
			}
			cboSkin.Sorted = true;
			cboSkin.Visible = cboSkin.Items.Count > 1;
			lblSkin.Visible = cboSkin.Visible;

			if (previous == null)
			{
				cboSkin.SelectedIndex = 0;
			}
			else
			{
				cboSkin.SelectedItem = previous;
			}
		}

		private void WorkingCasesChanged(object sender, Case e)
		{
			UpdateLineCount();
		}

		public override void Quit()
		{
			picPortrait.Destroy();
		}

		private void UpdateMarkers(List<string> markers)
		{
			picPortrait.SetMarkers(markers);
		}

		private void UpdatePreviewImage(CharacterImage image)
		{
			picPortrait.SetImage(image);
		}

		private void UpdatePreview(DialogueLine line)
		{
			picPortrait.SetText(line);
		}

		private void UpdateLineCount()
		{
			if (_character != null)
			{
				lblLinesOfDialogue.Text = $"Unique lines: {(_character.Behavior.UniqueLines.ToString())}";
			}
			else
			{
				lblLinesOfDialogue.Visible = false;
			}
		}

		private void cboSkin_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (_character == null) { return; }
			SkinLink current = cboSkin.SelectedItem as SkinLink;
			_character.CurrentSkin = current?.Costume;

			//update images in use to use new skin
			ImageLibrary library = ImageLibrary.Get(_character);
			library.UpdateSkin(_character.CurrentSkin);
			Workspace.SendMessage(WorkspaceMessages.SkinChanged);
		}

		private void chkText_CheckedChanged(object sender, System.EventArgs e)
		{
			bool preview = chkText.Checked;
			Config.Set("PreviewText", preview);
			picPortrait.ShowTextBox = preview;
		}

		private void cmdReference_Click(object sender, System.EventArgs e)
		{
			splitContainer1.Panel2Collapsed = !splitContainer1.Panel2Collapsed;
			cmdReference.Image = splitContainer1.Panel2Collapsed ? Properties.Resources.ChevronUp : Properties.Resources.ChevronDown;
			splitContainer1.Panel1.Invalidate(true);
			if (!splitContainer1.Panel2Collapsed && tabTags.Controls.Count == 0)
			{
				BuildReference();
			}
		}

		private void BuildReference()
		{
			tabsReference.Width = splitContainer1.Panel2.Width;
			tabsReference.Height = splitContainer1.Panel2.Height - stripReference.Height;
			TagGuide guide = new TagGuide();
			tabTags.Controls.Add(guide);
			guide.Dock = System.Windows.Forms.DockStyle.Fill;
			TargetReport report = new TargetReport(this._character);
			tabTargets.Controls.Add(report);
			report.Dock = DockStyle.Fill;
			SkinManager.UpdateSkin(report, SkinManager.Instance.CurrentSkin);
		}

		private void splitContainer1_Panel1_Resize(object sender, System.EventArgs e)
		{
			splitContainer1.Panel1.Invalidate(true);
		}
	}
}
