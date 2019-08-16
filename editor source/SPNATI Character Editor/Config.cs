﻿using Desktop;
using Desktop.Providers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace SPNATI_Character_Editor
{
	public static class Config
	{
		/// <summary>
		/// List of released versions since update tracking was added, used for determining which updates a user skipped and providing info about those
		/// </summary>
		public static readonly string[] VersionHistory = new string[] { "v3.0", "v3.0.1", "v3.1", "v3.2", "v3.3", "v3.3.1", "v3.4", "v3.4.1", "v3.5", "v3.6",
			"v3.7", "v3.7.1", "v3.8", "v3.8.1", "v3.8.2", "v4.0b", "v4.0.1b", "v4.0.2b", "v4.0.3b", "v4.0", "v4.1", "v4.2", "v4.2.1", "v4.3", "v4.4b", "v5.0b", "v5.0",
			"v5.1" };

		/// <summary>
		/// Current Version
		/// </summary>
		public static string Version { get { return VersionHistory[VersionHistory.Length - 1]; } }

		private static Dictionary<string, string> _settings = new Dictionary<string, string>();

		/// <summary>
		/// Gets whether a version predates the target version
		/// </summary>
		/// <param name="version"></param>
		/// <param name="targetVersion"></param>
		/// <returns></returns>
		public static bool VersionPredates(string version, string targetVersion)
		{
			if (string.IsNullOrEmpty(version))
			{
				return true;
			}

			bool passedTarget = false;
			for (int i = 0; i < VersionHistory.Length; i++)
			{
				string v = VersionHistory[i];
				if (v == targetVersion)
				{
					passedTarget = true;
				}
				if (v == version)
				{
					return !passedTarget;
				}
			}
			return true; //version that predates VersionHistory
		}

		/// <summary>
		/// Gets a string configuration setting
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public static string GetString(string key)
		{
			return _settings.Get(key.ToLower()) ?? "";
		}

		/// <summary>
		/// Gets a boolean configuration setting
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		public static bool GetBoolean(string key)
		{
			string setting = _settings.Get(key.ToLower()) ?? "";
			return !string.IsNullOrEmpty(setting) && setting != "0";
		}

		public static int GetInt(string key)
		{
			string setting = _settings.Get(key.ToLower()) ?? "0";
			int value;
			int.TryParse(setting, out value);
			return value;
		}

		/// <summary>
		/// Sets a configuration setting
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public static void Set(string key, string value)
		{
			_settings[key.ToLower()] = value;
		}

		/// <summary>
		/// Sets a boolean configuration setting
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public static void Set(string key, bool value)
		{
			_settings[key.ToLower()] = (value ? "1" : "0");
		}

		/// <summary>
		/// Sets a numeric configuration setting
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		public static void Set(string key, int value)
		{
			_settings[key.ToLower()] = value.ToString();
		}

		static Config()
		{
			//3.0 and up use config.ini. Older versions use settings.ini. Using different filenames to allow side-by-side installs since the structure was changed

			string filename = Path.Combine(AppDataDirectory, "config.ini");
			if (File.Exists(filename))
			{
				ReadSettings(filename);
			}
			else
			{
				filename = Path.Combine(AppDataDirectory, "settings.ini");
				if (File.Exists(filename))
				{
					ReadLegacySettings(filename);
				}
			}
		}

		private static void ReadSettings(string file)
		{
			string[] lines = File.ReadAllLines(file);
			try
			{
				for (int i = 0; i < lines.Length; i++)
				{
					string line = lines[i];
					string[] kvp = line.Split(new char[] { '=' }, 2);
					string key = kvp[0].ToLower();
					string value = kvp[1];
					Set(key, value);
				}
			}
			catch
			{
			}
		}

		private static void ReadLegacySettings(string file)
		{
			string[] lines = File.ReadAllLines(file);
			try
			{
				Set(Settings.GameDirectory, lines[0]);
				Set(Settings.LastCharacter, lines[1]);
				Set(Settings.LastVersionRun, lines[5]);
			}
			catch { }
		}

		public static void Save()
		{
			string dataDir = AppDataDirectory;
			string filename = Path.Combine(dataDir, "config.ini");
			if (!Directory.Exists(dataDir))
			{
				Directory.CreateDirectory(dataDir);
			}

			SaveRecentRecords();

			List<string> lines = new List<string>();
			foreach (KeyValuePair<string, string> kvp in _settings)
			{
				lines.Add($"{kvp.Key.ToLower()}={kvp.Value}");
			}
			File.WriteAllLines(filename, lines);
		}

		public static void LoadRecentRecords<T>()
		{
			string[] keys = GetString("Recent" + typeof(T).Name).Split('|');
			foreach (string key in keys)
			{
				IRecord record = null;
				if (typeof(T) == typeof(Character))
				{
					record = CharacterDatabase.Get(key);
				}
				else if (typeof(T) == typeof(Costume))
				{
					record = CharacterDatabase.GetSkin(key);
				}
				if (record != null)
				{
					RecordLookup.AddToRecent(typeof(T), record);
				}
			}
		}

		private static void SaveRecentRecords()
		{
			SaveRecords<Character>();
			SaveRecords<Costume>();
		}

		private static void SaveRecords<T>()
		{
			List<IRecord> list = RecordLookup.GetRecentRecords<T>();
			List<string> keys = new List<string>();
			foreach (IRecord record in list)
			{
				keys.Add(record.Key);
			}
			Set("Recent" + typeof(T).Name, string.Join("|", keys));
		}

		/// <summary>
		/// Gets the executable's root directory
		/// </summary>
		public static string ExecutableDirectory
		{
			get	{ return Path.GetDirectoryName(Application.ExecutablePath);	}
		}

		/// <summary>
		/// Gets where SPNATI is located
		/// </summary>
		public static string SpnatiDirectory
		{
			get { return GetString(Settings.GameDirectory); }
		}

		/// <summary>
		/// Gets where SPNATI is located
		/// </summary>
		public static string KisekaeDirectory
		{
			get { return GetString(Settings.KisekaeDirectory); }
			set { Set(Settings.KisekaeDirectory, value); }
		}

		/// <summary>
		/// Gets the program's %appdata% path
		/// </summary>
		public static string AppDataDirectory
		{
			get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "SPNATI"); }
		}

		/// <summary>
		/// Retrieves the root directory for a character
		/// </summary>
		public static string GetRootDirectory(Character character)
		{
			if (character == null || string.IsNullOrEmpty(character.FolderName))
				return "";
			return GetRootDirectory(character.FolderName);
		}


		/// <summary>
		/// Retrieves the backup directory for a character
		/// </summary>
		public static string GetBackupDirectory(Character character)
		{
			if (character == null || string.IsNullOrEmpty(character.FolderName))
				return "";
			return Path.Combine(AppDataDirectory, character.FolderName);
		}

		/// <summary>
		/// Retrieves the full directory name for a folder
		/// </summary>
		/// <param name="character"></param>
		/// <returns></returns>
		public static string GetRootDirectory(string folder)
		{
			if (GetString(Settings.GameDirectory) == null || folder == null)
				return "";
			return Path.Combine(GetString(Settings.GameDirectory), "opponents", folder);
		}

		/// <summary>
		/// Gets the current user
		/// </summary>
		public static string UserName
		{
			get { return GetString(Settings.UserName); }
			set { Set(Settings.UserName, value); }
		}

		/// <summary>
		/// How many minutes to auto-save
		/// </summary>
		public static int AutoSaveInterval
		{
			get { return GetInt(Settings.AutoSaveInterval); }
			set
			{
				Set(Settings.AutoSaveInterval, value);
				Shell.Instance.AutoTickFrequency = value * 60000;
			}
		}

		/// <summary>
		/// How many minutes to auto-backup
		/// </summary>
		public static bool BackupEnabled
		{
			get { return !GetBoolean("disableautobackup"); }
			set
			{
				Set("disableautobackup", !value);
			}
		}

		/// <summary>
		/// How frequently to backup
		/// </summary>
		public static int BackupPeriod
		{
			get
			{
				int value = GetInt("backupperiod");
				int result;
				if (value == 0)
				{
					result = 30;
				}
				else
				{
					result = value;
				}
				return result;
			}
			set
			{
				Set("backupperiod", value);
			}
		}

		/// <summary>
		/// How long to keep backups
		/// </summary>
		public static int BackupLifeTime
		{
			get
			{
				int value = GetInt("backuplife");
				int result;
				if (value == 0)
				{
					result = 30;
				}
				else
				{
					result = value;
				}
				return result;
			}
			set
			{
				Set("backuplife", value);
			}
		}

		/// <summary>
		/// Whether variable intellisense is enabled
		/// </summary>
		public static bool UseIntellisense
		{
			get { return !GetBoolean(Settings.DisableIntellisense); }
			set { Set(Settings.DisableIntellisense, !value); }
		}

		/// <summary>
		/// Whether prefixless images are available in dialogue
		/// </summary>
		public static bool UsePrefixlessImages
		{
			get { return !GetBoolean(Settings.HideNoPrefix); }
			set { Set(Settings.HideNoPrefix, !value); }
		}

		/// <summary>
		/// Filter of prefixes to hide from dialogue poses
		/// </summary>
		public static string PrefixFilter
		{
			get { return GetString(Settings.PrefixFilter); }
			set { Set(Settings.PrefixFilter, value); }
		}

		/// <summary>
		/// Load other character info up front in banter wizard
		/// </summary>
		public static bool AutoLoadBanterWizard
		{
			get { return GetBoolean("autoloadbanter"); }
			set { Set("autoloadbanter", value); }
		}

		/// <summary>
		/// Auto-open record select for targets, markers, etc. in dialogue
		/// </summary>
		public static bool AutoOpenConditions
		{
			get { return !GetBoolean("autocondition"); }
			set { Set("autocondition", !value); }
		}

		public static bool SeenMacroHelp
		{
			get { return GetBoolean("macrohelp"); }
			set { Set("macrohelp", value); }
		}

		public static bool SuppressDefaults
		{
			get { return GetBoolean("suppressdefaultlines"); }
			set { Set("suppressdefaultlines", value); }
		}

		public static bool UseSimpleTree
		{
			get { return GetBoolean("simpletree"); }
			set { Set("simpletree", value); }
		}

		public static bool DevMode
		{
			get { return GetBoolean(Settings.DevMode); }
			set { Set(Settings.DevMode, value); }
		}

		public static string Skin
		{
			get { return GetString("skin"); }
			set { Set("skin", value); }
		}

		public static bool ColorTargetedLines
		{
			get { return GetBoolean("colortargets"); }
			set { Set("colortargets", value); }
		}

		public static bool DisableWorkflowTracer
		{
			get { return GetBoolean("workflowtracer"); }
			set { Set("workflowtracer", value); }
		}

		public static bool HideEmptyCases
		{
			get { return GetBoolean("hideemptycases"); }
			set { Set("hideemptycases", value); }
		}

		public static int ImportMethod
		{
			get { return GetInt("import"); }
			set { Set("import", value); }
		}

		public static HashSet<string> AutoPauseDirectives
		{
			get
			{
				HashSet<string> set = new HashSet<string>();
				string items = GetString("autopause") ?? "";
				foreach (string item in items.Split(','))
				{
					set.Add(item);
				}
				return set;
			}
			set
			{
				string items = string.Join(",", value);
				Set("autopause", items);
			}
		}

		public static void SaveMacros(string key)
		{
			MacroProvider provider = new MacroProvider();
			int index = 0;
			foreach (IRecord record in provider.GetRecords(""))
			{
				index++;
				Macro macro = record as Macro;
				Set($"Macro{key}{index}", macro.Serialize());
			}
			Set($"Macro{key}0", index);

			Save();
		}

		public static void LoadMacros<T>(string key)
		{
			MacroProvider provider = new MacroProvider();
			int count = GetInt($"Macro{key}0");
			for (int i = 1; i <= count; i++)
			{
				string value = GetString($"Macro{key}{i}");
				Macro macro = Macro.Deserialize(value);
				if (macro != null)
				{
					provider.Add(typeof(T), macro);
				}
			}
		}

		/// <summary>
		/// Last ending that was opened
		/// </summary>
		public static string LastEnding
		{
			get { return GetString("lastending"); }
			set { Set("lastending", value); }
		}
	}

	public static class Settings
	{
		public static readonly string GameDirectory = "game";
		public static readonly string KisekaeDirectory = "kkl";
		public static readonly string LastCharacter = "last";
		public static readonly string LastVersionRun = "version";
		public static readonly string UserName = "username";
		public static readonly string AutoSaveInterval = "autosave";
		public static readonly string DisableIntellisense = "nointellisense";
		public static readonly string HideNoPrefix = "hidenoprefix";
		public static readonly string PrefixFilter = "prefixfilter";
		public static readonly string HideImages = "safemode";

		#region Settings that probably only make sense for debugging
		public static readonly string LoadOnlyLastCharacter = "loadlast";
		public static readonly string DevMode = "devmode";
		#endregion
	}
}
