﻿namespace SPNATI_Character_Editor.Activities
{
	partial class Loader
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
			this.progressBar = new Desktop.Skinning.SkinnedProgressBar();
			this.lblProgress = new Desktop.Skinning.SkinnedLabel();
			this.SuspendLayout();
			// 
			// progressBar
			// 
			this.progressBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.progressBar.Location = new System.Drawing.Point(289, 273);
			this.progressBar.Name = "progressBar";
			this.progressBar.Size = new System.Drawing.Size(276, 23);
			this.progressBar.TabIndex = 0;
			// 
			// lblProgress
			// 
			this.lblProgress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.lblProgress.Font = new System.Drawing.Font("Microsoft Sans Serif", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.lblProgress.Level = Desktop.Skinning.SkinnedLabelLevel.Heading;
			this.lblProgress.Location = new System.Drawing.Point(3, 220);
			this.lblProgress.Name = "lblProgress";
			this.lblProgress.Size = new System.Drawing.Size(856, 50);
			this.lblProgress.TabIndex = 1;
			this.lblProgress.Text = "Loading...";
			this.lblProgress.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// Loader
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.lblProgress);
			this.Controls.Add(this.progressBar);
			this.Name = "Loader";
			this.Size = new System.Drawing.Size(862, 564);
			this.ResumeLayout(false);

		}

		#endregion

		private Desktop.Skinning.SkinnedProgressBar progressBar;
		private Desktop.Skinning.SkinnedLabel lblProgress;
	}
}
