﻿using Desktop;
using Desktop.Providers;
using System.Collections.Generic;
using System.Windows.Forms;

namespace SPNATI_Character_Editor.Controls
{
	public partial class MarkerControl : UserControl
	{
		private Character _character;

		public bool ShowWhen
		{
			get
			{
				return ColWhen.Visible;
			}
			set
			{
				ColWhen.Visible = value;
			}
		}

		public bool AlwaysPersistent
		{
			get
			{
				return !ColPersistent.Visible;
			}
			set
			{
				ColPersistent.Visible = !value;
			}
		}

		public List<MarkerOperation> GetMarkers()
		{

			gridMarkers.EndEdit();
			List<MarkerOperation> list = new List<MarkerOperation>();
			foreach (DataGridViewRow row in gridMarkers.Rows)
			{
				string name = row.Cells[nameof(ColName)].Value?.ToString();
				if (string.IsNullOrEmpty(name))
				{
					continue;
				}
				string op = row.Cells[nameof(ColOperator)].Value?.ToString();
				string value = row.Cells[nameof(ColValue)].Value?.ToString();
				bool perTarget = row.Cells[nameof(ColPerTarget)].Value != null ? (bool)row.Cells[nameof(ColPerTarget)].Value : false;
				bool persistent = AlwaysPersistent || (row.Cells[nameof(ColPersistent)].Value != null ? (bool)row.Cells[nameof(ColPersistent)].Value : false);
				string when = row.Cells[nameof(ColWhen)].Value?.ToString();

				if (persistent)
				{
					_character.Behavior.PersistentMarkers.Add(name);
				}
				else
				{
					_character.Behavior.PersistentMarkers.Remove(name);
				}
				if (!_character.Markers.Value.Contains(name))
				{
					_character.Markers.Value.Add(new Marker(name));
				}

				if (perTarget)
				{
					name += "*";
				}
				MarkerOperation marker = new MarkerOperation()
				{
					Name = name,
					Operator = op,
					Value = value,
					When = when,
				};
				list.Add(marker);

				
			}
			return list;
		}

		public void SetMarkers(List<MarkerOperation> markers, Character character)
		{
			_character = character;
			gridMarkers.Rows.Clear();
			if (markers != null)
			{
				foreach (MarkerOperation marker in markers)
				{
					bool perTarget = false;
					string name = marker.Name;
					if (name.EndsWith("*"))
					{
						name = name.Substring(0, name.Length - 1);
						perTarget = true;
					}
					bool persistent = character.Behavior.PersistentMarkers.Contains(name);
					gridMarkers.Rows.Add(name, marker.Operator, marker.Value, perTarget, persistent, marker.When);
				}
			}

		}

		public MarkerControl()
		{
			InitializeComponent();
			ColPerTarget.TrueValue = true;
			ColOperator.RecordType = typeof(MarkerOperatorCategory);
			ColWhen.RecordType = typeof(MarkerWhen);
		}
	}

	public class MarkerOperatorCategory : Category
	{
		public MarkerOperatorCategory(string key, string value) : base(key, value)
		{
		}
	}

	public class MarkerOperatorProvider : CategoryProvider<MarkerOperatorCategory>
	{
		public override string GetLookupCaption()
		{
			return "Choose an operator";
		}

		protected override MarkerOperatorCategory[] GetCategoryValues()
		{
			return new MarkerOperatorCategory[] {
					new MarkerOperatorCategory("+", "+"),
					new MarkerOperatorCategory("-", "-"),
					new MarkerOperatorCategory("*", "*"),
					new MarkerOperatorCategory("/", "/"),
					new MarkerOperatorCategory("%", "%"),
					new MarkerOperatorCategory("=", "="),
				};
		}
	}

	public class MarkerWhen : Category
	{
		public MarkerWhen(string key, string value) : base(key, value)
		{
		}
	}

	public class MarkerWhenProvider : CategoryProvider<MarkerWhen>
	{
		public override string GetLookupCaption()
		{
			return "Choose when to execute marker operation";
		}

		protected override MarkerWhen[] GetCategoryValues()
		{
			return new MarkerWhen[] {
					new MarkerWhen("game", "In game"),
					new MarkerWhen("gallery", "From gallery"),
					new MarkerWhen("always", "Always"),
				};
		}
	}
}
