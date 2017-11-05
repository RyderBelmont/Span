﻿using System.Collections.Generic;
using System.Drawing;

namespace KisekaeImporter.ImageImport
{
	/// <summary>
	/// List of pose data
	/// </summary>
	public class PoseList
	{
		/// <summary>
		/// Cropping information
		/// </summary>
		public Rect Crop = new Rect(0, 0, 600, 1400);

		/// <summary>
		/// List of poses
		/// </summary>
		public List<ImageMetadata> Poses = new List<ImageMetadata>();
	}

	public struct Rect
	{
		public int Top;
		public int Right;
		public int Bottom;
		public int Left;

		public Rect(int l, int t, int r, int b)
		{
			Top = t;
			Right = r;
			Bottom = b;
			Left = l;
		}

		public RectangleF ToRectangle(float zoom)
		{
			float l = Left * zoom;
			float t = Top * zoom;
			float r = Right * zoom;
			float b = Bottom * zoom;
			return new RectangleF(l, t, (r - l), (b - t));
		}

		public override string ToString()
		{
			return string.Format("{0},{1},{2},{3}", Left, Top, Right - Left, Bottom - Top);
		}

		public static bool operator ==(Rect a, Rect b)
		{
			return a.Left == b.Left && a.Top == b.Top && a.Right == b.Right && a.Bottom == b.Bottom;
		}

		public static bool operator !=(Rect a, Rect b)
		{
			return !(a == b);
		}

		public override bool Equals(object obj)
		{
			if (obj is Rect)
				return this == (Rect)obj;
			return base.Equals(obj);
		}

		public override int GetHashCode()
		{
			return base.GetHashCode();
		}
	}

	public static class RectExtensions
	{
		public static Rect ToRect(this RectangleF rect, float zoom)
		{
			float l = rect.Left / zoom;
			float r = rect.Right / zoom;
			float t = rect.Top / zoom;
			float b = rect.Bottom / zoom;
			return new Rect((int)l, (int)t, (int)r, (int)b);
		}
	}
}
