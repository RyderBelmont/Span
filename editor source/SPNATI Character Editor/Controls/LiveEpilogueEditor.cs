﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Desktop;
using Desktop.CommonControls;
using SPNATI_Character_Editor.Actions;
using SPNATI_Character_Editor.EpilogueEditing;
using SPNATI_Character_Editor.EpilogueEditor;

namespace SPNATI_Character_Editor.Controls
{
	public partial class LiveEpilogueEditor : UserControl
	{
		private Character _character;
		private Epilogue _epilogue;
		private LiveScene _scene;
		private Scene _sourceScene;
		private SceneTransition _sceneTransition;
		private UndoManager _history = new UndoManager();
		private float _time;
		private float _playbackTime;
		private float _elapsedTime;
		private bool _playing;
		private ILabel _labelData;
		private ILabel _sublabelData;
		private DateTime _lastTick;
		private bool _cameraLocked;
		private float _savedTime;

		public LiveEpilogueEditor()
		{
			InitializeComponent();
			_history.CommandApplied += _history_CommandApplied;
			timeline.CommandHistory = _history;
			timeline.DataSelected += Timeline_DataSelected;
			timeline.TimeChanged += Timeline_TimeChanged;
			timeline.PlaybackTimeChanged += Timeline_PlaybackTimeChanged;
			timeline.ElapsedTimeChanged += Timeline_ElapsedTimeChanged;
			timeline.WidgetSelected += Timeline_WidgetSelected;
			timeline.PlaybackChanged += Timeline_PlaybackChanged;
			timeline.UIRequested += Timeline_UIRequested;

			canvas.UndoManager = _history;
			canvas.ObjectSelected += Canvas_ObjectSelected;
			canvas.AddToolBarButton(Properties.Resources.VideoCamera, "Toggle scene preview", true, ToggleCamera);
			canvas.AddToolBarButton(Properties.Resources.Play, "Play scene", true, TogglePlay);
			canvas.CanvasClicked += Canvas_CanvasClicked;
			canvas.CustomPaint += Canvas_CustomPaint;

			table.RecordFilter = RecordFilter;
			table.PropertyChanged += Table_PropertyChanged;
			subTable.RecordFilter = SubRecordFilter;
		}

		private bool RecordFilter(PropertyRecord record)
		{
			if (table.Data is LiveKeyframe)
			{
				return FilterKeyframeProperty(table, record);
			}
			else if (table.Data is LiveObject)
			{
				return FilterObjectProperty(table, record);
			}
			else if (table.Data is Scene && _sceneTransition != null)
			{
				return record.Key == "effect" || record.Key == "ease" || record.Key == "time" || record.Key == "color";
			}
			return true;
		}

		private bool SubRecordFilter(PropertyRecord record)
		{
			if (subTable.Data is LiveKeyframe)
			{
				return FilterKeyframeProperty(subTable, record);
			}
			else if (subTable.Data is LiveObject)
			{
				return FilterObjectProperty(subTable, record);
			}
			else if (subTable.Data is Scene && _sceneTransition != null)
			{
				return record.Key == "effect" || record.Key == "ease" || record.Key == "time" || record.Key == "color";
			}
			return true;
		}

		private bool FilterKeyframeProperty(PropertyTable dataTable, PropertyRecord record)
		{
			LiveKeyframe keyframe = dataTable.Data as LiveKeyframe;
			return keyframe.FilterRecord(record);
		}

		private bool FilterObjectProperty(PropertyTable dataTable, PropertyRecord record)
		{
			LiveObject obj = dataTable.Data as LiveObject;
			return obj.FilterRecord(record.Key);
		}

		private void Canvas_CanvasClicked(object sender, System.EventArgs e)
		{
			timeline.ResumePlayback();
		}

		private void ToggleCamera(ToolStripButton btn)
		{
			_cameraLocked = btn.Checked;
			ToggleCamera(_cameraLocked);
		}

		private void ToggleCamera(bool locked)
		{
			_scene.LockToCamera = locked;
			canvas.AllowZoom = !locked;
			canvas.FitScreen();
			canvas.InvalidateCanvas();
		}

