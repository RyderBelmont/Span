﻿using SPNATI_Character_Editor.IO;
using System;
using System.Xml.Serialization;

namespace SPNATI_Character_Editor
{
	/// <summary>
	/// Data represenation of meta.xml
	/// </summary>
	/// <remarks>
	/// PROPERTY ORDER IS IMPORTANT - Order determines attribute order in generated XML files
	/// </remarks>
	[XmlRoot("opponent")]
	public class Metadata : IHookSerialization
	{
		[XmlNewLine]
		[XmlElement("enabled")]
		public bool Enabled;

		[XmlElement("first")]
		public string FirstName;

		[XmlElement("last")]
		public string LastName;

		[XmlElement("label")]
		public string Label;

		[XmlElement("pic")]
		public string Portrait;

		[XmlElement("gender")]
		public string Gender;

		[XmlElement("height")]
		public string Height;

		[XmlElement("from")]
		public string Source;

		[XmlElement("writer")]
		public string Writer;

		[XmlElement("artist")]
		public string Artist;

		[XmlElement("description")]
		public string Description;

		[XmlElement("has_ending")]
		public bool HasEnding;

		[XmlElement("layers")]
		public int Layers;

		[XmlElement("release")]
		public string ReleaseNumber;

		public Metadata()
		{
		}

		public Metadata(Character c)
		{
			PopulateFromCharacter(c);
		}

		/// <summary>
		/// Builds the meta data from a character instance
		/// </summary>
		/// <param name="c"></param>
		public void PopulateFromCharacter(Character c)
		{
			FirstName = c.FirstName;
			LastName = c.LastName;
			Label = c.Label;
			Gender = c.Gender;
			Layers = c.Layers;
			HasEnding = c.Endings.Count > 0;
		}

		public void OnBeforeSerialize()
		{
			
		}

		public void OnAfterDeserialize()
		{
			//Encoding these doesn't need to be done in OnBeforeSerialize because the serializer does it automatically
			Description = XMLHelper.DecodeEntityReferences(Description);
		}
	}
}
