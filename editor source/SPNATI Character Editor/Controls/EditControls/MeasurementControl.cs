﻿using System;
using System.Text.RegularExpressions;
using Desktop;
using Desktop.CommonControls;
using System.Drawing;
using System.IO;
using SPNATI_Character_Editor.Controls;

namespace SPNATI_Character_Editor.EditControls
{
	public partial class MeasurementControl : PropertyEditControl
	{
		private Directive _directive;
		private bool _allowPercentages;

		public MeasurementControl()
		{
			InitializeComponent();
		}

		protected override void OnSetParameters(EditControlAttribute parameters)
		{
			MeasurementAttribute attrib = parameters as MeasurementAttribute;
			valValue.Minimum = attrib.Minimum;
			valValue.Maximum = attrib.Maximum;
			_allowPercentages = attrib.AllowPercentages;
		}

		protected override void RemoveHandlers()
		{
			radPct.CheckedChanged -= ValueChanged;
			radPx.CheckedChanged -= ValueChanged;
			valValue.TextChanged -= ValueChanged;
		}

		protected override void AddHandlers()
		{
			radPct.CheckedChanged += ValueChanged;
			radPx.CheckedChanged += ValueChanged;
			valValue.TextChanged += ValueChanged;
			chkCentered.CheckedChanged += ChkCentered_CheckedChanged;
		}

		private void ChkCentered_CheckedChanged(object sender, EventArgs e)
		{
			valValue.Enabled = !chkCentered.Checked;
			Save();
		}

		protected override void OnBoundData()
		{
			_directive = Data as Directive;

			bool isText = (_directive != null && _directive.DirectiveType == "text");
			int value;
			string text = GetValue()?.ToString();
			Regex regex = new Regex(@"^(-?\d+)(px|%)?$");
			if (!string.IsNullOrEmpty(text))
			{
				Match match = regex.Match(text);
				if (match.Success)
				{
					int.TryParse(match.Groups[1].Value, out value);
					valValue.Value = Math.Max(valValue.Minimum, Math.Min(valValue.Maximum, value));
					valValue.Text = valValue.Value.ToString();
				}
			}
			else
			{
				valValue.Text = "";
			}

			if (isText)
			{
				radPct.Checked = true;
				radPct.Visible = false;
				radPx.Visible = false;
				lblPct.Left = valValue.Left + valValue.Width;
				lblPct.Visible = true;
				chkCentered.Enabled = true;

				if (Property == "X")
				{
					chkCentered.Left = lblPct.Left + lblPct.Width + 5;
					chkCentered.Visible = true;

					if (text == "centered")
					{
						chkCentered.Checked = true;
						valValue.Enabled = false;
					}
					else
					{
						chkCentered.Checked = false;
					}
				}
			}
			else
			{
				radPct.Visible = _allowPercentages;
				radPx.Visible = true;
				lblPct.Visible = false;
				chkCentered.Visible = false;
				chkCentered.Enabled = false;
				valValue.Enabled = true;
				if (text != null && text.EndsWith("%") && _allowPercentages)
				{
					radPct.Checked = true;
				}
				else
				{
					radPx.Checked = true;
				}
			}
		}

		protected override void OnBindingUpdated(string property)
		{
			if (property == "Background")
			{
				EpilogueContext context = Context as EpilogueContext;
				ISkin character = context?.Character;
				string file = GetBindingValue(property)?.ToString();
				if (!string.IsNullOrEmpty(file) && character != null)
				{
					if (file.StartsWith("/"))
					{
						file = Path.Combine(Config.SpnatiDirectory, file.Substring(1));
					}
					else
					{
						file = Path.Combine(character.GetDirectory(), file);
					}
					if (File.Exists(file))
					{
						using (Bitmap bmp = new Bitmap(file))
						{
							if (Property == "Width")
							{
								RemoveHandlers();
								valValue.Value = bmp.Width;
								radPx.Checked = true;
								chkCentered.Checked = false;
								AddHandlers();
								Save();
							}
							else if (Property == "Height")
							{
								RemoveHandlers();
								valValue.Value = bmp.Height;
								radPx.Checked = true;
								chkCentered.Checked = false;
								AddHandlers();
								Save();
							}
						}
					}
				}
			}
		}

		private void ValueChanged(object sender, EventArgs e)
		{
			Save();
		}

		protected override void OnClear()
		{
			RemoveHandlers();
			radPx.Checked = true;
			valValue.Text = "";
			Save();
			AddHandlers();
		}

		protected override void OnSave()
		{
			if (chkCentered.Checked)
			{
				SetValue("centered");
				return;
			}

			if (string.IsNullOrEmpty(valValue.Text))
			{
				SetValue(null);
				return;
			}
			bool pctUnits = radPct.Checked && _allowPercentages;
			int value = (int)valValue.Value;
			string v = value.ToString();
			if (pctUnits)
			{
				v += "%";
			}
			SetValue(v);
		}
	}

	public class MeasurementAttribute : EditControlAttribute
	{
		public override Type EditControlType { get { return typeof(MeasurementControl); } }

		public int Minimum = -100000;
		public int Maximum = 100000;
		public bool AllowPercentages = true;
	}
}