		private void TogglePlay(ToolStripButton button)
		{
			bool enabled = button.Checked;
			if (enabled)
			{
				_savedTime = _time;
				canvas.DisallowEdit = true;
				canvas.LockToolbar(true, button);
				button.Image = Properties.Resources.Stop;
				splitContainer1.Panel1Collapsed = true;
				splitContainer2.Panel1Collapsed = true;
				ToggleCamera(true);
				timeline.PauseOnBreaks = true;
				timeline.CurrentTime = 0;
				timeline.EnablePlayback(true);
			}
			else
			{
				button.Image = Properties.Resources.Play;
				canvas.DisallowEdit = false;
				canvas.LockToolbar(false, button);
				splitContainer1.Panel1Collapsed = false;
				splitContainer2.Panel1Collapsed = false;
				ToggleCamera(_cameraLocked);
				timeline.PauseOnBreaks = false;
				timeline.EnablePlayback(false);
				timeline.CurrentTime = _savedTime;
			}
		}

		private void _history_CommandApplied(object sender, CommandEventArgs e)
		{
			UpdateToolbar();
		}

		private void Timeline_PlaybackChanged(object sender, bool enabled)
		{
			_playing = enabled;
			canvas.SetPlayback(enabled);
			if (!_playing)
			{
				UpdateToolbar();
			}
		}

		public void SetActive(bool active)
		{
			_lastTick = DateTime.Now;
			tmrRealtime.Enabled = active;
		}

		private void Canvas_ObjectSelected(object sender, CanvasSelectionArgs args)
		{
			timeline.SelectWidgetWithData(args.Object);
		}

		private void Timeline_WidgetSelected(object sender, ITimelineObject widget)
		{
			canvas.SelectData(widget?.GetData());
		}

		private void Timeline_PlaybackTimeChanged(object sender, float time)
		{
			_playbackTime = time;
			canvas.UpdateTime(_time, _playbackTime, _elapsedTime);
		}

		private void Timeline_ElapsedTimeChanged(object sender, float time)
		{
			_elapsedTime = time;
			canvas.UpdateTime(_time, _playbackTime, _elapsedTime);
		}

		private void Timeline_TimeChanged(object sender, float time)
		{
			_time = time;
			canvas.UpdateTime(time, _playbackTime, _elapsedTime);
			UpdateToolbar();
		}

		public void SetEpilogue(Character character, Epilogue epilogue)
		{
			_character = character;
			SaveScene();
			_scene = null;
			_sourceScene = null;
			_epilogue = epilogue;
			lstScenes.Items.Clear();
			if (epilogue != null)
			{
				foreach (Scene scene in epilogue.Scenes)
				{
					lstScenes.Items.Add(scene);
				}
			}
			if (lstScenes.Items.Count > 0)
			{
				lstScenes.SelectedIndex = 0;
			}
		}

		public void Destroy()
		{
			LiveImageCache.Clear();
		}

		private void Timeline_DataSelected(object sender, DataSelectionArgs data)
		{
			if (data.Data == null && data.PreviewData == null)
			{
				SetTableData(null, null);
				SetSubTableData(null, null);
			}
			if (data.Data is LiveKeyframe || data.PreviewData is LiveKeyframe)
			{
				//when selecting a keyframe, also select the object
				LiveKeyframe kf = data.Data as LiveKeyframe ?? data.PreviewData as LiveKeyframe;
				SetTableData(kf.Data, null);
				SetSubTableData(data.Data, data.PreviewData);
			}
			else
			{
				//when selecting something else, clear the sub-table if it doesn't belong to that object, or open the current frame
				LiveKeyframe kf = subTable.Data as LiveKeyframe;
				if (kf != null && kf.Data != data.Data)
				{
					SetSubTableData(null, null);
				}
			}
			UpdateToolbar();
		}

		private void SetTableData(object data, object previewData)
		{
			if (_labelData != null)
			{
				_labelData.LabelChanged -= _labelData_LabelChanged;
				_labelData = null;
			}
			table.SetDataAsync(data, previewData);
			_labelData = data as ILabel;
			if (_labelData != null)
			{
				_labelData.LabelChanged += _labelData_LabelChanged;
				lblDataCaption.Text = _labelData.GetLabel();
			}
			else
			{
				lblDataCaption.Text = data?.ToString();
			}
		}
		private void SetSubTableData(object data, object previewData)
		{
			if (_sublabelData != null)
			{
				_sublabelData.LabelChanged -= _labelData_LabelChanged;
				_sublabelData = null;
			}
			subTable.SetDataAsync(data, previewData);
			_sublabelData = data as ILabel;
			if (_sublabelData != null)
			{
				_sublabelData.LabelChanged += _labelData_LabelChanged;
				lblSubTable.Text = _sublabelData.GetLabel();
			}
			else
			{
				lblSubTable.Text = data?.ToString();
			}
		}

