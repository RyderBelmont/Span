﻿using System;
using System.Windows.Forms;

namespace Desktop.CommonControls
{
	public class SelectablePanel : DBPanel
	{
		public SelectablePanel()
		{
			this.SetStyle(ControlStyles.Selectable, true);
			this.TabStop = true;
		}
		protected override void OnMouseDown(MouseEventArgs e)
		{
			this.Focus();
			base.OnMouseDown(e);
		}
		protected override bool IsInputKey(Keys keyData)
		{
			return keyData == Keys.Up || keyData == Keys.Down
				? true
				: keyData == Keys.Left || keyData == Keys.Right ? true : base.IsInputKey(keyData);
		}
		protected override void OnEnter(EventArgs e)
		{
			this.Invalidate();
			base.OnEnter(e);
		}
		protected override void OnLeave(EventArgs e)
		{
			this.Invalidate();
			base.OnLeave(e);
		}
		protected override void OnPaint(PaintEventArgs pe)
		{
			base.OnPaint(pe);
			if (this.Focused)
			{
				var rc = this.ClientRectangle;
				rc.Inflate(-2, -2);
				ControlPaint.DrawFocusRectangle(pe.Graphics, rc);
			}
		}
	}
}
