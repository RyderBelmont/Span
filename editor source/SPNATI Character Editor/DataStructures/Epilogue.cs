﻿using SPNATI_Character_Editor.IO;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace SPNATI_Character_Editor
{
	public class Epilogue
	{
		[XmlAttribute("gender")]
		public string Gender = "any";

		[XmlAttribute("playerStartingLayers")]
		public string PlayerStartingLayers;

		[XmlAttribute("markers")]
		public string AllMarkers;

		[XmlAttribute("not-markers")]
		public string NotMarkers;

		[XmlAttribute("any-markers")]
		public string AnyMarkers;

		[XmlAttribute("alsoplaying-markers")]
		public string AlsoPlayingAllMarkers;

		[XmlAttribute("alsoplaying-not-markers")]
		public string AlsoPlayingNotMarkers;

		[XmlAttribute("alsoplaying-any-markers")]
		public string AlsoPlayingAnyMarkers;

		[XmlAttribute("alsoPlaying")]
		public string AlsoPlaying;

        [XmlAttribute("img")]
        public string GalleryImage;

		[XmlElement("title")]
		public string Title = "New Ending";

		[XmlNewLine(XmlNewLinePosition.Both)]
		[XmlElement("screen")]
		public List<Screen> Screens = new List<Screen>();

		public override string ToString()
		{
			string text = Title;
			if (Gender == "male")
				return text + " (m)";
			else if (Gender == "female")
				return text + " (f)";
			return text;
		}
	}
}
