﻿namespace SPNATI_Character_Editor.Activities
{
	partial class SituationEditor
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
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
			this.gridCases = new Desktop.Skinning.SkinnedDataGridView();
			this.gridLines = new SPNATI_Character_Editor.Controls.DialogueGrid();
			this.ColName = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.ColDescription = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.ColPriority = new Desktop.Skinning.SkinnedDataGridViewComboBoxColumn();
			this.ColStages = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.ColTrigger = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.ColJump = new Desktop.Skinning.SkinnedDataGridViewButtonColumn();
			this.ColDelete = new Desktop.Skinning.SkinnedDataGridViewButtonColumn();
			((System.ComponentModel.ISupportInitialize)(this.gridCases)).BeginInit();
			this.SuspendLayout();
			// 
			// gridCases
			// 
			this.gridCases.AllowUserToAddRows = false;
			this.gridCases.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridCases.BackgroundColor = System.Drawing.Color.White;
			this.gridCases.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.gridCases.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle1.BackColor = System.Drawing.Color.White;
			dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
			dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
			dataGridViewCellStyle1.Padding = new System.Windows.Forms.Padding(0, 5, 0, 5);
			dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.gridCases.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
			this.gridCases.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.gridCases.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColName,
            this.ColDescription,
            this.ColPriority,
            this.ColStages,
            this.ColTrigger,
            this.ColJump,
            this.ColDelete});
			dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
			dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
			dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
			dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.gridCases.DefaultCellStyle = dataGridViewCellStyle2;
			this.gridCases.EnableHeadersVisualStyles = false;
			this.gridCases.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
			this.gridCases.GridColor = System.Drawing.Color.LightGray;
			this.gridCases.Location = new System.Drawing.Point(0, 0);
			this.gridCases.MultiSelect = false;
			this.gridCases.Name = "gridCases";
			this.gridCases.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
			dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle3.BackColor = System.Drawing.Color.White;
			dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
			dataGridViewCellStyle3.ForeColor = System.Drawing.Color.Black;
			dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.gridCases.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
			this.gridCases.Size = new System.Drawing.Size(999, 458);
			this.gridCases.TabIndex = 0;
			this.gridCases.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridCases_CellContentClick);
			this.gridCases.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.gridCases_CellPainting);
			this.gridCases.SelectionChanged += new System.EventHandler(this.gridCases_SelectionChanged);
			this.gridCases.UserDeletingRow += new System.Windows.Forms.DataGridViewRowCancelEventHandler(this.gridCases_UserDeletingRow);
			// 
			// gridLines
			// 
			this.gridLines.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridLines.Location = new System.Drawing.Point(0, 462);
			this.gridLines.Name = "gridLines";
			this.gridLines.ReadOnly = false;
			this.gridLines.Size = new System.Drawing.Size(999, 169);
			this.gridLines.TabIndex = 1;
			this.gridLines.HighlightRow += new System.EventHandler<int>(this.gridLines_HighlightRow);
			// 
			// ColName
			// 
			this.ColName.HeaderText = "Name";
			this.ColName.Name = "ColName";
			this.ColName.Width = 125;
			// 
			// ColDescription
			// 
			this.ColDescription.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.ColDescription.HeaderText = "Description";
			this.ColDescription.Name = "ColDescription";
			// 
			// ColPriority
			// 
			this.ColPriority.AutoComplete = false;
			this.ColPriority.DisplayMember = null;
			this.ColPriority.HeaderText = "Priority";
			this.ColPriority.Name = "ColPriority";
			this.ColPriority.Sorted = false;
			this.ColPriority.Width = 90;
			// 
			// ColStages
			// 
			this.ColStages.HeaderText = "Stages";
			this.ColStages.Name = "ColStages";
			this.ColStages.ReadOnly = true;
			this.ColStages.Width = 80;
			// 
			// ColTrigger
			// 
			this.ColTrigger.HeaderText = "Trigger";
			this.ColTrigger.Name = "ColTrigger";
			this.ColTrigger.ReadOnly = true;
			this.ColTrigger.Width = 150;
			// 
			// ColJump
			// 
			this.ColJump.FieldType = Desktop.Skinning.SkinnedFieldType.Primary;
			this.ColJump.Flat = false;
			this.ColJump.HeaderText = "";
			this.ColJump.Name = "ColJump";
			this.ColJump.Resizable = System.Windows.Forms.DataGridViewTriState.True;
			this.ColJump.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
			this.ColJump.Width = 24;
			// 
			// ColDelete
			// 
			this.ColDelete.FieldType = Desktop.Skinning.SkinnedFieldType.Primary;
			this.ColDelete.Flat = false;
			this.ColDelete.HeaderText = "";
			this.ColDelete.Name = "ColDelete";
			this.ColDelete.Width = 21;
			// 
			// SituationEditor
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.gridLines);
			this.Controls.Add(this.gridCases);
			this.Name = "SituationEditor";
			this.Size = new System.Drawing.Size(999, 634);
			((System.ComponentModel.ISupportInitialize)(this.gridCases)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private Desktop.Skinning.SkinnedDataGridView gridCases;
		private Controls.DialogueGrid gridLines;
		private System.Windows.Forms.DataGridViewTextBoxColumn ColName;
		private System.Windows.Forms.DataGridViewTextBoxColumn ColDescription;
		private Desktop.Skinning.SkinnedDataGridViewComboBoxColumn ColPriority;
		private System.Windows.Forms.DataGridViewTextBoxColumn ColStages;
		private System.Windows.Forms.DataGridViewTextBoxColumn ColTrigger;
		private Desktop.Skinning.SkinnedDataGridViewButtonColumn ColJump;
		private Desktop.Skinning.SkinnedDataGridViewButtonColumn ColDelete;
	}
}
