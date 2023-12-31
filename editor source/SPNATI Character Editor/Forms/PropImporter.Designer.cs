﻿namespace SPNATI_Character_Editor
{
	partial class PropImporter
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
			this.cmdOK = new Desktop.Skinning.SkinnedButton();
			this.gridMissingImages = new Desktop.Skinning.SkinnedDataGridView();
			this.ColImageName = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.ColImage = new System.Windows.Forms.DataGridViewImageColumn();
			this.ColAssign = new System.Windows.Forms.DataGridViewButtonColumn();
			this.label1 = new Desktop.Skinning.SkinnedLabel();
			this.label2 = new Desktop.Skinning.SkinnedLabel();
			this.label3 = new Desktop.Skinning.SkinnedLabel();
			this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			this.lblReady = new Desktop.Skinning.SkinnedLabel();
			this.cmdCancel = new Desktop.Skinning.SkinnedButton();
			this.dataGridViewButtonColumn1 = new System.Windows.Forms.DataGridViewButtonColumn();
			this.skinnedPanel1 = new Desktop.Skinning.SkinnedPanel();
			((System.ComponentModel.ISupportInitialize)(this.gridMissingImages)).BeginInit();
			this.skinnedPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// cmdOK
			// 
			this.cmdOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cmdOK.Background = Desktop.Skinning.SkinnedBackgroundType.Surface;
			this.cmdOK.Enabled = false;
			this.cmdOK.FieldType = Desktop.Skinning.SkinnedFieldType.Secondary;
			this.cmdOK.Flat = false;
			this.cmdOK.Location = new System.Drawing.Point(196, 3);
			this.cmdOK.Name = "cmdOK";
			this.cmdOK.Size = new System.Drawing.Size(75, 23);
			this.cmdOK.TabIndex = 1;
			this.cmdOK.Text = "Import";
			this.cmdOK.UseVisualStyleBackColor = true;
			this.cmdOK.Click += new System.EventHandler(this.cmdOK_Click);
			// 
			// gridMissingImages
			// 
			this.gridMissingImages.AllowUserToAddRows = false;
			this.gridMissingImages.AllowUserToDeleteRows = false;
			this.gridMissingImages.AllowUserToResizeColumns = false;
			this.gridMissingImages.AllowUserToResizeRows = false;
			this.gridMissingImages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMissingImages.BackgroundColor = System.Drawing.Color.White;
			this.gridMissingImages.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.gridMissingImages.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle1.BackColor = System.Drawing.Color.White;
			dataGridViewCellStyle1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
			dataGridViewCellStyle1.ForeColor = System.Drawing.Color.Black;
			dataGridViewCellStyle1.Padding = new System.Windows.Forms.Padding(0, 5, 0, 5);
			dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.gridMissingImages.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
			this.gridMissingImages.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.gridMissingImages.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColImageName,
            this.ColImage,
            this.ColAssign});
			dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle2.BackColor = System.Drawing.Color.White;
			dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
			dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
			dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.gridMissingImages.DefaultCellStyle = dataGridViewCellStyle2;
			this.gridMissingImages.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
			this.gridMissingImages.EnableHeadersVisualStyles = false;
			this.gridMissingImages.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
			this.gridMissingImages.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(142)))), ((int)(((byte)(153)))), ((int)(((byte)(243)))));
			this.gridMissingImages.Location = new System.Drawing.Point(12, 106);
			this.gridMissingImages.Name = "gridMissingImages";
			this.gridMissingImages.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
			dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle3.BackColor = System.Drawing.Color.White;
			dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
			dataGridViewCellStyle3.ForeColor = System.Drawing.Color.Black;
			dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.gridMissingImages.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
			this.gridMissingImages.RowHeadersVisible = false;
			this.gridMissingImages.RowTemplate.Height = 100;
			this.gridMissingImages.Size = new System.Drawing.Size(331, 407);
			this.gridMissingImages.TabIndex = 0;
			this.gridMissingImages.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridMissingImages_CellContentClick);
			// 
			// ColImageName
			// 
			this.ColImageName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.ColImageName.HeaderText = "Name";
			this.ColImageName.Name = "ColImageName";
			this.ColImageName.ReadOnly = true;
			// 
			// ColImage
			// 
			this.ColImage.HeaderText = "Image";
			this.ColImage.ImageLayout = System.Windows.Forms.DataGridViewImageCellLayout.Zoom;
			this.ColImage.Name = "ColImage";
			this.ColImage.ReadOnly = true;
			// 
			// ColAssign
			// 
			this.ColAssign.HeaderText = "Source";
			this.ColAssign.Name = "ColAssign";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
			this.label1.ForeColor = System.Drawing.Color.Black;
			this.label1.Highlight = Desktop.Skinning.SkinnedHighlight.Normal;
			this.label1.Level = Desktop.Skinning.SkinnedLabelLevel.Normal;
			this.label1.Location = new System.Drawing.Point(12, 36);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(106, 13);
			this.label1.TabIndex = 2;
			this.label1.Text = "We Need Your Help!";
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
			this.label2.ForeColor = System.Drawing.Color.Black;
			this.label2.Highlight = Desktop.Skinning.SkinnedHighlight.Normal;
			this.label2.Level = Desktop.Skinning.SkinnedLabelLevel.Normal;
			this.label2.Location = new System.Drawing.Point(9, 59);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(334, 44);
			this.label2.TabIndex = 3;
			this.label2.Text = "A pose uses one or more external images, which the editor can\'t import without kn" +
    "owing where they came from. Please pull your source images into the table below." +
    "";
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
			this.label3.ForeColor = System.Drawing.Color.Black;
			this.label3.Highlight = Desktop.Skinning.SkinnedHighlight.Normal;
			this.label3.Level = Desktop.Skinning.SkinnedLabelLevel.Normal;
			this.label3.Location = new System.Drawing.Point(12, 516);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(331, 45);
			this.label3.TabIndex = 4;
			this.label3.Text = "Note: The editor will copy these images into your character\'s \"attachments\" subfo" +
    "lder, so if you edit your images, you will need to edit the copy to reflect the " +
    "changes in later imports.";
			// 
			// openFileDialog1
			// 
			this.openFileDialog1.FileName = "openFileDialog1";
			// 
			// lblReady
			// 
			this.lblReady.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.lblReady.Font = new System.Drawing.Font("Segoe UI", 28F);
			this.lblReady.ForeColor = System.Drawing.Color.Green;
			this.lblReady.Highlight = Desktop.Skinning.SkinnedHighlight.Good;
			this.lblReady.Level = Desktop.Skinning.SkinnedLabelLevel.Finished;
			this.lblReady.Location = new System.Drawing.Point(12, 38);
			this.lblReady.Name = "lblReady";
			this.lblReady.Size = new System.Drawing.Size(331, 64);
			this.lblReady.TabIndex = 5;
			this.lblReady.Text = "Good to Go!";
			this.lblReady.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.lblReady.Visible = false;
			// 
			// cmdCancel
			// 
			this.cmdCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.cmdCancel.Background = Desktop.Skinning.SkinnedBackgroundType.Surface;
			this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.cmdCancel.FieldType = Desktop.Skinning.SkinnedFieldType.Primary;
			this.cmdCancel.Flat = true;
			this.cmdCancel.ForeColor = System.Drawing.Color.White;
			this.cmdCancel.Location = new System.Drawing.Point(277, 3);
			this.cmdCancel.Name = "cmdCancel";
			this.cmdCancel.Size = new System.Drawing.Size(75, 23);
			this.cmdCancel.TabIndex = 6;
			this.cmdCancel.Text = "Cancel";
			this.cmdCancel.UseVisualStyleBackColor = true;
			this.cmdCancel.Click += new System.EventHandler(this.cmdCancel_Click);
			// 
			// dataGridViewButtonColumn1
			// 
			this.dataGridViewButtonColumn1.HeaderText = "Source";
			this.dataGridViewButtonColumn1.Name = "dataGridViewButtonColumn1";
			// 
			// skinnedPanel1
			// 
			this.skinnedPanel1.Controls.Add(this.cmdCancel);
			this.skinnedPanel1.Controls.Add(this.cmdOK);
			this.skinnedPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.skinnedPanel1.Location = new System.Drawing.Point(0, 569);
			this.skinnedPanel1.Name = "skinnedPanel1";
			this.skinnedPanel1.PanelType = Desktop.Skinning.SkinnedBackgroundType.PrimaryDark;
			this.skinnedPanel1.Size = new System.Drawing.Size(355, 30);
			this.skinnedPanel1.TabIndex = 7;
			// 
			// PropImporter
			// 
			this.AcceptButton = this.cmdOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.cmdCancel;
			this.ClientSize = new System.Drawing.Size(355, 599);
			this.ControlBox = false;
			this.Controls.Add(this.skinnedPanel1);
			this.Controls.Add(this.lblReady);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.gridMissingImages);
			this.Name = "PropImporter";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Import Images";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PropImporter_FormClosing);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.PropImporter_FormClosed);
			((System.ComponentModel.ISupportInitialize)(this.gridMissingImages)).EndInit();
			this.skinnedPanel1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private Desktop.Skinning.SkinnedButton cmdOK;
		private Desktop.Skinning.SkinnedDataGridView gridMissingImages;
		private Desktop.Skinning.SkinnedLabel label1;
		private Desktop.Skinning.SkinnedLabel label2;
		private Desktop.Skinning.SkinnedLabel label3;
		private System.Windows.Forms.OpenFileDialog openFileDialog1;
		private System.Windows.Forms.DataGridViewTextBoxColumn ColImageName;
		private System.Windows.Forms.DataGridViewImageColumn ColImage;
		private System.Windows.Forms.DataGridViewButtonColumn ColAssign;
		private Desktop.Skinning.SkinnedLabel lblReady;
		private Desktop.Skinning.SkinnedButton cmdCancel;
		private System.Windows.Forms.DataGridViewButtonColumn dataGridViewButtonColumn1;
		private Desktop.Skinning.SkinnedPanel skinnedPanel1;
	}
}