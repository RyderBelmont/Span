﻿using Desktop;
using Desktop.Messaging;
using Desktop.Skinning;
using SPNATI_Character_Editor.EpilogueEditor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace SPNATI_Character_Editor.Controls
{
	public partial class CharacterImageBox : UserControl, ISkinControl
	{
		private const int ScreenMargin = 5;
		private const float TextPercent = 0.2f;
		private const int TextMargin = 2;
		private const int TextBorder = 2;
		private const int TextPadding = 10;
		private const float TextBuffer = 0.1f; //90% height of textbox row
		private const string FontName = "Trebuchet MS";
		private const int ArrowSize = 15;
		
		private float _time;
		private Mailbox _mailbox;
		private Image _singleUseImage;
		private Image _imageReference;
		private bool _animating;
		private DialogueLine _line = null;
		private string _text = null;
		private float _percent = 0.5f;
		private List<string> _markers = new List<string>();

		private ISkin _character;

		private PoseMapping _currentPose;
		private int _currentStage;
		private ImageReference _reference;

		private DateTime _lastTick;

		private Font _textFont;
		private Font _italicFont;
		private Pen _textBorder;

		public Matrix SceneTransform;
		public LivePose Pose;
		public bool AutoPlayback = true;

		public CharacterImageBox()
		{
			InitializeComponent();

			if (Shell.Instance != null)
			{
				_mailbox = Shell.Instance.PostOffice.GetMailbox();
				_mailbox.Subscribe(DesktopMessages.ToggleImages, OnToggleImages);
			}

			_textBorder = new Pen(Color.Black, TextBorder);
			UpdateFont();
		}

		public void SetCharacter(ISkin character)
		{
			if (_character != character)
			{
				_currentPose = null;
				if (_character != null && _character is INotifyPropertyChanged)
				{
					((INotifyPropertyChanged)_character).PropertyChanged -= Character_PropertyChanged;
				}
				_character = character;
				if (_character != null && _character is INotifyPropertyChanged)
				{
					((INotifyPropertyChanged)_character).PropertyChanged += Character_PropertyChanged;
				}
			}
		}

		private void Character_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "CurrentSkin")
			{
				PoseMapping pose = _currentPose;
				if (pose != null)
				{
					_currentPose = null;
					SetImage(pose, _currentStage);
				}
			}
		}

		public bool ShowTextBox
		{
			get
			{
				return Config.GetBoolean(Settings.ShowPreviewText);
			}
		}

		private void OnToggleImages()
		{
			UpdateSceneTransform();
			canvas.Invalidate();
		}

		private void UpdateFont()
		{
			_textFont?.Dispose();
			_italicFont?.Dispose();

			int screenWidth = (int)(canvas.Height * 1.33f);

			const float BaseSize = 12;
			float size = BaseSize * (screenWidth / 1000f);
			_textFont = new Font("Trebuchet MS", size == 0 ? BaseSize : size);
			_italicFont = new Font(_textFont, FontStyle.Italic);
			UpdateRichText();
		}

		private void UpdateSceneTransform()
		{
			SceneTransform = new Matrix();
			int screenHeight = canvas.Height - ScreenMargin * 2;
			int availableHeight = ShowTextBox ? (int)(screenHeight * (1 - TextPercent)) : (int)(screenHeight * 0.9f);
			float screenScale = availableHeight / (Pose == null ? 1400.0f : Pose.BaseHeight);
			SceneTransform.Scale(screenScale, screenScale, MatrixOrder.Append); // scale to display
			SceneTransform.Translate(canvas.Width * 0.5f, screenHeight - availableHeight, MatrixOrder.Append); // center horizontally
		}

		public void Destroy()
		{
			if (_imageReference != null && _animating)
			{
				ImageAnimator.StopAnimate(_imageReference, OnFrameChanged);
			}

			if (_reference != null)
			{
				ImageCache.Release(_reference.FileName);
				_reference = null;
			}

			if (_singleUseImage != null)
			{
				_singleUseImage.Dispose();
				_singleUseImage = null;
			}
			Pose = null;
		}

		public void SetText(DialogueLine line)
		{
			_line = line;
			if (line == null || line.Text == null || string.IsNullOrEmpty(line.Text))
			{
				_text = null;
				txtPreview.Text = "";
				txtPreview.Visible = false;
			}
			else
			{
				txtPreview.Visible = true;
				_text = line.Text;
				UpdateRichText();
				_percent = 0.5f;
				if (!string.IsNullOrEmpty(line.Location) && line.Location.EndsWith("%"))
				{
					int percent;
					if (int.TryParse(line.Location.Substring(0, line.Location.Length - 1), out percent))
					{
						_percent = percent / 100.0f;
					}
				}
			}
			canvas.Invalidate();
		}

		private void UpdateRichText()
		{
			string previewText = _text ?? "";
			if (!Config.GetBoolean(Settings.DisablePreviewFormatting))
			{
				previewText = previewText.Replace("<i>", "\\i ")
					.Replace("</i>", "\\i0 ")
					.Replace("<b>", "\\b ")
					.Replace("</b>", "\\b0 ")
					.Replace("<br>", "\r\n")
					.Replace("<br/>", "\r\n");
			}
			int fontSize = (int)(_textFont.SizeInPoints * 2);
			Color foreColor = SkinManager.Instance.CurrentSkin.Surface.ForeColor;
			string colortable = @"{\colortbl ;\red" + foreColor.R + @"\green" + foreColor.G + @"\blue" + foreColor.B + ";}";
			string rtf = @"{\rtf1\ansi\ansicpg1252\deff0\nouicompat\deflang1033{\fonttbl{\f0\fnil\fcharset0 Trebuchet MS;}}" + colortable + 
				@"\viewkind4\uc1\pard\sl220\slmult1\qc\cf1\f0\fs" + fontSize + @"\lang9 " +
				previewText + @"}";
			txtPreview.Rtf = rtf;
			txtPreview.ForeColor = SkinManager.Instance.CurrentSkin.Surface.ForeColor;
		}

			public void SetMarkers(List<string> markers)
		{
			_markers = markers;
		}

		public void SetImage(Image image, bool disposeImage = true)
		{
			Destroy();
			tmrTick.Stop();
			_currentPose = null;
			_currentStage = -1;
			if (disposeImage)
			{
				_singleUseImage = image;
			}
			_imageReference = image;
			canvas.Invalidate();
		}

		public void SetImage(PoseMapping pose, int stage)
		{
			if (_currentPose == pose && _currentStage == stage && _imageReference != null)
			{
				return;
			}
			Destroy();

			UpdateSceneTransform();
			tmrTick.Stop();
			_currentPose = pose;
			_currentStage = stage;
			if (pose != null)
			{
				PoseReference poseRef = pose.GetPose(stage);
				if (poseRef != null)
				{
					if (poseRef.Pose == null)
					{
						string file = Path.Combine(_character.Skin.GetDirectory(), poseRef.FileName);
						if (!File.Exists(file))
						{
							file = Path.Combine(_character.GetDirectory(), poseRef.FileName);
						}
						_reference = ImageCache.Get(file);
						_imageReference = _reference?.Image;
						if (ImageAnimator.CanAnimate(_imageReference))
						{
							_animating = true;
							ImageAnimator.Animate(_imageReference, OnFrameChanged);
						}
					}
					else
					{
						Pose p = _character.Skin.CustomPoses.Find(cp => cp.Id == poseRef.Pose.Id);
						if (p == null)
						{
							p = poseRef.Pose;
						}
						Pose = new LivePose(_character, p, _currentStage);
						if (AutoPlayback)
						{
							_time = 0;
							_lastTick = DateTime.Now;
							tmrTick.Enabled = true;
						}
					}
				}
				else
				{
					_imageReference = null;
				}
			}
			else
			{
				_imageReference = null;
			}
			canvas.Invalidate();
		}

		private void OnFrameChanged(object sender, EventArgs e)
		{
			canvas.Invalidate();
		}

		private void canvas_Paint(object sender, PaintEventArgs e)
		{
			if (Config.GetBoolean(Settings.HideImages))
				return;

			Graphics g = e.Graphics;

			int screenHeight = canvas.Height - ScreenMargin * 2;
			if (_line == null || _line.Layer != "over")
			{
				DrawSpeechBubble(g, screenHeight);
			}

			if (Pose != null)
			{
				foreach (LiveSprite sprite in Pose.DrawingOrder)
				{
					sprite.Draw(g, SceneTransform, _markers, true);
				}
			}
			else if (_imageReference != null)
			{
				ImageAnimator.UpdateFrames();

				//scale to the height
				float availableHeight = ShowTextBox ? screenHeight * (1 - TextPercent) : screenHeight * 0.9f;
				int width = (int)(_imageReference.Width / (float)_imageReference.Height * availableHeight);
				g.DrawImage(_imageReference, canvas.Width / 2 - width / 2, screenHeight - availableHeight + ScreenMargin, width, availableHeight);
			}

			if (_line != null && _line.Layer == "over")
			{
				DrawSpeechBubble(g, screenHeight);
			}
		}

		private void DrawSpeechBubble(Graphics g, int screenHeight)
		{
			bool showText = Config.GetBoolean(Settings.ShowPreviewText);
			if (showText && !string.IsNullOrEmpty(_text))
			{
				bool formatText = !Config.GetBoolean(Settings.DisablePreviewFormatting);
				int textboxHeight = (int)(screenHeight * TextPercent);
				int topPadding = (int)(textboxHeight * TextBuffer);
				textboxHeight -= topPadding;

				int boxWidth = canvas.Width;
				int maxWidth = (int)(0.245f * 1.3333333f * screenHeight);
				boxWidth = Math.Min(boxWidth, maxWidth);

				RectangleF bounds = new RectangleF(TextMargin + TextBorder + TextPadding + canvas.Width / 2 - boxWidth / 2,
								topPadding + TextBorder + TextPadding,
								boxWidth - TextMargin * 2 - TextPadding * 2,
								textboxHeight - TextBorder * 2 - TextPadding * 2);

				StringFormat sf = new StringFormat() { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
				SizeF size = g.MeasureString(_text, _textFont, (int)bounds.Width, sf);
				if (txtPreview.Height > bounds.Height)
				{
					textboxHeight += (int)(txtPreview.Height - bounds.Height);
				}
				bounds.Height = Math.Max(txtPreview.Height, bounds.Height);
				txtPreview.Top = (int)(bounds.Top + bounds.Height / 2 - txtPreview.Height / 2) + 2;

				const int TopOffset = 4;
				using (SolidBrush br = new SolidBrush(SkinManager.Instance.CurrentSkin.FieldBackColor))
				{
					g.FillRectangle(br, bounds.Left - TextMargin - TextPadding, topPadding + TopOffset, bounds.Width + TextMargin * 2 + TextPadding * 2, textboxHeight - TopOffset);
					g.DrawRectangle(_textBorder, bounds.Left - TextMargin - TextPadding, topPadding + TopOffset, bounds.Width + TextMargin * 2 + TextPadding * 2, textboxHeight - TopOffset);
					if (_line.Direction != "none")
					{
						Point[] triangle = new Point[] {
						new Point((int)(canvas.Width * _percent) - ArrowSize, topPadding  + textboxHeight - 1),
						new Point((int)(canvas.Width * _percent) + ArrowSize, topPadding  + textboxHeight - 1),
						new Point((int)(canvas.Width * _percent), topPadding + textboxHeight + ArrowSize - 1),
					};
						g.FillPolygon(br, triangle);
						g.DrawLine(_textBorder, triangle[0], triangle[2]);
						g.DrawLine(_textBorder, triangle[1], triangle[2]);
					}
				}

				sf.Dispose();
			}
		}

		/// <summary>
		/// Converts what's currently visible into an image
		/// </summary>
		/// <returns></returns>
		public Bitmap GetImage()
		{
			Bitmap bmp = new Bitmap(canvas.Width, canvas.Height);
			canvas.DrawToBitmap(bmp, new Rectangle(0, 0, bmp.Width, bmp.Height));
			return bmp;
		}

		public void SetTime(float time)
		{
			_time = time;
			Pose.UpdateTime(_time, _time, true);
			canvas.Invalidate();
			canvas.Update();
		}

		private void tmrTick_Tick(object sender, EventArgs e)
		{
			DateTime now = DateTime.Now;
			TimeSpan elapsed = now - _lastTick;
			float elapsedSec = (float)elapsed.TotalSeconds;
			_lastTick = now;
			_time += elapsedSec;

			if (Pose == null)
			{
				tmrTick.Enabled = false;
				return;
			}

			Pose.UpdateTime(_time, _time, true);
			canvas.Invalidate();
		}

		private void CharacterImageBox_Resize(object sender, EventArgs e)
		{
			int screenHeight = canvas.Height - ScreenMargin * 2;
			int boxWidth = canvas.Width;
			int maxWidth = (int)(0.245f * 1.3333333f * screenHeight);
			boxWidth = Math.Min(boxWidth, maxWidth);
			boxWidth -= TextBorder * 2 + TextPadding * 2;
			txtPreview.Width = boxWidth;
			txtPreview.Left = canvas.Width / 2 - txtPreview.Width / 2;
			UpdateFont();
			UpdateSceneTransform();
		}

		public void OnUpdateSkin(Skin skin)
		{
			canvas.BackColor = skin.Background.Normal;
			txtPreview.BackColor = skin.FieldBackColor;
			txtPreview.ForeColor = skin.Surface.ForeColor;
		}

		private void txtPreview_ContentsResized(object sender, ContentsResizedEventArgs e)
		{
			((RichTextBox)sender).Height = e.NewRectangle.Height + 5;
		}
	}
}
