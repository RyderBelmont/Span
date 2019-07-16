﻿using Desktop;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace SPNATI_Character_Editor.Activities
{
	[Activity(typeof(Character), 2)]
	public partial class TagEditor : Activity
	{
		private Character _character;
		private BindableTagList _bindings;
		private bool _pendingWardrobeChange;
		private bool _initialized;

		public TagEditor()
		{
			InitializeComponent();
		}

		public override string Caption
		{
			get { return "Tags"; }
		}

		protected override void OnInitialize()
		{
			_character = Record as Character;
			SubscribeWorkspace(WorkspaceMessages.WardrobeUpdated, OnWardrobeChanged);
			SubscribeWorkspace(WorkspaceMessages.SkinChanged, OnSkinChanged);
		}

		protected override void OnFirstActivate()
		{
			LoadTags();
			_initialized = true;
			_pendingWardrobeChange = false;
		}

		protected override void OnActivate()
		{
			if (_pendingWardrobeChange)
			{
				PopulateData();
			}
		}

		private void OnWardrobeChanged()
		{
			_pendingWardrobeChange = true;
		}

		private void OnSkinChanged()
		{
			tagGrid.Refresh();
		}

		/// <summary>
		/// Populates the Tags grid with the character's tags
		/// </summary>
		private void LoadTags()
		{
			TagDictionary dictionary = TagDatabase.Dictionary;
			_bindings = new BindableTagList(_character);

			foreach (Tag tag in dictionary.Tags)
			{
				_bindings.Add(tag.Value);
			}

			//Fill the tag group
			string gender = _character.Gender;
			foreach (TagGroup group in dictionary.Groups)
			{
				if (group.Hidden)
				{
					continue;
				}

				if (string.IsNullOrEmpty(group.Gender) || group.Gender == gender)
				{
					toc.Items.Add(group);
				}
			}
			
			PopulateData();
			if (toc.Items.Count > 0)
			{
				toc.SelectedIndex = 0;
			}
		}

		private void PopulateData()
		{
			tagList.SetData(_bindings, _character);

			tagGrid.SetCharacter(_character, _bindings);
			tagGrid.Visible = _initialized;
		}

		public override void Save()
		{
			SaveTags();
		}

		/// <summary>
		/// Saves the Tags grid into the current character
		/// </summary>
		private void SaveTags()
		{
			_bindings.SaveIntoCharacter();
		}

		private void toc_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			TagGroup group = toc.SelectedItem as TagGroup;
			tagGrid.SetGroup(group);
			tagGrid.Visible = true;
		}
	}
}
