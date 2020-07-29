﻿using Desktop.Skinning;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;

namespace Desktop
{
	public partial class WorkspaceControl : UserControl
	{
		public IWorkspace Workspace { get; set; }

		private Dictionary<IActivity, TabPage> _tabs = new Dictionary<IActivity, TabPage>();
		private Dictionary<IActivity, TabPage> _sideTabs = new Dictionary<IActivity, TabPage>();
		private HashSet<TabPage> _spacers = new HashSet<TabPage>();
		private bool _forceSwitch = false;
		private bool _activated;

		public WorkspaceControl()
		{
			InitializeComponent();
		}

		public IEnumerable<IActivity> Activities
		{
			get
			{
				foreach (IActivity activity in _tabs.Keys)
				{
					yield return activity;
				}
				foreach (IActivity activity in _sideTabs.Keys)
				{
					yield return activity;
				}
			}
		}

		internal void AddActivity(IActivity activity)
		{
			activity.Activated += Activity_Activated;
			Control ctl = activity as Control;
			if (ctl == null)
				throw new ArgumentException($"Expected activity {activity.GetType().Name} to be an instance of Control");

			if (activity.GetType().GetCustomAttribute<SpacerAttribute>() != null)
			{
				AddSpacer(activity.Pane);
			}

			//Create the tab page
			TabPage page = new TabPage();
			page.Text = activity.Caption;
			page.Tag = activity;
			ctl.Dock = DockStyle.Fill;
			page.Controls.Add(ctl);
			switch (activity.Pane)
			{
				case WorkspacePane.Main:
					tabActivities.TabPages.Add(page);
					_tabs[activity] = page;
					break;
				case WorkspacePane.Sidebar:
					tabSidebarActivities.TabPages.Add(page);
					_sideTabs[activity] = page;
					break;
			}

			UpdateTabVisibility();
		}

		private void AddSpacer(WorkspacePane pane)
		{
			TabPage page = new TabPage();
			page.Tag = "spacer";
			switch (pane)
			{
				case WorkspacePane.Main:
					tabActivities.TabPages.Add(page);
					break;
				case WorkspacePane.Sidebar:
					tabSidebarActivities.TabPages.Add(page);
					break;
			}
			_spacers.Add(page);
		}

		internal void RemoveActivity(IActivity activity)
		{
			activity.Activated -= Activity_Activated;
			switch (activity.Pane)
			{
				case WorkspacePane.Main:
					TabPage tab = _tabs[activity];
					tabActivities.TabPages.Remove(tab);
					_tabs.Remove(activity);
					break;
				case WorkspacePane.Sidebar:
					tab = _sideTabs[activity];
					tabSidebarActivities.TabPages.Remove(tab);
					_sideTabs.Remove(activity);
					break;
			}

			UpdateTabVisibility();
		}

		private void Activity_Activated(object sender, EventArgs e)
		{
			_forceSwitch = true;
			TabPage tab;
			IActivity activity = sender as IActivity;
			if (_tabs.TryGetValue(activity, out tab))
			{
				tabActivities.SelectedTab = tab;
			}
			else if (_sideTabs.TryGetValue(activity, out tab))
			{
				if (!_activated)
				{
					tabSidebarActivities.SelectedTab = tab;
					splitContainer1.SplitterDistance = Math.Min(splitContainer1.Width - 10, splitContainer1.Width - Math.Max(100, activity.SidebarWidth));
					_activated = true;
				}
			}
			_forceSwitch = false;
		}

		private void UpdateTabVisibility()
		{
			if (_tabs.Count == 0)
				return;
			TabPage page = tabActivities.TabPages[0];
			Control ctl = page.Tag as Control;

			DockTabPage(_tabs, tabActivities, splitContainer1.Panel1, stripActivities, splitContainer2);

			//Update sidebar visibility
			if (tabSidebarActivities.TabPages.Count == 0)
			{
				splitContainer1.Panel2Collapsed = true;
			}
			else
			{
				splitContainer1.Panel2Collapsed = false;
				DockTabPage(_sideTabs, tabSidebarActivities, sidebar, stripSidebar, tabSidebarActivities);
			}
		}

		private void DockTabPage(Dictionary<IActivity, TabPage> tabs, TabControl tabControl, Control parentControl, Control tabStrip, Control tabContainer)
		{
			TabPage page = tabControl.TabPages[0];
			Control ctl = page.Tag as Control;

			if (tabs.Count == 1)
			{
				//Move the control out of the tabstrip
				parentControl.Controls.Add(ctl);
				tabContainer.Visible = false;
				tabStrip.Visible = false;
			}
			else
			{
				//put the control back into the tabstrip
				page.Controls.Add(ctl);
				tabContainer.Visible = true;
				tabStrip.Visible = true;
			}
		}

		private void OnSelectingTab(object sender, TabControlCancelEventArgs e)
		{
			if (_spacers.Contains(e.TabPage))
			{
				e.Cancel = true;
				return;
			}

			if (_forceSwitch)
				return;
			IActivity activity = e.TabPage.Tag as IActivity;
			if (!Shell.Instance.Activate(activity))
				e.Cancel = true;
		}

		public bool IsSidebarExpanded
		{
			get { return Workspace.ActiveSidebarActivity != null && !splitContainer1.Panel2Collapsed; }
		}

		public void ToggleSidebar(bool expanded)
		{
			if (Workspace.ActiveSidebarActivity == null) { return; }

			splitContainer1.Panel2Collapsed = !expanded;
		}

		public void ShowBanner(string text, SkinnedHighlight highlight)
		{
			if (string.IsNullOrEmpty(text))
			{
				splitContainer2.Panel1Collapsed = true;
			}
			else
			{
				grpBanner.Text = text;
				if (highlight == SkinnedHighlight.Bad)
				{
					grpBanner.PanelType = SkinnedBackgroundType.Critical;
				}
				else
				{
					grpBanner.PanelType = SkinnedBackgroundType.Surface;
				}
				splitContainer2.Panel1Collapsed = false;
			}

		}
	}
}
