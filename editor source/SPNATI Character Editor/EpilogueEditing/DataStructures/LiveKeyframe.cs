﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using Desktop;
using Desktop.CommonControls;
using Desktop.CommonControls.PropertyControls;
using Desktop.DataStructures;

namespace SPNATI_Character_Editor.EpilogueEditor
{
	/// <summary>
	/// A "live" keyframe part of active animation rather than just the static definition behind an animation that comes from XML
	/// </summary>
	public class LiveKeyframe : BindableObject, ILabel
	{
		public event EventHandler LabelChanged;

		public LiveAnimatedObject Data;

		[Float(DisplayName = "Time", Key = "time", GroupOrder = 0, Increment = 0.1f)]
		public float Time
		{
			get { return Get<float>(); }
			set
			{
				Set(value);
				LabelChanged?.Invoke(this, EventArgs.Empty);
			}
		}

		public HashSet<string> TrackedProperties { get; }

		[Float(DisplayName = "X", Key = "x", GroupOrder = 15, Minimum = -100000, Maximum = 100000, DecimalPlaces = 0)]
		public float? X
		{
			get { return Get<float?>(); }
			set { Set(value); }
		}

		[Float(DisplayName = "Y", Key = "y", GroupOrder = 20, Minimum = -100000, Maximum = 100000, DecimalPlaces = 0)]
		public float? Y
		{
			get { return Get<float?>(); }
			set { Set(value); }
		}

		/// <summary>
		/// Data specific to a single property within the keyframe
		/// </summary>
		public ObservableDictionary<string, LiveKeyframeMetadata> PropertyMetadata
		{
			get { return Get<ObservableDictionary<string, LiveKeyframeMetadata>>(); }
			set { Set(value); }
		}

		public LiveKeyframe()
		{
			PropertyMetadata = new ObservableDictionary<string, LiveKeyframeMetadata>();
			TrackedProperties = new HashSet<string>();
			TrackedProperties.Add("X");
			TrackedProperties.Add("Y");
		}

		public virtual bool FilterRecord(PropertyRecord record)
		{
			if (record.Key == "time" && Time == 0)
			{
				return false;
			}
			return true;
		}

		public KeyframeType GetFrameType(string property)
		{
			if (string.IsNullOrEmpty(property)) { return KeyframeType.Normal; }
			LiveKeyframeMetadata metadata = GetMetadata(property, false);
			if (metadata == null) { return KeyframeType.Normal; }
			return metadata.FrameType;
		}

		public override string ToString()
		{
			return $"{Time}";
		}

		public string GetLabel()
		{
			if (Data == null) { return "???"; }
			return $"Keyframe: {Data.Id} ({Time}s)";
		}

		/// <summary>
		/// Gets whether this keyframe has any data set in it
		/// </summary>
		public bool IsEmpty
		{
			get
			{
				foreach (string propName in TrackedProperties)
				{
					if (HasProperty(propName))
					{
						return false;
					}
				}
				return true;
			}
		}

		/// <summary>
		/// Gets how many animated properties are set on this frame
		/// </summary>
		public int PropertyCount
		{
			get
			{
				int count = 0;
				foreach (string propName in TrackedProperties)
				{
					if (HasProperty(propName))
					{
						count++;
					}
				}
				return count;
			}
		}

		public LiveKeyframeMetadata GetMetadata(string property, bool addIfNew)
		{
			if (!PropertyMetadata.ContainsKey(property))
			{
				if (addIfNew)
				{
					LiveKeyframeMetadata metadata = new LiveKeyframeMetadata(property);
					PropertyMetadata.Add(property, metadata);
					return metadata;
				}
				else
				{
					return new LiveKeyframeMetadata(property);
				}
			}
			return PropertyMetadata[property];
		}

		public virtual Keyframe CreateKeyframeDefinition(Directive directive)
		{
			Type keyframeType = typeof(Keyframe);
			string time = Time.ToString(CultureInfo.InvariantCulture);
			Keyframe kf = directive.Keyframes.Find(k => k.Time == time) ?? new Keyframe();
			kf.Time = time;
			foreach (string property in TrackedProperties)
			{
				if (HasProperty(property))
				{
					MemberInfo mi = PropertyTypeInfo.GetMemberInfo(keyframeType, property);
					if (mi == null)
					{
						throw new InvalidOperationException($"No property on Keyframe called {property} found.");
					}
					Type dataType = mi.GetDataType();
					if (dataType == typeof(int))
					{
						mi.SetValue(kf, Get<int>(property));
					}
					else if (dataType == typeof(float))
					{
						mi.SetValue(kf, Get<float>(property));
					}
					else if (dataType == typeof(bool))
					{
						mi.SetValue(kf, Get<bool>(property));
					}
					else
					{
						string value = Convert.ToString(Get<object>(property), CultureInfo.InvariantCulture);
						mi.SetValue(kf, value);
					}
				}
			}
			return kf;
		}
	}
}