		private void _labelData_LabelChanged(object sender, System.EventArgs e)
		{
			if (_labelData != null)
			{
				lblDataCaption.Text = _labelData.GetLabel();
				if (_labelData == _scene)
				{
					_sourceScene.Name = _scene.Name;
					lstScenes.RefreshListItems();
				}
			}
			if (_sublabelData != null)
			{
				lblSubTable.Text = _sublabelData.GetLabel();
			}
		}

		private void SaveScene()
		{
			if (_scene == null) { return; }
		}

		private void lstScenes_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			Scene scene = lstScenes.SelectedItem as Scene;
			SetScene(scene);
		}

		private void SetScene(Scene newScene)
		{
			if (_sourceScene == newScene)
			{
				if (_sourceScene.Transition) { return; }
				SetTableData(_scene, null);
				return;
			}
			SaveScene();
			_sceneTransition = null;
			if (_scene != null)
			{
				_scene.PropertyChanged -= _scene_PropertyChanged;
			}
			_sourceScene = newScene;
			_scene = null;
			if (_sourceScene != null)
			{
				if (_sourceScene.Transition)
				{
					_sceneTransition = new SceneTransition(_sourceScene, canvas.CanvasWidth, canvas.CanvasHeight);
					_scene = null;
					canvas.CustomDraw = true;
				}
				else
				{
					canvas.CustomDraw = false;
					_scene = new LiveScene(_sourceScene, _character);
					_scene.PropertyChanged += _scene_PropertyChanged;
				}
			}
			bool enabled = _scene != null;
			EpilogueContext context = new EpilogueContext(_character, _epilogue, _sourceScene);
			context.Context = CharacterContext.Pose;
			table.Context = context;
			subTable.Context = context;
			canvas.SetData(_character, _scene);
			timeline.SetData(_scene);
			object data = _scene;
			if (_sceneTransition != null)
			{
				data = _sourceScene;
			}
			SetTableData(data, null);
			tsToolbar.Enabled = enabled;
			canvas.Enabled = enabled;
			table.Enabled = data != null;
			subTable.Enabled = data != null;
			timeline.Enabled = enabled;
			canvas.FitScreen();
		}

