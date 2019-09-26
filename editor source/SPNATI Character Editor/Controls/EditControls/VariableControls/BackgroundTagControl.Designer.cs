﻿using Desktop.CommonControls;
using Desktop.Skinning;
using System.Drawing;
using System.Windows.Forms;

namespace SPNATI_Character_Editor.Controls.EditControls.VariableControls
{
	partial class BackgroundTagControl
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.recTag = new Desktop.CommonControls.RecordField();
			this.skinnedLabel1 = new Desktop.Skinning.SkinnedLabel();
			this.panelBoolean = new System.Windows.Forms.Panel();
			this.radFalse = new Desktop.Skinning.SkinnedRadioButton();
			this.radTrue = new Desktop.Skinning.SkinnedRadioButton();
			this.panelBoolean.SuspendLayout();
			this.SuspendLayout();
			// 
			// recTag
			// 
			this.recTag.AllowCreate = false;
			this.recTag.Location = new System.Drawing.Point(53, 0);
			this.recTag.Name = "recTag";
			this.recTag.PlaceholderText = null;
			this.recTag.Record = null;
			this.recTag.RecordContext = null;
			this.recTag.RecordFilter = null;
			this.recTag.RecordKey = null;
			this.recTag.RecordType = null;
			this.recTag.Size = new System.Drawing.Size(127, 20);
			this.recTag.TabIndex = 0;
			this.recTag.UseAutoComplete = true;
			// 
			// skinnedLabel1
			// 
			this.skinnedLabel1.AutoSize = true;
			this.skinnedLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
			this.skinnedLabel1.ForeColor = System.Drawing.SystemColors.ControlText;
			this.skinnedLabel1.Highlight = Desktop.Skinning.SkinnedHighlight.Normal;
			this.skinnedLabel1.Level = Desktop.Skinning.SkinnedLabelLevel.Normal;
			this.skinnedLabel1.Location = new System.Drawing.Point(-2, 4);
			this.skinnedLabel1.Name = "skinnedLabel1";
			this.skinnedLabel1.Size = new System.Drawing.Size(49, 13);
			this.skinnedLabel1.TabIndex = 1;
			this.skinnedLabel1.Text = "Property:";
			// 
			// panelBoolean
			// 
			this.panelBoolean.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.panelBoolean.Controls.Add(this.radFalse);
			this.panelBoolean.Controls.Add(this.radTrue);
			this.panelBoolean.Location = new System.Drawing.Point(188, 0);
			this.panelBoolean.Name = "panelBoolean";
			this.panelBoolean.Size = new System.Drawing.Size(123, 21);
			this.panelBoolean.TabIndex = 2;
			// 
			// radFalse
			// 
			this.radFalse.AutoSize = true;
			this.radFalse.FieldType = Desktop.Skinning.SkinnedFieldType.Primary;
			this.radFalse.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
			this.radFalse.Location = new System.Drawing.Point(56, 2);
			this.radFalse.Name = "radFalse";
			this.radFalse.Size = new System.Drawing.Size(50, 17);
			this.radFalse.TabIndex = 1;
			this.radFalse.TabStop = true;
			this.radFalse.Text = "False";
			this.radFalse.UseVisualStyleBackColor = true;
			// 
			// radTrue
			// 
			this.radTrue.AutoSize = true;
			this.radTrue.FieldType = Desktop.Skinning.SkinnedFieldType.Primary;
			this.radTrue.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
			this.radTrue.Location = new System.Drawing.Point(3, 2);
			this.radTrue.Name = "radTrue";
			this.radTrue.Size = new System.Drawing.Size(47, 17);
			this.radTrue.TabIndex = 0;
			this.radTrue.TabStop = true;
			this.radTrue.Text = "True";
			this.radTrue.UseVisualStyleBackColor = true;
			// 
			// BackgroundTagControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.panelBoolean);
			this.Controls.Add(this.skinnedLabel1);
			this.Controls.Add(this.recTag);
			this.Name = "BackgroundTagControl";
			this.Size = new System.Drawing.Size(389, 21);
			this.panelBoolean.ResumeLayout(false);
			this.panelBoolean.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private ExpressionTest _expression;

		private RecordField recTag;
		private SkinnedLabel skinnedLabel1;
		private Panel panelBoolean;
		private SkinnedRadioButton radFalse;
		private SkinnedRadioButton radTrue;
	}
}
