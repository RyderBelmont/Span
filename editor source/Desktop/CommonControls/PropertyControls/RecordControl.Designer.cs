﻿namespace Desktop.CommonControls.PropertyControls
{
	partial class RecordControl
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
			this.recField = new Desktop.CommonControls.RecordField();
			this.SuspendLayout();
			// 
			// recField
			// 
			this.recField.Dock = System.Windows.Forms.DockStyle.Fill;
			this.recField.Location = new System.Drawing.Point(0, 0);
			this.recField.Name = "recField";
			this.recField.PlaceholderText = null;
			this.recField.Record = null;
			this.recField.RecordKey = null;
			this.recField.Size = new System.Drawing.Size(150, 20);
			this.recField.TabIndex = 0;
			this.recField.RecordChanged += new System.EventHandler<RecordEventArgs>(this.recField_RecordChanged);
			// 
			// RecordControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.recField);
			this.Name = "RecordControl";
			this.Size = new System.Drawing.Size(150, 20);
			this.ResumeLayout(false);

		}

		#endregion

		private Desktop.CommonControls.RecordField recField;
	}
}
