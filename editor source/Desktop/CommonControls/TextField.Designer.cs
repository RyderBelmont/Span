﻿namespace Desktop.CommonControls
{
	partial class TextField
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
			this.txtField = new System.Windows.Forms.TextBox();
			this.lblPlaceholder = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// valField
			// 
			this.txtField.Dock = System.Windows.Forms.DockStyle.Fill;
			this.txtField.Location = new System.Drawing.Point(0, 0);
			this.txtField.Name = "valField";
			this.txtField.Size = new System.Drawing.Size(150, 20);
			this.txtField.TabIndex = 0;
			this.txtField.Enter += new System.EventHandler(this.valField_Enter);
			this.txtField.Leave += new System.EventHandler(this.valField_Leave);
			// 
			// lblPlaceholder
			// 
			this.lblPlaceholder.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
			| System.Windows.Forms.AnchorStyles.Left)
			| System.Windows.Forms.AnchorStyles.Right)));
			this.lblPlaceholder.BackColor = System.Drawing.Color.White;
			this.lblPlaceholder.Cursor = System.Windows.Forms.Cursors.IBeam;
			this.lblPlaceholder.ForeColor = System.Drawing.SystemColors.ControlDark;
			this.lblPlaceholder.Location = new System.Drawing.Point(1, 2);
			this.lblPlaceholder.Margin = new System.Windows.Forms.Padding(0);
			this.lblPlaceholder.Name = "lblPlaceholder";
			this.lblPlaceholder.Size = new System.Drawing.Size(132, 16);
			this.lblPlaceholder.TabIndex = 1;
			this.lblPlaceholder.Click += new System.EventHandler(this.lblPlaceholder_Click);
			// 
			// TextField
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.lblPlaceholder);
			this.Controls.Add(this.txtField);
			this.Name = "TextField";
			this.Size = new System.Drawing.Size(150, 20);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TextBox txtField;
		private System.Windows.Forms.Label lblPlaceholder;
	}
}