		private void _scene_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (e.PropertyName == "Name")
			{
				_sourceScene.Name = _scene.Name;
				lstScenes.RefreshListItems();
			}
		}

		private void UpdateToolbar()
		{
			if (_playing) { return; }
			KeyframedWidget selectedWidget = timeline.SelectedObject as KeyframedWidget;

			tsRemoveSprite.Enabled = tsAddEndFrame.Enabled = (selectedWidget != null);
			tsAddKeyframe.Enabled = false;
			tsRemoveKeyframe.Enabled = false;
			tsFrameType.Enabled = false;
			if (selectedWidget != null)
			{
				tsRemoveSprite.Enabled = true;
				LiveKeyframe kf = selectedWidget.Data.Keyframes.Find(k => k.Time == _time);
				if (kf == null)
				{
					float start = selectedWidget.GetStart();
					if (_time > start)
					{
						tsAddKeyframe.Enabled = true;
					}
				}
				if (selectedWidget.SelectedFrame != null && selectedWidget.SelectedFrame.Time != 0)
				{
					tsRemoveKeyframe.Enabled = true;
					tsFrameType.Enabled = true;
				}
			}
		}

		private void addSpriteToolStripMenuItem_Click(object sender, System.EventArgs e)
		{
			openFileDialog1.UseAbsolutePaths = true;
			if (openFileDialog1.ShowDialog(_character, "") == DialogResult.OK)
			{
				string src = openFileDialog1.FileName;
				LiveSprite sprite = _scene.AddSprite(_time, src);
				sprite.AddValue<string>(0, "Src", src);

				string id = Path.GetFileNameWithoutExtension(src);
				int hyphen = id.IndexOf('-');
				if (hyphen == 1 || hyphen == 2)
				{
					id = id.Substring(hyphen + 1);
				}
				sprite.Id = _scene.GetUniqueId(id);
				timeline.SelectObject(timeline.CreateWidget(sprite));
			}
		}

		private void addSpeechBubbleToolStripMenuItem_Click(object sender, System.EventArgs e)
		{
			LiveBubble bubble = _scene.AddBubble(_time);
			timeline.SelectObject(timeline.CreateWidget(bubble));
		}

		private void addWaitForInputToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (_time == 0) { return; }
			ITimelineBreak brk = _scene.AddBreak(_time);
			if (brk != null)
			{
				timeline.AddBreak(brk);
				timeline.CurrentTime = brk.Time;
			}
		}

		private void addEmitterToolStripMenuItem_Click(object sender, System.EventArgs e)
		{
			LiveEmitter emitter = _scene.AddEmitter(_time);
			timeline.SelectObject(timeline.CreateWidget(emitter));
		}

		private void tmrRealtime_Tick(object sender, System.EventArgs e)
		{
			DateTime now = DateTime.Now;
			float elapsedSec = (float)(now - _lastTick).TotalSeconds;
			float elapsed = elapsedSec * 1000;
			if (_sceneTransition != null)
			{
				_sceneTransition.Update(elapsedSec);
				canvas.InvalidateCanvas();
			}
			else
			{
				_scene?.UpdateRealTime(elapsed, canvas.Playing);
			}
			_lastTick = now;
		}

		private void Timeline_UIRequested(object sender, object data)
		{
			LiveSpriteKeyframe frame = data as LiveSpriteKeyframe;
			if (frame != null)
			{
				openFileDialog1.UseAbsolutePaths = true;
				if (openFileDialog1.ShowDialog(_character, frame.Src ?? "") == DialogResult.OK)
				{
					string src = openFileDialog1.FileName;
					frame.Src = src;
				}
			}
		}

		private void tsRefresh_Click(object sender, EventArgs e)
		{
			LiveImageCache.Refresh();
			foreach (LiveObject obj in _scene.Tracks)
			{
				if (obj is LiveSprite)
				{
					LiveSprite sprite = obj as LiveSprite;
					if (!string.IsNullOrEmpty(sprite.Src))
					{
						sprite.Image = LiveImageCache.Get(sprite.Src);
						if (sprite.Image != null)
						{
							sprite.Width = sprite.Image.Width;
							sprite.Height = sprite.Image.Height;
						}
						else
						{
							sprite.Width = 100;
							sprite.Height = 100;
						}
					}
				}
			}
			canvas.InvalidateCanvas();
		}

		private void tsAddKeyframe_Click(object sender, EventArgs e)
		{
			if (timeline.SelectedObject == null) { return; }
			KeyframedWidget widget = timeline.SelectedObject as KeyframedWidget;
			//TODO: Make this a command
			LiveKeyframe frame = widget.Data.AddKeyframe(_time - widget.GetStart());
			widget.SelectKeyframe(frame, null, false);
			UpdateToolbar();
		}

		private void tsRemoveKeyframe_Click(object sender, EventArgs e)
		{
			if (timeline.SelectedObject == null) { return; }
			KeyframedWidget widget = timeline.SelectedObject as KeyframedWidget;
			LiveKeyframe frame = widget.SelectedFrame;
			if (frame != null)
			{
				DeleteKeyframeCommand command = new DeleteKeyframeCommand(widget.Data, frame);
				_history.Commit(command);
			}
			UpdateToolbar();
		}

		private void tsAddEndFrame_Click(object sender, EventArgs e)
		{
			if (timeline.SelectedObject == null) { return; }
			KeyframedWidget widget = timeline.SelectedObject as KeyframedWidget;
			LiveAnimatedObject sprite = widget.Data;
			if (sprite.Keyframes.Count > 0)
			{
				LiveKeyframe kf = widget.Data.Keyframes[0];
				kf = sprite.CopyKeyframe(kf, new HashSet<string>());
				PasteKeyframeCommand command = new PasteKeyframeCommand(sprite, kf, widget.GetEnd(timeline.Duration));
				_history.Commit(command);
				timeline.CurrentTime = command.NewKeyframe.Time;
			}
			UpdateToolbar();
		}

		private void tsFrameType_Click(object sender, EventArgs e)
		{
			if (timeline.SelectedObject == null) { return; }
			KeyframedWidget widget = timeline.SelectedObject as KeyframedWidget;
			LiveKeyframe frame = widget.SelectedFrame;
			if (frame != null)
			{
				HashSet<string> props = new HashSet<string>();
				foreach (string p in widget.SelectedProperties)
				{
					props.Add(p);
				}
				ToggleKeyframeTypeCommand command = new ToggleKeyframeTypeCommand(widget.Data, frame, props);
				_history.Commit(command);
			}
			UpdateToolbar();
		}

		private void tsRemove_Click(object sender, EventArgs e)
		{
			if (timeline.SelectedObject == null) { return; }
			if (MessageBox.Show($"Are you sure you want to remove {timeline.SelectedObject.ToString()}?", "Remove Object", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
			{
				timeline.RemoveSelectedWidget();
				UpdateToolbar();
			}
		}

		private void Canvas_CustomPaint(object sender, CanvasPaintArgs e)
		{
			Graphics g = e.Graphics;
			_sceneTransition.Draw(g, e.Width, e.Height);
		}

		private void Table_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (_sceneTransition == null)
			{
				return;
			}

			_sceneTransition = new SceneTransition(_sourceScene, canvas.CanvasWidth, canvas.CanvasHeight);
		}

		private void tsAddSprite_Click(object sender, EventArgs e)
		{
			AddScene();
		}

		private void tsAddTransition_Click(object sender, EventArgs e)
		{
			AddSceneTransition();
		}

		private void AddScene()
		{
			Scene scene = _sourceScene;

			Scene newScene = new Scene(100, 100);
			if (scene != null)
			{
				int index = _epilogue.Scenes.IndexOf(scene);
				lstScenes.Items.Insert(index + 1, newScene);
				_epilogue.Scenes.Insert(index + 1, newScene);
			}
			else
			{
				_epilogue.Scenes.Add(newScene);
				lstScenes.Items.Add(newScene);
			}
			lstScenes.SelectedItem = newScene;
		}

		private void AddSceneTransition()
		{
			Scene scene = _sourceScene;

			Scene transition = new Scene(true)
			{
				Transition = true
			};

			if (scene != null)
			{
				int index = _epilogue.Scenes.IndexOf(scene);
				lstScenes.Items.Insert(index + 1, transition);
				_epilogue.Scenes.Insert(index + 1, transition);
			}
			else
			{
				_epilogue.Scenes.Add(transition);
				lstScenes.Items.Add(transition);
			}
			lstScenes.SelectedItem = transition;
		}

		private void tsRemoveSprite_Click(object sender, EventArgs e)
		{
			Scene scene = _sourceScene;
			if (scene != null)
			{
				if (MessageBox.Show("Are you sure you want to permanently remove this scene?", "Remove Scene", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
				{
					int index = lstScenes.SelectedIndex;
					_epilogue.Scenes.Remove(scene);
					lstScenes.Items.Remove(scene);
					index = Math.Min(index, lstScenes.Items.Count - 1);
					lstScenes.SelectedIndex = index;
				}
			}
		}

		private void tsMoveUp_Click(object sender, EventArgs e)
		{
			if (_sourceScene == null) { return; }
			int index = lstScenes.SelectedIndex;
			if (index > 0)
			{
				lstScenes.SelectedIndexChanged -= lstScenes_SelectedIndexChanged;
				lstScenes.Items.RemoveAt(index);
				lstScenes.Items.Insert(index - 1, _sourceScene);
				_epilogue.Scenes.RemoveAt(index);
				_epilogue.Scenes.Insert(index - 1, _sourceScene);
				lstScenes.SelectedIndex = index - 1;
				lstScenes.SelectedIndexChanged += lstScenes_SelectedIndexChanged;
			}
		}

		private void tsMoveDown_Click(object sender, EventArgs e)
		{
			if (_sourceScene == null) { return; }
			int index = lstScenes.SelectedIndex;
			if (index < lstScenes.Items.Count - 1)
			{
				lstScenes.SelectedIndexChanged -= lstScenes_SelectedIndexChanged;
				lstScenes.Items.RemoveAt(index);
				lstScenes.Items.Insert(index + 1, _sourceScene);
				_epilogue.Scenes.RemoveAt(index);
				_epilogue.Scenes.Insert(index + 1, _sourceScene);
				lstScenes.SelectedIndex = index + 1;
				lstScenes.SelectedIndexChanged += lstScenes_SelectedIndexChanged;
			}
		}
	}
}
