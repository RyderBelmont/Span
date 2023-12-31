﻿namespace Desktop.CommonControls.PropertyControls
{
	partial class SliderControl
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
			this.slider = new Desktop.Skinning.SkinnedSlider();
			this.valValue = new Desktop.CommonControls.NumericField();
			((System.ComponentModel.ISupportInitialize)(this.slider)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.valValue)).BeginInit();
			this.SuspendLayout();
			// 
			// slider
			// 
			this.slider.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
			this.slider.FieldType = Desktop.Skinning.SkinnedFieldType.Primary;
			this.slider.Increment = 10;
			this.slider.Location = new System.Drawing.Point(0, 0);
			this.slider.Maximum = 100;
			this.slider.Minimum = 0;
			this.slider.Name = "slider";
			this.slider.Size = new System.Drawing.Size(105, 20);
			this.slider.TabIndex = 0;
			this.slider.TickFrequency = 25;
			this.slider.Value = 0;
			// 
			// valValue
			// 
			this.valValue.Anchor = System.Windows.Forms.AnchorStyles.Right;
			this.valValue.DecimalPlaces = 0;
			this.valValue.Increment = new decimal(new int[] {
            10,
            0,
            0,
            0});
			this.valValue.Location = new System.Drawing.Point(111, 0);
			this.valValue.Maximum = new decimal(new int[] {
            100,
            0,
            0,
            0});
			this.valValue.Minimum = new decimal(new int[] {
            0,
            0,
            0,
            0});
			this.valValue.Name = "valValue";
			this.valValue.PlaceholderText = "";
			this.valValue.Size = new System.Drawing.Size(39, 20);
			this.valValue.TabIndex = 1;
			this.valValue.Value = new decimal(new int[] {
            0,
            0,
            0,
            0});
			// 
			// SliderControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.valValue);
			this.Controls.Add(this.slider);
			this.Name = "SliderControl";
			this.Size = new System.Drawing.Size(150, 20);
			((System.ComponentModel.ISupportInitialize)(this.slider)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.valValue)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private Desktop.Skinning.SkinnedSlider slider;
		private Desktop.CommonControls.NumericField valValue;
	}
}
